using GrayKeeper.Models;
using System.Collections.Generic;

namespace GrayKeeper.Interfaces;

public interface IExcelExportService
{
    void Export(
        Trip trip,
        IEnumerable<ExpenseEntry> expenses);
}