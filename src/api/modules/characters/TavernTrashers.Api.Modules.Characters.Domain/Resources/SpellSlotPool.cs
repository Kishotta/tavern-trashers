using TavernTrashers.Api.Common.Domain.Entities;
using TavernTrashers.Api.Common.Domain.Results;

namespace TavernTrashers.Api.Modules.Characters.Domain.Resources;

public sealed class SpellSlotPool : Entity
{
	private const int MinLevel = 1;
	private const int MaxLevel = 9;

	private readonly List<SpellSlotLevel> _levels = [];
	private SpellSlotPool() { }

	public Guid CharacterId { get; private set; }
	public SpellSlotPoolKind Kind { get; private set; }
	public IReadOnlyCollection<SpellSlotLevel> Levels => _levels.AsReadOnly();

	public static SpellSlotPool CreateDefault(Guid characterId) =>
		CreatePool(characterId, SpellSlotPoolKind.Standard);

	public static SpellSlotPool CreatePactMagic(Guid characterId) =>
		CreatePool(characterId, SpellSlotPoolKind.PactMagic);

	public Result UseSlot(int level)
	{
		if (level is < MinLevel or > MaxLevel)
			return SpellSlotPoolErrors.InvalidLevel(level);

		var slotLevel = _levels.SingleOrDefault(l => l.Level == level);
		if (slotLevel is null)
			return SpellSlotPoolErrors.InvalidLevel(level);

		return slotLevel.Use();
	}

	public void Restore()
	{
		foreach (var level in _levels)
			level.Restore();
	}

	public Result SetMaxSlots(int level, int max)
	{
		if (level is < MinLevel or > MaxLevel)
			return SpellSlotPoolErrors.InvalidLevel(level);

		var slotLevel = _levels.SingleOrDefault(l => l.Level == level);
		if (slotLevel is null)
			return SpellSlotPoolErrors.InvalidLevel(level);

		return slotLevel.SetMax(max);
	}

	public ResetTrigger GetResetTrigger() => Kind switch
	{
		SpellSlotPoolKind.Standard  => ResetTrigger.LongRest,
		SpellSlotPoolKind.PactMagic => ResetTrigger.ShortRest,
		_                           => ResetTrigger.Manual,
	};

	private static SpellSlotPool CreatePool(Guid characterId, SpellSlotPoolKind kind)
	{
		var pool = new SpellSlotPool
		{
			Id          = Guid.NewGuid(),
			CharacterId = characterId,
			Kind        = kind,
		};

		for (var level = MinLevel; level <= MaxLevel; level++)
			pool._levels.Add(SpellSlotLevel.CreateDefault(pool.Id, level));

		return pool;
	}
}
