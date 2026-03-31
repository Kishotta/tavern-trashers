using TavernTrashers.Api.Modules.Characters.Domain.Resources;

namespace TavernTrashers.Api.Modules.Characters.Domain.Tests.Resources;

public class HitPointsTests
{
	private static readonly Guid CharacterId = Guid.NewGuid();

	private static HitPoints CreateTracker(int baseMaxHitPoints = 20) =>
		HitPoints.Create(CharacterId, baseMaxHitPoints).Value;

	[Fact]
	public void Create_WithValidData_Succeeds()
	{
		var result = HitPoints.Create(CharacterId, 20);

		Assert.True(result.IsSuccess);
		Assert.Equal(20, result.Value.BaseMaxHitPoints);
		Assert.Equal(20, result.Value.CurrentHitPoints);
		Assert.Equal(0, result.Value.TemporaryHitPoints);
		Assert.Equal(0, result.Value.MaxHitPointReduction);
		Assert.Equal(20, result.Value.EffectiveMaxHitPoints);
	}

	[Fact]
	public void Create_WithZeroBaseMaxHitPoints_Succeeds()
	{
		var result = HitPoints.Create(CharacterId, 0);

		Assert.True(result.IsSuccess);
		Assert.Equal(0, result.Value.BaseMaxHitPoints);
		Assert.Equal(0, result.Value.CurrentHitPoints);
	}

	[Theory]
	[InlineData(-1)]
	[InlineData(-100)]
	public void Create_WithNegativeBaseMaxHitPoints_Fails(int baseMaxHitPoints)
	{
		var result = HitPoints.Create(CharacterId, baseMaxHitPoints);

		Assert.True(result.IsFailure);
	}

	[Fact]
	public void EffectiveMaxHitPoints_IsDerivedFromBaseMaxHitPointsMinusReduction()
	{
		var tracker = CreateTracker(baseMaxHitPoints: 30);
		tracker.ApplyMaxHitPointReduction(10);

		Assert.Equal(30, tracker.BaseMaxHitPoints);
		Assert.Equal(10, tracker.MaxHitPointReduction);
		Assert.Equal(20, tracker.EffectiveMaxHitPoints);
	}

	[Fact]
	public void TakeDamage_AbsorbsTemporaryHitPointsFirst()
	{
		var tracker = CreateTracker(baseMaxHitPoints: 20);
		tracker.SetTemporaryHitPoints(5);

		tracker.TakeDamage(3);

		Assert.Equal(2, tracker.TemporaryHitPoints);
		Assert.Equal(20, tracker.CurrentHitPoints);
	}

	[Fact]
	public void TakeDamage_OverflowReducesCurrentHitPoints()
	{
		var tracker = CreateTracker(baseMaxHitPoints: 20);
		tracker.SetTemporaryHitPoints(5);

		tracker.TakeDamage(8);

		Assert.Equal(0, tracker.TemporaryHitPoints);
		Assert.Equal(17, tracker.CurrentHitPoints);
	}

	[Fact]
	public void TakeDamage_ExactlyExhaustsTemporaryHitPoints()
	{
		var tracker = CreateTracker(baseMaxHitPoints: 20);
		tracker.SetTemporaryHitPoints(5);

		tracker.TakeDamage(5);

		Assert.Equal(0, tracker.TemporaryHitPoints);
		Assert.Equal(20, tracker.CurrentHitPoints);
	}

	[Fact]
	public void TakeDamage_WithNoTemporaryHitPoints_ReducesCurrentHitPointsDirectly()
	{
		var tracker = CreateTracker(baseMaxHitPoints: 20);

		tracker.TakeDamage(8);

		Assert.Equal(0, tracker.TemporaryHitPoints);
		Assert.Equal(12, tracker.CurrentHitPoints);
	}

	[Fact]
	public void TakeDamage_CannotReduceCurrentHitPointsBelowZero()
	{
		var tracker = CreateTracker(baseMaxHitPoints: 10);

		tracker.TakeDamage(15);

		Assert.Equal(0, tracker.CurrentHitPoints);
	}

	[Fact]
	public void TakeDamage_WithZeroAmount_Succeeds()
	{
		var tracker = CreateTracker(baseMaxHitPoints: 20);

		var result = tracker.TakeDamage(0);

		Assert.True(result.IsSuccess);
		Assert.Equal(20, tracker.CurrentHitPoints);
	}

	[Fact]
	public void TakeDamage_WithNegativeAmount_Fails()
	{
		var tracker = CreateTracker();

		var result = tracker.TakeDamage(-1);

		Assert.True(result.IsFailure);
	}

	[Fact]
	public void Heal_IncreasesCurrentHitPoints()
	{
		var tracker = CreateTracker(baseMaxHitPoints: 20);
		tracker.TakeDamage(10);

		tracker.Heal(5);

		Assert.Equal(15, tracker.CurrentHitPoints);
	}

	[Fact]
	public void Heal_CannotExceedEffectiveMaxHitPoints()
	{
		var tracker = CreateTracker(baseMaxHitPoints: 20);
		tracker.TakeDamage(5);

		tracker.Heal(100);

		Assert.Equal(20, tracker.CurrentHitPoints);
	}

