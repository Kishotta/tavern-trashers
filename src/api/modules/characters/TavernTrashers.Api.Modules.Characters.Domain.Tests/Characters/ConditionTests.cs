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

		Assert.Equal(Conditions.None, character.Conditions);
	}

	[Theory]
	[InlineData(Conditions.Blinded)]
	[InlineData(Conditions.Charmed)]
	[InlineData(Conditions.Deafened)]
	[InlineData(Conditions.Frightened)]
	[InlineData(Conditions.Grappled)]
	[InlineData(Conditions.Invisible)]
	[InlineData(Conditions.Poisoned)]
	[InlineData(Conditions.Prone)]
	[InlineData(Conditions.Restrained)]
	public void ApplyCondition_SetsCondition(Conditions condition)
	{
		var character = CreateCharacter();

		character.ApplyCondition(condition);

		Assert.True(character.Conditions.HasFlag(condition));
	}

	[Theory]
	[InlineData(Conditions.Blinded)]
	[InlineData(Conditions.Charmed)]
	[InlineData(Conditions.Deafened)]
	[InlineData(Conditions.Frightened)]
	[InlineData(Conditions.Grappled)]
	[InlineData(Conditions.Invisible)]
	[InlineData(Conditions.Poisoned)]
	[InlineData(Conditions.Prone)]
	[InlineData(Conditions.Restrained)]
	public void RemoveCondition_ClearsCondition(Conditions condition)
	{
		var character = CreateCharacter();
		character.ApplyCondition(condition);

		character.RemoveCondition(condition);

		Assert.False(character.Conditions.HasFlag(condition));
	}

	[Theory]
	[InlineData(Conditions.Paralyzed)]
	[InlineData(Conditions.Petrified)]
	[InlineData(Conditions.Stunned)]
	public void ApplyCondition_ImpliesIncapacitated(Conditions condition)
	{
		var character = CreateCharacter();

		character.ApplyCondition(condition);

		Assert.True(character.Conditions.HasFlag(condition));
		Assert.True(character.Conditions.HasFlag(Conditions.Incapacitated));
	}

	[Fact]
	public void ApplyCondition_Unconscious_ImpliesIncapacitatedAndProne()
	{
		var character = CreateCharacter();

		character.ApplyCondition(Conditions.Unconscious);

		Assert.True(character.Conditions.HasFlag(Conditions.Unconscious));
		Assert.True(character.Conditions.HasFlag(Conditions.Incapacitated));
		Assert.True(character.Conditions.HasFlag(Conditions.Prone));
	}

	[Theory]
	[InlineData(Conditions.Paralyzed)]
	[InlineData(Conditions.Petrified)]
	[InlineData(Conditions.Stunned)]
	public void RemoveCondition_ClearsImpliedIncapacitated_WhenNoOtherConditionImpliesIt(Conditions condition)
	{
		var character = CreateCharacter();
		character.ApplyCondition(condition);

		character.RemoveCondition(condition);

		Assert.False(character.Conditions.HasFlag(condition));
		Assert.False(character.Conditions.HasFlag(Conditions.Incapacitated));
	}

	[Fact]
	public void RemoveCondition_Unconscious_ClearsImpliedIncapacitatedAndProne()
	{
		var character = CreateCharacter();
		character.ApplyCondition(Conditions.Unconscious);

		character.RemoveCondition(Conditions.Unconscious);

		Assert.False(character.Conditions.HasFlag(Conditions.Unconscious));
		Assert.False(character.Conditions.HasFlag(Conditions.Incapacitated));
		Assert.False(character.Conditions.HasFlag(Conditions.Prone));
	}

	[Theory]
	[InlineData(Conditions.Paralyzed, Conditions.Stunned)]
	[InlineData(Conditions.Paralyzed, Conditions.Petrified)]
	[InlineData(Conditions.Stunned, Conditions.Petrified)]
	[InlineData(Conditions.Paralyzed, Conditions.Unconscious)]
	[InlineData(Conditions.Stunned, Conditions.Unconscious)]
	[InlineData(Conditions.Petrified, Conditions.Unconscious)]
	public void RemoveCondition_KeepsIncapacitated_WhenAnotherConditionStillImpliesIt(
		Conditions firstCondition,
		Conditions secondCondition)
	{
		var character = CreateCharacter();
		character.ApplyCondition(firstCondition);
		character.ApplyCondition(secondCondition);

		character.RemoveCondition(firstCondition);

		Assert.False(character.Conditions.HasFlag(firstCondition));
		Assert.True(character.Conditions.HasFlag(Conditions.Incapacitated));
	}

	[Fact]
	public void RemoveCondition_Unconscious_ClearsProneEvenIfDirectlyApplied()
	{
		var character = CreateCharacter();
		character.ApplyCondition(Conditions.Prone);
		character.ApplyCondition(Conditions.Unconscious);

		character.RemoveCondition(Conditions.Unconscious);

		Assert.False(character.Conditions.HasFlag(Conditions.Unconscious));
		Assert.False(character.Conditions.HasFlag(Conditions.Prone));
	}

	[Fact]
	public void RemoveCondition_WhenBothParalyzedAndStunnedActive_RemovingOneKeepsIncapacitated()
	{
		var character = CreateCharacter();
		character.ApplyCondition(Conditions.Paralyzed);
		character.ApplyCondition(Conditions.Stunned);

		character.RemoveCondition(Conditions.Paralyzed);
		character.RemoveCondition(Conditions.Stunned);

		Assert.False(character.Conditions.HasFlag(Conditions.Incapacitated));
	}

	[Fact]
	public void ApplyCondition_Incapacitated_Directly_SetsIncapacitated()
	{
		var character = CreateCharacter();

		character.ApplyCondition(Conditions.Incapacitated);

		Assert.True(character.Conditions.HasFlag(Conditions.Incapacitated));
	}

	[Fact]
	public void RemoveCondition_Incapacitated_Directly_ClearsIncapacitated()
	{
		var character = CreateCharacter();
		character.ApplyCondition(Conditions.Incapacitated);

		character.RemoveCondition(Conditions.Incapacitated);

		Assert.False(character.Conditions.HasFlag(Conditions.Incapacitated));
	}

	[Fact]
	public void AllThreeParentConditions_ThenRemoveAll_ClearsIncapacitated()
	{
		var character = CreateCharacter();
		character.ApplyCondition(Conditions.Paralyzed);
		character.ApplyCondition(Conditions.Petrified);
		character.ApplyCondition(Conditions.Stunned);

		character.RemoveCondition(Conditions.Paralyzed);
		Assert.True(character.Conditions.HasFlag(Conditions.Incapacitated));

		character.RemoveCondition(Conditions.Petrified);
		Assert.True(character.Conditions.HasFlag(Conditions.Incapacitated));

		character.RemoveCondition(Conditions.Stunned);
		Assert.False(character.Conditions.HasFlag(Conditions.Incapacitated));
	}
}
