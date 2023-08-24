using Fixie;

namespace ClientPricingSystem.Tests.Configuration.Unit;
class UnitTestExecution : IExecution
{
    public async Task Run(TestSuite testSuite)
    {
        foreach (var testClass in testSuite.TestClasses)
        {
            foreach (var test in testClass.Tests)
            {
                await test.Run();
            }
        }
    }
}