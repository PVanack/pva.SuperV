using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NSubstitute;
using pva.SuperV.Api;
using pva.SuperV.Api.Services.Classes;
using pva.SuperV.Api.Services.FieldFormatters;
using pva.SuperV.Api.Services.HistoryRepositories;
using pva.SuperV.Api.Services.Projects;

namespace pva.SuperV.ApiTests
{
    public class TestProjectApplication : WebApplicationFactory<WebApiProgram>
    {
        public IProjectService? MockedProjectService { get; private set; }
        public IClassService? MockedClassService { get; private set; }
        public IFieldFormatterService? MockFieldFormatterService { get; private set; }
        public IHistoryRepositoryService? MockHistoryRepositoryService { get; private set; }

        protected override IHost CreateHost(IHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                ServiceDescriptor? item = services.FirstOrDefault(descriptor => descriptor.ServiceType == typeof(IProjectService));
                if (item is not null)
                {
                    services.Remove(item);
                }
                MockedProjectService = Substitute.For<IProjectService>();
                services.AddSingleton<IProjectService>(provider => MockedProjectService!);

                item = services.FirstOrDefault(descriptor => descriptor.ServiceType == typeof(IClassService));
                if (item is not null)
                {
                    services.Remove(item);
                }
                MockedClassService = Substitute.For<IClassService>();
                services.AddSingleton<IClassService>(provider => MockedClassService!);

                item = services.FirstOrDefault(descriptor => descriptor.ServiceType == typeof(IFieldFormatterService));
                if (item is not null)
                {
                    services.Remove(item);
                }
                MockFieldFormatterService = Substitute.For<IFieldFormatterService>();
                services.AddSingleton<IFieldFormatterService>(provider => MockFieldFormatterService!);

                item = services.FirstOrDefault(descriptor => descriptor.ServiceType == typeof(IHistoryRepositoryService));
                if (item is not null)
                {
                    services.Remove(item);
                }
                MockHistoryRepositoryService = Substitute.For<IHistoryRepositoryService>();
                services.AddSingleton<IHistoryRepositoryService>(provider => MockHistoryRepositoryService!);
            });
            return base.CreateHost(builder);
        }
    }
}
