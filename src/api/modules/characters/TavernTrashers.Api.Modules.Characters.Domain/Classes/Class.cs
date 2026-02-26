using TavernTrashers.Api.Common.Domain.Entities;
using TavernTrashers.Api.Common.Domain.Results;

namespace TavernTrashers.Api.Modules.Characters.Domain.Classes;

public sealed class Class : Entity
{
	private Class() { }

	public string Name { get; private set; } = string.Empty;
	public DateTime? DeletedAt { get; private set; }
	public bool IsDeleted => DeletedAt.HasValue;

	public static Result<Class> Create(string name)
	{
		if (string.IsNullOrWhiteSpace(name))
			return ClassErrors.InvalidName();

		return new Class
		{
			Id   = Guid.NewGuid(),
			Name = name.Trim(),
		};
	}

	public Result Rename(string name)
	{
		if (string.IsNullOrWhiteSpace(name))
			return ClassErrors.InvalidName();

		Name = name.Trim();
		return Result.Success();
	}

	public void Delete() => DeletedAt = DateTime.UtcNow;
}
