using LiveChartsCore.Defaults;
using System;

namespace GrayKeeper.Charts;

public class BalancePoint : ObservablePoint
{
    public DateTime? Date { get; }

    public BalancePoint(
        double x,
        double y,
        DateTime? date = null)
        : base(x, y)
    {
        Date = date;
    }
}