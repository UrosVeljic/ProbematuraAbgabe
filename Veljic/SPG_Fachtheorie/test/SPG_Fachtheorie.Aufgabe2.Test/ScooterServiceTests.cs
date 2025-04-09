using Microsoft.EntityFrameworkCore;
using SPG_Fachtheorie.Aufgabe2.Infrastructure;
using SPG_Fachtheorie.Aufgabe2.Model;
using SPG_Fachtheorie.Aufgabe2.Services;
using System.Linq;
using Xunit;

namespace SPG_Fachtheorie.Aufgabe2.Test
{
    [Collection("Sequential")]
    public class ScooterServiceTests
    {
        /// <summary>
        /// Generates database in C:\Scratch\Aufgabe2_Test\Debug\net8.0\scooter.db
        /// </summary>
        private ScooterContext GetEmptyDbContext()
        {
            var options = new DbContextOptionsBuilder()
                .UseSqlite("DataSource=scooter.db")
                .Options;
            var db = new ScooterContext(options);
            db.Database.EnsureDeleted();
            db.Database.EnsureCreated();
            return db;
        }

        [Fact]
        public void CreateDatabaseSuccessTest()
        {
            using var db = GetEmptyDbContext();
            db.Seed();
            Assert.True(db.Scooters.Count() > 0);
        }

        /// <summary>
        /// WITH Triplog AS (
        ///     SELECT u.Id as UserId, t.Id AS TripId, u.FreeKilometers, s.PricePerKilometer,
        ///         MIN(tl.MileageInMeters) AS MinMileage,
        ///         MAX(tl.MileageInMeters) AS MaxMileage
        ///     FROM Users u INNER JOIN Trips t ON (u.Id = t.UserId)
        ///     INNER JOIN TripLogs tl ON (t.Id = tl.TripId)
        ///     INNER JOIN Scooters s ON (s.Id = t.ScooterId)
        ///     WHERE t.End IS NOT NULL
        ///     GROUP BY u.Id, t.Id, u.FreeKilometers, s.PricePerKilometer
        /// )
        /// SELECT t.*,
        ///     (t.MaxMileage - t.MinMileage)/1000.0 AS DistanceTraveled,
        ///     MAX(0, t.PricePerKilometer * ((t.MaxMileage - t.MinMileage)/1000.0 - t.FreeKilometers)) AS TripCost
        /// FROM Triplog t;
        /// | UserId | TripId | FreeKilometers | PricePerKilometer | MinMileage | MaxMileage | DistanceTraveled | TripCost           |
        /// | ------ | ------ | -------------- | ----------------- | ---------- | ---------- | ---------------- | ------------------ |
        /// | 1      | 3      | 0              | 0.15              | 21475      | 50630      | 29.155           | 4.37325            |
        /// | 1      | 12     | 0              | 0.18              | 2865       | 13084      | 10.219           | 1.83942            |
        /// | 2      | 2      | 7              | 0.15              | 14947      | 20905      | 5.958            | 0                  |
        /// | 2      | 14     | 7              | 0.18              | 16080      | 46875      | 30.795           | 4.2831             |
        /// | 3      | 6      | 0              | 0.2               | 28149      | 42964      | 14.815           | 2.963              |
        /// | 3      | 8      | 0              | 0.2               | 44907      | 61497      | 16.59            | 3.318              |
        /// </summary>
        [Fact]
        public void CalculateTripInfos_ReturnsCorrectResult()
        {
            using var db = GetEmptyDbContext();
            db.Seed();
            var service = new ScooterService(db);
            var result = service.CalculateTripInfos();
            Assert.True(result[1].Count == 2);
            Assert.True(result[2].Count == 2);
            Assert.True(result[3].Count == 2);
            Assert.Contains(result[1], t => t.DistanceTraveled == 29.155M && t.TripCost == 4.37325M);
            Assert.Contains(result[1], t => t.DistanceTraveled == 10.219M && t.TripCost == 1.83942M);
            Assert.Contains(result[2], t => t.DistanceTraveled == 5.958M && t.TripCost == 0M);
            Assert.Contains(result[2], t => t.DistanceTraveled == 30.795M && t.TripCost == 4.2831M);
            Assert.Contains(result[3], t => t.DistanceTraveled == 14.815M && t.TripCost == 2.963M);
            Assert.Contains(result[3], t => t.DistanceTraveled == 16.59M && t.TripCost == 3.318M);
        }

    }
}