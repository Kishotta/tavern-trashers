namespace TavernTrashers.Api.Common.Domain.Auditing;

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
public sealed class NotAuditableAttribute : Attribute;