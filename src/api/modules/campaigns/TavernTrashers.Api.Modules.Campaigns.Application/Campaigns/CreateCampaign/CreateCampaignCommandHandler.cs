using TavernTrashers.Api.Common.Application.Messaging;
using TavernTrashers.Api.Common.Domain.Results;
using TavernTrashers.Api.Common.Domain.Results.Extensions;
using TavernTrashers.Api.Modules.Campaigns.Application.Abstractions.Data;
using TavernTrashers.Api.Modules.Campaigns.Domain.Campaigns;

namespace TavernTrashers.Api.Modules.Campaigns.Application.Campaigns.CreateCampaign;

internal sealed class CreateCampaignCommandHandler(
	ICampaignRepository campaigns,
	IUnitOfWork unitOfWork) 
	: ICommandHandler<CreateCampaignCommand, CampaignResponse>
{
	public async Task<Result<CampaignResponse>> Handle(CreateCampaignCommand command, CancellationToken cancellationToken) =>
		await Campaign
		   .Create(command.Name, command.Description)
		   .Do(campaigns.Add)
		   .DoAsync(async _ => await unitOfWork.SaveChangesAsync(cancellationToken))
		   .TransformAsync(campaign => (CampaignResponse)campaign);
}