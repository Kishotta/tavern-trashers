using TavernTrashers.Api.Common.Application.Messaging;
using TavernTrashers.Api.Common.Domain.Results;
using TavernTrashers.Api.Modules.Characters.Domain.Campaigns;

namespace TavernTrashers.Api.Modules.Characters.Application.Campaigns;

public sealed record CreateCampaignReadModelCommand(Guid CampaignId, string Title) : ICommand;

internal sealed class CreateCampaignReadModelCommandHandler(ICampaignReadModelRepository campaignReadModelRepository)
	: ICommandHandler<CreateCampaignReadModelCommand>
{
	public async Task<Result> Handle(CreateCampaignReadModelCommand command, CancellationToken cancellationToken)
	{
		var existing = await campaignReadModelRepository.GetAsync(command.CampaignId, cancellationToken);
		if (existing.IsSuccess)
			return Result.Success();

		var campaign = CampaignReadModel.Create(command.CampaignId, command.Title);
		campaignReadModelRepository.Add(campaign);
		return Result.Success();
	}
}
