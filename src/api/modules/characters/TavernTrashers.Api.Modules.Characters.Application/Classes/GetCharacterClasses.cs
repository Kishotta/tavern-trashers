using TavernTrashers.Api.Common.Application.Messaging;
using TavernTrashers.Api.Common.Domain.Results;
using TavernTrashers.Api.Common.Domain.Results.Extensions;
using TavernTrashers.Api.Modules.Characters.Domain.Classes;

namespace TavernTrashers.Api.Modules.Characters.Application.Classes;

public sealed record GetCharacterClassesQuery : IQuery<IReadOnlyCollection<CharacterClassResponse>>;

internal sealed class GetCharacterClassesQueryHandler(ICharacterClassRepository characterClassRepository)
	: IQueryHandler<GetCharacterClassesQuery, IReadOnlyCollection<CharacterClassResponse>>
{
	public async Task<Result<IReadOnlyCollection<CharacterClassResponse>>> Handle(
		GetCharacterClassesQuery query,
		CancellationToken cancellationToken) =>
		await characterClassRepository
		   .GetAllAsync(cancellationToken)
		   .TransformAsync(classes => classes
			   .Select(c => (CharacterClassResponse)c)
			   .ToList()
			   .AsReadOnly());
}
