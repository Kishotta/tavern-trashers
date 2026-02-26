using Shouldly;
using TavernTrashers.Api.Common.Domain.Tests;
using TavernTrashers.Api.Modules.Characters.Domain.Classes;

namespace TavernTrashers.Api.Modules.Characters.Domain.Tests.Classes;

public class ClassTests : TestBase
{
	[Fact]
	public void Create_WithValidName_Succeeds()
	{
		var result = Class.Create("Fighter");

		result.IsSuccess.ShouldBeTrue();
		result.Value.Name.ShouldBe("Fighter");
		result.Value.IsDeleted.ShouldBeFalse();
	}

	[Fact]
	public void Create_TrimsWhitespace()
	{
		var result = Class.Create("  Wizard  ");

		result.IsSuccess.ShouldBeTrue();
		result.Value.Name.ShouldBe("Wizard");
	}

	[Theory]
	[InlineData("")]
	[InlineData("   ")]
	public void Create_WithEmptyName_Fails(string name)
	{
		var result = Class.Create(name);

		result.IsFailure.ShouldBeTrue();
		result.Error.Code.ShouldBe("Classes.InvalidName");
	}

	[Fact]
	public void Rename_WithValidName_UpdatesName()
	{
		var @class = Class.Create("Fighter").Value;

		var result = @class.Rename("Barbarian");

		result.IsSuccess.ShouldBeTrue();
		@class.Name.ShouldBe("Barbarian");
	}

	[Fact]
	public void Rename_TrimsWhitespace()
	{
		var @class = Class.Create("Fighter").Value;

		var result = @class.Rename("  Rogue  ");

		result.IsSuccess.ShouldBeTrue();
		@class.Name.ShouldBe("Rogue");
	}

	[Theory]
	[InlineData("")]
	[InlineData("   ")]
	public void Rename_WithEmptyName_Fails(string name)
	{
		var @class = Class.Create("Fighter").Value;

		var result = @class.Rename(name);

		result.IsFailure.ShouldBeTrue();
		result.Error.Code.ShouldBe("Classes.InvalidName");
	}

	[Fact]
	public void Delete_SetsDeletedAt()
	{
		var @class = Class.Create("Fighter").Value;

		@class.Delete();

		@class.IsDeleted.ShouldBeTrue();
		@class.DeletedAt.ShouldNotBeNull();
	}

	[Fact]
	public void Delete_SetsPreciseTimestamp()
	{
		var before = DateTime.UtcNow;
		var @class = Class.Create("Fighter").Value;

		@class.Delete();

		var after = DateTime.UtcNow;
		@class.DeletedAt.ShouldNotBeNull();
		@class.DeletedAt!.Value.ShouldBeInRange(before, after);
	}
}
