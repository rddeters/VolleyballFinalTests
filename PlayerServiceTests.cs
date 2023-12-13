using Microsoft.EntityFrameworkCore;
using VolleyballFinal.Controllers.Service;
using VolleyballFinal.Models;
using Xunit;

namespace VolleyballFinalTests
{
    public class PlayerServiceTests
    {
        private readonly TeamContext _context;
        private readonly PlayerService _service;

        public PlayerServiceTests()
        {
            var options = new DbContextOptionsBuilder<TeamContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            _context = new TeamContext(options);

            SeedDatabase();
            _service = new PlayerService(_context);
        }

        private void SeedDatabase()
        {
            _context.Player.AddRange(
                new Player { PlayerId = 1, PlayerName = "Matias Sanchez", Number = 1, TeamName = "Argentina", Position = "Setter", Age = "27", Height = "175cm" },
                new Player { PlayerId = 2, PlayerName = "Federico Pereyra", Number = 2, TeamName = "Argentina", Position = "Opposite Hitter", Age = "35", Height = "200cm" },
                new Player { PlayerId = 3, PlayerName = "Cristian Poglajen", Number = 6, TeamName = "Argentina", Position = "Outside Hitter", Age = "34", Height = "195cm" }
            );
            _context.SaveChanges();
        }

        public void Dispose()
        {
            _context.Dispose();
        }

        [Fact]
        public void GetAllPlayers_ReturnsAllPlayers()
        {
            // Act
            var result = _service.GetAllPlayers();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(3, result.Count);
            Assert.Contains(result, p => p.PlayerName == "Matias Sanchez");
        }

        [Fact]
        public void GetPlayersByPosition_WithValidPosition_ReturnsFilteredPlayers()
        {
            // Act
            var result = _service.GetPlayersByPosition("Setter");

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal("Matias Sanchez", result.First().PlayerName);
        }

        [Fact]
        public void GetPlayerById_WithValidId_ReturnsCorrectPlayer()
        {
            // Act
            var result = _service.GetPlayerById(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Matias Sanchez", result.PlayerName);
        }

        [Fact]
        public void AddOrUpdatePlayer_WithNewPlayer_AddsPlayer()
        {
            // Arrange
            var newPlayer = new Player { PlayerId = 4, PlayerName = "Facundo Conte", Number = 7, TeamName = "Argentina", Position = "Outside Hitter", Age = "34", Height = "197cm" };

            // Act
            _service.AddOrUpdatePlayer(newPlayer);
            _context.SaveChanges();

            // Assert
            var playerInDb = _context.Player.Find(4);
            Assert.NotNull(playerInDb);
            Assert.Equal("Facundo Conte", playerInDb.PlayerName);
        }

        [Fact]
        public void UpdatePlayer_WithExistingPlayer_UpdatesPlayer()
        {
            // Arrange
            var playerToUpdate = _context.Player.Find(1);
            playerToUpdate.Height = "180cm";

            // Act
            _service.AddOrUpdatePlayer(playerToUpdate);
            _context.SaveChanges();

            // Assert
            var updatedPlayer = _context.Player.Find(1);
            Assert.Equal("180cm", updatedPlayer.Height);
        }

        [Fact]
        public void DeletePlayer_WithExistingPlayer_DeletesPlayer()
        {
            // Arrange
            var playerToDelete = _context.Player.Find(1);

            // Act
            _service.DeletePlayer(playerToDelete);
            _context.SaveChanges();

            // Assert
            var deletedPlayer = _context.Player.Find(1);
            Assert.Null(deletedPlayer);
        }
    }
}
