using Microsoft.AspNetCore.Mvc.Testing;
using pva.SuperV.Api;

[assembly: DoNotParallelize]
namespace pva.SuperV.TestsScenarios
{
    public class TestProjectApplication : WebApplicationFactory<WebApiProgram>;
}
