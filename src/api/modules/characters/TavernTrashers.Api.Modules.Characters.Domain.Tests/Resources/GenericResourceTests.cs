using TavernTrashers.Api.Modules.Characters.Domain.Characters;
using TavernTrashers.Api.Modules.Characters.Domain.Resources;

namespace TavernTrashers.Api.Modules.Characters.Domain.Tests.Resources;

public class GenericResourceTests
{
	private static readonly Guid CharacterId = Guid.NewGuid();

	// ── Creation ────────────────────────────────────────────────────────────

	[Fact]
	public void Create_WithValidData_Succeeds()
	{
		var result = GenericResource.Create(CharacterId, "Action", 1, ResourceDirection.Spending, ResetTrigger.PerRound);

		Assert.True(result.IsSuccess);
		Assert.Equal("Action", result.Value.Name);
		Assert.Equal(1, result.Value.MaxAmount);
		Assert.Equal(ResourceDirection.Spending, result.Value.Direction);
		Assert.Equal(ResetTrigger.PerRound, result.Value.ResetTriggers);
	}

	[Theory]
	[InlineData("")]
	[InlineData("   ")]
	public void Create_WithEmptyName_Fails(string name)
	{
		var result = GenericResource.Create(CharacterId, name, 1, ResourceDirection.Spending, ResetTrigger.PerRound);

		Assert.True(result.IsFailure);
	}

	[Fact]
	public void Create_WithNegativeMaxAmount_Fails()
	{
		var result = GenericResource.Create(CharacterId, "Action", -1, ResourceDirection.Spending, ResetTrigger.PerRound);

		Assert.True(result.IsFailure);
	}

	// ── Initial Current Amount ───────────────────────────────────────────────

	[Fact]
	public void Create_SpendingResource_InitializesCurrentToMax()
	{
		var result = GenericResource.Create(CharacterId, "Action", 3, ResourceDirection.Spending, ResetTrigger.PerRound);

		Assert.Equal(3, result.Value.CurrentAmount);
		Assert.Equal(3, result.Value.MaxAmount);
	}

	[Fact]
	public void Create_AccumulatingResource_InitializesCurrentToZero()
	{
		var result = GenericResource.Create(CharacterId, "Exhaustion", 6, ResourceDirection.Accumulating, ResetTrigger.LongRest);

		Assert.Equal(0, result.Value.CurrentAmount);
		Assert.Equal(6, result.Value.MaxAmount);
	}

	// ── Use (Decrement) ──────────────────────────────────────────────────────

	[Fact]
	public void Use_WhenCurrentIsAboveZero_Decrements()
	{
		var resource = GenericResource.Create(CharacterId, "Action", 1, ResourceDirection.Spending, ResetTrigger.PerRound).Value;

		var result = resource.Use();

		Assert.True(result.IsSuccess);
		Assert.Equal(0, resource.CurrentAmount);
	}

	[Fact]
	public void Use_WhenCurrentIsZero_Fails()
	{
		var resource = GenericResource.Create(CharacterId, "Action", 1, ResourceDirection.Spending, ResetTrigger.PerRound).Value;
		resource.Use();

		var result = resource.Use();

		Assert.True(result.IsFailure);
		Assert.Equal(0, resource.CurrentAmount);
	}

	// ── Apply (Increment) ────────────────────────────────────────────────────

	[Fact]
	public void Apply_WhenCurrentIsBelowMax_Increments()
	{
		var resource = GenericResource.Create(CharacterId, "Exhaustion", 6, ResourceDirection.Accumulating, ResetTrigger.LongRest).Value;

		var result = resource.Apply();

		Assert.True(result.IsSuccess);
		Assert.Equal(1, resource.CurrentAmount);
	}

	[Fact]
	public void Apply_WhenCurrentIsAtMax_Fails()
	{
		var resource = GenericResource.Create(CharacterId, "Exhaustion", 1, ResourceDirection.Accumulating, ResetTrigger.LongRest).Value;
		resource.Apply();

		var result = resource.Apply();

		Assert.True(result.IsFailure);
		Assert.Equal(1, resource.CurrentAmount);
	}

	// ── Restore ──────────────────────────────────────────────────────────────

	[Fact]
	public void Restore_SpendingResource_SetsCurrentToMax()
	{
		var resource = GenericResource.Create(CharacterId, "Action", 3, ResourceDirection.Spending, ResetTrigger.PerRound).Value;
		resource.Use();
		resource.Use();

		resource.Restore();

		Assert.Equal(3, resource.CurrentAmount);
	}

	[Fact]
	public void Restore_AccumulatingResource_SetsCurrentToZero()
	{
		var resource = GenericResource.Create(CharacterId, "Exhaustion", 6, ResourceDirection.Accumulating, ResetTrigger.LongRest).Value;
		resource.Apply();
		resource.Apply();

		resource.Restore();

		Assert.Equal(0, resource.CurrentAmount);
	}

	// ── Default Set ──────────────────────────────────────────────────────────

	[Fact]
	public void Create_Character_HasDefaultGenericResources()
	{
		var character = Character.Create("Gandalf", 1, Guid.NewGuid(), Guid.NewGuid()).Value;

		var names = character.GenericResources.Select(r => r.Name).ToList();

		Assert.Contains("Action", names);
		Assert.Contains("Bonus Action", names);
		Assert.Contains("Reaction", names);
		Assert.Contains("Heroic Inspiration", names);
		Assert.Contains("Exhaustion", names);
	}

	[Fact]
	public void Create_Character_Action_IsSpendingWithMaxOne()
	{
		var character = Character.Create("Gandalf", 1, Guid.NewGuid(), Guid.NewGuid()).Value;

		var action = character.GenericResources.Single(r => r.Name == "Action");

		Assert.Equal(ResourceDirection.Spending, action.Direction);
		Assert.Equal(1, action.MaxAmount);
		Assert.Equal(1, action.CurrentAmount);
	}

	[Fact]
	public void Create_Character_Exhaustion_IsAccumulatingWithMaxSix()
	{
		var character = Character.Create("Gandalf", 1, Guid.NewGuid(), Guid.NewGuid()).Value;

		var exhaustion = character.GenericResources.Single(r => r.Name == "Exhaustion");

		Assert.Equal(ResourceDirection.Accumulating, exhaustion.Direction);
		Assert.Equal(6, exhaustion.MaxAmount);
		Assert.Equal(0, exhaustion.CurrentAmount);
	}
}
