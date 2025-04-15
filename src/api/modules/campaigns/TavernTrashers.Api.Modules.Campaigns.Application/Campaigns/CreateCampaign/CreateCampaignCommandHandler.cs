using TavernTrashers.Api.Common.Application.Messaging;
using TavernTrashers.Api.Common.Domain.Results;
using TavernTrashers.Api.Common.Domain.Results.Extensions;
using TavernTrashers.Api.Modules.Campaigns.Application.Abstractions.Data;
using TavernTrashers.Api.Modules.Campaigns.Domain.Campaigns;
using TavernTrashers.Api.Modules.Campaigns.Domain.Players;

namespace TavernTrashers.Api.Modules.Campaigns.Application.Campaigns.CreateCampaign;

internal sealed class CreateCampaignCommandHandler(
	IPlayerRepository playerRepository,
	ICampaignRepository campaignRepository,
	IUnitOfWork unitOfWork)
	: ICommandHandler<CreateCampaignCommand, CampaignResponse>
{
	public async Task<Result<CampaignResponse>> Handle(
		CreateCampaignCommand command,
		CancellationToken cancellationToken) =>
		await playerRepository
		   .GetAsync(command.DungeonMasterId, cancellationToken)
		   .ThenAsync(dungeonMaster => Campaign.Create(dungeonMaster, command.Name, command.Description, string.Empty))
		   .DoAsync(campaignRepository.Add)
		   .DoAsync(_ => unitOfWork.SaveChangesAsync(cancellationToken))
		   .TransformAsync(campaign => (CampaignResponse)campaign);
}