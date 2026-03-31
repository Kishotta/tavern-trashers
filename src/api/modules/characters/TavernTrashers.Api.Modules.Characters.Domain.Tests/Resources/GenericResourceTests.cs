using TavernTrashers.Api.Modules.Characters.Domain.Resources;

namespace TavernTrashers.Api.Modules.Characters.Domain.Tests.Resources;

public class GenericResourceTests
{
	private static readonly Guid CharacterId = Guid.NewGuid();

	private static GenericResource CreateSpending(int maxUses = 3) =>
		GenericResource.Create(CharacterId, "Test Resource", maxUses, ResourceDirection.Spending, SourceCategory.Custom, [ResetTrigger.Manual]).Value;

	private static GenericResource CreateAccumulating(int maxUses = 6) =>
		GenericResource.Create(CharacterId, "Test Resource", maxUses, ResourceDirection.Accumulating, SourceCategory.Custom, [ResetTrigger.LongRest]).Value;

	[Fact]
	public void Create_WithValidData_Succeeds()
	{
		var result = GenericResource.Create(CharacterId, "Action", 1, ResourceDirection.Spending, SourceCategory.Core, [ResetTrigger.PerRound]);

		Assert.True(result.IsSuccess);
		Assert.Equal("Action", result.Value.Name);
		Assert.Equal(1, result.Value.MaxUses);
		Assert.Equal(ResourceDirection.Spending, result.Value.Direction);
	}

	[Theory]
	[InlineData("")]
	[InlineData("   ")]
	public void Create_WithEmptyName_Fails(string name)
	{
		var result = GenericResource.Create(CharacterId, name, 1, ResourceDirection.Spending, SourceCategory.Core, [ResetTrigger.Manual]);

		Assert.True(result.IsFailure);
	}

	[Fact]
	public void Create_WithNegativeMaxUses_Fails()
	{
		var result = GenericResource.Create(CharacterId, "Action", -1, ResourceDirection.Spending, SourceCategory.Core, [ResetTrigger.Manual]);

		Assert.True(result.IsFailure);
	}

	[Fact]
	public void Create_SpendingResource_StartsAtMax()
	{
		var resource = CreateSpending(maxUses: 3);

		Assert.Equal(3, resource.CurrentUses);
		Assert.Equal(3, resource.MaxUses);
	}

	[Fact]
	public void Create_AccumulatingResource_StartsAtZero()
	{
		var resource = CreateAccumulating(maxUses: 6);

		Assert.Equal(0, resource.CurrentUses);
		Assert.Equal(6, resource.MaxUses);
	}

	[Fact]
	public void Use_DecrementsCurrentUses()
	{
		var resource = CreateSpending(maxUses: 3);

		var result = resource.Use();

		Assert.True(result.IsSuccess);
		Assert.Equal(2, resource.CurrentUses);
	}

	[Fact]
	public void Use_WhenEmpty_Fails()
	{
		var resource = CreateSpending(maxUses: 1);
		resource.Use();

		var result = resource.Use();

		Assert.True(result.IsFailure);
		Assert.Equal(0, resource.CurrentUses);
	}

	[Fact]
	public void Apply_IncrementsCurrentUses()
	{
		var resource = CreateAccumulating(maxUses: 6);

		var result = resource.Apply();

		Assert.True(result.IsSuccess);
		Assert.Equal(1, resource.CurrentUses);
	}

	[Fact]
	public void Apply_WhenFull_Fails()
	{
		var resource = CreateAccumulating(maxUses: 1);
		resource.Apply();

		var result = resource.Apply();

		Assert.True(result.IsFailure);
		Assert.Equal(1, resource.CurrentUses);
	}

	[Fact]
	public void Restore_SpendingResource_SetsToMax()
	{
		var resource = CreateSpending(maxUses: 3);
		resource.Use();
		resource.Use();
		Assert.Equal(1, resource.CurrentUses);

		resource.Restore();

		Assert.Equal(3, resource.CurrentUses);
	}

	[Fact]
	public void Restore_AccumulatingResource_SetsToZero()
	{
		var resource = CreateAccumulating(maxUses: 6);
		resource.Apply();
		resource.Apply();
		Assert.Equal(2, resource.CurrentUses);

		resource.Restore();

		Assert.Equal(0, resource.CurrentUses);
	}

	[Fact]
	public void HasResetTrigger_ReturnsTrue_WhenTriggerMatches()
	{
		var resource = GenericResource.Create(CharacterId, "Action", 1, ResourceDirection.Spending, SourceCategory.Core, [ResetTrigger.PerRound, ResetTrigger.Manual]).Value;

		Assert.True(resource.HasResetTrigger(ResetTrigger.PerRound));
		Assert.True(resource.HasResetTrigger(ResetTrigger.Manual));
	}

	[Fact]
	public void HasResetTrigger_ReturnsFalse_WhenTriggerDoesNotMatch()
	{
		var resource = GenericResource.Create(CharacterId, "Action", 1, ResourceDirection.Spending, SourceCategory.Core, [ResetTrigger.PerRound]).Value;

		Assert.False(resource.HasResetTrigger(ResetTrigger.LongRest));
	}
}
