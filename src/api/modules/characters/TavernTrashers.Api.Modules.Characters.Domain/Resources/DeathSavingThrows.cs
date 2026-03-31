using TavernTrashers.Api.Common.Domain.Entities;
using TavernTrashers.Api.Common.Domain.Results;
using TavernTrashers.Api.Modules.Characters.Domain.Characters.Events;

namespace TavernTrashers.Api.Modules.Characters.Domain.Resources;

public sealed class DeathSavingThrows : Entity
{
	private const int MaxCount = 3;

	private DeathSavingThrows() { }

	public Guid CharacterId { get; private set; }
	public int Successes { get; private set; }
	public int Failures { get; private set; }

	public static DeathSavingThrows CreateDefault(Guid characterId) =>
		new()
		{
			Id          = Guid.NewGuid(),
			CharacterId = characterId,
			Successes   = 0,
			Failures    = 0,
		};

	public Result RecordSuccess()
	{
		if (Successes >= MaxCount)
			return DeathSavingThrowsErrors.AlreadyAtMaxSuccesses();

		Successes++;

		if (Successes == MaxCount)
			RaiseDomainEvent(new CharacterStabilizedDomainEvent(CharacterId));

		return Result.Success();
	}

	public Result RecordFailure()
	{
		if (Failures >= MaxCount)
			return DeathSavingThrowsErrors.AlreadyAtMaxFailures();

		Failures++;

		if (Failures == MaxCount)
			RaiseDomainEvent(new CharacterDiedDomainEvent(CharacterId));

		return Result.Success();
	}

	public void Reset()
	{
		Successes = 0;
		Failures  = 0;
	}
}
