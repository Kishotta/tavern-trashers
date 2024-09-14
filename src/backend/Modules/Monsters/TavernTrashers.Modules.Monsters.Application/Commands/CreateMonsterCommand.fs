namespace TavernTrashers.Modules.Monsters.Application.Commands

open System
open TavernTrashers.Modules.Monsters.Domain

type CreateMonsterCommand = {
    Name: string
    AbilityScores: AbilityScores
}

module CreateMonsterCommandHandler =
    let handle (repository: IMonsterRepository) (command: CreateMonsterCommand) =
        let monsterId = Guid.NewGuid()
        let monster = {
            Id = monsterId
            Name = command.Name
            AbilityScores = command.AbilityScores
        }
        repository.Add(monster)
        monster