using TavernTrashers.Api.Modules.Characters.Domain.Resources;

namespace TavernTrashers.Api.Modules.Characters.Domain.Tests.Resources;

public class HpTrackerTests
{
	private static readonly Guid CharacterId = Guid.NewGuid();

	[Fact]
	public void Create_WithValidBaseMaxHp_Succeeds()
	{
		var result = HpTracker.Create(CharacterId, baseMaxHp: 50);

		Assert.True(result.IsSuccess);
		Assert.Equal(50, result.Value.BaseMaxHp);
		Assert.Equal(50, result.Value.CurrentHp);
		Assert.Equal(0, result.Value.MaxHpReduction);
		Assert.Equal(0, result.Value.TemporaryHp);
		Assert.Equal(50, result.Value.EffectiveMaxHp);
	}

	[Fact]
	public void Create_WithZeroBaseMaxHp_Succeeds()
	{
		var result = HpTracker.Create(CharacterId, baseMaxHp: 0);

		Assert.True(result.IsSuccess);
		Assert.Equal(0, result.Value.BaseMaxHp);
		Assert.Equal(0, result.Value.CurrentHp);
	}

	[Fact]
	public void Create_WithNegativeBaseMaxHp_Fails()
	{
		var result = HpTracker.Create(CharacterId, baseMaxHp: -10);

		Assert.True(result.IsFailure);
	}

	[Fact]
	public void SetBaseMaxHp_UpdatesBaseMaxHp()
	{
		var tracker = HpTracker.Create(CharacterId, baseMaxHp: 50).Value;

		var result = tracker.SetBaseMaxHp(60);

		Assert.True(result.IsSuccess);
		Assert.Equal(60, tracker.BaseMaxHp);
		Assert.Equal(60, tracker.EffectiveMaxHp);
	}

	[Fact]
	public void SetBaseMaxHp_WhenCurrentHpExceedsNewMax_AdjustsCurrentHp()
	{
		var tracker = HpTracker.Create(CharacterId, baseMaxHp: 50).Value;

		var result = tracker.SetBaseMaxHp(30);

		Assert.True(result.IsSuccess);
		Assert.Equal(30, tracker.BaseMaxHp);
		Assert.Equal(30, tracker.CurrentHp);
	}

	[Fact]
	public void TakeDamage_WithNoTemporaryHp_ReducesCurrentHp()
	{
		var tracker = HpTracker.Create(CharacterId, baseMaxHp: 50).Value;

		var result = tracker.TakeDamage(10);

		Assert.True(result.IsSuccess);
		Assert.Equal(40, tracker.CurrentHp);
		Assert.Equal(0, tracker.TemporaryHp);
	}

	[Fact]
	public void TakeDamage_WithTemporaryHp_ExhaustsTemporaryHpFirst()
	{
		var tracker = HpTracker.Create(CharacterId, baseMaxHp: 50).Value;
		tracker.SetTemporaryHp(10);

		var result = tracker.TakeDamage(5);

		Assert.True(result.IsSuccess);
		Assert.Equal(50, tracker.CurrentHp); // Current HP unchanged
		Assert.Equal(5, tracker.TemporaryHp); // Temp HP reduced
	}

	[Fact]
	public void TakeDamage_WhenDamageExceedsTemporaryHp_ReducesBoth()
	{
		var tracker = HpTracker.Create(CharacterId, baseMaxHp: 50).Value;
		tracker.SetTemporaryHp(10);

		var result = tracker.TakeDamage(15);

		Assert.True(result.IsSuccess);
		Assert.Equal(45, tracker.CurrentHp); // Reduced by overflow (15-10)
		Assert.Equal(0, tracker.TemporaryHp); // Temp HP depleted
	}

	[Fact]
	public void TakeDamage_CannotReduceCurrentHpBelowZero()
	{
		var tracker = HpTracker.Create(CharacterId, baseMaxHp: 50).Value;

		var result = tracker.TakeDamage(100);

		Assert.True(result.IsSuccess);
		Assert.Equal(0, tracker.CurrentHp);
	}

	[Fact]
	public void TakeDamage_WithNegativeDamage_Fails()
	{
		var tracker = HpTracker.Create(CharacterId, baseMaxHp: 50).Value;

		var result = tracker.TakeDamage(-10);

		Assert.True(result.IsFailure);
	}

	[Fact]
	public void TakeDamage_WithZeroDamage_DoesNothing()
	{
		var tracker = HpTracker.Create(CharacterId, baseMaxHp: 50).Value;

		var result = tracker.TakeDamage(0);

		Assert.True(result.IsSuccess);
		Assert.Equal(50, tracker.CurrentHp);
	}

