using TavernTrashers.Api.Common.Application.Messaging;
using TavernTrashers.Api.Common.Domain.Results;
using TavernTrashers.Api.Common.Domain.Results.Extensions;
using TavernTrashers.Api.Modules.Characters.Domain.Characters;

namespace TavernTrashers.Api.Modules.Characters.Application.Characters;

public sealed record GetCharacterQuery(Guid CharacterId) : IQuery<CharacterResponse>;

internal sealed class GetCharacterQueryHandler(ICharacterRepository characterRepository)
	: IQueryHandler<GetCharacterQuery, CharacterResponse>
{
	public async Task<Result<CharacterResponse>> Handle(
		GetCharacterQuery query,
		CancellationToken cancellationToken) =>
		await characterRepository
		   .GetAsync(query.CharacterId, cancellationToken)
		   .TransformAsync(character => (CharacterResponse)character);
}
