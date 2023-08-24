using Fixie;
using ClientPricingSystem.Tests.Configuration.Unit;
using ClientPricingSystem.Tests.Configuration.Integration;

namespace ClientPricingSystem.Tests.Configuration;
public class TestProject : ITestProject
{
    public void Configure(TestConfiguration configuration, TestEnvironment environment)
    {
        configuration.Conventions.Add<UnitTestDiscovery, UnitTestExecution>();
        configuration.Conventions.Add<IntegrationTestDiscovery, IntegrationTestExecution>();
    }
}

