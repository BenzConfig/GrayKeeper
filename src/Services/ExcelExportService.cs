using ClosedXML.Excel;
using GrayKeeper.Interfaces;
using GrayKeeper.Models;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GrayKeeper.Services;

public class ExcelExportService : IExcelExportService
{
    public void Export(Trip trip, IEnumerable<ExpenseEntry> expenses)
    {
        if (trip == null)
            return;

        var expenseList = expenses?.ToList() ?? new List<ExpenseEntry>();

        // СУММЫ ПО КАТЕГОРИЯМ
        decimal fuel = expenseList.Sum(x => x.Fuel);
        decimal rent = expenseList.Sum(x => x.Rent);
        decimal perDiem = expenseList.Sum(x => x.PerDiem);
        decimal tickets = expenseList.Sum(x => x.Tickets);
        decimal other = expenseList.Sum(x => x.Other);
        decimal totalExpenses = fuel + rent + perDiem + tickets + other;

        // КОЛИЧЕСТВО ДНЕЙ КОМАНДИРОВКИ
        int days = (trip.EndDate.Date - trip.StartDate.Date).Days + 1;

        // ВЫБОР ФАЙЛА
        var dialog = new SaveFileDialog
        {
            Title = "Сохранить отчёт",
            Filter = "Excel файл (*.xlsx)|*.xlsx",
            FileName = $"journey {trip.StartDate:dd.MM.yy}-{trip.EndDate:dd.MM.yy}.xlsx",
            DefaultExt = ".xlsx",
            AddExtension = true
        };

        if (dialog.ShowDialog() != true)
            return;

        // СОЗДАНИЕ EXCEL
        using var workbook = new XLWorkbook();
              var worksheet = workbook.Worksheets.Add("trip");

        // СТРОКА 1 — ЗАГОЛОВКИ
        worksheet.Cell(1, 1).Value = "Дата";
        worksheet.Cell(1, 2).Value = "Кол-во дней";
        worksheet.Cell(1, 3).Value = "ГСМ";
        worksheet.Cell(1, 4).Value = "Амортизация";
        worksheet.Cell(1, 5).Value = "Билеты";
        worksheet.Cell(1, 6).Value = "нал (сумма)/б/нал";
        worksheet.Cell(1, 7).Value = "Гостиница";
        worksheet.Cell(1, 8).Value = "нал (сумма)/б/нал";
        worksheet.Cell(1, 9).Value = "Суточные";
        worksheet.Cell(1, 10).Value = "Прочие";
        worksheet.Cell(1, 11).Value = "Итог";

        // СТРОКА 2 — ДАННЫЕ
        worksheet.Cell(2, 1).Value = $"{trip.StartDate:dd.MM.yyyy}-{trip.EndDate:dd.MM.yyyy}";
        worksheet.Cell(2, 2).Value = days;
        worksheet.Cell(2, 3).Value = fuel;
        worksheet.Cell(2, 5).Value = tickets;
        worksheet.Cell(2, 7).Value = rent;
        worksheet.Cell(2, 9).Value = perDiem;
        worksheet.Cell(2, 10).Value = other;
        worksheet.Cell(2, 11).Value = totalExpenses;

        // ОФОРМЛЕНИЕ ЗАГОЛОВКОВ
        var headerRange = worksheet.Range(1, 1, 1, 11);
            headerRange.Style.Font.Bold = false;
            headerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            headerRange.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

        // ОФОРМЛЕНИЕ ДАННЫХ
        var dataRange = worksheet.Range(2, 1, 2, 11);
            dataRange.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            dataRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

        // ТЕКСТОВЫЙ ФОРМАТ
        worksheet.Cell(2, 3).Style.NumberFormat.Format = "@";
        worksheet.Cell(2, 5).Style.NumberFormat.Format = "@";
        worksheet.Cell(2, 7).Style.NumberFormat.Format = "@";
        worksheet.Cell(2, 8).Style.NumberFormat.Format = "@";
        worksheet.Cell(2, 9).Style.NumberFormat.Format = "@";
        worksheet.Cell(2, 10).Style.NumberFormat.Format = "@";

        // ГРАНИЦЫ
        var usedRange = worksheet.Range(1, 1, 2, 11);
            usedRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            usedRange.Style.Border.InsideBorder = XLBorderStyleValues.Thin;

        // ШИРИНА КОЛОНОК
        worksheet.Column(1).Width = 25;
        worksheet.Column(2).Width = 15;
        worksheet.Column(3).Width = 15;
        worksheet.Column(4).Width = 15;
        worksheet.Column(5).Width = 15;
        worksheet.Column(6).Width = 20;
        worksheet.Column(7).Width = 15;
        worksheet.Column(8).Width = 20;
        worksheet.Column(9).Width = 15;
        worksheet.Column(10).Width = 15;
        worksheet.Column(11).Width = 15;

        // ВЫСОТА СТРОК
        worksheet.Row(1).Height = 25;
        worksheet.Row(2).Height = 15;

        // СОХРАНЕНИЕ
        workbook.SaveAs(dialog.FileName);
    }
}