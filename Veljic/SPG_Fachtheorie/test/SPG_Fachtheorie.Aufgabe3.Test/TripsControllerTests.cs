using Microsoft.AspNetCore.Http;
using SPG_Fachtheorie.Aufgabe2.Model;
using SPG_Fachtheorie.Aufgabe3.Commands;
using SPG_Fachtheorie.Aufgabe3.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace Spg.Fachtheorie.Aufgabe3.API.Test
{
    [Collection("Sequential")]
    public class TripsControllerTests : IClassFixture<TestWebApplicationFactory>
    {
        private readonly TestWebApplicationFactory _factory;

        public TripsControllerTests(TestWebApplicationFactory factory)
        {
            _factory = factory;
        }





        // Mein Fehler war, nicht die selbe ID benutzt zu haben, aufpassen!
        [Theory]
        [InlineData(true, 1)]
        [InlineData(false, 0)]
        public async Task GetTripsByKeyTest(bool includeTrips, int numberOfTripsExpected)
        {
            _factory.InitializeDatabase(db =>
            {
                var trip = new Trip("TR101", new User("veljicuros@gmail.com", 10), new Scooter("TR101", "Honda Civic", 1.65M), new DateTime(2025, 08, 26), null, null);


                var tripLog = new TripLog(trip, new DateTime(2025, 08, 30), new Location(10, 20), 50);
                db.Trips.Add(trip);
                db.TripLogs.Add(tripLog);
                db.SaveChanges();
            });

            var (statusCode, tripWithLog) = await _factory.GetHttpContent
                <TripDto>(
                $"/trips/TR101?includeLog={includeTrips}");
            Assert.True(statusCode == HttpStatusCode.OK);
            Assert.NotNull(tripWithLog);
            Assert.True(tripWithLog.Key == "TR101");
            Assert.True(tripWithLog.Logs.Count == numberOfTripsExpected);


        }


        // fehler gehabt "includelog= habe ich nicht auf false gesetzt 
        [Theory]
        [InlineData("TR101", HttpStatusCode.OK)]
        [InlineData("AT", HttpStatusCode.BadRequest)]
        [InlineData("TR1", HttpStatusCode.NotFound)]
        public async Task GetTripsWithInvalidKeyTest(
            string key, HttpStatusCode expectedStatusCode)
        {
            _factory.InitializeDatabase(db =>
            {
                var trip = new Trip("TR101", new User("ukiveljic@gmail.com", 10), new Scooter("1", "Rolla", 10), new DateTime(2025, 08, 30), null, null);
                var tripLog = new TripLog(trip, new DateTime(2025, 08, 30), new Location(10, 20), 50);
                db.Trips.Add(trip);
                db.TripLogs.Add(tripLog);
                db.SaveChanges();
            });

            var (statusCode, tripWithLog) = await _factory.GetHttpContent
                <TripDto>($"/trips/{key}?includeLog=false");
            Assert.True(statusCode == expectedStatusCode);


        }


        [Theory]
        [InlineData("TR101", HttpStatusCode.OK)]
        [InlineData("AT", HttpStatusCode.BadRequest)]
        [InlineData("TR1", HttpStatusCode.NotFound)]
        public async Task UpdateTripTest(string key, HttpStatusCode exptectedStatusCode)
        {
            _factory.InitializeDatabase(db =>
            {
                var trip = new Trip("TR101", new User("ukiveljic@gmail.com", 10),
                    new Scooter("1", "Rolla", 10),
                    new DateTime(2025, 08, 30),
                    null, null);  // Trip ist noch nicht beendet
                db.Trips.Add(trip);
                db.SaveChanges();
            });

            var updateTripCmd = new UpdateTripCommand(new DateTime(2025, 08, 30), 10, 14);

            var (statusCode, jsonElement) = await _factory.PatchHttpContent(
                $"/trips/{key}", updateTripCmd);

            Assert.True(statusCode == exptectedStatusCode);

            if (exptectedStatusCode == HttpStatusCode.OK)
            {
                var tripFromDb = _factory.QueryDatabase(db => db.Trips.First());

                Assert.Equal(updateTripCmd.End, tripFromDb.End);
                Assert.NotNull(tripFromDb.ParkingLocation);
                Assert.Equal(updateTripCmd.Latitude, tripFromDb.ParkingLocation.Latitude);
                Assert.Equal(updateTripCmd.Longitude, tripFromDb.ParkingLocation.Longitude);
            }
        }

    }
}

