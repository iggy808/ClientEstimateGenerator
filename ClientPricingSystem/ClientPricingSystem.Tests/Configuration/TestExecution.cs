using Fixie;

class TestExecution : IExecution
{
    public async Task Run(TestSuite testSuite)
    {
        // Iterate over the test *classes* explicitly.
        foreach (var testClass in testSuite.TestClasses)
        {
            // Construct a shared instance once per test class.
            //var instance = testClass.Construct();

            foreach (var test in testClass.Tests)
            {
                // Provide the shared instance.
                //await test.Run(instance);
                await test.Run();
            }
        }
    }
}