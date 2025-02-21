using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using pva.SuperV.Api.Routes.Classes;
using pva.SuperV.Api.Routes.FieldFormatters;
using pva.SuperV.Api.Routes.Projects;
using pva.SuperV.Model.Classes;
using pva.SuperV.Model.FieldFormatters;
using pva.SuperV.Model.Projects;
using Scalar.AspNetCore;
using System.Text.Json.Serialization;

namespace pva.SuperV.Api
{
    public class WebApiProgram
    {
        public class GlobalExceptionHandler : IExceptionFilter
        {
            public void OnException(ExceptionContext context)
            {
                // Log the exception or perform additional actions
                context.Result = new ObjectResult($"Global error: {context.Exception.Message}")
                {
                    StatusCode = 500,
                };
                context.ExceptionHandled = true;
            }
        }

        public static void Main(string[] args)
        {
            new WebApiProgram()
                .Run(args);
        }

        private void Run(string[] args)
        {
            var builder = WebApplication.CreateSlimBuilder(args);

            builder.Services
                .ConfigureHttpJsonOptions(options =>
                {
                    options.SerializerOptions.TypeInfoResolverChain.Insert(0, AppJsonSerializerContext.Default);

                })
                .AddHttpLogging(logging =>
                {
                    logging.LoggingFields = HttpLoggingFields.All;
                })
                .AddProblemDetails()
                .AddSingleton<IProjectService, ProjectService>()
                .AddSingleton<IClassService, ClassService>()
                .AddSingleton<IFieldFormatterService, FieldFormatterService>();
            builder.Services.AddOpenApi(options =>
            {
                options.AddDocumentTransformer((document, context, cancellationToken) =>
                {
                    document.Info = new()
                    {
                        Title = "SuperV API",
                        Version = "v1",
                        Description = "API for accessing SuperV projects."
                    };
                    return Task.CompletedTask;
                });
            });
            builder.Services.AddHttpLogging(o => { });
            builder.Logging.AddConsole();
            builder.Logging.AddJsonConsole();

            var app = builder.Build();

            app.UseHttpLogging();
            app.UseExceptionHandler(exceptionHandlerApp =>
            {
                exceptionHandlerApp.Run(async httpContext =>
                {
                    var pds = httpContext.RequestServices.GetService<IProblemDetailsService>();
                    if (pds == null
                        || !await pds.TryWriteAsync(new() { HttpContext = httpContext }))
                    {
                        // Fallback behavior
                        await httpContext.Response.WriteAsync("Fallback: An error occurred.");
                    }
                });
            });
            app.UseStatusCodePages();
            app.UseDeveloperExceptionPage();

            app.MapOpenApi();
            app.MapScalarApiReference();

            app.MapProjectEndpoints()
               .MapClassEndpoints()
               .MapFieldFormatterEndpoints();

            app.Run();
        }
    }

    [JsonSerializable(typeof(List<ProjectModel>))]
    [JsonSerializable(typeof(ProjectModel))]
    [JsonSerializable(typeof(CreateProjectRequest))]
    [JsonSerializable(typeof(FieldFormatterModel))]
    [JsonSerializable(typeof(EnumFormatterModel))]
    [JsonSerializable(typeof(List<FieldFormatterModel>))]
    [JsonSerializable(typeof(List<ClassModel>))]
    [JsonSerializable(typeof(ClassModel))]
    internal partial class AppJsonSerializerContext : JsonSerializerContext
    {
    }
}
