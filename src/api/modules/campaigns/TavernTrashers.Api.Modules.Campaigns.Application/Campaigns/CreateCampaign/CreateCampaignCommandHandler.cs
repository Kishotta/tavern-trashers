using TavernTrashers.Api.Common.Application.Messaging;
using TavernTrashers.Api.Common.Domain.Results;
using TavernTrashers.Api.Modules.Campaigns.Application.Abstractions.Data;
using TavernTrashers.Api.Modules.Campaigns.Domain.Campaigns;

namespace TavernTrashers.Api.Modules.Campaigns.Application.Campaigns.CreateCampaign;

internal sealed class CreateCampaignCommandHandler(
	ICampaignRepository campaigns,
	IUnitOfWork unitOfWork) 
	: ICommandHandler<CreateCampaignCommand, CampaignResponse>
{
	public async Task<Result<CampaignResponse>> Handle(CreateCampaignCommand command, CancellationToken cancellationToken)
	{
		var campaign = Campaign.Create(command.Name, command.Description);
		
		campaigns.Add(campaign);

		await unitOfWork.SaveChangesAsync(cancellationToken);

		return (CampaignResponse)campaign;
	}
}