using TavernTrashers.Api.Common.Domain.Entities;
using TavernTrashers.Api.Common.Domain.Results;
using TavernTrashers.Api.Modules.Dice.Domain.AbstractSyntaxTree;
using TavernTrashers.Api.Modules.Dice.Domain.RecursiveDescentParser;

namespace TavernTrashers.Api.Modules.Dice.Domain.Rolls;

public sealed class Roll : Entity
{
	private readonly List<Roll> _children = [];
	private Roll() { }

	public Roll? Parent { get; private set; }
	public IReadOnlyCollection<Roll> Children => _children.AsReadOnly();

	public string Expression { get; private set; } = string.Empty;
	public Result<IExpressionNode> DiceExpression => new DiceParser(Expression).ParseExpression();
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

	public static Roll Reroll(Roll original, RollOutcome rollOutcome, DateTime rolledAtUtc)
	{
		var rollEntry = new Roll
		{
			Id          = Guid.NewGuid(),
			Parent      = original,
			Expression  = original.Expression,
			Total       = rollOutcome.Total,
			Minimum     = rollOutcome.Minimum,
			Maximum     = rollOutcome.Maximum,
			Average     = rollOutcome.Average,
			RawRolls    = rollOutcome.RawRolls,
			KeptRolls   = rollOutcome.KeptRolls,
			RolledAtUtc = rolledAtUtc,
			ContextJson = original.ContextJson,
		};

		return rollEntry;
	}
}