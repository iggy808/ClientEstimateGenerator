using Fixie;
using System.Reflection;

namespace ClientPricingSystem.Tests.Configuration.Integration;
class IntegrationTestDiscovery : IDiscovery
{
    public IEnumerable<Type> TestClasses(IEnumerable<Type> concreteClasses)
       => concreteClasses.Where(x => x.Has<IntegrationTestCollectionAttribute>());

    public IEnumerable<MethodInfo> TestMethods(IEnumerable<MethodInfo> publicMethods)
       => publicMethods.Where(x => x.Has<IntegrationTestMethodAttribute>());
}