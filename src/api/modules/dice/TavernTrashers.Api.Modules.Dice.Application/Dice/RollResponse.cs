using TavernTrashers.Api.Modules.Dice.Domain.Rolls;

namespace TavernTrashers.Api.Modules.Dice.Application.Dice;

public sealed record RollResponse(
	Guid Id,
	string Expression,
	int Total,
	int Minimum,
	int Maximum,
	double Average,
	IReadOnlyList<int> RawRolls,
	IReadOnlyList<int> KeptRolls,
	DateTime RolledAtUtc)
{
	public IReadOnlyCollection<RollResponse> Children { get; init; } = new List<RollResponse>();

	public static implicit operator RollResponse(Roll roll) =>
		new(
			roll.Id,
			roll.Expression,
			roll.Total,
			roll.Minimum,
			roll.Maximum,
			roll.Average,
			roll.RawRolls,
			roll.KeptRolls,
			roll.RolledAtUtc)
		{
			Children = roll.Children.Select(c => (RollResponse)c).ToList().AsReadOnly(),
		};
}