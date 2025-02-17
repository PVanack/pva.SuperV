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
            });
            return base.CreateHost(builder);
        }
    }
}
