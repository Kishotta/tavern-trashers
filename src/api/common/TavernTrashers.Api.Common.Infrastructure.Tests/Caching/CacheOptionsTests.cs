using Shouldly;
using TavernTrashers.Api.Common.Application.Caching;
using TavernTrashers.Api.Common.Infrastructure.Caching;

namespace TavernTrashers.Api.Common.Infrastructure.Tests.Caching;

public class CacheOptionsTests
{
	[Fact]
	public void Create_Should_Return_Default_When_Expiration_Is_Null()
	{
		// Act
		var options = CacheOptions.Create(null);

		// Assert
		options.AbsoluteExpirationRelativeToNow.ShouldBe(TimeSpan.FromMinutes(2));
		options.SlidingExpiration.ShouldBeNull();
	}

	[Fact]
	public void Create_Should_Return_Absolute_Expiration_When_Type_Is_Absolute()
	{
		// Arrange
		var expiration = TimeSpan.FromMinutes(10);

		// Act
		var options = CacheOptions.Create(expiration, CacheExpirationType.Absolute);

		// Assert
		options.AbsoluteExpirationRelativeToNow.ShouldBe(expiration);
		options.SlidingExpiration.ShouldBeNull();
	}

	[Fact]
	public void Create_Should_Return_Sliding_Expiration_When_Type_Is_Sliding()
	{
		// Arrange
		var expiration = TimeSpan.FromMinutes(15);

		// Act
		var options = CacheOptions.Create(expiration, CacheExpirationType.Sliding);

		// Assert
		options.SlidingExpiration.ShouldBe(expiration);
		options.AbsoluteExpirationRelativeToNow.ShouldBeNull();
	}

	[Fact]
	public void Create_Should_Default_To_Absolute_When_Type_Not_Specified()
	{
		// Arrange
		var expiration = TimeSpan.FromMinutes(5);

		// Act
		var options = CacheOptions.Create(expiration);

		// Assert
		options.AbsoluteExpirationRelativeToNow.ShouldBe(expiration);
		options.SlidingExpiration.ShouldBeNull();
	}
}
