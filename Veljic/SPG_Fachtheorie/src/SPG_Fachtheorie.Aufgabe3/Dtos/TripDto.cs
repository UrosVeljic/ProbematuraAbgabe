namespace SPG_Fachtheorie.Aufgabe3.Dtos
{
    public record TripDto(string Key, string UserEmail, string ScooterManufacturerId, string Begin, string? End,
        List<TripLogDto> Logs);
}