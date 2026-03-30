using TavernTrashers.Api.Common.Application.Messaging;
using TavernTrashers.Api.Common.Domain.Results;
using TavernTrashers.Api.Modules.Characters.Domain.Campaigns;

namespace TavernTrashers.Api.Modules.Characters.Application.Campaigns;

public sealed record UpdateCampaignReadModelCommand(Guid CampaignId, string Title) : ICommand;

internal sealed class UpdateCampaignReadModelCommandHandler(ICampaignReadModelRepository campaignReadModelRepository)
	: ICommandHandler<UpdateCampaignReadModelCommand>
{
	public async Task<Result> Handle(UpdateCampaignReadModelCommand command, CancellationToken cancellationToken)
	{
		var campaign = await campaignReadModelRepository.GetAsync(command.CampaignId, cancellationToken);
		if (campaign is null)
		{
			campaignReadModelRepository.Add(CampaignReadModel.Create(command.CampaignId, command.Title));
			return Result.Success();
		}

		campaign.UpdateTitle(command.Title);
		return Result.Success();
	}
}
