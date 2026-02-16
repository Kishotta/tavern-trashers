using MediatR;
using Shouldly;
using TavernTrashers.Api.Common.Application.Messaging;

namespace TavernTrashers.Api.Common.Application.Tests;

public class RequestExtensionsTests
{
	private class TestCommand : IBaseRequest
	{
	}

	[Fact]
	public void GetModuleName_Should_Extract_Module_Name_From_Namespace()
	{
		// Arrange
		var request = new TestCommand();

		// Act
		var moduleName = request.GetModuleName();

		// Assert
		moduleName.ShouldBe("Application");
	}
}
