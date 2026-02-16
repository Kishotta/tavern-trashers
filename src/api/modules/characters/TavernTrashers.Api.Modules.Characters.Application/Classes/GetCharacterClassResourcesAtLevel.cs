using TavernTrashers.Api.Common.Application.Caching;
using TavernTrashers.Api.Common.Application.Messaging;
using TavernTrashers.Api.Common.Domain.Results;
using TavernTrashers.Api.Common.Domain.Results.Extensions;
using TavernTrashers.Api.Modules.Characters.Domain.Classes;

namespace TavernTrashers.Api.Modules.Characters.Application.Classes;

public sealed record GetCharacterClassResourcesAtLevelResponse(
	Guid Id,
	string Name,
	IReadOnlyCollection<ResourceDefinitionCountResponse> ResourceDefinitions);

public sealed record ResourceDefinitionCountResponse(string Name, int Amount);

public sealed record GetCharacterClassResourcesAtLevelQuery(Guid ClassId, int Level) : ICachingQuery<GetCharacterClassResourcesAtLevelResponse>
{
	public string CacheKey => $"classes:{ClassId}:resources-at-level:{Level}";
	public TimeSpan CacheDuration => TimeSpan.FromMinutes(30);
}

internal sealed class GetCharacterClassResourcesAtLevelQueryHandler(ICharacterClassRepository characterClassRepository)
	: IQueryHandler<GetCharacterClassResourcesAtLevelQuery, GetCharacterClassResourcesAtLevelResponse>
{
	public async Task<Result<GetCharacterClassResourcesAtLevelResponse>> Handle(GetCharacterClassResourcesAtLevelQuery request, CancellationToken cancellationToken) =>
		await characterClassRepository
		   .GetAsync(request.ClassId, cancellationToken)
		   .TransformAsync(cc => new GetCharacterClassResourcesAtLevelResponse(
				cc.Id,
				cc.Name,
				cc.ResourceDefinitions
				   .Select(rd => new ResourceDefinitionCountResponse(rd.Name, rd.GetAmountAtLevel(request.Level)))
				   .ToList()));
}