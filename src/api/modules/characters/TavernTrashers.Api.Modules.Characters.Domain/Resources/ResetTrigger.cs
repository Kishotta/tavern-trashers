namespace TavernTrashers.Api.Modules.Characters.Domain.Resources;

[Flags]
public enum ResetTrigger
{
	None      = 0,
	PerRound  = 1,
	ShortRest = 2,
	LongRest  = 4,
	Manual    = 8,
}
