using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using SPG_Fachtheorie.Aufgabe2.Infrastructure;
using SPG_Fachtheorie.Aufgabe2.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace SPG_Fachtheorie.Aufgabe2.Services
{
    public class ScooterService
    {
        private readonly ScooterContext _db;
        public record TripInfo(decimal DistanceTraveled, decimal TripCost);
        public ScooterService(ScooterContext db)
        {
            _db = db;
        }

        public Dictionary<int, List<TripInfo>> CalculateTripInfos()
        {
            return _db.Users
                .Include(u => u.Trips)
                    .ThenInclude(t => t.TripLogs)
                .Include(u => u.Trips)
                    .ThenInclude(t => t.Scooter)
                .ToList()
                .ToDictionary(
                    u => u.Id,
                    u => u.Trips
                        .Where(t => t.End != null && t.TripLogs.Any())
                        .Select(t =>
                        {
                            var min = t.TripLogs.Min(l => l.MileageInMeters);
                            var max = t.TripLogs.Max(l => l.MileageInMeters);
                            var distance = (max - min) / 1000M;
                            var price = Math.Max(0, (distance - t.User.FreeKilometers)) * t.Scooter.PricePerKilometer;
                            return new TripInfo(distance, price);
                        }).ToList()
                );
        }


    }
}