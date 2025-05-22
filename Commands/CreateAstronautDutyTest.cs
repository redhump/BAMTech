using System;
using Microsoft.EntityFrameworkCore;
using Moq;
using StargateAPI.Business.Commands;
using StargateAPI.Business.Data;
using System.Data;
using Dapper;
using Microsoft.AspNetCore.Http;

namespace StargateAPI.Test.Commands
{
	public class CreateAstronautDutyTest
    {
        private StargateContext GetDbContext()
        {
            var options = new DbContextOptionsBuilder<StargateContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            return new StargateContext(options);
        }

        [Fact]
        public async Task Handler_Should_Add_Astronaut_Duty_When_Valid()
        {
            // Arrange
            var context = GetDbContext();

            var person = new Person { Name = "John Doe" };
            await context.People.AddAsync(person);
            await context.SaveChangesAsync();

            var handler = new CreateAstronautDutyHandler(context);

            var request = new CreateAstronautDuty
            {
                Name = "John Doe",
                DutyTitle = "COMMANDER",
                Rank = "Captain",
                DutyStartDate = DateTime.Today
            };

            // Act
            var result = await handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Id.HasValue);

            var createdDuty = await context.AstronautDuties.FindAsync(result.Id.Value);
            Assert.NotNull(createdDuty);
            Assert.Equal("COMMANDER", createdDuty.DutyTitle);

            var createdDetails = await context.AstronautDetails
                .FirstOrDefaultAsync(ad => ad.PersonId == person.Id);
            Assert.NotNull(createdDetails);
            Assert.Equal("Captain", createdDetails.CurrentRank);
        }

        [Fact]
        public async Task PreProcessor_Should_ThrowError_If_Person_Not_Found()
        {
            // Arrange
            var context = GetDbContext();

            var preProcessor = new CreateAstronautDutyPreProcessor(context);

            var request = new CreateAstronautDuty
            {
                Name = "NotExist",
                DutyTitle = "COMMANDER",
                Rank = "Captain",
                DutyStartDate = DateTime.Today
            };

            // Act & Assert
            await Assert.ThrowsAsync<BadHttpRequestException>(() =>
                preProcessor.Process(request, CancellationToken.None));
        }

        [Fact]
        public async Task PreProcessor_Should_ThrowError_When_Duty_Already_Exists()
        {
            // Arrange
            var context = GetDbContext();

            var person = new Person { Name = "John Doe" };
            await context.People.AddAsync(person);

            var duty = new AstronautDuty
            {
                PersonId = person.Id,
                DutyTitle = "COMMANDER",
                Rank = "Captain",
                DutyStartDate = DateTime.Today
            };
            await context.AstronautDuties.AddAsync(duty);
            await context.SaveChangesAsync();

            var preProcessor = new CreateAstronautDutyPreProcessor(context);

            var request = new CreateAstronautDuty
            {
                Name = "John Doe",
                DutyTitle = "COMMANDER",
                Rank = "Captain",
                DutyStartDate = DateTime.Today
            };

            // Act & Assert
            await Assert.ThrowsAsync<BadHttpRequestException>(() =>
                preProcessor.Process(request, CancellationToken.None));
        }


        [Fact]
        public async Task Handler_Should_Set_CareerEndDate_When_Retired()
        {
            // Arrange
            var context = GetDbContext();

            var person = new Person { Name = "John Doe" };
            await context.People.AddAsync(person);
            await context.SaveChangesAsync();

            var handler = new CreateAstronautDutyHandler(context);

            var request = new CreateAstronautDuty
            {
                Name = "John Doe",
                DutyTitle = "RETIRED",
                Rank = "Admiral",
                DutyStartDate = new DateTime(2024, 5, 1).AddDays(-1)
            };

            // Act
            var result = await handler.Handle(request, CancellationToken.None);

            // Assert
            var astronautDetail = await context.AstronautDetails
                .FirstOrDefaultAsync(ad => ad.PersonId == person.Id);

            Assert.NotNull(astronautDetail);
            Assert.Equal(new DateTime(2024, 4, 30), astronautDetail.CareerEndDate);
        }

    }
}

