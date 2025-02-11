namespace TavernTrashers.Api.Modules.Users.Domain.Permissions;

public sealed class Role
{
	public string Name { get; private set; }
    
	public static readonly Role Administrator = new("Administrator");
	public static readonly Role DungeonMaster = new("Dungeon Master");
	public static readonly Role Player = new("Player");

	private Role(string name)
	{
		Name = name;
	}
}