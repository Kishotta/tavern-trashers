namespace TavernTrashers.Api.Modules.Characters.Domain.Resources;

internal static class DefaultGenericResourceFactory
{
	internal static IEnumerable<GenericResource> CreateDefaults(Guid characterId)
	{
		yield return GenericResource.Create(characterId, "Action",             1, ResourceDirection.Spending,      ResetTrigger.PerRound).Value;
		yield return GenericResource.Create(characterId, "Bonus Action",       1, ResourceDirection.Spending,      ResetTrigger.PerRound).Value;
		yield return GenericResource.Create(characterId, "Reaction",           1, ResourceDirection.Spending,      ResetTrigger.PerRound).Value;
		yield return GenericResource.Create(characterId, "Heroic Inspiration", 1, ResourceDirection.Spending,      ResetTrigger.PerRound | ResetTrigger.Manual).Value;
		yield return GenericResource.Create(characterId, "Exhaustion",         6, ResourceDirection.Accumulating,  ResetTrigger.LongRest).Value;
	}
}
