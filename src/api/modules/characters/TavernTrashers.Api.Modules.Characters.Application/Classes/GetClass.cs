using TavernTrashers.Api.Common.Application.Caching;
using TavernTrashers.Api.Common.Application.Messaging;
using TavernTrashers.Api.Common.Domain.Results;
using TavernTrashers.Api.Common.Domain.Results.Extensions;
using TavernTrashers.Api.Modules.Characters.Domain.Classes;

namespace TavernTrashers.Api.Modules.Characters.Application.Classes;

public sealed record GetClassQuery(Guid ClassId) : ICachingQuery<ClassResponse>
{
	public string CacheKey => $"classes:{ClassId}";
}

internal sealed class GetClassQueryHandler(IClassRepository classRepository)
	: IQueryHandler<GetClassQuery, ClassResponse>
{
	public async Task<Result<ClassResponse>> Handle(
		GetClassQuery query,
		CancellationToken cancellationToken) =>
		await classRepository
		   .GetAsync(query.ClassId, cancellationToken)
		   .TransformAsync(c => (ClassResponse)c);
}
