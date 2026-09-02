using GrayKeeper.Infrastructure;
using GrayKeeper.Interfaces;
using GrayKeeper.Models;
using GrayKeeper.Services;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;
using System.Windows.Input;
using GrayKeeper.Charts;

namespace GrayKeeper.ViewModels;

public class MainViewModel : BaseViewModel
{
    private readonly BalanceChartBuilder _balanceChartBuilder;

    private readonly TripStorageService _tripStorage;
    private readonly IStorageService _storage;
    private readonly IDialogService _dialogService;

    public ICommand EditTripCommand { get; }

    public ISeries[] BalanceSeries { get; private set; } = [];

    public Axis[] BalanceXAxes { get; private set; } = [];

    public Axis[] BalanceYAxes { get; private set; } = [];

    public ObservableCollection<ExpenseEntry> CurrentExpenses { get; } = new();
    public ICollectionView CurrentExpensesView { get; }

    public decimal TotalIncome
    {
        get
        {
            return CurrentExpenses.Sum(x => x.Income);
        }
    }

    public decimal TotalExpenses
    {
        get
        {
            return CurrentExpenses.Sum(x =>
                x.Rent +
                x.Fuel +
                x.PerDiem +
                x.Tickets +
                x.Other);
        }
    }

    public decimal Balance => TotalIncome - TotalExpenses;

    public ObservableCollection<Trip> Trips { get; } = new();

    public ICollectionView TripsView { get; }

    private string _searchText = "";

    public string SearchText
    {
        get => _searchText;

        set
        {
            if (SetProperty(ref _searchText, value))
                TripsView.Refresh();
        }
    }

    private Trip? _selectedTrip;
    public Trip? SelectedTrip
    {
        get => _selectedTrip;

        set
        {
            if (SetProperty(ref _selectedTrip, value))
            {
                CurrentExpenses.Clear();

                if (_selectedTrip != null)
                {
                    var loaded = _tripStorage.LoadTrip(_selectedTrip);

                    foreach (var item in _tripStorage.LoadExpenses(_selectedTrip))
                    {
                        CurrentExpenses.Add(item);
                    }

                    _selectedTrip.Expenses = CurrentExpenses;
                }

                CurrentExpensesView.Refresh();

                OnPropertyChanged(nameof(TotalIncome));
                OnPropertyChanged(nameof(TotalExpenses));
                OnPropertyChanged(nameof(Balance));

                UpdateBalanceChart();

            }
        }
    }

    private ExpenseEntry? _selectedExpense;
    public ExpenseEntry? SelectedExpense
    {
        get => _selectedExpense;

        set
        {
            if (SetProperty(ref _selectedExpense, value))
            {
                OnPropertyChanged();

                OnPropertyChanged(nameof(IsExpenseSelected));
                OnPropertyChanged(nameof(AddButtonIcon));
            }
        }
    }

    public bool IsExpenseSelected =>
        SelectedExpense != null;

    public ICommand AddTripCommand { get; }
    public ICommand DeleteTripCommand { get; }
    public ICommand DeleteExpenseCommand { get; }

    public MainViewModel()
    {

        _balanceChartBuilder = new BalanceChartBuilder();

        _tripStorage = new TripStorageService();

        _storage = new JsonStorageService();
        _dialogService = new DialogService();

        TripsView = CollectionViewSource.GetDefaultView(Trips);
        TripsView.Filter = obj => obj is Trip trip && TripFilter.Match(trip, SearchText);

        CurrentExpensesView =
        CollectionViewSource.GetDefaultView(CurrentExpenses);

        CurrentExpensesView.SortDescriptions.Add(
            new SortDescription(
                nameof(ExpenseEntry.Date),
                ListSortDirection.Ascending));

        EditTripCommand = new RelayCommand(t => EditTrip(t as Trip));

        AddTripCommand = new RelayCommand(_ => AddTrip());

        DeleteTripCommand = new RelayCommand(trip 
            => DeleteTrip(trip as Trip));

        DeleteExpenseCommand = new RelayCommand(_ => DeleteSelectedExpense());


        foreach (var trip in _storage.LoadTrips())
        {
            var loadedTrip = _tripStorage.LoadTrip(trip);

            Trips.Add(loadedTrip);
        }


        if (Trips.Count > 0)
        {
            CreateBalanceChart();

            SelectedTrip = Trips.First();
        }
        else
        {
            CreateBalanceChart();
        }

    }

