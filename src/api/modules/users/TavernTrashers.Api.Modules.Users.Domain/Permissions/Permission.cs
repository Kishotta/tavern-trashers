namespace TavernTrashers.Api.Modules.Users.Domain.Permissions;


public sealed class Permission(string code)
{
	public static readonly Permission GetUser = new("users:read");
	public static readonly Permission ModifyUser = new("users:update");
	
	public static readonly Permission GetCampaign = new("campaigns:read");
	public static readonly Permission ModifyCampaign = new("campaigns:update");
    
	public string Code { get; } = code;
}