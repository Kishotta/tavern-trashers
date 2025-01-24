namespace TavernTrashers.Api.Modules.Campaigns.Domain.Campaigns;

public record CampaignId(Guid Value)
{
	public static CampaignId Create() => new(Guid.NewGuid());
	
	public static implicit operator Guid(CampaignId self) => self.Value;
	
	public static implicit operator CampaignId(Guid value) => new(value);
}