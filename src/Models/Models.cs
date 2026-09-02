namespace GrayKeeper.Models;

public class Expense
{

    // Дата расхода
    public DateTime Date { get; set; }


    // Расходы

    public decimal Rent { get; set; }

    public decimal Daily { get; set; }

    public decimal Fuel { get; set; }

    public decimal Tickets { get; set; }

    public decimal Other { get; set; }


    // Поступление денег

    public decimal Income { get; set; }


    // Общая сумма расходов

    public decimal TotalExpense
    {
        get
        {
            return Rent +
                   Daily +
                   Fuel +
                   Tickets +
                   Other;
        }
    }


    // Остаток

    public decimal Balance
    {
        get
        {
            return Income - TotalExpense;
        }
    }

}