    private void CreateBalanceChart()
    {
        BalanceXAxes = _balanceChartBuilder.BuildXAxes();
        BalanceYAxes = _balanceChartBuilder.BuildYAxes();

        OnPropertyChanged(nameof(BalanceXAxes));
        OnPropertyChanged(nameof(BalanceYAxes));
    }

    private void UpdateBalanceChart()
    {
        BalanceSeries = _balanceChartBuilder.BuildSeries(CurrentExpenses);

        OnPropertyChanged(nameof(BalanceSeries));
    }

    public string AddButtonIcon
    {
        get
        {
            return SelectedExpense == null
                ? "/Assets/add.png"
                : "/Assets/refresh.png";
        }
    }

    private void EditTrip(Trip? trip)
    {
        if (trip == null)
            return;

        var edited = _dialogService.ShowTripDialog(trip);

        if (edited == null)
            return;

        trip.Organization = edited.Organization;
        trip.City = edited.City;
        trip.StartDate = edited.StartDate;
        trip.EndDate = edited.EndDate;

        TripsView.Refresh();

        _tripStorage.SaveTrip(trip);
        _storage.SaveTrips(Trips);
    }

    private void DeleteSelectedExpense()
    {
        if (SelectedExpense == null || SelectedTrip == null)
            return;

        CurrentExpenses.Remove(SelectedExpense);

        _tripStorage.SaveTrip(SelectedTrip);

        SelectedExpense = null;

        OnPropertyChanged(nameof(TotalIncome));
        OnPropertyChanged(nameof(TotalExpenses));
        OnPropertyChanged(nameof(Balance));

        UpdateBalanceChart();
    }

    public void AddExpense(ExpenseEntry entry)
    {
        if (SelectedTrip == null)
            return;

        if (SelectedExpense != null)
        {
            SelectedExpense.Date = entry.Date;
            SelectedExpense.Rent = entry.Rent;
            SelectedExpense.Fuel = entry.Fuel;
            SelectedExpense.PerDiem = entry.PerDiem;
            SelectedExpense.Tickets = entry.Tickets;
            SelectedExpense.Other = entry.Other;
            SelectedExpense.Income = entry.Income;
        }

        else
        {
            CurrentExpenses.Add(entry);
        }

        CurrentExpensesView.Refresh();

        _tripStorage.SaveTrip(SelectedTrip);
        SelectedExpense = null;

        OnPropertyChanged(nameof(TotalIncome));
        OnPropertyChanged(nameof(TotalExpenses));
        OnPropertyChanged(nameof(Balance));

        UpdateBalanceChart();

    }

    private void AddTrip()
    {
        var trip = _dialogService.ShowTripDialog();

        if (trip == null)
            return;


        if (string.IsNullOrWhiteSpace(trip.Title))
        {
            trip.Title = $"Командировка №{Trips.Count + 1}";
        }


        Trips.Add(trip);

        SelectedTrip = trip;


        _tripStorage.SaveTrip(trip);
        _storage.SaveTrips(Trips);
    }

    private void DeleteTrip(Trip? trip)
    {
        if (trip == null)
            return;

        if (!_dialogService.ConfirmDelete())
            return;

        Trips.Remove(trip);

        if (SelectedTrip == trip)
            SelectedTrip = Trips.FirstOrDefault();

        _storage.SaveTrips(Trips);
    }

}