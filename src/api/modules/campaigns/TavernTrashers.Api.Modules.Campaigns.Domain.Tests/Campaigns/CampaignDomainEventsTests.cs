using Shouldly;
using TavernTrashers.Api.Common.Domain.Tests;
using TavernTrashers.Api.Modules.Campaigns.Domain.Campaigns;
using TavernTrashers.Api.Modules.Campaigns.Domain.Campaigns.DomainEvents;

namespace TavernTrashers.Api.Modules.Campaigns.Domain.Tests.Campaigns;

public class CampaignDomainEventsTests : TestBase
{
	[Fact]
	public void Create_ShouldRaise_CampaignCreatedDomainEvent()
	{
		// Arrange
		const string title       = "Test Campaign";
		const string description = "Test Description";

		// Act
		var result   = Campaign.Create(title, description);
		var campaign = result.Value;

		// Assert
		var createdDomainEvent = AssertDomainEventWasPublished<CampaignCreatedDomainEvent>(campaign);
		createdDomainEvent.CampaignId.ShouldBe(campaign.Id);
	}

	[Fact]
	public void UpdateDetails_ShouldRaise_CampaignDetailsUpdatedDomainEvent()
	{
		// Arrange
		var          campaign       = Campaign.Create("Test", "Desc").Value;
		var          playerId       = Guid.NewGuid();
		const string newTitle       = "Updated Title";
		const string newDescription = "Updated Description";

		// Act
		campaign.UpdateDetails(playerId, newTitle, newDescription);

		// Assert
		var createdDomainEvent = AssertDomainEventWasPublished<CampaignCreatedDomainEvent>(campaign);
		createdDomainEvent.CampaignId.ShouldBe(campaign.Id);
	}
}