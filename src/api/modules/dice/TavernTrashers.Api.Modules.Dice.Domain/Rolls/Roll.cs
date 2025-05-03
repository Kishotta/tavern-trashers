using TavernTrashers.Api.Common.Domain.Entities;
using TavernTrashers.Api.Common.Domain.Results;

namespace TavernTrashers.Api.Modules.Dice.Domain.Rolls;

public static class RollErrors
{
	public static Error NotFound(Guid rollId) =>
		Error.NotFound(
			"Rolls.NotFound",
			$"Roll with ID '{rollId}' not found.");
}

public sealed class Roll : Entity
{
	private Roll() { }
	public string Expression { get; private set; } = string.Empty;
	public int Total { get; private set; }
	public int Minimum { get; private set; }
	public int Maximum { get; private set; }
	public double Average { get; private set; }
	public DateTime RolledAtUtc { get; private set; }
	public string ContextJson { get; private set; } = "{}";

	public IReadOnlyList<int> RawRolls { get; private set; } = [];
	public IReadOnlyList<int> KeptRolls { get; private set; } = [];

	public static Roll Create(string expression, RollOutcome rollOutcome, DateTime rolledAtUtc, string contextJson)
	{
		var rollEntry = new Roll
		{
			Id          = Guid.NewGuid(),
			Expression  = expression,
			Total       = rollOutcome.Total,
			Minimum     = rollOutcome.Minimum,
			Maximum     = rollOutcome.Maximum,
			Average     = rollOutcome.Average,
			RawRolls    = rollOutcome.RawRolls,
			KeptRolls   = rollOutcome.KeptRolls,
			RolledAtUtc = rolledAtUtc,
			ContextJson = contextJson,
		};

		return rollEntry;
	}
}