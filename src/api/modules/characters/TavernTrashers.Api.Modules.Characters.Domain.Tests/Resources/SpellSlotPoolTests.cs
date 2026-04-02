using TavernTrashers.Api.Modules.Characters.Domain.Resources;

namespace TavernTrashers.Api.Modules.Characters.Domain.Tests.Resources;

public class SpellSlotPoolTests
{
	private static readonly Guid CharacterId = Guid.NewGuid();

	private static SpellSlotPool CreateStandard() =>
		SpellSlotPool.CreateDefault(CharacterId);

	private static SpellSlotPool CreatePactMagic() =>
		SpellSlotPool.CreatePactMagic(CharacterId);

	[Fact]
	public void CreateDefault_CreatesStandardPool_WithNineLevels()
	{
		var pool = CreateStandard();

		Assert.Equal(SpellSlotPoolKind.Standard, pool.Kind);
		Assert.Equal(9, pool.Levels.Count);
	}

	[Fact]
	public void CreateDefault_AllLevels_HaveZeroMaxAndCurrent()
	{
		var pool = CreateStandard();

		foreach (var level in pool.Levels)
		{
			Assert.Equal(0, level.MaxUses);
			Assert.Equal(0, level.CurrentUses);
		}
	}

	[Fact]
	public void CreateDefault_HasCorrectLevelNumbers()
	{
		var pool = CreateStandard();

		var levels = pool.Levels.OrderBy(l => l.Level).Select(l => l.Level).ToList();
		Assert.Equal([1, 2, 3, 4, 5, 6, 7, 8, 9], levels);
	}

	[Fact]
	public void CreatePactMagic_CreatesPactMagicPool_WithNineLevels()
	{
		var pool = CreatePactMagic();

		Assert.Equal(SpellSlotPoolKind.PactMagic, pool.Kind);
		Assert.Equal(9, pool.Levels.Count);
	}

	[Fact]
	public void GetResetTrigger_Standard_ReturnsLongRest()
	{
		var pool = CreateStandard();

		Assert.Equal(ResetTrigger.LongRest, pool.GetResetTrigger());
	}

	[Fact]
	public void GetResetTrigger_PactMagic_ReturnsShortRest()
	{
		var pool = CreatePactMagic();

		Assert.Equal(ResetTrigger.ShortRest, pool.GetResetTrigger());
	}

	[Fact]
	public void SetMaxSlots_SetsMaxUsesForSpecifiedLevel()
	{
		var pool = CreateStandard();

		var result = pool.SetMaxSlots(1, 4);

		Assert.True(result.IsSuccess);
		Assert.Equal(4, pool.Levels.Single(l => l.Level == 1).MaxUses);
	}

	[Fact]
	public void SetMaxSlots_DoesNotAffectOtherLevels()
	{
		var pool = CreateStandard();

		pool.SetMaxSlots(1, 4);

		foreach (var level in pool.Levels.Where(l => l.Level != 1))
			Assert.Equal(0, level.MaxUses);
	}

	[Theory]
	[InlineData(0)]
	[InlineData(-1)]
	[InlineData(10)]
	public void SetMaxSlots_WithInvalidLevel_Fails(int level)
	{
		var pool = CreateStandard();

		var result = pool.SetMaxSlots(level, 4);

		Assert.True(result.IsFailure);
	}

	[Fact]
	public void SetMaxSlots_WithNegativeMax_Fails()
	{
		var pool = CreateStandard();

		var result = pool.SetMaxSlots(1, -1);

		Assert.True(result.IsFailure);
	}

	[Fact]
	public void SetMaxSlots_ClampsCurrentUsesToNewMax()
	{
		var pool = CreateStandard();
		pool.SetMaxSlots(1, 4);
		pool.UseSlot(1);
		pool.UseSlot(1);
		Assert.Equal(2, pool.Levels.Single(l => l.Level == 1).CurrentUses);

		pool.SetMaxSlots(1, 1);

		Assert.Equal(1, pool.Levels.Single(l => l.Level == 1).CurrentUses);
	}

	[Fact]
	public void UseSlot_DecrementsCurrentUsesAtSpecifiedLevel()
	{
		var pool = CreateStandard();
		pool.SetMaxSlots(2, 3);

		var result = pool.UseSlot(2);

		Assert.True(result.IsSuccess);
		Assert.Equal(2, pool.Levels.Single(l => l.Level == 2).CurrentUses);
	}

	[Fact]
	public void UseSlot_DoesNotAffectOtherLevels()
	{
		var pool = CreateStandard();
		pool.SetMaxSlots(1, 2);
		pool.SetMaxSlots(2, 3);

		pool.UseSlot(2);

		Assert.Equal(2, pool.Levels.Single(l => l.Level == 1).CurrentUses);
		Assert.Equal(2, pool.Levels.Single(l => l.Level == 2).CurrentUses);
	}

	[Fact]
	public void UseSlot_WhenEmpty_Fails()
	{
		var pool = CreateStandard();
		pool.SetMaxSlots(1, 1);
		pool.UseSlot(1);

		var result = pool.UseSlot(1);

		Assert.True(result.IsFailure);
		Assert.Equal(0, pool.Levels.Single(l => l.Level == 1).CurrentUses);
	}

	[Theory]
	[InlineData(0)]
	[InlineData(-1)]
	[InlineData(10)]
	public void UseSlot_WithInvalidLevel_Fails(int level)
	{
		var pool = CreateStandard();

		var result = pool.UseSlot(level);

		Assert.True(result.IsFailure);
	}

	[Fact]
	public void Restore_SetsAllLevelsBackToMax()
	{
		var pool = CreateStandard();
		pool.SetMaxSlots(1, 4);
		pool.SetMaxSlots(2, 3);
		pool.SetMaxSlots(3, 2);
		pool.UseSlot(1);
		pool.UseSlot(2);
		pool.UseSlot(2);

		pool.Restore();

		Assert.Equal(4, pool.Levels.Single(l => l.Level == 1).CurrentUses);
		Assert.Equal(3, pool.Levels.Single(l => l.Level == 2).CurrentUses);
		Assert.Equal(2, pool.Levels.Single(l => l.Level == 3).CurrentUses);
	}

	[Fact]
	public void Restore_StandardPool_RestoresAllLevels()
	{
		var pool = CreateStandard();
		pool.SetMaxSlots(1, 2);
		pool.SetMaxSlots(2, 1);
		pool.UseSlot(1);
		pool.UseSlot(2);

		pool.Restore();

		Assert.Equal(2, pool.Levels.Single(l => l.Level == 1).CurrentUses);
		Assert.Equal(1, pool.Levels.Single(l => l.Level == 2).CurrentUses);
	}

	[Fact]
	public void Restore_PactMagicPool_RestoresAllLevels()
	{
		var pool = CreatePactMagic();
		pool.SetMaxSlots(1, 2);
		pool.UseSlot(1);
		pool.UseSlot(1);

		pool.Restore();

		Assert.Equal(2, pool.Levels.Single(l => l.Level == 1).CurrentUses);
	}
}
