using TavernTrashers.Api.Common.Application.Messaging;
using TavernTrashers.Api.Common.Domain.Results;
using TavernTrashers.Api.Common.Domain.Results.Extensions;
using TavernTrashers.Api.Modules.Campaigns.Application.Abstractions.Data;
using TavernTrashers.Api.Modules.Campaigns.Application.Campaigns.CreateCampaign;
using TavernTrashers.Api.Modules.Campaigns.Domain.Campaigns;

namespace TavernTrashers.Api.Modules.Campaigns.Application.Campaigns.DeleteCampaign;

internal sealed class DeleteCampaignCommandHandler(
	ICampaignRepository campaigns,
	IUnitOfWork unitOfWork) 
	: ICommandHandler<DeleteCampaignCommand, CampaignResponse>
{
	public async Task<Result<CampaignResponse>> Handle(DeleteCampaignCommand command, CancellationToken cancellationToken) =>
		await campaigns
		   .GetAsync(command.CampaignId, cancellationToken)
		   .DoAsync(campaigns.Remove)
		   .DoAsync(async _ => await unitOfWork.SaveChangesAsync(cancellationToken))
		   .TransformAsync(campaign => (CampaignResponse)campaign);
}