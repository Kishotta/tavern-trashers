using TavernTrashers.Api.Common.Infrastructure.Modules;
using TavernTrashers.Api.Modules.Campaigns.Infrastructure;
using TavernTrashers.Api.Modules.Users.Infrastructure;

namespace TavernTrashers.Api;

public static class ModuleRepository
{
	public static readonly IEnumerable<IModule> Modules = [
		new CampaignsModule(),
		new UsersModule()
	];
}