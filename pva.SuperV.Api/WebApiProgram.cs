using pva.SuperV.Api.Routes.Classes;
using pva.SuperV.Api.Routes.Projects;
using pva.SuperV.Model;
using Scalar.AspNetCore;
using System.Text.Json.Serialization;

namespace pva.SuperV.Api
{
    public  class WebApiProgram
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
                .AddSingleton<IProjectService, ProjectService>()
                .AddSingleton<IClassService, ClassService>();
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

            app.MapOpenApi();
            app.MapScalarApiReference();

            app.MapProjectEndpoints()
               .MapClassEndpoints();

            app.Run();
        }
    }

    [JsonSerializable(typeof(List<ProjectModel>))]
    [JsonSerializable(typeof(ProjectModel))]
    [JsonSerializable(typeof(CreateProjectRequest))]
    [JsonSerializable(typeof(List<ClassModel>))]
    [JsonSerializable(typeof(ClassModel))]
    internal partial class AppJsonSerializerContext : JsonSerializerContext
    {
    }
}
