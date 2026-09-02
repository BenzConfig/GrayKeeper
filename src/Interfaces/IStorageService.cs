using GrayKeeper.Models;

namespace GrayKeeper.Interfaces;

public interface IStorageService
{
    List<Trip> LoadTrips();

    void SaveTrips(IEnumerable<Trip> trips);
}