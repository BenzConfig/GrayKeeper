using GrayKeeper.Models;

namespace GrayKeeper.Filters;

public static class TripFilter
{
    public static bool Match(Trip trip, string searchText)
    {
        if (string.IsNullOrWhiteSpace(searchText))
            return true;

        return
            (trip.Title?.Contains(searchText,
                StringComparison.OrdinalIgnoreCase) ?? false)
            ||
            (trip.Organization?.Contains(searchText,
                StringComparison.OrdinalIgnoreCase) ?? false)
            ||
            (trip.City?.Contains(searchText,
                StringComparison.OrdinalIgnoreCase) ?? false);
    }
}