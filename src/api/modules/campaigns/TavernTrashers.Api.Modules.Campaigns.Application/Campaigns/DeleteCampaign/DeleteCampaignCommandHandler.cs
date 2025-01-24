using TavernTrashers.Api.Common.Application.Messaging;
using TavernTrashers.Api.Common.Domain.Results;
using TavernTrashers.Api.Modules.Campaigns.Application.Abstractions.Data;
using TavernTrashers.Api.Modules.Campaigns.Application.Campaigns.CreateCampaign;
using TavernTrashers.Api.Modules.Campaigns.Domain.Campaigns;

namespace TavernTrashers.Api.Modules.Campaigns.Application.Campaigns.DeleteCampaign;

internal sealed class DeleteCampaignCommandHandler(
	ICampaignRepository campaigns,
	IUnitOfWork unitOfWork) 
	: ICommandHandler<DeleteCampaignCommand, CampaignResponse>
{
	public async Task<Result<CampaignResponse>> Handle(DeleteCampaignCommand command, CancellationToken cancellationToken)
	{
		var campaign = await campaigns.GetAsync(command.CampaignId, cancellationToken);
		if (campaign.IsFailure)
			return campaign.Error;
		
		campaigns.Remove(campaign);

		await unitOfWork.SaveChangesAsync(cancellationToken);

		return (CampaignResponse)campaign.Value;
	}
}