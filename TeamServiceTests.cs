using Microsoft.EntityFrameworkCore;
using VolleyballFinal.Controllers.Service;
using VolleyballFinal.Models;
using Xunit;

namespace VolleyballFinalTests
{
    public class TeamServiceTests
    {
        private readonly TeamContext _context;
        private readonly TeamService _service;

        public TeamServiceTests()
        {
            var options = new DbContextOptionsBuilder<TeamContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            _context = new TeamContext(options);

            SeedDatabase();
            _service = new TeamService(_context);
        }

        private void SeedDatabase()
        {
            _context.Teams.AddRange(
                new Team { Id = 1, TeamName = "Argentina", Location = "Argentina", LeagueType = "2020 Olympics", Category = "Indoor", Gender = "Men" },
                new Team { Id = 2, TeamName = "Brazil", Location = "Brazil", LeagueType = "2020 Olympics", Category = "Indoor", Gender = "Men" },
                new Team { Id = 3, TeamName = "Canada", Location = "Canada", LeagueType = "2020 Olympics", Category = "Indoor", Gender = "Men" }
            );
            _context.SaveChanges();
        }

        public void Dispose()
        {
            _context.Dispose();
        }

        [Fact]
        public void GetAllTeams_ReturnsAllTeams()
        {
            // Act
            var result = _service.GetAllTeams();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(3, result.Count);
            Assert.Contains(result, t => t.TeamName == "Argentina");
        }

        [Fact]
        public void GetTeamsByLeague_WithValidLeagueType_ReturnsFilteredTeams()
        {
            // Act
            var result = _service.GetTeamsByLeague("2020 Olympics");

            // Assert
            Assert.NotNull(result);
            Assert.Equal(3, result.Count); 
        }

        [Fact]
        public void GetTeamsByLeague_WithInvalidLeagueType_ReturnsEmptyList()
        {
            // Act
            var result = _service.GetTeamsByLeague("NonExistingLeague");

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public void GetTeam_WithValidId_ReturnsCorrectTeam()
        {
            // Act
            var result = _service.GetTeam(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Argentina", result.TeamName);
        }

        [Fact]
        public void GetTeam_WithInvalidId_ReturnsNull()
        {
            // Act
            var result = _service.GetTeam(99);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void AddTeam_WithNewTeam_AddsTeam()
        {
            // Arrange
            var newTeam = new Team { Id = 4, TeamName = "France", Location = "France", LeagueType = "2020 Olympics", Category = "Indoor", Gender = "Men" };

            // Act
            _service.AddTeam(newTeam);
            _context.SaveChanges();

            // Assert
            var teamInDb = _context.Teams.Find(newTeam.Id);
            Assert.NotNull(teamInDb);
            Assert.Equal("France", teamInDb.TeamName);
        }

        [Fact]
        public void UpdateTeam_WithExistingTeam_UpdatesTeam()
        {
            // Arrange
            var teamToUpdate = _context.Teams.Find(1);
            teamToUpdate.Location = "New Argentina";

            // Act
            _service.UpdateTeam(teamToUpdate);
            _context.SaveChanges();

            // Assert
            var updatedTeam = _context.Teams.Find(1);
            Assert.Equal("New Argentina", updatedTeam.Location);
        }

        [Fact]
        public void DeleteTeam_WithExistingTeam_DeletesTeam()
        {
            // Arrange
            var teamToDelete = _context.Teams.Find(1);

            // Act
            _service.DeleteTeam(teamToDelete);
            _context.SaveChanges();

            // Assert
            var deletedTeam = _context.Teams.Find(1);
            Assert.Null(deletedTeam);
        }
    }
}