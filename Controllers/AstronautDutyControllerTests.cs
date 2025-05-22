using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using StargateAPI.Business.Commands;
using StargateAPI.Controllers;
using System.Net;
using StargateAPI.Business.Queries;
using FluentAssertions;

namespace StargateAPI.Test.Controllers
{
	public class AstronautDutyControllerTests
	{
        private readonly Mock<IMediator> mediatorMock;
        private readonly AstronautDutyController astronautController;

        public AstronautDutyControllerTests()
        {
            mediatorMock = new Mock<IMediator>();
            astronautController = new AstronautDutyController(mediatorMock.Object);
        }

        [Fact]
        public async Task GetAstronautDutiesByName_ReturnsOk_WhenSuccess()
        {
            // Arrange
            var name = "John Doe";
            var expectedResponse = new GetPersonByNameResult { Success = true };
            mediatorMock
                .Setup(m => m.Send(It.IsAny<GetPersonByName>(), default))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await astronautController.GetAstronautDutiesByName(name);

            // Assert
            var okResult = result as ObjectResult;
            okResult.Should().NotBeNull();
            okResult.StatusCode.Should().Be((int)HttpStatusCode.OK);
            okResult.Value.Should().Be(expectedResponse);
        }

        [Fact]
        public async Task GetAstronautDutiesByName_ReturnsError_WhenExceptionThrown()
        {
            // Arrange
            var name = "Jane Doe";
            mediatorMock
                .Setup(m => m.Send(It.IsAny<GetPersonByName>(), default))
                .ThrowsAsync(new Exception("Unexpected error"));

            // Act
            var result = await astronautController.GetAstronautDutiesByName(name);

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result);
            var response = Assert.IsType<BaseResponse>(objectResult.Value);
            Assert.False(response.Success);
            Assert.Equal("Unexpected error", response.Message);
        }

        [Fact]
        public async Task CreateAstronautDuty_ReturnsOk_WhenSuccess()
        {
            // Arrange
            var request = new CreateAstronautDuty { Name = "John", Rank = "1LT", DutyTitle = "Commander" };
            var expectedResponse = new CreateAstronautDutyResult { Success = true };
            mediatorMock
                .Setup(m => m.Send(request, default))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await astronautController.CreateAstronautDuty(request);

            // Assert
            var okResult = result as ObjectResult;
            okResult.Should().NotBeNull();
            okResult.StatusCode.Should().Be((int)HttpStatusCode.OK);
            okResult.Value.Should().Be(expectedResponse);
        }

    }
}

