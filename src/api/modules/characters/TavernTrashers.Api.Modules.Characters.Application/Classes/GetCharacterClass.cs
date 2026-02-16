using TavernTrashers.Api.Common.Application.Messaging;
using TavernTrashers.Api.Common.Domain.Results;
using TavernTrashers.Api.Common.Domain.Results.Extensions;
using TavernTrashers.Api.Modules.Characters.Domain.Classes;

namespace TavernTrashers.Api.Modules.Characters.Application.Classes;

public sealed record GetCharacterClassQuery(Guid ClassId) : IQuery<CharacterClassResponse>;

internal sealed class GetCharacterClassQueryHandler(ICharacterClassRepository characterClassRepository)
	: IQueryHandler<GetCharacterClassQuery, CharacterClassResponse>
{
	public async Task<Result<CharacterClassResponse>> Handle(
		GetCharacterClassQuery query,
		CancellationToken cancellationToken) =>
		await characterClassRepository
		   .GetAsync(query.ClassId, cancellationToken)
		   .TransformAsync(characterClass => (CharacterClassResponse)characterClass);
}
