using GrayKeeper.Models;
using System.Collections.ObjectModel;
using System.IO;
using System.Text.Json;

namespace GrayKeeper.Services;

public class TripStorageService
{
    private readonly string folder;

    public TripStorageService()
    {
        folder = Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory,
            "Data",
            "Trips");

        Directory.CreateDirectory(folder);
    }

    private string GetFile(Trip trip)
    {
        var safeName = string.IsNullOrWhiteSpace(trip.Organization)
            ? "Командировка"
            : trip.Organization;


        safeName = string.Concat(
            safeName
                .Where(c => !Path.GetInvalidFileNameChars()
                .Contains(c)));


        return Path.Combine(
            folder,
            $"{safeName}_{trip.StartDate:yyyyMMdd}_{trip.Id[..8]}.json");
    }

    public void SaveTrip(Trip trip)
    {
        var json = JsonSerializer.Serialize(
            trip,
            new JsonSerializerOptions
            {
                WriteIndented = true
            });

        File.WriteAllText(
            GetFile(trip),
            json);
    }

    public Trip LoadTrip(Trip trip)
    {
        var file = GetFile(trip);


        if (!File.Exists(file))
            return trip;


        var json = File.ReadAllText(file);


        return JsonSerializer.Deserialize<Trip>(json)
               ?? trip;
    }

    public ObservableCollection<ExpenseEntry> LoadExpenses(Trip trip)
    {
        var loaded = LoadTrip(trip);

        return loaded.Expenses;
    }

}