namespace ClientPricingSystem.Tests.Configuration;

// Attributes used for unit test discovery and execution
[AttributeUsage(AttributeTargets.Class)]
public class UnitTestCollectionAttribute : Attribute { }

[AttributeUsage(AttributeTargets.Method)]
public class UnitTestMethodAttribute : Attribute { }

// Attributes used for integration test discovery and execution
[AttributeUsage(AttributeTargets.Class)]
public class IntegrationTestCollectionAttribute : Attribute { }

[AttributeUsage(AttributeTargets.Method)]
public class IntegrationTestMethodAttribute : Attribute { }

// Attributes used for validation test discovery and execution
[AttributeUsage(AttributeTargets.Class)]
public class ValidationTestCollectionAttribute : Attribute { }

[AttributeUsage(AttributeTargets.Method)]
public class ValidationTestMethodAttribute : Attribute { }