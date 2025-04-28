using MudBlazor.Services;
using pva.SuperV.Blazor.Components;
using pva.SuperV.Blazor.Services;
using pva.SuperV.Model.Services;

namespace pva.SuperV.Blazor
{
    public static class Program
    {
        public const string SuperVRestClientName = "SuperVRestClient";
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services
                .AddRazorComponents(options
                    => options.DetailedErrors = builder.Environment.IsDevelopment())
                .AddInteractiveServerComponents(options =>
                {
                    options.DetailedErrors = true;
                })
                .AddInteractiveWebAssemblyComponents();
            builder.Services.AddMudServices();

            builder.Services
                .AddScoped<State>()
                .AddScoped<IClassService, ClassService>((service) => new(BuildHttpClient(builder)))
                .AddScoped<IProjectService, ProjectService>((service) => new(BuildHttpClient(builder)))
                .AddScoped<IFieldFormatterService, FieldFormatterService>((service) => new(BuildHttpClient(builder)))
                .AddScoped<IHistoryRepositoryService, HistoryRepositoryService>((service) => new(BuildHttpClient(builder)));

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseWebAssemblyDebugging();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.UseAntiforgery();

            app.MapStaticAssets();
            app.MapRazorComponents<App>()
                .AddInteractiveServerRenderMode()
                .AddInteractiveWebAssemblyRenderMode()
                .AddAdditionalAssemblies(typeof(Client._Imports).Assembly);

            app.Run();
        }

        private static HttpClient BuildHttpClient(WebApplicationBuilder builder)
        {
            return new HttpClient() { BaseAddress = new Uri(builder.Configuration["SuperVApiUrl"]!) };
        }
    }
}
