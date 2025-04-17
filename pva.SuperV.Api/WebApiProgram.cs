using Microsoft.AspNetCore.HttpLogging;
using pva.SuperV.Api.Routes.Classes;
using pva.SuperV.Api.Routes.FieldDefinitions;
using pva.SuperV.Api.Routes.FieldFormatters;
using pva.SuperV.Api.Routes.FieldProcessings;
using pva.SuperV.Api.Routes.HistoryRepositories;
using pva.SuperV.Api.Routes.HistoryValues;
using pva.SuperV.Api.Routes.Instances;
using pva.SuperV.Api.Routes.Projects;
using pva.SuperV.Api.Services.Classes;
using pva.SuperV.Api.Services.FieldDefinitions;
using pva.SuperV.Api.Services.FieldFormatters;
using pva.SuperV.Api.Services.FieldProcessings;
using pva.SuperV.Api.Services.History;
using pva.SuperV.Api.Services.HistoryRepositories;
using pva.SuperV.Api.Services.Instances;
using pva.SuperV.Api.Services.Projects;
using pva.SuperV.Model;
using pva.SuperV.Model.Classes;
using pva.SuperV.Model.FieldDefinitions;
using pva.SuperV.Model.FieldFormatters;
using pva.SuperV.Model.FieldProcessings;
using pva.SuperV.Model.HistoryRepositories;
using pva.SuperV.Model.HistoryRetrieval;
using pva.SuperV.Model.Instances;
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
                .AddSingleton<IFieldDefinitionService, FieldDefinitionService>()
                .AddSingleton<IFieldProcessingService, FieldProcessingService>()
                .AddSingleton<IInstanceService, InstanceService>()
                .AddSingleton<IFieldValueService, FieldValueService>()
                .AddSingleton<IHistoryValuesService, HistoryValuesService>();
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
            var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
            builder.Services.AddCors(options =>
            {
                options.AddPolicy(name: MyAllowSpecificOrigins,
                                  policy =>
                                      policy
                                      .AllowAnyMethod()
                                      .AllowAnyHeader()
                                      .SetIsOriginAllowed(origin => true)
                                      .AllowCredentials()
                                  );
            });

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
               .MapFieldDefinitionEndpoints()
               .MapFieldProcessingEndpoints()
               .MapInstancesEndpoints()
               .MapHistoryValuesEndpoints();
            app.UseCors(MyAllowSpecificOrigins);
            app.Run();
        }
    }

    [JsonSerializable(typeof(IFormFile))]
    [JsonSerializable(typeof(FormFile))]

    [JsonSerializable(typeof(List<ProjectModel>))]
    [JsonSerializable(typeof(ProjectModel))]
    [JsonSerializable(typeof(CreateProjectRequest))]
    [JsonSerializable(typeof(UpdateProjectRequest))]
    [JsonSerializable(typeof(ProjectPagedSearchRequest))]
    [JsonSerializable(typeof(PagedSearchResult<ProjectModel>))]

    [JsonSerializable(typeof(List<FieldFormatterModel>))]
    [JsonSerializable(typeof(FieldFormatterModel))]
    [JsonSerializable(typeof(EnumFormatterModel))]
    [JsonSerializable(typeof(CreateFieldFormatterRequest))]
    [JsonSerializable(typeof(PagedSearchResult<FieldFormatterModel>))]
    [JsonSerializable(typeof(FieldFormatterPagedSearchRequest))]

    [JsonSerializable(typeof(List<ClassModel>))]
    [JsonSerializable(typeof(ClassModel))]
    [JsonSerializable(typeof(PagedSearchResult<ClassModel>))]
    [JsonSerializable(typeof(ClassPagedSearchRequest))]

    [JsonSerializable(typeof(List<HistoryRepositoryModel>))]
    [JsonSerializable(typeof(HistoryRepositoryModel))]

    [JsonSerializable(typeof(List<FieldDefinitionModel>))]
    [JsonSerializable(typeof(FieldDefinitionModel))]
    [JsonSerializable(typeof(BoolFieldDefinitionModel))]
    [JsonSerializable(typeof(DateTimeFieldDefinitionModel))]
    [JsonSerializable(typeof(DoubleFieldDefinitionModel))]
    [JsonSerializable(typeof(FloatFieldDefinitionModel))]
    [JsonSerializable(typeof(IntFieldDefinitionModel))]
    [JsonSerializable(typeof(LongFieldDefinitionModel))]
    [JsonSerializable(typeof(ShortFieldDefinitionModel))]
    [JsonSerializable(typeof(StringFieldDefinitionModel))]
    [JsonSerializable(typeof(TimeSpanFieldDefinitionModel))]
    [JsonSerializable(typeof(UintFieldDefinitionModel))]
    [JsonSerializable(typeof(UlongFieldDefinitionModel))]
    [JsonSerializable(typeof(UshortFieldDefinitionModel))]
    [JsonSerializable(typeof(PagedSearchResult<FieldDefinitionModel>))]
    [JsonSerializable(typeof(FieldDefinitionPagedSearchRequest))]

    [JsonSerializable(typeof(List<FieldValueProcessingModel>))]
    [JsonSerializable(typeof(FieldValueProcessingModel))]
    [JsonSerializable(typeof(AlarmStateProcessingModel))]
    [JsonSerializable(typeof(HistorizationProcessingModel))]

    [JsonSerializable(typeof(List<InstanceModel>))]
    [JsonSerializable(typeof(InstanceModel))]
    [JsonSerializable(typeof(BoolFieldValueModel))]
    [JsonSerializable(typeof(DateTimeFieldValueModel))]
    [JsonSerializable(typeof(DoubleFieldValueModel))]
    [JsonSerializable(typeof(FloatFieldValueModel))]
    [JsonSerializable(typeof(IntFieldValueModel))]
    [JsonSerializable(typeof(LongFieldValueModel))]
    [JsonSerializable(typeof(ShortFieldValueModel))]
    [JsonSerializable(typeof(StringFieldValueModel))]
    [JsonSerializable(typeof(TimeSpanFieldValueModel))]
    [JsonSerializable(typeof(UintFieldValueModel))]
    [JsonSerializable(typeof(UlongFieldValueModel))]
    [JsonSerializable(typeof(UshortFieldValueModel))]
    [JsonSerializable(typeof(PagedSearchResult<InstanceModel>))]
    [JsonSerializable(typeof(InstancePagedSearchRequest))]

    [JsonSerializable(typeof(HistoryRequestModel))]
    [JsonSerializable(typeof(HistoryStatisticsRequestModel))]
    [JsonSerializable(typeof(HistoryRawResultModel))]
    [JsonSerializable(typeof(List<HistoryFieldModel>))]
    [JsonSerializable(typeof(List<HistoryRawRowModel>))]
    internal partial class AppJsonSerializerContext : JsonSerializerContext
    {
    }
}
