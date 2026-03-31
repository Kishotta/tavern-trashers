using TavernTrashers.Api.Modules.Characters.Domain.Characters.Events;
using TavernTrashers.Api.Modules.Characters.Domain.Resources;

namespace TavernTrashers.Api.Modules.Characters.Domain.Tests.Resources;

public class DeathSavingThrowsTests
{
	private static readonly Guid CharacterId = Guid.NewGuid();

	private static DeathSavingThrows CreateDefault() =>
		DeathSavingThrows.CreateDefault(CharacterId);

	[Fact]
	public void CreateDefault_InitializesSuccessesAndFailuresToZero()
	{
		var dst = CreateDefault();

		Assert.Equal(0, dst.Successes);
		Assert.Equal(0, dst.Failures);
		Assert.Equal(CharacterId, dst.CharacterId);
	}

	[Fact]
	public void RecordSuccess_IncrementsSuccessCounter()
	{
		var dst = CreateDefault();

		var result = dst.RecordSuccess();

		Assert.True(result.IsSuccess);
		Assert.Equal(1, dst.Successes);
	}

	[Fact]
	public void RecordSuccess_CanRecordUpToThreeSuccesses()
	{
		var dst = CreateDefault();

		dst.RecordSuccess();
		dst.RecordSuccess();
		var result = dst.RecordSuccess();

		Assert.True(result.IsSuccess);
		Assert.Equal(3, dst.Successes);
	}

	[Fact]
	public void RecordSuccess_AtMaximum_Fails()
	{
		var dst = CreateDefault();
		dst.RecordSuccess();
		dst.RecordSuccess();
		dst.RecordSuccess();

		var result = dst.RecordSuccess();

		Assert.True(result.IsFailure);
		Assert.Equal(3, dst.Successes);
	}

	[Fact]
	public void RecordFailure_IncrementsFailureCounter()
	{
		var dst = CreateDefault();

		var result = dst.RecordFailure();

		Assert.True(result.IsSuccess);
		Assert.Equal(1, dst.Failures);
	}

	[Fact]
	public void RecordFailure_CanRecordUpToThreeFailures()
	{
		var dst = CreateDefault();

		dst.RecordFailure();
		dst.RecordFailure();
		var result = dst.RecordFailure();

		Assert.True(result.IsSuccess);
		Assert.Equal(3, dst.Failures);
	}

	[Fact]
	public void RecordFailure_AtMaximum_Fails()
	{
		var dst = CreateDefault();
		dst.RecordFailure();
		dst.RecordFailure();
		dst.RecordFailure();

		var result = dst.RecordFailure();

		Assert.True(result.IsFailure);
		Assert.Equal(3, dst.Failures);
	}

	[Fact]
	public void Reset_SetsSuccessesAndFailuresToZero()
	{
		var dst = CreateDefault();
		dst.RecordSuccess();
		dst.RecordSuccess();
		dst.RecordFailure();

		dst.Reset();

		Assert.Equal(0, dst.Successes);
		Assert.Equal(0, dst.Failures);
	}

	[Fact]
	public void SuccessesAndFailures_AreTrackedIndependently()
	{
		var dst = CreateDefault();

		dst.RecordSuccess();
		dst.RecordSuccess();
		dst.RecordFailure();

		Assert.Equal(2, dst.Successes);
		Assert.Equal(1, dst.Failures);
	}

	[Fact]
	public void RecordSuccess_OnThirdSuccess_RaisesCharacterStabilizedDomainEvent()
	{
		var dst = CreateDefault();
		dst.RecordSuccess();
		dst.RecordSuccess();

		dst.RecordSuccess();

		var events = dst.GetDomainEvents();
		var stabilized = Assert.Single(events.OfType<CharacterStabilizedDomainEvent>());
		Assert.Equal(CharacterId, stabilized.CharacterId);
	}

	[Fact]
	public void RecordSuccess_BeforeThirdSuccess_DoesNotRaiseCharacterStabilizedDomainEvent()
	{
		var dst = CreateDefault();

		dst.RecordSuccess();
		dst.RecordSuccess();

		Assert.Empty(dst.GetDomainEvents().OfType<CharacterStabilizedDomainEvent>());
	}

	[Fact]
	public void RecordFailure_OnThirdFailure_RaisesCharacterDiedDomainEvent()
	{
		var dst = CreateDefault();
		dst.RecordFailure();
		dst.RecordFailure();

		dst.RecordFailure();

		var events = dst.GetDomainEvents();
		var died = Assert.Single(events.OfType<CharacterDiedDomainEvent>());
		Assert.Equal(CharacterId, died.CharacterId);
	}

	[Fact]
	public void RecordFailure_BeforeThirdFailure_DoesNotRaiseCharacterDiedDomainEvent()
	{
		var dst = CreateDefault();

		dst.RecordFailure();
		dst.RecordFailure();

		Assert.Empty(dst.GetDomainEvents().OfType<CharacterDiedDomainEvent>());
	}
}
