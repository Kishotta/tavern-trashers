using TavernTrashers.Api.Modules.Characters.Domain.Characters;
using TavernTrashers.Api.Modules.Characters.Domain.ClassLevels;
using TavernTrashers.Api.Modules.Characters.Domain.Resources;

namespace TavernTrashers.Api.Modules.Characters.Application.Characters;

public sealed record CharacterResponse(
	Guid Id,
	string Name,
	int Level,
	Guid OwnerId,
	Guid CampaignId,
	Conditions Conditions,
	HitPointsResponse HitPoints,
	DeathSavingThrowsResponse DeathSavingThrows,
	IReadOnlyCollection<GenericResourceResponse> GenericResources,
	IReadOnlyCollection<SpellSlotPoolResponse> SpellSlotPools)
{
	public static implicit operator CharacterResponse(Character character) =>
		new(
			character.Id,
			character.Name,
			character.Level,
			character.OwnerId,
			character.CampaignId,
			character.Conditions,
			character.HitPoints,
			character.DeathSavingThrows,
			character.GenericResources
			   .Where(r => r.MaxUses > 0)
			   .Select(r => (GenericResourceResponse)r)
			   .ToList()
			   .AsReadOnly(),
			character.SpellSlotPools
			   .Select(p => (SpellSlotPoolResponse)p)
			   .ToList()
			   .AsReadOnly());
}

public sealed record ClassLevelResponse(
	Guid Id,
	Guid CharacterClassId,
	string ClassName,
	int Level)
{
	public static implicit operator ClassLevelResponse(ClassLevel classLevel) =>
		new(
			classLevel.Id,
			classLevel.CharacterClassId,
			classLevel.CharacterClass?.Name ?? string.Empty,
			classLevel.Level);
}

public sealed record CharacterResourceResponse(
	Guid Id,
	Guid ResourceDefinitionId,
	string ResourceName,
	int CurrentAmount,
	int MaxAmount)
{
	public static implicit operator CharacterResourceResponse(CharacterResource resource) =>
		new(
			resource.Id,
			resource.ResourceDefinitionId,
			resource.ResourceDefinition?.Name ?? string.Empty,
			resource.CurrentAmount,
			resource.MaxAmount);
}

public sealed record GenericResourceResponse(
	Guid Id,
	string Name,
	int CurrentUses,
	int MaxUses,
	ResourceDirection Direction,
	SourceCategory SourceCategory,
	IReadOnlyCollection<ResetTrigger> ResetTriggers)
{
	public static implicit operator GenericResourceResponse(GenericResource resource) =>
		new(
			resource.Id,
			resource.Name,
			resource.CurrentUses,
			resource.MaxUses,
			resource.Direction,
			resource.SourceCategory,
			resource.GetResetTriggers());
}

public sealed record HitPointsResponse(
	Guid Id,
	int BaseMaxHitPoints,
	int CurrentHitPoints,
	int TemporaryHitPoints,
	int MaxHitPointReduction,
	int EffectiveMaxHitPoints)
{
	public static implicit operator HitPointsResponse(HitPoints hitPoints) =>
		new(
			hitPoints.Id,
			hitPoints.BaseMaxHitPoints,
			hitPoints.CurrentHitPoints,
			hitPoints.TemporaryHitPoints,
			hitPoints.MaxHitPointReduction,
			hitPoints.EffectiveMaxHitPoints);
}

public sealed record DeathSavingThrowsResponse(
	Guid Id,
	int Successes,
	int Failures)
{
	public static implicit operator DeathSavingThrowsResponse(DeathSavingThrows deathSavingThrows) =>
		new(
			deathSavingThrows.Id,
			deathSavingThrows.Successes,
			deathSavingThrows.Failures);
}

public sealed record SpellSlotLevelResponse(int Level, int CurrentUses, int MaxUses)
{
	public static implicit operator SpellSlotLevelResponse(SpellSlotLevel level) =>
		new(level.Level, level.CurrentUses, level.MaxUses);
}

public sealed record SpellSlotPoolResponse(
	Guid Id,
	SpellSlotPoolKind Kind,
	IReadOnlyCollection<SpellSlotLevelResponse> Levels)
{
	public static implicit operator SpellSlotPoolResponse(SpellSlotPool pool) =>
		new(
			pool.Id,
			pool.Kind,
			pool.Levels
			   .Where(l => l.MaxUses > 0)
			   .OrderBy(l => l.Level)
			   .Select(l => (SpellSlotLevelResponse)l)
			   .ToList()
			   .AsReadOnly());
}
