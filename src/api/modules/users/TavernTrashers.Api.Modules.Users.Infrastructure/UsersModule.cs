using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using TavernTrashers.Api.Common.Application.Authorization;
using TavernTrashers.Api.Common.Infrastructure.Modules;
using TavernTrashers.Api.Common.SourceGenerators;
using TavernTrashers.Api.Modules.Users.Application.Abstractions.Identity;
using TavernTrashers.Api.Modules.Users.Domain.Users;
using TavernTrashers.Api.Modules.Users.Infrastructure.Authorization;
using TavernTrashers.Api.Modules.Users.Infrastructure.Database;
using TavernTrashers.Api.Modules.Users.Infrastructure.Identity;
using TavernTrashers.Api.Modules.Users.Infrastructure.Inbox;
using TavernTrashers.Api.Modules.Users.Infrastructure.Outbox;
using TavernTrashers.Api.Modules.Users.Infrastructure.Users;
using Module = TavernTrashers.Api.Common.Infrastructure.Modules.Module;

namespace TavernTrashers.Api.Modules.Users.Infrastructure;

[GenerateModuleBoilerplate(ModuleName, ModuleSchema)]
public class UsersModule : Module
{
	public const string ModuleName = "Users";
	public const string ModuleSchema = "users";
	public override string Name => ModuleName;
	public override string Schema => ModuleSchema;

	public override Assembly ApplicationAssembly => Application.AssemblyReference.Assembly;
	public override Assembly PresentationAssembly => Presentation.AssemblyReference.Assembly;

	protected override void AddDatabase(IHostApplicationBuilder builder) =>
		builder
		   .AddStandardModuleDatabase<UsersDbContext>(Name, Schema)
		   .AddScoped<IUserRepository, UserRepository>();

	protected override void ConfigureServices(IHostApplicationBuilder builder)
	{
		ConfigureOutbox<OutboxOptions, ProcessOutboxJob, ConfigureProcessOutboxJob>(builder);
		ConfigureInbox<InboxOptions, ProcessInboxJob, ConfigureProcessInboxJob>(builder);

		builder.Services.AddScoped<IPermissionService, PermissionService>();

		builder.Services
		   .Configure<KeyCloakOptions>(builder.Configuration.GetSection($"{Name}:KeyCloak"))
		   .AddTransient<IIdentityProviderService, KeyCloakIdentityProviderService>()
		   .AddTransient<KeyCloakAuthDelegatingHandler>();

		builder.Services
		   .AddHttpClient<IKeyCloakClient, KeyCloakClient>((serviceProvider, httpClient) =>
			{
				var keyCloakOptions = serviceProvider.GetRequiredService<IOptions<KeyCloakOptions>>().Value;
				httpClient.BaseAddress = new(keyCloakOptions.AdminUrl);
			})
		   .AddHttpMessageHandler<KeyCloakAuthDelegatingHandler>();

		builder.Services.AddHttpClient<KeyCloakTokenClient>((serviceProvider, httpClient) =>
		{
			var keyCloakOptions = serviceProvider.GetRequiredService<IOptions<KeyCloakOptions>>().Value;
			httpClient.BaseAddress = new(keyCloakOptions.TokenUrl);
		});
	}
}