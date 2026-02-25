using Aspire.Hosting;
using Aspire.Hosting.ApplicationModel;
using Aspire.Hosting.Testing;

namespace TavernTrashers.AppHost.Tests;

public class AppHostFixture : IAsyncLifetime
{
	private DistributedApplication? _app;

	public DistributedApplication App => _app ?? throw new InvalidOperationException("App has not been initialized.");

	/// <summary>
	///     Creates an <see cref="HttpClient" /> for the given resource that bypasses SSL certificate
	///     validation. This is necessary in CI where the Aspire dev certificate is not trusted.
	/// </summary>
	public HttpClient CreateHttpClient(string resourceName, string? endpointName = null)
	{
		var endpoint = App.GetEndpoint(resourceName, endpointName);
		var handler  = new HttpClientHandler { ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator };
		return new HttpClient(handler) { BaseAddress = endpoint };
	}

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
