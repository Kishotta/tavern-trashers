using TavernTrashers.Api.Modules.Characters.Domain.Characters;

namespace TavernTrashers.Api.Modules.Characters.Domain.Tests.Characters;

public class ConditionTests
{
	private static readonly Guid OwnerId    = Guid.NewGuid();
	private static readonly Guid CampaignId = Guid.NewGuid();

	private static Character CreateCharacter() =>
		Character.Create("Test Character", 1, OwnerId, CampaignId).Value;

	[Fact]
	public void Create_HasNoConditions()
	{
		var character = CreateCharacter();

		Assert.Equal(Condition.None, character.Conditions);
	}

	[Theory]
	[InlineData(Condition.Blinded)]
	[InlineData(Condition.Charmed)]
	[InlineData(Condition.Deafened)]
	[InlineData(Condition.Frightened)]
	[InlineData(Condition.Grappled)]
	[InlineData(Condition.Invisible)]
	[InlineData(Condition.Poisoned)]
	[InlineData(Condition.Prone)]
	[InlineData(Condition.Restrained)]
	public void ApplyCondition_SetsCondition(Condition condition)
	{
		var character = CreateCharacter();

		character.ApplyCondition(condition);

		Assert.True(character.Conditions.HasFlag(condition));
	}

	[Theory]
	[InlineData(Condition.Blinded)]
	[InlineData(Condition.Charmed)]
	[InlineData(Condition.Deafened)]
	[InlineData(Condition.Frightened)]
	[InlineData(Condition.Grappled)]
	[InlineData(Condition.Invisible)]
	[InlineData(Condition.Poisoned)]
	[InlineData(Condition.Prone)]
	[InlineData(Condition.Restrained)]
	public void RemoveCondition_ClearsCondition(Condition condition)
	{
		var character = CreateCharacter();
		character.ApplyCondition(condition);

		character.RemoveCondition(condition);

		Assert.False(character.Conditions.HasFlag(condition));
	}

	[Theory]
	[InlineData(Condition.Paralyzed)]
	[InlineData(Condition.Petrified)]
	[InlineData(Condition.Stunned)]
	public void ApplyCondition_ImpliesIncapacitated(Condition condition)
	{
		var character = CreateCharacter();

		character.ApplyCondition(condition);

		Assert.True(character.Conditions.HasFlag(condition));
		Assert.True(character.Conditions.HasFlag(Condition.Incapacitated));
	}

	[Fact]
	public void ApplyCondition_Unconscious_ImpliesIncapacitatedAndProne()
	{
		var character = CreateCharacter();

		character.ApplyCondition(Condition.Unconscious);

		Assert.True(character.Conditions.HasFlag(Condition.Unconscious));
		Assert.True(character.Conditions.HasFlag(Condition.Incapacitated));
		Assert.True(character.Conditions.HasFlag(Condition.Prone));
	}

	[Theory]
	[InlineData(Condition.Paralyzed)]
	[InlineData(Condition.Petrified)]
	[InlineData(Condition.Stunned)]
	public void RemoveCondition_ClearsImpliedIncapacitated_WhenNoOtherConditionImpliesIt(Condition condition)
	{
		var character = CreateCharacter();
		character.ApplyCondition(condition);

		character.RemoveCondition(condition);

		Assert.False(character.Conditions.HasFlag(condition));
		Assert.False(character.Conditions.HasFlag(Condition.Incapacitated));
	}

	[Fact]
	public void RemoveCondition_Unconscious_ClearsImpliedIncapacitatedAndProne()
	{
		var character = CreateCharacter();
		character.ApplyCondition(Condition.Unconscious);

		character.RemoveCondition(Condition.Unconscious);

		Assert.False(character.Conditions.HasFlag(Condition.Unconscious));
		Assert.False(character.Conditions.HasFlag(Condition.Incapacitated));
		Assert.False(character.Conditions.HasFlag(Condition.Prone));
	}

	[Theory]
	[InlineData(Condition.Paralyzed, Condition.Stunned)]
	[InlineData(Condition.Paralyzed, Condition.Petrified)]
	[InlineData(Condition.Stunned, Condition.Petrified)]
	[InlineData(Condition.Paralyzed, Condition.Unconscious)]
	[InlineData(Condition.Stunned, Condition.Unconscious)]
	[InlineData(Condition.Petrified, Condition.Unconscious)]
	public void RemoveCondition_KeepsIncapacitated_WhenAnotherConditionStillImpliesIt(
		Condition firstCondition,
		Condition secondCondition)
	{
		var character = CreateCharacter();
		character.ApplyCondition(firstCondition);
		character.ApplyCondition(secondCondition);

		character.RemoveCondition(firstCondition);

		Assert.False(character.Conditions.HasFlag(firstCondition));
		Assert.True(character.Conditions.HasFlag(Condition.Incapacitated));
	}

	[Fact]
	public void RemoveCondition_Unconscious_ClearsProneEvenIfDirectlyApplied()
	{
		var character = CreateCharacter();
		character.ApplyCondition(Condition.Prone);
		character.ApplyCondition(Condition.Unconscious);

		character.RemoveCondition(Condition.Unconscious);

		Assert.False(character.Conditions.HasFlag(Condition.Unconscious));
		Assert.False(character.Conditions.HasFlag(Condition.Prone));
	}

	[Fact]
	public void RemoveCondition_WhenBothParalyzedAndStunnedActive_RemovingOneKeepsIncapacitated()
	{
		var character = CreateCharacter();
		character.ApplyCondition(Condition.Paralyzed);
		character.ApplyCondition(Condition.Stunned);

		character.RemoveCondition(Condition.Paralyzed);
		character.RemoveCondition(Condition.Stunned);

		Assert.False(character.Conditions.HasFlag(Condition.Incapacitated));
	}

	[Fact]
	public void ApplyCondition_Incapacitated_Directly_SetsIncapacitated()
	{
		var character = CreateCharacter();

		character.ApplyCondition(Condition.Incapacitated);

		Assert.True(character.Conditions.HasFlag(Condition.Incapacitated));
	}

	[Fact]
	public void RemoveCondition_Incapacitated_Directly_ClearsIncapacitated()
	{
		var character = CreateCharacter();
		character.ApplyCondition(Condition.Incapacitated);

		character.RemoveCondition(Condition.Incapacitated);

		Assert.False(character.Conditions.HasFlag(Condition.Incapacitated));
	}

	[Fact]
	public void AllThreeParentConditions_ThenRemoveAll_ClearsIncapacitated()
	{
		var character = CreateCharacter();
		character.ApplyCondition(Condition.Paralyzed);
		character.ApplyCondition(Condition.Petrified);
		character.ApplyCondition(Condition.Stunned);

		character.RemoveCondition(Condition.Paralyzed);
		Assert.True(character.Conditions.HasFlag(Condition.Incapacitated));

		character.RemoveCondition(Condition.Petrified);
		Assert.True(character.Conditions.HasFlag(Condition.Incapacitated));

		character.RemoveCondition(Condition.Stunned);
		Assert.False(character.Conditions.HasFlag(Condition.Incapacitated));
	}
}