	[Fact]
	public void Heal_IncreasesCurrentHp()
	{
		var tracker = HpTracker.Create(CharacterId, baseMaxHp: 50).Value;
		tracker.TakeDamage(20);

		var result = tracker.Heal(10);

		Assert.True(result.IsSuccess);
		Assert.Equal(40, tracker.CurrentHp);
	}

	[Fact]
	public void Heal_CannotExceedEffectiveMaxHp()
	{
		var tracker = HpTracker.Create(CharacterId, baseMaxHp: 50).Value;
		tracker.TakeDamage(10);

		var result = tracker.Heal(20);

		Assert.True(result.IsSuccess);
		Assert.Equal(50, tracker.CurrentHp); // Capped at effective max
	}

	[Fact]
	public void Heal_WithMaxHpReduction_CannotExceedEffectiveMaxHp()
	{
		var tracker = HpTracker.Create(CharacterId, baseMaxHp: 50).Value;
		tracker.ApplyMaxHpReduction(10);
		tracker.TakeDamage(10);

		var result = tracker.Heal(20);

		Assert.True(result.IsSuccess);
		Assert.Equal(40, tracker.CurrentHp); // Capped at effective max (50 - 10)
	}

	[Fact]
	public void Heal_WithNegativeHealing_Fails()
	{
		var tracker = HpTracker.Create(CharacterId, baseMaxHp: 50).Value;

		var result = tracker.Heal(-10);

		Assert.True(result.IsFailure);
	}

	[Fact]
	public void Heal_WithZeroHealing_DoesNothing()
	{
		var tracker = HpTracker.Create(CharacterId, baseMaxHp: 50).Value;
		tracker.TakeDamage(10);

		var result = tracker.Heal(0);

		Assert.True(result.IsSuccess);
		Assert.Equal(40, tracker.CurrentHp);
	}

	[Fact]
	public void SetTemporaryHp_SetsTemporaryHp()
	{
		var tracker = HpTracker.Create(CharacterId, baseMaxHp: 50).Value;

		var result = tracker.SetTemporaryHp(15);

		Assert.True(result.IsSuccess);
		Assert.Equal(15, tracker.TemporaryHp);
	}

	[Fact]
	public void SetTemporaryHp_WhenHigherValueExists_KeepsHigher()
	{
		var tracker = HpTracker.Create(CharacterId, baseMaxHp: 50).Value;
		tracker.SetTemporaryHp(20);

		var result = tracker.SetTemporaryHp(10);

		Assert.True(result.IsSuccess);
		Assert.Equal(20, tracker.TemporaryHp); // Keeps the higher value
	}

	[Fact]
	public void SetTemporaryHp_WhenLowerValueExists_TakesHigher()
	{
		var tracker = HpTracker.Create(CharacterId, baseMaxHp: 50).Value;
		tracker.SetTemporaryHp(10);

		var result = tracker.SetTemporaryHp(20);

		Assert.True(result.IsSuccess);
		Assert.Equal(20, tracker.TemporaryHp); // Takes the new higher value
	}

	[Fact]
	public void SetTemporaryHp_WithNegativeValue_Fails()
	{
		var tracker = HpTracker.Create(CharacterId, baseMaxHp: 50).Value;

		var result = tracker.SetTemporaryHp(-10);

		Assert.True(result.IsFailure);
	}

	[Fact]
	public void ApplyMaxHpReduction_ReducesEffectiveMaxHp()
	{
		var tracker = HpTracker.Create(CharacterId, baseMaxHp: 50).Value;

		var result = tracker.ApplyMaxHpReduction(10);

		Assert.True(result.IsSuccess);
		Assert.Equal(10, tracker.MaxHpReduction);
		Assert.Equal(40, tracker.EffectiveMaxHp);
	}

	[Fact]
	public void ApplyMaxHpReduction_WhenCurrentHpExceedsNewMax_AdjustsCurrentHp()
	{
		var tracker = HpTracker.Create(CharacterId, baseMaxHp: 50).Value;

		var result = tracker.ApplyMaxHpReduction(20);

		Assert.True(result.IsSuccess);
		Assert.Equal(20, tracker.MaxHpReduction);
		Assert.Equal(30, tracker.EffectiveMaxHp);
		Assert.Equal(30, tracker.CurrentHp); // Adjusted down
	}

	[Fact]
	public void ApplyMaxHpReduction_CanBeCalledMultipleTimes()
	{
		var tracker = HpTracker.Create(CharacterId, baseMaxHp: 50).Value;
		tracker.ApplyMaxHpReduction(10);

		var result = tracker.ApplyMaxHpReduction(5);

		Assert.True(result.IsSuccess);
		Assert.Equal(15, tracker.MaxHpReduction); // Cumulative
		Assert.Equal(35, tracker.EffectiveMaxHp);
	}

