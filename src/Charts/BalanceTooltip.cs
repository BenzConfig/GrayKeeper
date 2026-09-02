using LiveChartsCore;
using LiveChartsCore.Drawing;
using LiveChartsCore.Drawing.Layouts;
using LiveChartsCore.Kernel;
using LiveChartsCore.SkiaSharpView.Drawing;
using LiveChartsCore.SkiaSharpView.Drawing.Geometries;
using LiveChartsCore.SkiaSharpView.Drawing.Layouts;
using LiveChartsCore.SkiaSharpView.Painting;
using LiveChartsCore.SkiaSharpView.SKCharts;
using SkiaSharp;
using System;
using System.IO;
using System.Collections.Generic;

namespace GrayKeeper.Charts;

public class BalanceTooltip : SKDefaultTooltip
{
    private static readonly SolidColorPaint TooltipTextPaint =
        new(
            new SKColor(165, 165, 165))
        {
            SKTypeface = SKTypeface.FromFamilyName("Arial")
        };

    private static readonly SolidColorPaint TooltipBackgroundPaint =
        new(
            new SKColor(37, 37, 37));

    private static readonly SolidColorPaint TooltipBorderPaint =
        new(
            new SKColor(70, 70, 70),
            1);


    // ============================================================
    // ИНИЦИАЛИЗАЦИЯ
    // ============================================================

    protected override void Initialize(Chart chart)
    {
        Wedge = 0;

        Geometry.Fill =
            TooltipBackgroundPaint;

        Geometry.Stroke =
            TooltipBorderPaint;

        Geometry.BorderRadius = 8;

        Geometry.Wedge = 0;

        Geometry.WedgeThickness = 0;

        this.Animate(
            new Animation(
                EasingFunctions.EaseOut,
                TimeSpan.FromMilliseconds(150)));
    }


    // ============================================================
    // СОДЕРЖИМОЕ TOOLTIP
    // ============================================================

    protected override Layout<SkiaSharpDrawingContext> GetLayout(
        IEnumerable<ChartPoint> foundPoints,
        Chart chart)
    {
        var layout = new StackLayout
        {
            Orientation = ContainerOrientation.Vertical,

            HorizontalAlignment = Align.Middle,
            VerticalAlignment = Align.Middle,

            Padding = new Padding(
                10,
                8)
        };


        foreach (var point in foundPoints)
        {
            // Получаем исходную модель точки.
            if (point.Context.DataSource is not BalancePoint balancePoint)
                continue;

            // Начальная точка (0, 0) и точки пересечения
            // с Y = 0 не имеют даты.
            if (!balancePoint.Date.HasValue)
                continue;


            // ----------------------------------------------------
            // ДАТА
            // ----------------------------------------------------

            var dateLabel =
                new LabelGeometry
                {
                    Text =
                        balancePoint.Date.Value
                            .ToString("dd.MM.yyyy"),

                    Paint =
                        TooltipTextPaint,

                    TextSize = 11,

                    Padding =
                        new Padding(
                            0,
                            0,
                            0,
                            4),

                    HorizontalAlign =
                        Align.Start,

                    VerticalAlign =
                        Align.Start
                };


            // ----------------------------------------------------
            // БАЛАНС
            // ----------------------------------------------------

            var balanceLabel =
                new LabelGeometry
                {
                    Text =
                        $"Баланс: {balancePoint.Y:N0} ₽",

                    Paint =
                        TooltipTextPaint,

                    TextSize = 11,

                    Padding =
                        new Padding(
                            0,
                            0),

                    HorizontalAlign =
                        Align.Start,

                    VerticalAlign =
                        Align.Start
                };


            // ----------------------------------------------------
            // ДОБАВЛЯЕМ
            // ----------------------------------------------------

            layout.Children.Add(dateLabel);
            layout.Children.Add(balanceLabel);
        }


        return layout;
    }
}