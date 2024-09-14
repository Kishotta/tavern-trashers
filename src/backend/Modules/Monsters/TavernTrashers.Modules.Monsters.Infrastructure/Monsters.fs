namespace TavernTrashers.Modules.Monsters.Infrastructure

open System
open Microsoft.Extensions.DependencyInjection
open TavernTrashers.Modules.Monsters.Domain

type InMemoryMonsterRepository() =
    let mutable monsters : Monster list = []
    
    interface IMonsterRepository with
        member this.GetAll() = monsters
        member this.GetById(id: Guid) = monsters |> List.tryFind (fun m -> m.Id = id)
        member this.Add(monster: Monster) = monsters <- monster :: monsters

type MonsterModule =
   static member addModule (services: IServiceCollection) =
        services.AddSingleton<IMonsterRepository, InMemoryMonsterRepository>() |> ignore