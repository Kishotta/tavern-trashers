using TavernTrashers.Api.Modules.Characters.Domain.Classes;

namespace TavernTrashers.Api.Modules.Characters.Application.Classes;

public sealed record ClassResponse(Guid Id, string Name)
{
	public static implicit operator ClassResponse(Class @class) =>
		new(@class.Id, @class.Name);
}
