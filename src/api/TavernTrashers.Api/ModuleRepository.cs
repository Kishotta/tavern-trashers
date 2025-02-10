using TavernTrashers.Api.Common.Infrastructure;
using TavernTrashers.Api.Common.Infrastructure.Modules;

namespace TavernTrashers.Api;

public static class ModuleRepository
{
	public static readonly IEnumerable<IModule> Modules = [
		new Modules.Campaigns.Infrastructure.CampaignsModule(),
	];
}