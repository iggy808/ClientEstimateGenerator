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
                var instance = testClass.Construct();

                var method = testClass.Type.GetMethod("Setup");
                if (method != null)
                    await method.Call(instance);

                await test.Run(instance);
            }
        }
    }
}