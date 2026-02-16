using TavernTrashers.Api.Common.Domain.Entities;
using TavernTrashers.Api.Common.Domain.Results;

namespace TavernTrashers.Api.Modules.Characters.Domain.Resources;

public sealed class CharacterResource : Entity
{
	private CharacterResource() { }

	public Guid CharacterId { get; private set; }
	public Guid ResourceDefinitionId { get; private set; }
	public ResourceDefinition ResourceDefinition { get; private set; } = null!;
	public int CurrentAmount { get; private set; }
	public int MaxAmount { get; private set; }

	public Result Spend(int amount)
	{
		if (amount <= 0)
			return CharacterResourceErrors.InvalidAmount();

		if (CurrentAmount < amount)
			return CharacterResourceErrors.InsufficientResources(ResourceDefinitionId, CurrentAmount, amount);

		CurrentAmount -= amount;
		return Result.Success();
	}

	public Result Restore(int amount)
	{
		if (amount <= 0)
			return CharacterResourceErrors.InvalidAmount();

		CurrentAmount = Math.Min(CurrentAmount + amount, MaxAmount);
		return Result.Success();
	}

	internal void SetMax(int max)
	{
		MaxAmount     = max;
		CurrentAmount = Math.Min(CurrentAmount, MaxAmount);
	}

	public static CharacterResource Create(Guid characterId, Guid resourceDefinitionId, int maxAmount) =>
		new()
		{
			Id                   = Guid.NewGuid(),
			CharacterId          = characterId,
			ResourceDefinitionId = resourceDefinitionId,
			CurrentAmount        = maxAmount,
			MaxAmount            = maxAmount,
		};
}