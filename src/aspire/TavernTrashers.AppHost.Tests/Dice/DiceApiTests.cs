using System.Net;
using System.Net.Http.Json;
using Shouldly;

namespace TavernTrashers.AppHost.Tests.Dice;

[Trait("Category", "Integration")]
public class DiceApiTests(AppHostFixture fixture) : IClassFixture<AppHostFixture>
{
	[Fact]
	public async Task GetRolls_ReturnsOk()
	{
		using var httpClient = fixture.CreateHttpClient("api");

		var response = await httpClient.GetAsync("/dice/rolls");

		response.StatusCode.ShouldBe(HttpStatusCode.OK);
	}

	[Fact]
	public async Task RollDice_WithValidExpression_ReturnsOk()
	{
		using var httpClient = fixture.CreateHttpClient("api");

		var response = await httpClient.PostAsJsonAsync(
			"/dice/rolls",
			new { Expression = "2d6" });

		response.StatusCode.ShouldBe(HttpStatusCode.OK);
	}

	[Fact]
	public async Task RollDice_WithInvalidExpression_ReturnsBadRequest()
	{
		using var httpClient = fixture.CreateHttpClient("api");

		var response = await httpClient.PostAsJsonAsync(
			"/dice/rolls",
			new { Expression = "not-a-dice-expression" });

		response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
	}
}
