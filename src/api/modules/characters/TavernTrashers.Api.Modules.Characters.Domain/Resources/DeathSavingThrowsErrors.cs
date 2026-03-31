using TavernTrashers.Api.Common.Domain.Results;

namespace TavernTrashers.Api.Modules.Characters.Domain.Resources;

public static class DeathSavingThrowsErrors
{
	public static Error AlreadyAtMaxSuccesses() =>
		Error.Validation(
			"DeathSavingThrows.AlreadyAtMaxSuccesses",
			"Death saving throw successes are already at the maximum of 3.");

	public static Error AlreadyAtMaxFailures() =>
		Error.Validation(
			"DeathSavingThrows.AlreadyAtMaxFailures",
			"Death saving throw failures are already at the maximum of 3.");
}
