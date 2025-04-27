using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NSubstitute;
using pva.SuperV.Api;
using pva.SuperV.Model.Services;

namespace pva.SuperV.ApiTests
{
    public class TestProjectApplication : WebApplicationFactory<WebApiProgram>
    {
        public IProjectService? MockedProjectService { get; private set; }
        public IClassService? MockedClassService { get; private set; }
        public IFieldFormatterService? MockedFieldFormatterService { get; private set; }
        public IHistoryRepositoryService? MockedHistoryRepositoryService { get; private set; }
        public IFieldDefinitionService? MockedFieldDefinitionService { get; private set; }
        public IFieldProcessingService? MockedFieldProcessingService { get; private set; }
        public IInstanceService? MockedInstanceService { get; private set; }
        public IFieldValueService? MockedFieldValueService { get; private set; }
        public IHistoryValuesService? MockedHistoryValueService { get; private set; }

        protected override IHost CreateHost(IHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                MockedProjectService = CreateMockedService<IProjectService>(services, MockedProjectService);
                MockedClassService = CreateMockedService<IClassService>(services, MockedClassService);
                MockedFieldFormatterService = CreateMockedService<IFieldFormatterService>(services, MockedFieldFormatterService);
                MockedHistoryRepositoryService = CreateMockedService<IHistoryRepositoryService>(services, MockedHistoryRepositoryService);
                MockedFieldDefinitionService = CreateMockedService<IFieldDefinitionService>(services, MockedFieldDefinitionService);
                MockedFieldProcessingService = CreateMockedService<IFieldProcessingService>(services, MockedFieldProcessingService);
                MockedInstanceService = CreateMockedService<IInstanceService>(services, MockedInstanceService);
                MockedFieldValueService = CreateMockedService<IFieldValueService>(services, MockedFieldValueService);
                MockedHistoryValueService = CreateMockedService<IHistoryValuesService>(services, MockedHistoryValueService);
            });
            return base.CreateHost(builder);
        }

        private static T? CreateMockedService<T>(IServiceCollection services, T? mockedService) where T : class
        {
            ServiceDescriptor? item = services.FirstOrDefault(descriptor => descriptor.ServiceType == typeof(T));
            if (item is not null)
            {
                services.Remove(item);
            }
            mockedService = Substitute.For([typeof(T)], []) as T;
            services.AddSingleton<T>(provider => mockedService!);
            return mockedService;
        }
    }
}
