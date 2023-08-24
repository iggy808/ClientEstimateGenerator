using ClientPricingSystem.Tests.Configuration;
using Fixie;
using System.Reflection;

class TestDiscovery : IDiscovery
{
    public IEnumerable<Type> TestClasses(IEnumerable<Type> concreteClasses)
       => concreteClasses.Where(x => x.Has<TestCollectionAttribute>());

    public IEnumerable<MethodInfo> TestMethods(IEnumerable<MethodInfo> publicMethods)
       => publicMethods.Where(x => x.Has<TestMethodAttribute>());
}