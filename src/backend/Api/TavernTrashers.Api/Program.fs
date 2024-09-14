open Microsoft.AspNetCore.Builder
open Microsoft.Extensions.DependencyInjection
open TavernTrashers.Modules.Monsters.Infrastructure

let builder = WebApplication.CreateBuilder()

builder.Services.AddControllers() |> ignore

MonsterModule.addModule builder.Services

let app = builder.Build()

app.MapControllers() |> ignore

app.Run()