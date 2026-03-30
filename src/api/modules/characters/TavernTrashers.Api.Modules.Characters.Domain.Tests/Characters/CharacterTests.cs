using TavernTrashers.Api.Modules.Characters.Domain.Characters;

namespace TavernTrashers.Api.Modules.Characters.Domain.Tests.Characters;

public class CharacterTests
{
	private static readonly Guid OwnerId = Guid.NewGuid();
	private static readonly Guid CampaignId = Guid.NewGuid();

	[Fact]
	public void Create_WithValidData_Succeeds()
	{
		var result = Character.Create("Gandalf", 10, OwnerId, CampaignId);

		Assert.True(result.IsSuccess);
		Assert.Equal("Gandalf", result.Value.Name);
		Assert.Equal(10, result.Value.Level);
		Assert.Equal(OwnerId, result.Value.OwnerId);
		Assert.Equal(CampaignId, result.Value.CampaignId);
	}

	[Theory]
	[InlineData("")]
	[InlineData("   ")]
	public void Create_WithEmptyName_Fails(string name)
	{
		var result = Character.Create(name, 1, OwnerId, CampaignId);

		Assert.True(result.IsFailure);
	}

	[Theory]
	[InlineData(0)]
	[InlineData(-1)]
	[InlineData(21)]
	[InlineData(100)]
	public void Create_WithInvalidLevel_Fails(int level)
	{
		var result = Character.Create("Aragorn", level, OwnerId, CampaignId);

		Assert.True(result.IsFailure);
	}

	[Theory]
	[InlineData(1)]
	[InlineData(10)]
	[InlineData(20)]
	public void Create_WithValidLevel_Succeeds(int level)
	{
		var result = Character.Create("Legolas", level, OwnerId, CampaignId);

		Assert.True(result.IsSuccess);
		Assert.Equal(level, result.Value.Level);
	}
}
