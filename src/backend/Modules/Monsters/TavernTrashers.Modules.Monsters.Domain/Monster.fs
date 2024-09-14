namespace TavernTrashers.Modules.Monsters.Domain

open System

type AbilityScores = {
    Strength: int
    Dexterity: int
    Constitution: int
    Intelligence: int
    Wisdom: int
    Charisma: int
}

type Monster = {
    Id: Guid
    Name: string
    AbilityScores: AbilityScores
}

type IMonsterRepository =
    abstract member GetAll: unit -> Monster list
    abstract member GetById: Guid -> Monster option
    abstract member Add: Monster -> unit
