using TavernTrashers.Api.Common.Application.Caching;
using TavernTrashers.Api.Common.Application.Messaging;
using TavernTrashers.Api.Common.Domain.Results;
using TavernTrashers.Api.Common.Domain.Results.Extensions;
using TavernTrashers.Api.Modules.Characters.Domain.Classes;

namespace TavernTrashers.Api.Modules.Characters.Application.Classes;

public sealed record GetCharacterClassQuery(Guid ClassId) : ICachingQuery<CharacterClassResponse>
{
	public string CacheKey => $"classes:{ClassId}";
	public TimeSpan CacheDuration => TimeSpan.FromMinutes(30);
	public CacheExpirationType CacheExpirationType => CacheExpirationType.Absolute;
}

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