	[Fact]
	public void ApplyMaxHpReduction_WithNegativeValue_Fails()
	{
		var tracker = HpTracker.Create(CharacterId, baseMaxHp: 50).Value;

		var result = tracker.ApplyMaxHpReduction(-10);

		Assert.True(result.IsFailure);
	}

	[Fact]
	public void RemoveMaxHpReduction_IncreasesEffectiveMaxHp()
	{
		var tracker = HpTracker.Create(CharacterId, baseMaxHp: 50).Value;
		tracker.ApplyMaxHpReduction(20);

		var result = tracker.RemoveMaxHpReduction(10);

		Assert.True(result.IsSuccess);
		Assert.Equal(10, tracker.MaxHpReduction);
		Assert.Equal(40, tracker.EffectiveMaxHp);
	}

	[Fact]
	public void RemoveMaxHpReduction_CannotReduceBelowZero()
	{
		var tracker = HpTracker.Create(CharacterId, baseMaxHp: 50).Value;
		tracker.ApplyMaxHpReduction(10);

		var result = tracker.RemoveMaxHpReduction(20);

		Assert.True(result.IsSuccess);
		Assert.Equal(0, tracker.MaxHpReduction); // Clamped to 0
		Assert.Equal(50, tracker.EffectiveMaxHp);
	}

	[Fact]
	public void RemoveMaxHpReduction_WithNegativeValue_Fails()
	{
		var tracker = HpTracker.Create(CharacterId, baseMaxHp: 50).Value;

		var result = tracker.RemoveMaxHpReduction(-10);

		Assert.True(result.IsFailure);
	}

	[Fact]
	public void SetCurrentHp_DirectlySetsCurrentHp()
	{
		var tracker = HpTracker.Create(CharacterId, baseMaxHp: 50).Value;

		var result = tracker.SetCurrentHp(25);

		Assert.True(result.IsSuccess);
		Assert.Equal(25, tracker.CurrentHp);
	}

	[Fact]
	public void SetCurrentHp_CanExceedEffectiveMaxHp()
	{
		var tracker = HpTracker.Create(CharacterId, baseMaxHp: 50).Value;

		var result = tracker.SetCurrentHp(100);

		Assert.True(result.IsSuccess);
		Assert.Equal(100, tracker.CurrentHp); // DM override allows this
	}

	[Fact]
	public void SetCurrentHp_WithNegativeValue_Fails()
	{
		var tracker = HpTracker.Create(CharacterId, baseMaxHp: 50).Value;

		var result = tracker.SetCurrentHp(-10);

		Assert.True(result.IsFailure);
	}

	[Fact]
	public void SetMaxHpReduction_DirectlySetsMaxHpReduction()
	{
		var tracker = HpTracker.Create(CharacterId, baseMaxHp: 50).Value;

		var result = tracker.SetMaxHpReduction(15);

		Assert.True(result.IsSuccess);
		Assert.Equal(15, tracker.MaxHpReduction);
		Assert.Equal(35, tracker.EffectiveMaxHp);
	}

	[Fact]
	public void SetMaxHpReduction_WhenCurrentHpExceedsNewMax_AdjustsCurrentHp()
	{
		var tracker = HpTracker.Create(CharacterId, baseMaxHp: 50).Value;

		var result = tracker.SetMaxHpReduction(30);

		Assert.True(result.IsSuccess);
		Assert.Equal(30, tracker.MaxHpReduction);
		Assert.Equal(20, tracker.EffectiveMaxHp);
		Assert.Equal(20, tracker.CurrentHp); // Adjusted down
	}

	[Fact]
	public void SetMaxHpReduction_WithNegativeValue_Fails()
	{
		var tracker = HpTracker.Create(CharacterId, baseMaxHp: 50).Value;

		var result = tracker.SetMaxHpReduction(-10);

		Assert.True(result.IsFailure);
	}

	[Fact]
	public void EffectiveMaxHp_IsAlwaysDerived()
	{
		var tracker = HpTracker.Create(CharacterId, baseMaxHp: 50).Value;
		Assert.Equal(50, tracker.EffectiveMaxHp);

		tracker.ApplyMaxHpReduction(10);
		Assert.Equal(40, tracker.EffectiveMaxHp);

		tracker.SetBaseMaxHp(60);
		Assert.Equal(50, tracker.EffectiveMaxHp); // 60 - 10

		tracker.RemoveMaxHpReduction(10);
		Assert.Equal(60, tracker.EffectiveMaxHp);
	}
}
