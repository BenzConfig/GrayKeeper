using GrayKeeper.Models;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.Measure;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using System.Collections.ObjectModel;

namespace GrayKeeper.Charts;

public class BalanceChartBuilder
{
    private static readonly SKColor AxisColor = new(80, 80, 80);
    private static readonly SKColor GridColor = new(45, 45, 45);
    private static readonly SKColor GreenColor = new(80, 220, 120);
    private static readonly SKColor RedColor = new(255, 80, 80);
    private static readonly SKColor GreenFillColor = new(80, 220, 120, 45);
    private static readonly SKColor RedFillColor = new(255, 80, 80, 45);

    public SolidColorPaint TooltipTextPaint { get; } =
        new SolidColorPaint(
            new SKColor(160, 160, 160))
        {
            SKTypeface = SKTypeface.FromFamilyName("Segoe UI")
        };

    public SolidColorPaint TooltipBackgroundPaint { get; } =
        new SolidColorPaint(
            new SKColor(37, 37, 37));

    // ============================================================
    // ОСНОВНЫЕ СЕРИИ
    // ============================================================
    public ISeries[] BuildSeries(
        ObservableCollection<ExpenseEntry> expenses)
    {
        var points =
            CalculateBalancePoints(expenses);

        var series =
            new List<ISeries>();


        // --------------------------------------------------------
        // Визуальные оси
        // --------------------------------------------------------

        series.Add(
            CreateHorizontalAxis());

        series.Add(
            CreateVerticalAxis());


        if (points.Count < 1)
            return series.ToArray();


        // --------------------------------------------------------
        // Разбиваем график на участки
        // относительно Y = 0
        // --------------------------------------------------------

        var segments =
            SplitIntoSegments(points);


        foreach (var segment in segments)
        {
            if (segment.Count < 2)
                continue;


            bool positive =
                segment.Any(p => (p.Y ?? 0) > 0);

            bool negative =
                segment.Any(p => (p.Y ?? 0) < 0);


            // ----------------------------------------------------
            // Заливка
            // ----------------------------------------------------

            series.Add(
                CreateFill(
                    segment,
                    positive && !negative));


            // ----------------------------------------------------
            // Линия
            // ----------------------------------------------------

            series.Add(
                CreateLine(
                    segment,
                    positive && !negative));
        }


        return series.ToArray();
    }


    // ============================================================
    // РАСЧЁТ БАЛАНСА
    // ============================================================
    private List<BalancePoint> CalculateBalancePoints(
        IEnumerable<ExpenseEntry> expenses)
    {
        var result = new List<BalancePoint>();

        decimal balance = 0;

        var ordered = expenses
            .OrderBy(x => x.Date)
            .ToList();


        // Начальная точка
        result.Add(
            new BalancePoint(
                0,
                0));


        for (int i = 0; i < ordered.Count; i++)
        {
            var item = ordered[i];

            balance += item.Income;

            balance -=
                item.Rent +
                item.Fuel +
                item.PerDiem +
                item.Tickets +
                item.Other;


            result.Add(
                new BalancePoint(
                    i + 1,
                    (double)balance,
                    item.Date));
        }


        return result;
    }


    // ============================================================
    // РАЗБИВКА ГРАФИКА НА ЗЕЛЁНЫЕ / КРАСНЫЕ УЧАСТКИ
    // ============================================================
    private List<List<BalancePoint>> SplitIntoSegments(
        List<BalancePoint> points)
    {
        var result =
            new List<List<BalancePoint>>();

        if (points.Count == 0)
            return result;

        var current =
            new List<BalancePoint>
            {
            points[0]
            };

        for (int i = 1; i < points.Count; i++)
        {
            var previous = points[i - 1];
            var next = points[i];

            double previousY = previous.Y ?? 0;
            double nextY = next.Y ?? 0;

            // ----------------------------------------------------
            // Есть пересечение Y = 0
            // ----------------------------------------------------

            if (
                (previousY < 0 && nextY > 0) ||
                (previousY > 0 && nextY < 0)
            )
            {
                double crossingX =
                    previous.X!.Value +
                    (
                        (0 - previousY) /
                        (nextY - previousY)
                    )
                    *
                    (
                        next.X!.Value -
                        previous.X!.Value
                    );

                var zeroPoint =
                    new BalancePoint(
                        crossingX,
                        0,
                        null);

                current.Add(zeroPoint);

                result.Add(current);

                current =
                    new List<BalancePoint>
                    {
                    zeroPoint,
                    next
                    };
            }
            else
            {
                current.Add(next);
            }
        }

        if (current.Count >= 2)
            result.Add(current);

        return result;
    }


