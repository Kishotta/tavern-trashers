using Aspire.Hosting;
using Aspire.Hosting.ApplicationModel;
using Aspire.Hosting.Testing;

namespace TavernTrashers.AppHost.Tests;

public class AppHostFixture : IAsyncLifetime
{
	private DistributedApplication? _app;

	public DistributedApplication App => _app ?? throw new InvalidOperationException("App has not been initialized.");

	public async Task InitializeAsync()
	{
		var appHost = await DistributedApplicationTestingBuilder
			.CreateAsync<Projects.TavernTrashers_AppHost>(["--no-web"]);

		_app = await appHost.BuildAsync();

		await _app.StartAsync();

		await _app.ResourceNotifications
			.WaitForResourceAsync("api", KnownResourceStates.Running)
			.WaitAsync(TimeSpan.FromMinutes(5));
	}

	public async Task DisposeAsync()
	{
		if (_app is not null)
			await _app.DisposeAsync();
	}
}
