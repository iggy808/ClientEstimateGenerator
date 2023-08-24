using Fixie;
using System.Reflection;

namespace ClientPricingSystem.Tests.Configuration.Unit;
class UnitTestDiscovery : IDiscovery
{
    public IEnumerable<Type> TestClasses(IEnumerable<Type> concreteClasses)
       => concreteClasses.Where(x => x.Has<UnitTestCollectionAttribute>());

    public IEnumerable<MethodInfo> TestMethods(IEnumerable<MethodInfo> publicMethods)
       => publicMethods.Where(x => x.Has<UnitTestMethodAttribute>());
}