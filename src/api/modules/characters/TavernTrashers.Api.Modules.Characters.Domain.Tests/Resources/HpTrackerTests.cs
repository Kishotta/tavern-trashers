using TavernTrashers.Api.Modules.Characters.Domain.Resources;

namespace TavernTrashers.Api.Modules.Characters.Domain.Tests.Resources;

public class HpTrackerTests
{
	private static readonly Guid CharacterId = Guid.NewGuid();

	private static HpTracker CreateTracker(int baseMaxHp = 20) =>
		HpTracker.Create(CharacterId, baseMaxHp).Value;

	[Fact]
	public void Create_WithValidData_Succeeds()
	{
		var result = HpTracker.Create(CharacterId, 20);

		Assert.True(result.IsSuccess);
		Assert.Equal(20, result.Value.BaseMaxHp);
		Assert.Equal(20, result.Value.CurrentHp);
		Assert.Equal(0, result.Value.TemporaryHp);
		Assert.Equal(0, result.Value.MaxHpReduction);
		Assert.Equal(20, result.Value.EffectiveMaxHp);
	}

	[Theory]
	[InlineData(0)]
	[InlineData(-1)]
	[InlineData(-100)]
	public void Create_WithInvalidBaseMaxHp_Fails(int baseMaxHp)
	{
		var result = HpTracker.Create(CharacterId, baseMaxHp);

		Assert.True(result.IsFailure);
	}

	[Fact]
	public void EffectiveMaxHp_IsDerivedFromBaseMaxHpMinusReduction()
	{
		var tracker = CreateTracker(baseMaxHp: 30);
		tracker.ApplyMaxHpReduction(10);

		Assert.Equal(30, tracker.BaseMaxHp);
		Assert.Equal(10, tracker.MaxHpReduction);
		Assert.Equal(20, tracker.EffectiveMaxHp);
	}

	[Fact]
	public void TakeDamage_AbsorbsTemporaryHpFirst()
	{
		var tracker = CreateTracker(baseMaxHp: 20);
		tracker.SetTemporaryHp(5);

		tracker.TakeDamage(3);

		Assert.Equal(2, tracker.TemporaryHp);
		Assert.Equal(20, tracker.CurrentHp);
	}

	[Fact]
	public void TakeDamage_OverflowReducesCurrentHp()
	{
		var tracker = CreateTracker(baseMaxHp: 20);
		tracker.SetTemporaryHp(5);

		tracker.TakeDamage(8);

		Assert.Equal(0, tracker.TemporaryHp);
		Assert.Equal(17, tracker.CurrentHp);
	}

	[Fact]
	public void TakeDamage_ExactlyExhaustsTemporaryHp()
	{
		var tracker = CreateTracker(baseMaxHp: 20);
		tracker.SetTemporaryHp(5);

		tracker.TakeDamage(5);

		Assert.Equal(0, tracker.TemporaryHp);
		Assert.Equal(20, tracker.CurrentHp);
	}

	[Fact]
	public void TakeDamage_WithNoTemporaryHp_ReducesCurrentHpDirectly()
	{
		var tracker = CreateTracker(baseMaxHp: 20);

		tracker.TakeDamage(8);

		Assert.Equal(0, tracker.TemporaryHp);
		Assert.Equal(12, tracker.CurrentHp);
	}

	[Fact]
	public void TakeDamage_CannotReduceCurrentHpBelowZero()
	{
		var tracker = CreateTracker(baseMaxHp: 10);

		tracker.TakeDamage(15);

		Assert.Equal(0, tracker.CurrentHp);
	}

	[Theory]
	[InlineData(0)]
	[InlineData(-1)]
	public void TakeDamage_WithInvalidAmount_Fails(int amount)
	{
		var tracker = CreateTracker();

		var result = tracker.TakeDamage(amount);

		Assert.True(result.IsFailure);
	}

	[Fact]
	public void Heal_IncreasesCurrentHp()
	{
		var tracker = CreateTracker(baseMaxHp: 20);
		tracker.TakeDamage(10);

		tracker.Heal(5);

		Assert.Equal(15, tracker.CurrentHp);
	}

	[Fact]
	public void Heal_CannotExceedEffectiveMaxHp()
	{
		var tracker = CreateTracker(baseMaxHp: 20);
		tracker.TakeDamage(5);

		tracker.Heal(100);

		Assert.Equal(20, tracker.CurrentHp);
	}

