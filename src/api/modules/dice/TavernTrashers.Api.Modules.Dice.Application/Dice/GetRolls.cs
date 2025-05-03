using TavernTrashers.Api.Common.Application.Messaging;
using TavernTrashers.Api.Common.Domain.Results;
using TavernTrashers.Api.Common.Domain.Results.Extensions;
using TavernTrashers.Api.Modules.Dice.Domain.Rolls;

namespace TavernTrashers.Api.Modules.Dice.Application.Dice;

public sealed record GetRollsQuery : IQuery<IReadOnlyCollection<RollResponse>>;

internal sealed class GetRollsQueryHandler(IRollRepository rollRepository)
	: IQueryHandler<GetRollsQuery, IReadOnlyCollection<RollResponse>>
{
	public async Task<Result<IReadOnlyCollection<RollResponse>>> Handle(
		GetRollsQuery query,
		CancellationToken cancellationToken) =>
		await rollRepository
		   .GetReadOnlyAsync(cancellationToken)
		   .TransformAsync(rolls => rolls
			   .Select(roll => (RollResponse)roll)
			   .ToList()
			   .AsReadOnly());
}