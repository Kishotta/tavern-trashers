using TavernTrashers.Api.Common.Domain.Entities;
using TavernTrashers.Api.Common.Domain.Results;

namespace TavernTrashers.Api.Modules.Characters.Domain.Resources;

public sealed class GenericResource : Entity
{
	private GenericResource() { }

	public Guid CharacterId { get; private set; }
	public string Name { get; private set; } = string.Empty;
	public string? SourceCategory { get; private set; }
	public int MaxAmount { get; private set; }
	public int CurrentAmount { get; private set; }
	public ResourceDirection Direction { get; private set; }
	public ResetTrigger ResetTriggers { get; private set; }

	public Result Use()
	{
		if (CurrentAmount <= 0)
			return GenericResourceErrors.InsufficientResources(Id, CurrentAmount);

		CurrentAmount -= 1;
		return Result.Success();
	}

	public Result Apply()
	{
		if (MaxAmount > 0 && CurrentAmount >= MaxAmount)
			return GenericResourceErrors.ResourceExceeded(Id, CurrentAmount, MaxAmount);

		CurrentAmount += 1;
		return Result.Success();
	}

	public void Restore()
	{
		CurrentAmount = Direction == ResourceDirection.Spending ? MaxAmount : 0;
	}

	public static Result<GenericResource> Create(
		Guid characterId,
		string name,
		int maxAmount,
		ResourceDirection direction,
		ResetTrigger resetTriggers,
		string? sourceCategory = null)
	{
		if (string.IsNullOrWhiteSpace(name))
			return GenericResourceErrors.InvalidName();

		if (maxAmount < 0)
			return GenericResourceErrors.InvalidMaxAmount();

		var initialAmount = direction == ResourceDirection.Spending ? maxAmount : 0;

		return new GenericResource
		{
			Id             = Guid.NewGuid(),
			CharacterId    = characterId,
			Name           = name.Trim(),
			MaxAmount      = maxAmount,
			CurrentAmount  = initialAmount,
			Direction      = direction,
			ResetTriggers  = resetTriggers,
			SourceCategory = sourceCategory,
		};
	}
}
