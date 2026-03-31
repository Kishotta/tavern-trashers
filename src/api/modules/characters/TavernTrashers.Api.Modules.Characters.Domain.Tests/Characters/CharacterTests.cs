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

	[Fact]
	public void Create_AutoPopulatesDefaultResources()
	{
		var result = Character.Create("Thorin", 1, OwnerId, CampaignId);

		Assert.True(result.IsSuccess);
		var resources = result.Value.GenericResources;
		Assert.Equal(5, resources.Count);
		Assert.Contains(resources, r => r.Name == "Action");
		Assert.Contains(resources, r => r.Name == "Bonus Action");
		Assert.Contains(resources, r => r.Name == "Reaction");
		Assert.Contains(resources, r => r.Name == "Heroic Inspiration");
		Assert.Contains(resources, r => r.Name == "Exhaustion");
	}

	[Fact]
	public void Create_DefaultActionResource_IsSpendingWithPerRoundReset()
	{
		var result = Character.Create("Bilbo", 1, OwnerId, CampaignId);
		var action = result.Value.GenericResources.Single(r => r.Name == "Action");

		Assert.Equal(1, action.MaxUses);
		Assert.Equal(1, action.CurrentUses);
		Assert.Equal(Domain.Resources.ResourceDirection.Spending, action.Direction);
		Assert.True(action.HasResetTrigger(Domain.Resources.ResetTrigger.PerRound));
	}

	[Fact]
	public void Create_DefaultExhaustionResource_IsAccumulatingWithLongRestReset()
	{
		var result = Character.Create("Bilbo", 1, OwnerId, CampaignId);
		var exhaustion = result.Value.GenericResources.Single(r => r.Name == "Exhaustion");

		Assert.Equal(6, exhaustion.MaxUses);
		Assert.Equal(0, exhaustion.CurrentUses);
		Assert.Equal(Domain.Resources.ResourceDirection.Accumulating, exhaustion.Direction);
		Assert.True(exhaustion.HasResetTrigger(Domain.Resources.ResetTrigger.LongRest));
	}

	[Fact]
	public void Create_AutomaticallyCreatesHpTracker()
	{
		var result = Character.Create("Frodo", 1, OwnerId, CampaignId);

		Assert.True(result.IsSuccess);
		Assert.NotNull(result.Value.HpTracker);
		Assert.Equal(0, result.Value.HpTracker.BaseMaxHp);
		Assert.Equal(0, result.Value.HpTracker.CurrentHp);
		Assert.Equal(0, result.Value.HpTracker.TemporaryHp);
		Assert.Equal(0, result.Value.HpTracker.MaxHpReduction);
		Assert.Equal(0, result.Value.HpTracker.EffectiveMaxHp);
	}

	[Fact]
	public void SetBaseMaxHp_UpdatesHpTracker()
	{
		var character = Character.Create("Sam", 1, OwnerId, CampaignId).Value;

		var result = character.SetBaseMaxHp(50);

		Assert.True(result.IsSuccess);
		Assert.Equal(50, character.HpTracker!.BaseMaxHp);
		Assert.Equal(0, character.HpTracker!.CurrentHp); // Current HP doesn't auto-increase
	}

	[Fact]
	public void TakeDamage_ReducesCurrentHp()
	{
		var character = Character.Create("Merry", 1, OwnerId, CampaignId).Value;
		character.SetBaseMaxHp(50);
		character.Heal(50); // Heal to full first

		var result = character.TakeDamage(15);

		Assert.True(result.IsSuccess);
		Assert.Equal(35, character.HpTracker!.CurrentHp);
	}

	[Fact]
	public void Heal_IncreasesCurrentHp()
	{
		var character = Character.Create("Pippin", 1, OwnerId, CampaignId).Value;
		character.SetBaseMaxHp(50);
		character.Heal(50); // Heal to full
		character.TakeDamage(20);

		var result = character.Heal(10);

		Assert.True(result.IsSuccess);
		Assert.Equal(40, character.HpTracker!.CurrentHp);
	}
}