	[Fact]
	public void Heal_CannotExceedEffectiveMaxHp_WhenReductionIsApplied()
	{
		var tracker = CreateTracker(baseMaxHp: 20);
		tracker.ApplyMaxHpReduction(5);
		tracker.TakeDamage(5);

		tracker.Heal(100);

		Assert.Equal(15, tracker.CurrentHp);
	}

	[Theory]
	[InlineData(0)]
	[InlineData(-1)]
	public void Heal_WithInvalidAmount_Fails(int amount)
	{
		var tracker = CreateTracker();

		var result = tracker.Heal(amount);

		Assert.True(result.IsFailure);
	}

	[Fact]
	public void SetTemporaryHp_TakesHigherValue_WhenExistingIsLower()
	{
		var tracker = CreateTracker();
		tracker.SetTemporaryHp(5);

		tracker.SetTemporaryHp(10);

		Assert.Equal(10, tracker.TemporaryHp);
	}

	[Fact]
	public void SetTemporaryHp_KeepsExistingValue_WhenNewValueIsLower()
	{
		var tracker = CreateTracker();
		tracker.SetTemporaryHp(10);

		tracker.SetTemporaryHp(5);

		Assert.Equal(10, tracker.TemporaryHp);
	}

	[Fact]
	public void SetTemporaryHp_WithNegativeValue_Fails()
	{
		var tracker = CreateTracker();

		var result = tracker.SetTemporaryHp(-1);

		Assert.True(result.IsFailure);
	}

	[Fact]
	public void ApplyMaxHpReduction_DecreasesEffectiveMaxHp()
	{
		var tracker = CreateTracker(baseMaxHp: 20);

		tracker.ApplyMaxHpReduction(5);

		Assert.Equal(5, tracker.MaxHpReduction);
		Assert.Equal(15, tracker.EffectiveMaxHp);
	}

	[Fact]
	public void ApplyMaxHpReduction_StacksMultipleReductions()
	{
		var tracker = CreateTracker(baseMaxHp: 30);

		tracker.ApplyMaxHpReduction(5);
		tracker.ApplyMaxHpReduction(3);

		Assert.Equal(8, tracker.MaxHpReduction);
		Assert.Equal(22, tracker.EffectiveMaxHp);
	}

	[Fact]
	public void ApplyMaxHpReduction_ClampsCurrentHpToEffectiveMax()
	{
		var tracker = CreateTracker(baseMaxHp: 20);

		tracker.ApplyMaxHpReduction(5);

		Assert.Equal(15, tracker.CurrentHp);
	}

	[Theory]
	[InlineData(0)]
	[InlineData(-1)]
	public void ApplyMaxHpReduction_WithInvalidAmount_Fails(int reduction)
	{
		var tracker = CreateTracker();

		var result = tracker.ApplyMaxHpReduction(reduction);

		Assert.True(result.IsFailure);
	}

	[Fact]
	public void RemoveMaxHpReduction_RestoresEffectiveMaxHpToBaseMaxHp()
	{
		var tracker = CreateTracker(baseMaxHp: 20);
		tracker.ApplyMaxHpReduction(5);

		tracker.RemoveMaxHpReduction();

		Assert.Equal(0, tracker.MaxHpReduction);
		Assert.Equal(20, tracker.EffectiveMaxHp);
	}

	[Fact]
	public void SetBaseMaxHp_UpdatesBaseMaxHp()
	{
		var tracker = CreateTracker(baseMaxHp: 10);

		tracker.SetBaseMaxHp(25);

		Assert.Equal(25, tracker.BaseMaxHp);
	}

	[Fact]
	public void SetBaseMaxHp_ClampsCurrentHpToNewEffectiveMax()
	{
		var tracker = CreateTracker(baseMaxHp: 20);

		tracker.SetBaseMaxHp(10);

		Assert.Equal(10, tracker.CurrentHp);
	}

	[Theory]
	[InlineData(0)]
	[InlineData(-1)]
	public void SetBaseMaxHp_WithInvalidValue_Fails(int baseMaxHp)
	{
		var tracker = CreateTracker();

		var result = tracker.SetBaseMaxHp(baseMaxHp);

		Assert.True(result.IsFailure);
	}
}
