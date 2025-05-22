using System;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using StargateAPI.Business.Commands;
using StargateAPI.Business.Data;

namespace StargateAPI.Test.Commands
{
    public class CreatePersonTests
    {
        private StargateContext GetDbContext()
        {
            var options = new DbContextOptionsBuilder<StargateContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            return new StargateContext(options);
        }

        [Fact]
        public async Task Handler_Should_Add_New_Person_And_Return_Id()
        {
            // Arrange
            var context = GetDbContext();

            var handler = new CreatePersonHandler(context);
            var command = new CreatePerson { Name = "John Doe" };

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().BeGreaterThan(0);
            var person = await context.People.FindAsync(result.Id);
            person.Should().NotBeNull();
            person!.Name.Should().Be("John Doe");
        }

        [Fact]
        public async Task PreProcessor_Should_Throw_If_Person_Exists()
        {
            // Arrange
            var context = GetDbContext();
            context.People.Add(new Person { Name = "Jane Doe" });
            await context.SaveChangesAsync();

            var preProcessor = new CreatePersonPreProcessor(context);
            var command = new CreatePerson { Name = "Jane Doe" };

            // Act
            var act = async () => await preProcessor.Process(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<BadHttpRequestException>()
                .WithMessage("Bad Request");
        }

        [Fact]
        public async Task PreProcessor_Should_Pass_If_Person_Does_Not_Exist()
        {
            // Arrange
            var context = GetDbContext();
            var preProcessor = new CreatePersonPreProcessor(context);

            var command = new CreatePerson { Name = "New Astronaut" };

            // Act & Assert
            await preProcessor.Invoking(p => p.Process(command, CancellationToken.None))
                .Should().NotThrowAsync();
        }
    }
}
