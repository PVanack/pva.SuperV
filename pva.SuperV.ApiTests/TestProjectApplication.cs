using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NSubstitute;
using pva.SuperV.Api;

namespace pva.SuperV.ApiTests
{
    public class TestProjectApplication : WebApplicationFactory<WebApiProgram>
    {
        public IProjectService? MockedProjectService { get; private set; }
        public IClassService? MockedClassService { get; private set; }

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
            });
            return base.CreateHost(builder);
        }
    }
}
