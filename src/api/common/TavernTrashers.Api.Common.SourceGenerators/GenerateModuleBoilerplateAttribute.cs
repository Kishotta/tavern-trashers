namespace TavernTrashers.Api.Common.SourceGenerators;

/// <summary>
///     Apply this attribute to a module class to automatically generate module-specific boilerplate classes (such as
///     IdempotentDomainEventHandler, IdempotentIntegrationEventHandler, ProcessOutboxJob, etc.).
/// </summary>
[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed class GenerateModuleBoilerplateAttribute(string moduleName, string moduleSchema) : Attribute
{
	public string ModuleName { get; } = moduleName;
	public string ModuleSchema { get; } = moduleSchema;
}