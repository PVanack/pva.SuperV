using Microsoft.AspNetCore.HttpLogging;
using pva.SuperV.Api.Routes.Classes;
using pva.SuperV.Api.Routes.FieldDefinitions;
using pva.SuperV.Api.Routes.FieldFormatters;
using pva.SuperV.Api.Routes.HistoryRepositories;
using pva.SuperV.Api.Routes.Projects;
using pva.SuperV.Api.Services.Classes;
using pva.SuperV.Api.Services.FieldDefinitions;
using pva.SuperV.Api.Services.FieldFormatters;
using pva.SuperV.Api.Services.HistoryRepositories;
using pva.SuperV.Api.Services.Projects;
using pva.SuperV.Model.Classes;
using pva.SuperV.Model.FieldDefinitions;
using pva.SuperV.Model.FieldFormatters;
using pva.SuperV.Model.HistoryRepositories;
using pva.SuperV.Model.Projects;
using Scalar.AspNetCore;
using System.Text.Json.Serialization;

namespace pva.SuperV.Api
{
    public class WebApiProgram
    {
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
                .AddSingleton<IFieldFormatterService, FieldFormatterService>()
                .AddSingleton<IHistoryRepositoryService, HistoryRepositoryService>()
                .AddSingleton<IFieldDefinitionService, FieldDefinitionService>();
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
               .MapFieldFormatterEndpoints()
               .MapHistoryRepositoryEndpoints()
               .MapFieldDefinitionEndpoints();

            app.Run();
        }
    }

    [JsonSerializable(typeof(List<ProjectModel>))]
    [JsonSerializable(typeof(ProjectModel))]
    [JsonSerializable(typeof(CreateProjectRequest))]

    [JsonSerializable(typeof(List<FieldFormatterModel>))]
    [JsonSerializable(typeof(FieldFormatterModel))]
    [JsonSerializable(typeof(EnumFormatterModel))]
    [JsonSerializable(typeof(CreateFieldFormatterRequest))]

    [JsonSerializable(typeof(List<ClassModel>))]
    [JsonSerializable(typeof(ClassModel))]

    [JsonSerializable(typeof(List<HistoryRepositoryModel>))]
    [JsonSerializable(typeof(HistoryRepositoryModel))]

    [JsonSerializable(typeof(List<FieldDefinitionModel>))]
    [JsonSerializable(typeof(FieldDefinitionModel))]
    [JsonSerializable(typeof(IntFieldDefinitionModel))]
    [JsonSerializable(typeof(BoolFieldDefinitionModel))]
    [JsonSerializable(typeof(ShortFieldDefinitionModel))]
    [JsonSerializable(typeof(UshortFieldDefinitionModel))]
    [JsonSerializable(typeof(IntFieldDefinitionModel))]
    [JsonSerializable(typeof(UintFieldDefinitionModel))]
    [JsonSerializable(typeof(LongFieldDefinitionModel))]
    [JsonSerializable(typeof(UlongFieldDefinitionModel))]
    [JsonSerializable(typeof(FloatFieldDefinitionModel))]
    [JsonSerializable(typeof(DoubleFieldDefinitionModel))]
    [JsonSerializable(typeof(StringFieldDefinitionModel))]
    [JsonSerializable(typeof(DateTimeFieldDefinitionModel))]
    [JsonSerializable(typeof(TimeSpanFieldDefinitionModel))]
    internal partial class AppJsonSerializerContext : JsonSerializerContext
    {
    }
}
