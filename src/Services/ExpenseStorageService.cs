using System.Collections.ObjectModel;
using System.IO;
using System.Text.Json;
using GrayKeeper.Models;

namespace GrayKeeper.Services;

public class ExpenseStorageService
{
    private readonly string _filePath;


    public ExpenseStorageService()
    {
        var folder = Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory,
            "Data");


        if (!Directory.Exists(folder))
            Directory.CreateDirectory(folder);


        _filePath = Path.Combine(
            folder,
            "expenses.json");
    }



    public ObservableCollection<ExpenseEntry> LoadExpenses()
    {
        if (!File.Exists(_filePath))
            return new ObservableCollection<ExpenseEntry>();


        var json = File.ReadAllText(_filePath);


        var items = JsonSerializer.Deserialize<List<ExpenseEntry>>(json);


        return items == null
            ? new ObservableCollection<ExpenseEntry>()
            : new ObservableCollection<ExpenseEntry>(items);
    }




    public void SaveExpenses(
        IEnumerable<ExpenseEntry> expenses)
    {

        var options = new JsonSerializerOptions
        {
            WriteIndented = true
        };


        var json = JsonSerializer.Serialize(
            expenses,
            options);


        File.WriteAllText(
            _filePath,
            json);
    }

}