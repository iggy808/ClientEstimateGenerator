using Fixie;

namespace ClientPricingSystem.Tests.Configuration;
public class TestProject : ITestProject
{
    public void Configure(TestConfiguration configuration, TestEnvironment environment)
    {
        configuration.Conventions.Add<TestDiscovery, TestExecution>();
    }
}

