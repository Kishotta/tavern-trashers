using FluentValidation;
using MediatR;
using TavernTrashers.Api.Common.Application.EventBus;
using TavernTrashers.Api.Common.Application.Exceptions;
using TavernTrashers.Api.Common.Application.Messaging;
using TavernTrashers.Api.Common.Domain.Results;
using TavernTrashers.Api.Common.Domain.Results.Extensions;
using TavernTrashers.Api.Modules.Campaigns.Domain.Campaigns;
using TavernTrashers.Api.Modules.Campaigns.Domain.Campaigns.Events;
using TavernTrashers.Api.Modules.Campaigns.IntegrationEvents;

namespace TavernTrashers.Api.Modules.Campaigns.Application.Campaigns;

public sealed record UpdateCampaignCommand(Guid CampaignId, string Title, string Description) : ICommand<CampaignResponse>;

internal sealed class UpdateCampaignCommandValidator : AbstractValidator<UpdateCampaignCommand>
{
    public UpdateCampaignCommandValidator()
    {
        RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Description).MaximumLength(2000);
    }
}

internal sealed class UpdateCampaignCommandHandler(ICampaignRepository campaignRepository)
    : ICommandHandler<UpdateCampaignCommand, CampaignResponse>
{
    public async Task<Result<CampaignResponse>> Handle(
        UpdateCampaignCommand command,
        CancellationToken cancellationToken)
    {
        var campaignResult = await campaignRepository.GetAsync(command.CampaignId, cancellationToken);
        if (campaignResult.IsFailure)
            return campaignResult.Error;

        var updateResult = campaignResult.Value.Update(command.Title, command.Description);
        if (updateResult.IsFailure)
            return updateResult.Error;

        return (CampaignResponse)campaignResult.Value;
    }
}

internal sealed class CampaignUpdatedDomainEventHandler(
    ISender sender,
    IEventBus eventBus)
    : DomainEventHandler<CampaignUpdatedDomainEvent>
{
    public override async Task Handle(
        CampaignUpdatedDomainEvent domainEvent,
        CancellationToken cancellationToken = default) =>
        await sender
           .Send(new GetCampaignQuery(domainEvent.CampaignId), cancellationToken)
           .EnsureSuccessAsync(error => new TavernTrashersException(nameof(GetCampaignQuery), error))
           .DoAsync(campaign => eventBus.PublishAsync(
                new CampaignUpdatedIntegrationEvent(
                    domainEvent.Id,
                    domainEvent.OccurredAtUtc,
                    campaign.Id,
                    campaign.Title), cancellationToken));
}
