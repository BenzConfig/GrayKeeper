using System.Collections.ObjectModel;

namespace GrayKeeper.Models;

public class Trip
{
    public string Id { get; set; } = Guid.NewGuid().ToString();

    public string Title { get; set; } = "";

    public string Organization { get; set; } = "";

    public string City { get; set; } = "";

    public string DateRange => $"{StartDate:dd.MM.yyyy} - {EndDate:dd.MM.yyyy}";

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public ObservableCollection<ExpenseEntry> Expenses { get; set; } = new();
}