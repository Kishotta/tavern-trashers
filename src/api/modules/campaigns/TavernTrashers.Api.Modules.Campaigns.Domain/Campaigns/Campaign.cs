using TavernTrashers.Api.Common.Domain.Auditing;
using TavernTrashers.Api.Common.Domain.Entities;
using TavernTrashers.Api.Common.Domain.Results;
using TavernTrashers.Api.Modules.Campaigns.Domain.Campaigns.Events;

namespace TavernTrashers.Api.Modules.Campaigns.Domain.Campaigns;

[Auditable]
public sealed class Campaign : Entity
{
    private Campaign() { }

    public string Title { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public Guid DmUserId { get; private set; }

    public static Result<Campaign> Create(string title, string description, Guid dmUserId)
    {
        if (string.IsNullOrWhiteSpace(title))
            return CampaignErrors.InvalidTitle();

        var campaign = new Campaign
        {
            Id = Guid.NewGuid(),
            Title = title.Trim(),
            Description = description.Trim(),
            DmUserId = dmUserId,
        };

        campaign.RaiseDomainEvent(new CampaignCreatedDomainEvent(campaign.Id, campaign.Title));

        return campaign;
    }

    public Result Update(string title, string description)
    {
        if (string.IsNullOrWhiteSpace(title))
            return CampaignErrors.InvalidTitle();

        Title = title.Trim();
        Description = description.Trim();

        RaiseDomainEvent(new CampaignUpdatedDomainEvent(Id, Title));

        return Result.Success();
    }
}
