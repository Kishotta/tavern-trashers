using Shouldly;
using TavernTrashers.Api.Common.Infrastructure.Clock;

namespace TavernTrashers.Api.Common.Infrastructure.Tests;

public class DateTimeProviderTests
{
	[Fact]
	public void UtcNow_Should_Return_Current_UTC_Time()
	{
		// Arrange
		var provider = new DateTimeProvider();
		var before = DateTime.UtcNow;

		// Act
		var actual = provider.UtcNow;
		var after = DateTime.UtcNow;

		// Assert
		actual.ShouldBeGreaterThanOrEqualTo(before);
		actual.ShouldBeLessThanOrEqualTo(after);
		actual.Kind.ShouldBe(DateTimeKind.Utc);
	}
}
