using Fixie;

namespace ClientPricingSystem.Tests.Configuration.Validation;
class ValidationTestExecution : IExecution
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