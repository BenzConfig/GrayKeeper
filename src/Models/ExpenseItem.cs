using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GrayKeeper.Models;

public class ExpenseItem
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public DateTime Date { get; set; } = DateTime.Today;
    public ExpenseType Type { get; set; }
    public decimal Amount { get; set; }
    public string Comment { get; set; } = "";
}