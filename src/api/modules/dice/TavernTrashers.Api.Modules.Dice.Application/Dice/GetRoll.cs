using TavernTrashers.Api.Common.Application.Messaging;
using TavernTrashers.Api.Common.Domain.Results;
using TavernTrashers.Api.Common.Domain.Results.Extensions;
using TavernTrashers.Api.Modules.Dice.Domain.Rolls;

namespace TavernTrashers.Api.Modules.Dice.Application.Dice;

public sealed record GetRollQuery(Guid RollId)
	: IQuery<RollResponse>;

internal sealed class GetRollQueryHandler(IRollRepository rollRepository) : IQueryHandler<GetRollQuery, RollResponse>
{
	public async Task<Result<RollResponse>> Handle(GetRollQuery query, CancellationToken cancellationToken) =>
		await rollRepository.GetAsync(query.RollId, cancellationToken)
		   .TransformAsync(roll => (RollResponse)roll);
}