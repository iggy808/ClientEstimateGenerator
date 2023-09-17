using Fixie;
using System.Reflection;

namespace ClientPricingSystem.Tests.Configuration.Validation;
class ValidationTestDiscovery : IDiscovery
{
    public IEnumerable<Type> TestClasses(IEnumerable<Type> concreteClasses)
       => concreteClasses.Where(x => x.Has<ValidationTestCollectionAttribute>());

    public IEnumerable<MethodInfo> TestMethods(IEnumerable<MethodInfo> publicMethods)
       => publicMethods.Where(x => x.Has<ValidationTestMethodAttribute>());
}