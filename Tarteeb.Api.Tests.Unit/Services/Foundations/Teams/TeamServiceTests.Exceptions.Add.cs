﻿//=================================
// Copyright (c) Coalition of Good-Hearted Engineers
// Free to use to bring order in your workplace
//=================================

using System.Threading.Tasks;
using EFxceptions.Models.Exceptions;
using FluentAssertions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Moq;
using Tarteeb.Api.Models.Teams;
using Tarteeb.Api.Models.Teams.Exceptions;
using Xunit;

namespace Tarteeb.Api.Tests.Unit.Services.Foundations.Teams
{
    public partial class TeamServiceTests
    {
        [Fact]
        public async Task ShouldThrowCriticalDependencyExceptionOnAddIfSqlErrorOccursAndLogItAsync()
        {
            //given
            Team someTeam = CreateRandomTeam();
            SqlException sqlException = CreateSqlException();

            var failedTeamStorageException = new FailedTeamStorageException(sqlException);

            var expectedTeamDependencyException =
                new TeamDependencyException(failedTeamStorageException);

            this.storageBrokerMock.Setup(broker =>
                broker.InsertTeamAsync(It.IsAny<Team>())).ThrowsAsync(sqlException);

            //when
            ValueTask<Team> addTeamTask = this.teamService.AddTeamAsync(someTeam);

            TeamDependencyException actualTeamDependencyException =
                await Assert.ThrowsAsync<TeamDependencyException>(addTeamTask.AsTask);

            //then
            actualTeamDependencyException.Should().BeEquivalentTo(expectedTeamDependencyException);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertTeamAsync(It.IsAny<Team>()), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCritical(It.Is(SameExceptionAs(
                    expectedTeamDependencyException))),Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldTrowDependencyValidationExceptionOnAddIfDuplicateErrorOccursAndLogItAsync()
        {
            // given
            Team someTeam = CreateRandomTeam();
            string someMessage = GetRandomString();
            var duplicateKeyException = new DuplicateKeyException(someMessage);

            var alreadyExistsTicketException =
                new AlreadyExistsTeamException(duplicateKeyException);

            var expectedTeamDependencyValidationException =
                new TeamDependencyValidationException(alreadyExistsTicketException);

            this.storageBrokerMock.Setup(broker => broker.InsertTeamAsync(It.IsAny<Team>()))
                .ThrowsAsync(duplicateKeyException);

            // when
            ValueTask<Team> addTeamTask = this.teamService.AddTeamAsync(someTeam);

            TeamDependencyValidationException actualTeamDependencyValidationException =
                await Assert.ThrowsAsync<TeamDependencyValidationException>(addTeamTask.AsTask);

            // then
            actualTeamDependencyValidationException.Should().BeEquivalentTo(
                expectedTeamDependencyValidationException);

            this.storageBrokerMock.Verify(broker => broker.InsertTeamAsync(
                It.IsAny<Team>()),Times.Once);

            this.loggingBrokerMock.Verify(broker => broker.LogError(It.Is(SameExceptionAs(
                expectedTeamDependencyValidationException))), Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyValidationExceptionOnAddIfDbConcurrencyErrorOccursAndLogItAsync()
        {
            //given
            Team someTeam = CreateRandomTeam();
            var dbUpdateConcurrencyException = new DbUpdateConcurrencyException();
            var lockedTeamException = new LockedTeamException(dbUpdateConcurrencyException);

            var expectedTeamDependencyValidationException = 
                new TeamDependencyValidationException(lockedTeamException);

            this.storageBrokerMock.Setup(broker=>broker.InsertTeamAsync(It.IsAny<Team>()))
                .ThrowsAsync(dbUpdateConcurrencyException);
            
            //when
            ValueTask<Team> addTeamTask = this.teamService.AddTeamAsync(someTeam);
            var actualTeamDependencyValidationException =
                await Assert.ThrowsAsync<TeamDependencyValidationException>(addTeamTask.AsTask);

            //then
            actualTeamDependencyValidationException.Should()
                .BeEquivalentTo(expectedTeamDependencyValidationException);

            this.storageBrokerMock.Verify(broker => broker.InsertTeamAsync(It.IsAny<Team>()),Times.Once);

            this.loggingBrokerMock.Verify(broker => broker.LogError(It.Is(
                SameExceptionAs(expectedTeamDependencyValidationException))), Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}