	[Fact]
	public void Heal_CannotExceedEffectiveMaxHitPoints_WhenReductionIsApplied()
	{
		var tracker = CreateTracker(baseMaxHitPoints: 20);
		tracker.ApplyMaxHitPointReduction(5);
		tracker.TakeDamage(5);

		tracker.Heal(100);

		Assert.Equal(15, tracker.CurrentHitPoints);
	}

	[Fact]
	public void Heal_WithZeroAmount_Succeeds()
	{
		var tracker = CreateTracker(baseMaxHitPoints: 20);
		tracker.TakeDamage(5);

		var result = tracker.Heal(0);

		Assert.True(result.IsSuccess);
		Assert.Equal(15, tracker.CurrentHitPoints);
	}

	[Fact]
	public void Heal_WithNegativeAmount_Fails()
	{
		var tracker = CreateTracker();

		var result = tracker.Heal(-1);

		Assert.True(result.IsFailure);
	}

	[Fact]
	public void SetTemporaryHitPoints_TakesHigherValue_WhenExistingIsLower()
	{
		var tracker = CreateTracker();
		tracker.SetTemporaryHitPoints(5);

		tracker.SetTemporaryHitPoints(10);

		Assert.Equal(10, tracker.TemporaryHitPoints);
	}

	[Fact]
	public void SetTemporaryHitPoints_KeepsExistingValue_WhenNewValueIsLower()
	{
		var tracker = CreateTracker();
		tracker.SetTemporaryHitPoints(10);

		tracker.SetTemporaryHitPoints(5);

		Assert.Equal(10, tracker.TemporaryHitPoints);
	}

	[Fact]
	public void SetTemporaryHitPoints_WithNegativeValue_Fails()
	{
		var tracker = CreateTracker();

		var result = tracker.SetTemporaryHitPoints(-1);

		Assert.True(result.IsFailure);
	}

	[Fact]
	public void ApplyMaxHitPointReduction_DecreasesEffectiveMaxHitPoints()
	{
		var tracker = CreateTracker(baseMaxHitPoints: 20);

		tracker.ApplyMaxHitPointReduction(5);

		Assert.Equal(5, tracker.MaxHitPointReduction);
		Assert.Equal(15, tracker.EffectiveMaxHitPoints);
	}

	[Fact]
	public void ApplyMaxHitPointReduction_StacksMultipleReductions()
	{
		var tracker = CreateTracker(baseMaxHitPoints: 30);

		tracker.ApplyMaxHitPointReduction(5);
		tracker.ApplyMaxHitPointReduction(3);

		Assert.Equal(8, tracker.MaxHitPointReduction);
		Assert.Equal(22, tracker.EffectiveMaxHitPoints);
	}

	[Fact]
	public void ApplyMaxHitPointReduction_ClampsCurrentHitPointsToEffectiveMax()
	{
		var tracker = CreateTracker(baseMaxHitPoints: 20);

		tracker.ApplyMaxHitPointReduction(5);

		Assert.Equal(15, tracker.CurrentHitPoints);
	}

	[Theory]
	[InlineData(0)]
	[InlineData(-1)]
	public void ApplyMaxHitPointReduction_WithInvalidAmount_Fails(int reduction)
	{
		var tracker = CreateTracker();

		var result = tracker.ApplyMaxHitPointReduction(reduction);

		Assert.True(result.IsFailure);
	}

	[Fact]
	public void RemoveMaxHitPointReduction_RestoresEffectiveMaxHitPointsToBaseMax()
	{
		var tracker = CreateTracker(baseMaxHitPoints: 20);
		tracker.ApplyMaxHitPointReduction(5);

		tracker.RemoveMaxHitPointReduction();

		Assert.Equal(0, tracker.MaxHitPointReduction);
		Assert.Equal(20, tracker.EffectiveMaxHitPoints);
	}

	[Fact]
	public void SetBaseMaxHitPoints_UpdatesBaseMaxHitPoints()
	{
		var tracker = CreateTracker(baseMaxHitPoints: 10);

		tracker.SetBaseMaxHitPoints(25);

		Assert.Equal(25, tracker.BaseMaxHitPoints);
	}

	[Fact]
	public void SetBaseMaxHitPoints_ClampsCurrentHitPointsToNewEffectiveMax()
	{
		var tracker = CreateTracker(baseMaxHitPoints: 20);

		tracker.SetBaseMaxHitPoints(10);

		Assert.Equal(10, tracker.CurrentHitPoints);
	}

	[Fact]
	public void SetBaseMaxHitPoints_WithZero_Succeeds()
	{
		var tracker = CreateTracker(baseMaxHitPoints: 20);

		var result = tracker.SetBaseMaxHitPoints(0);

		Assert.True(result.IsSuccess);
		Assert.Equal(0, tracker.BaseMaxHitPoints);
		Assert.Equal(0, tracker.CurrentHitPoints);
	}

	[Fact]
	public void SetBaseMaxHitPoints_WithNegativeValue_Fails()
	{
		var tracker = CreateTracker();

		var result = tracker.SetBaseMaxHitPoints(-1);

		Assert.True(result.IsFailure);
	}
}
