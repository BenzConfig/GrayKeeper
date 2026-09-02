using GrayKeeper.Interfaces;
using GrayKeeper.Models;
using System.IO;
using System.Text.Json;

namespace GrayKeeper.Services;

public class JsonStorageService : IStorageService
{
    private readonly string _folder;
    private readonly string _file;

    public JsonStorageService()
    {
        _folder = Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory,
            "Data");

        _file = Path.Combine(
            _folder,
            "trips.json");
    }

    public List<Trip> LoadTrips()
    {
        try
        {
            if (!Directory.Exists(_folder))
                Directory.CreateDirectory(_folder);

            if (!File.Exists(_file))
                return new List<Trip>();

            var json = File.ReadAllText(_file);

            return JsonSerializer.Deserialize<List<Trip>>(json)
                   ?? new List<Trip>();
        }
        catch
        {
            return new List<Trip>();
        }
    }

    public void SaveTrips(IEnumerable<Trip> trips)
    {
        if (!Directory.Exists(_folder))
            Directory.CreateDirectory(_folder);

        var json = JsonSerializer.Serialize( trips, new JsonSerializerOptions
            {
                WriteIndented = true
            });

        File.WriteAllText(_file, json);
    }

    }   