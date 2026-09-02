namespace GrayKeeper.Models;

public class ExpenseEntry
{
    public DateTime Date { get; set; }

    public decimal Rent { get; set; }

    public decimal Fuel { get; set; }

    public decimal PerDiem { get; set; }

    public decimal Tickets { get; set; }

    public decimal Other { get; set; }

    public decimal Income { get; set; }


    // Общая сумма расходов
    public decimal TotalExpense =>
        Rent +
        Fuel +
        PerDiem +
        Tickets +
        Other;
}