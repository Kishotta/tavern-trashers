namespace TavernTrashers.Api.Modules.Characters.Domain.Resources;

public sealed class ResourceAllowance
{
	private ResourceAllowance() { }

	public int Level { get; private set; }
	public int Amount { get; private set; }

	internal static ResourceAllowance Create(int level, int amount) =>
		new()
		{
			Level = level,
			Amount = amount,
		};
}
