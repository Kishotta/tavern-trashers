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
	public void Create_InitializesDeathSavingThrowsToZero()
	{
		var result = Character.Create("Gandalf", 1, OwnerId, CampaignId);

		Assert.True(result.IsSuccess);
		Assert.Equal(0, result.Value.DeathSavingThrows.Successes);
		Assert.Equal(0, result.Value.DeathSavingThrows.Failures);
	}

	[Fact]
	public void Heal_WhenCurrentHpBecomesPositive_ResetsDeathSavingThrows()
	{
		var character = Character.Create("Aragorn", 1, OwnerId, CampaignId).Value;
		character.SetHitPointFields(10, 0, null, null);
		character.RecordDeathSavingThrowSuccess();
		character.RecordDeathSavingThrowFailure();

		character.Heal(5);

		Assert.Equal(0, character.DeathSavingThrows.Successes);
		Assert.Equal(0, character.DeathSavingThrows.Failures);
	}

	[Fact]
	public void TakeDamage_WhenCurrentHpRemainsPositive_DoesNotChangeDST()
	{
		var character = Character.Create("Legolas", 1, OwnerId, CampaignId).Value;
		character.SetHitPointFields(10, 5, null, null);

		character.TakeDamage(3);

		Assert.Equal(0, character.DeathSavingThrows.Successes);
		Assert.Equal(0, character.DeathSavingThrows.Failures);
	}

	[Fact]
	public void TakeDamage_WhenCurrentHpDropsToZero_ResetsDeathSavingThrows()
	{
		var character = Character.Create("Frodo", 1, OwnerId, CampaignId).Value;
		character.SetHitPointFields(10, 5, null, null);
		character.RecordDeathSavingThrowSuccess();
		character.RecordDeathSavingThrowFailure();

		character.TakeDamage(5);

		Assert.Equal(0, character.DeathSavingThrows.Successes);
		Assert.Equal(0, character.DeathSavingThrows.Failures);
	}

	[Fact]
	public void TakeDamage_WhenAlreadyAtZeroHp_AutoRecordsOneFailure()
	{
		var character = Character.Create("Sam", 1, OwnerId, CampaignId).Value;
		character.SetHitPointFields(10, 0, null, null);

		character.TakeDamage(5);

		Assert.Equal(1, character.DeathSavingThrows.Failures);
		Assert.Equal(0, character.DeathSavingThrows.Successes);
	}

	[Fact]
	public void TakeDamage_WhenAlreadyAtZeroHpWithZeroDamage_DoesNotAutoRecordFailure()
	{
		var character = Character.Create("Pippin", 1, OwnerId, CampaignId).Value;
		character.SetHitPointFields(10, 0, null, null);

		character.TakeDamage(0);

		Assert.Equal(0, character.DeathSavingThrows.Failures);
	}

	[Fact]
	public void SetHitPointFields_WhenCurrentHpBecomesPositive_ResetsDeathSavingThrows()
	{
		var character = Character.Create("Gimli", 1, OwnerId, CampaignId).Value;
		character.SetHitPointFields(10, 0, null, null);
		character.RecordDeathSavingThrowSuccess();
		character.RecordDeathSavingThrowFailure();

		character.SetHitPointFields(null, 8, null, null);

		Assert.Equal(0, character.DeathSavingThrows.Successes);
		Assert.Equal(0, character.DeathSavingThrows.Failures);
	}

	[Fact]
	public void ResetDeathSavingThrows_ManualReset_ClearsBothCounters()
	{
		var character = Character.Create("Boromir", 1, OwnerId, CampaignId).Value;
		character.RecordDeathSavingThrowSuccess();
		character.RecordDeathSavingThrowSuccess();
		character.RecordDeathSavingThrowFailure();

		character.ResetDeathSavingThrows();

		Assert.Equal(0, character.DeathSavingThrows.Successes);
		Assert.Equal(0, character.DeathSavingThrows.Failures);
	}
}
