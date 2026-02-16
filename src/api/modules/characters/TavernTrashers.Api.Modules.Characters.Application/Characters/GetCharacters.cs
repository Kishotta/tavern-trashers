using TavernTrashers.Api.Common.Application.Messaging;
using TavernTrashers.Api.Common.Domain.Results;
using TavernTrashers.Api.Common.Domain.Results.Extensions;
using TavernTrashers.Api.Modules.Characters.Domain.Characters;

namespace TavernTrashers.Api.Modules.Characters.Application.Characters;

public sealed record GetCharactersQuery : IQuery<IReadOnlyCollection<CharacterResponse>>;

internal sealed class GetCharactersQueryHandler(ICharacterRepository characterRepository)
	: IQueryHandler<GetCharactersQuery, IReadOnlyCollection<CharacterResponse>>
{
	public async Task<Result<IReadOnlyCollection<CharacterResponse>>> Handle(
		GetCharactersQuery query,
		CancellationToken cancellationToken) =>
		await characterRepository
		   .GetAllAsync(cancellationToken)
		   .TransformAsync(characters => characters
			   .Select(character => (CharacterResponse)character)
			   .ToList()
			   .AsReadOnly());
}