    // ============================================================
    // ЛИНИЯ
    // ============================================================
    private ISeries CreateLine(
        List<BalancePoint> points,
        bool positive)
    {
        return new LineSeries<BalancePoint>
        {
            Values = points,

            Stroke = new SolidColorPaint(
                positive
                    ? GreenColor
                    : RedColor,
                1.5f),

            GeometrySize = 0,

            GeometryFill =
                new SolidColorPaint(
                    new SKColor(70, 70, 70)),

            GeometryStroke =
                new SolidColorPaint(
                    new SKColor(110, 110, 110),
                    1),

            LineSmoothness = 0,
            Fill = null,
            IsHoverable = true,

            XToolTipLabelFormatter = point =>
            {
                if (point.Context.DataSource is BalancePoint balancePoint &&
                    balancePoint.Date.HasValue)
                {
                    return balancePoint.Date.Value
                        .ToString("dd.MM.yyyy");
                }

                return string.Empty;
            },

            YToolTipLabelFormatter = point =>
            {
                if (point.Context.DataSource is BalancePoint balancePoint)
                {
                    return $"Баланс: {balancePoint.Y:N0} ₽";
                }

                return string.Empty;
            }

        };
    }


    // ============================================================
    // ЗАЛИВКА
    // ============================================================
    private ISeries CreateFill(
        List<BalancePoint> points,
        bool positive)
    {
        var fillPoints =
            new List<BalancePoint>();


        // Сначала сама траектория
        fillPoints.AddRange(points);


        // Затем возвращаемся по нулевой линии
        for (int i = points.Count - 1; i >= 0; i--)
        {
            fillPoints.Add(
                new BalancePoint(
                    points[i].X!.Value,
                    0,
                    null));
        }


        return new LineSeries<BalancePoint>
        {
            Values = fillPoints,

            Stroke = null,

            GeometrySize = 0,

            LineSmoothness = 0,

            Fill = new SolidColorPaint(
                positive
                    ? GreenFillColor
                    : RedFillColor),

            IsHoverable = false
        };


    }


    // ============================================================
    // ГОРИЗОНТАЛЬНАЯ ОСЬ Y = 0
    // ============================================================
    private ISeries CreateHorizontalAxis()
    {
        return new LineSeries<ObservablePoint>
        {
            Values =
            [
                new ObservablePoint(-1, 0),
        new ObservablePoint(100, 0)
            ],

            Stroke = new SolidColorPaint(
                AxisColor,
                1.5f),

            GeometrySize = 0,

            LineSmoothness = 0,

            Fill = null,

            IsHoverable = false
        };
    }


    // ============================================================
    // ВЕРТИКАЛЬНАЯ ОСЬ X = 0
    // ============================================================
    private ISeries CreateVerticalAxis()
    {
        return new LineSeries<ObservablePoint>
        {
            Values =
            [
                new ObservablePoint(0, -100000),
        new ObservablePoint(0, 100000)
            ],

            Stroke = new SolidColorPaint(
                AxisColor,
                1.5f),

            GeometrySize = 0,

            LineSmoothness = 0,

            Fill = null,

            IsHoverable = false
        };
    }


    // ============================================================
    // НАСТРОЙКА ОСИ X
    // ============================================================
    public Axis[] BuildXAxes(int pointCount)
    {
        return
        [
            new Axis
        {
            Position = AxisPosition.Start,
            LabelsPaint = null,
            TextSize = 0,
            TicksPaint = null,

            SeparatorsPaint =
                new SolidColorPaint(
                    GridColor,
                    1),

            ShowSeparatorLines = true,
            MinLimit = 0,
            MaxLimit = Math.Max(1, pointCount)
        }
        ];
    }


    // ============================================================
    // НАСТРОЙКА ОСИ Y
    // ============================================================
    public Axis[] BuildYAxes()
    {
        return
        [
            new Axis
            {
                Position = AxisPosition.Start,

                LabelsPaint = null,

                TextSize = 0,

                TicksPaint = null,

                SeparatorsPaint =
                    new SolidColorPaint(
                        GridColor,
                        1),

                ShowSeparatorLines = true,

                MinLimit = -100000,

                MaxLimit = 100000
            }
        ];
    }
}