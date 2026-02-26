using TavernTrashers.Api.Common.Application.Caching;
using TavernTrashers.Api.Common.Application.Messaging;
using TavernTrashers.Api.Common.Domain.Results;
using TavernTrashers.Api.Common.Domain.Results.Extensions;
using TavernTrashers.Api.Modules.Characters.Domain.Classes;

namespace TavernTrashers.Api.Modules.Characters.Application.Classes;

public sealed record GetClassesQuery : ICachingQuery<IReadOnlyCollection<ClassResponse>>
{
	public string CacheKey => "classes";
	public CacheExpirationType CacheExpirationType => CacheExpirationType.Absolute;
}

internal sealed class GetClassesQueryHandler(IClassRepository classRepository)
	: IQueryHandler<GetClassesQuery, IReadOnlyCollection<ClassResponse>>
{
	public async Task<Result<IReadOnlyCollection<ClassResponse>>> Handle(
		GetClassesQuery query,
		CancellationToken cancellationToken) =>
		await classRepository
		   .GetAllAsync(cancellationToken)
		   .TransformAsync(classes => classes
			   .Select(c => (ClassResponse)c)
			   .ToList()
			   .AsReadOnly());
}
