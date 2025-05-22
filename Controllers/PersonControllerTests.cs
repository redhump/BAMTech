using System.Net;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using StargateAPI.Business.Commands;
using StargateAPI.Business.Queries;
using StargateAPI.Controllers;

namespace StargateAPI.Test.Controllers
{
	public class PersonControllerTests
	{

		private readonly Mock<IMediator> mediatorMock;
		private readonly PersonController personController;

		public PersonControllerTests()
		{
			mediatorMock = new Mock<IMediator>();
			personController = new PersonController(mediatorMock.Object);
		}



		[Fact]
		public async Task GetPeople_ReturnsOk_WhenSuccessful()
		{
            // Arrange
            var expectedResponse = new GetPeopleResult { Success = true };
            mediatorMock.Setup(m => m.Send(It.IsAny<GetPeople>(), It.IsAny<CancellationToken>()))
                         .ReturnsAsync(expectedResponse);

            // Act
            var result = await personController.GetPeople();

            // Assert
            var okResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal((int)HttpStatusCode.OK, okResult.StatusCode);
        }

        [Fact]
        public async Task GetPeople_ReturnsError_OnException()
        {
            // Arrange
            mediatorMock.Setup(m => m.Send(It.IsAny<GetPeople>(), It.IsAny<CancellationToken>()))
                         .ThrowsAsync(new System.Exception("Some error"));

            // Act
            var result = await personController.GetPeople();

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result);
            var response = Assert.IsType<BaseResponse>(objectResult.Value);
            Assert.False(response.Success);
            Assert.Equal("Some error", response.Message);
        }


        [Fact]
        public async Task GetPersonByName_ReturnsOk_WhenSuccessful()
        {
            // Arrange
            var name = "John Doe";
            var expectedResponse = new GetPersonByNameResult { Success = true };
            mediatorMock.Setup(m => m.Send(It.Is<GetPersonByName>(r => r.Name == name), It.IsAny<CancellationToken>()))
                         .ReturnsAsync(expectedResponse);

            // Act
            var result = await personController.GetPersonByName(name);

            // Assert
            var okResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal((int)HttpStatusCode.OK, okResult.StatusCode);
        }

        [Fact]
        public async Task GetPersonByName_ReturnsError_OnException()
        {
            // Arrange
            var name = "Error Person";
            mediatorMock.Setup(m => m.Send(It.IsAny<GetPersonByName>(), It.IsAny<CancellationToken>()))
                         .ThrowsAsync(new System.Exception("Some error"));

            // Act
            var result = await personController.GetPersonByName(name);

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result);
            var response = Assert.IsType<BaseResponse>(objectResult.Value);
            Assert.False(response.Success);
            Assert.Equal("Some error", response.Message);
        }

        [Fact]
        public async Task CreatePerson_ReturnsOk_WhenSuccessful()
        {
            // Arrange
            var name = "Jane Doe";
            var expectedResponse = new CreatePersonResult { Success = true };
            mediatorMock.Setup(m => m.Send(It.Is<CreatePerson>(r => r.Name == name), It.IsAny<CancellationToken>()))
                         .ReturnsAsync(expectedResponse);

            // Act
            var result = await personController.CreatePerson(name);

            // Assert
            var okResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal((int)HttpStatusCode.OK, okResult.StatusCode);
        }

        [Fact]
        public async Task CreatePerson_ReturnsError_OnException()
        {
            // Arrange
            var name = "Error Person";
            mediatorMock.Setup(m => m.Send(It.IsAny<CreatePerson>(), It.IsAny<CancellationToken>()))
                         .ThrowsAsync(new System.Exception("Some error"));

            // Act
            var result = await personController.CreatePerson(name);

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result);
            var response = Assert.IsType<BaseResponse>(objectResult.Value);
            Assert.False(response.Success);
            Assert.Equal("Some error", response.Message);
        }


    }
}

