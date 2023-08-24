using Fixie;
using MongoDB.Driver;

namespace ClientPricingSystem.Tests.Configuration.Integration;
class IntegrationTestExecution : IExecution
{
    public async Task Run(TestSuite testSuite)
    {
        IMongoClient mongoClient = new MongoClient(TestDatabase.DefaultConnectionString);
        IMongoDatabase context = mongoClient.GetDatabase(TestDatabase.DatabaseName);

        foreach (var testClass in testSuite.TestClasses)
        {
            var instance = testClass.Construct(context);
            foreach (var test in testClass.Tests)
            {
                await test.Run(instance);
            }
        }
    }
}