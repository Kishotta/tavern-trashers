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

public sealed record CreateCampaignCommand(string Title, string Description, Guid DungeonMasterUserId) : ICommand<CampaignResponse>;

internal sealed class CreateCampaignCommandValidator : AbstractValidator<CreateCampaignCommand>
{
    public CreateCampaignCommandValidator()
    {
        RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Description).MaximumLength(2000);
    }
}

internal sealed class CreateCampaignCommandHandler(ICampaignRepository campaignRepository)
    : ICommandHandler<CreateCampaignCommand, CampaignResponse>
{
    public async Task<Result<CampaignResponse>> Handle(
        CreateCampaignCommand command,
        CancellationToken cancellationToken) =>
        await Task.FromResult(
            Campaign.Create(command.Title, command.Description, command.DungeonMasterUserId)
               .Do(campaignRepository.Add)
               .Transform(campaign => (CampaignResponse)campaign));
}

internal sealed class CampaignCreatedDomainEventHandler(
    ISender sender,
    IEventBus eventBus)
    : DomainEventHandler<CampaignCreatedDomainEvent>
{
    public override async Task Handle(
        CampaignCreatedDomainEvent domainEvent,
        CancellationToken cancellationToken = default) =>
        await sender
           .Send(new GetCampaignQuery(domainEvent.CampaignId), cancellationToken)
           .EnsureSuccessAsync(error => new TavernTrashersException(nameof(GetCampaignQuery), error))
           .DoAsync(campaign => eventBus.PublishAsync(
                new CampaignCreatedIntegrationEvent(
                    domainEvent.Id,
                    domainEvent.OccurredAtUtc,
                    campaign.Id,
                    campaign.Title), cancellationToken));
}
