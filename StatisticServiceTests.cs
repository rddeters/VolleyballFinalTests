using Microsoft.EntityFrameworkCore;
using Moq;
using VolleyballFinal.Controllers.Service;
using VolleyballFinal.Models;

namespace VolleyballFinalTests
{
    public class StatisticServiceTests
    { 
        private readonly TeamContext _context;
        private readonly StatisticService _service;
        private List<Statistic> _statistics;

        public StatisticServiceTests()
        {
            var options = new DbContextOptionsBuilder<TeamContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            _context = new TeamContext(options);

            SeedDatabase();
            _service = new StatisticService(_context);

            _statistics = _context.Statistics.ToList(); 
        }

        private void SeedDatabase()
        {
            _context.Statistics.AddRange(
                new Statistic { Id = 1, PlayerName = "Matias Sanchez", TotalPoints = 0, AttackPoints = 0, BlockPoints = 0, ServePoints = 0, Efficiency = 0.00 },
               new Statistic { Id = 2, PlayerName = "Federico Pereyra", TotalPoints = 7, AttackPoints = 5, BlockPoints = 2, ServePoints = 0, Efficiency = 31.25 },
               new Statistic { Id = 3, PlayerName = "Cristian Poglajen", TotalPoints = 28, AttackPoints = 25, BlockPoints = 2, ServePoints = 1, Efficiency = 50.00 }
           );
            _context.SaveChanges();
        }

        public void Dispose()
        {
            _context.Dispose();
        }

        [Fact]
        public void GetAllStatistics_ReturnsAllStatistics()
        {
            // Act
            var result = _service.GetAllStatistics();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(3, result.Count);
            Assert.Contains(result, s => s.PlayerName == "Matias Sanchez");
        }

        [Fact]
        public void GetStatisticById_WithValidId_ReturnsCorrectStatistic()
        {
            // Act
            var result = _service.GetStatisticById(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Matias Sanchez", result.PlayerName);
        }

        [Fact]
        public void GetStatisticById_WithInvalidId_ReturnsNull()
        {
            // Act
            var result = _service.GetStatisticById(99);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void AddOrUpdateStatistic_WithNewStatistic_AddsStatistic()
        {
            // Arrange
            var newStatistic = new Statistic
            {
                Id = 4,
                PlayerName = "Facundo Conte",
                TotalPoints = 125,
                AttackPoints = 111,
                BlockPoints = 7,
                ServePoints = 7,
                Efficiency = 45.12
            };

            // Act
            _service.AddOrUpdateStatistic(newStatistic);
            _context.SaveChanges();

            // Assert
            var statisticInDb = _context.Statistics.Find(newStatistic.Id);
            Assert.NotNull(statisticInDb); 
            Assert.Equal("Facundo Conte", statisticInDb.PlayerName);
            
        }

        [Fact]
        public void AddOrUpdateStatistic_WithExistingStatistic_UpdatesStatistic()
        {
            // Arrange
            var statisticToUpdate = _context.Statistics.Find(1);
            statisticToUpdate.PlayerName = "Updated Matias Sanchez";

            // Act
            _service.AddOrUpdateStatistic(statisticToUpdate);
            _context.SaveChanges();

            // Assert
            var updatedStatistic = _context.Statistics.Find(1);
            Assert.Equal("Updated Matias Sanchez", updatedStatistic.PlayerName);
        }

        [Fact]
        public void DeleteStatistic_WithExistingStatistic_DeletesStatistic()
        {
            // Arrange
            var statisticToDelete = _statistics.First();

            // Act
            _service.DeleteStatistic(statisticToDelete);
            _context.SaveChanges();

            // Assert
            var statisticInDb = _context.Statistics.Find(statisticToDelete.Id);
            Assert.Null(statisticInDb); 
        }
    }
}