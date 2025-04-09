using Bogus;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using SPG_Fachtheorie.Aufgabe2.Infrastructure;
using SPG_Fachtheorie.Aufgabe3.Commands;
using SPG_Fachtheorie.Aufgabe3.Dtos;
using System.Text.RegularExpressions;

namespace SPG_Fachtheorie.Aufgabe3.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class TripsController : ControllerBase
    {
        private readonly ScooterContext _db;
        private static Regex _tripKey = new Regex(@"^TR[0-9]+$", RegexOptions.Compiled);

        public TripsController(ScooterContext db)
        {
            _db = db;
        }






        // TODO: Implementiere die
        //     GET /trips/{key}?includeLog=true und
        //     PATCH /trips/{key}
        // Endpunkte.

        [HttpGet("{key}")]
        public async Task<ActionResult<TripDto>> GetTrip(
         string key, [FromQuery(Name = "includeLog")] bool includeLog)
        {
            if (!_tripKey.IsMatch(key))
                return Problem("Invalid key", statusCode: 400);

            var trip = await SearchTrips(key, includeLog);
            if (trip is null)
                return Problem("No trip found", statusCode: 404);

            return Ok(trip);
        }


        private async Task<TripDto?> SearchTrips(string key,bool includeLog)
        {
            return await _db.Trips
                .Where(p => p.Key == key)
                .Select(p => new TripDto(
                p.Key, p.User.Email, p.Scooter.ManufacturerId, p.Begin.ToString("F"), p.End.ToString(),
                includeLog
                ? p.TripLogs
                .Select(c => new TripLogDto(c.Timestamp.ToString("F"), c.Location.Longitude, c.Location.Latitude, c.MileageInMeters))
                .ToList()
                : new List<TripLogDto>()))
                .FirstOrDefaultAsync();

        }


        [HttpPatch("{key}")]
        public async Task<IActionResult> PatchTrip(string key, UpdateTripCommand cmd)
        {
            if (!_tripKey.IsMatch(key))
                return Problem("Invalid key", statusCode: 400);
            var trip = _db.Trips.FirstOrDefault(p => p.Key == key);
            if (trip is null)
                return Problem("Key not found", statusCode: 404);
            if (trip.ParkingLocation is not null && trip.End is not null)
                return Problem("Trip beendet", statusCode: 400);
            trip.End = cmd.End;
            trip.ParkingLocation = new Aufgabe2.Model.Location(cmd.Longitude, cmd.Latitude);

            // vergessen _db.savechanges zu machen 
            await _db.SaveChangesAsync();
            var tripFromDb = await SearchTrips(key, false);
            return Ok(tripFromDb);
        }







    }
}


