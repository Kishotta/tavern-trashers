namespace TavernTrashers.Api.Controllers

open Microsoft.AspNetCore.Mvc
open TavernTrashers.Modules.Monsters.Application.Commands
open TavernTrashers.Modules.Monsters.Domain

[<ApiController>]
[<Route("api/[controller]")>]
type MonstersController() =
    inherit ControllerBase()

    [<HttpGet>]
    member this.Get(repository: IMonsterRepository) =
        repository.GetAll()
        
    [<HttpGet("{id}")>]
    member this.Get(id: int) =
        this.Ok("Hello from F# with id: " + id.ToString())
    
    [<HttpPost>]
    member this.Post([<FromBody>] command: CreateMonsterCommand, repository: IMonsterRepository) =
        CreateMonsterCommandHandler.handle repository command
        |> this.Ok