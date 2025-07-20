namespace TavernTrashers.Api.Modules.Dice.Domain.Rolls;

public record RollOutcome(
	int Total,
	int Minimum,
	int Maximum,
	double Average,
	IReadOnlyList<DieResult> RawRolls,
	IReadOnlyList<DieResult> KeptRolls)
{
	public static RollOutcome Constant(int value) =>
		new(value,
			value,
			value,
			value,
			[],
			[]);
}