using GrayKeeper.Models;
using System.Collections.ObjectModel;

namespace GrayKeeper.ViewModels;

public class DashboardViewModel : BaseViewModel
{
    public DashboardViewModel()
    {
        Expenses = new ObservableCollection<ExpenseEntry>();
    }

    public ObservableCollection<ExpenseEntry> Expenses { get; set; }

    public void AddExpense(ExpenseEntry entry)
    {
        Expenses.Add(entry);
    }

}