namespace TavernTrashers.Api.Modules.Characters.Domain.Resources;

public static class DefaultResourceFactory
{
	public static IEnumerable<GenericResource> CreateDefaultResources(Guid characterId)
	{
		yield return GenericResource.Create(
			characterId,
			"Action",
			maxUses: 1,
			ResourceDirection.Spending,
			SourceCategory.Core,
			[ResetTrigger.PerRound]).Value;

		yield return GenericResource.Create(
			characterId,
			"Bonus Action",
			maxUses: 1,
			ResourceDirection.Spending,
			SourceCategory.Core,
			[ResetTrigger.PerRound]).Value;

		yield return GenericResource.Create(
			characterId,
			"Reaction",
			maxUses: 1,
			ResourceDirection.Spending,
			SourceCategory.Core,
			[ResetTrigger.PerRound]).Value;

		yield return GenericResource.Create(
			characterId,
			"Heroic Inspiration",
			maxUses: 1,
			ResourceDirection.Spending,
			SourceCategory.Core,
			[ResetTrigger.Manual]).Value;

		yield return GenericResource.Create(
			characterId,
			"Exhaustion",
			maxUses: 6,
			ResourceDirection.Accumulating,
			SourceCategory.Core,
			[ResetTrigger.LongRest]).Value;
	}
}
