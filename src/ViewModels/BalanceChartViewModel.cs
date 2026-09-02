using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using System.Collections.ObjectModel;
using GrayKeeper.Models;

namespace GrayKeeper.ViewModels;

public class BalanceChartViewModel : BaseViewModel
{
    public ObservableCollection<ExpenseEntry> Expenses { get; }

    public ISeries[] Series { get; }

    public BalanceChartViewModel(ObservableCollection<ExpenseEntry> expenses)
    {
        Expenses = expenses;

        Series =
        [
            new LineSeries<decimal>
            {
                Values = GetBalancePoints()
            }
        ];
    }

    private ObservableCollection<decimal> GetBalancePoints()
    {
        decimal balance = 0;

        var result = new ObservableCollection<decimal>();

        foreach (var item in Expenses.OrderBy(x => x.Date))
        {
            balance += item.Income;

            balance -=
                item.Rent +
                item.Fuel +
                item.PerDiem +
                item.Tickets +
                item.Other;

            result.Add(balance);
        }

        return result;
    }

}   