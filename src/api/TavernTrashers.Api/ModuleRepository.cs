using TavernTrashers.Api.Common.Infrastructure;

namespace TavernTrashers.Api;

public static class ModuleRepository
{
	public static readonly IEnumerable<IModule> Modules = [
		new Modules.Campaigns.Infrastructure.CampaignsModule(),
	];
}