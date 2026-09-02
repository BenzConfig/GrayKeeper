using System.Windows.Controls;
using GrayKeeper.ViewModels;

namespace GrayKeeper.Controls.Dashboard;

public partial class BalanceChart : UserControl
{
    public BalanceChart()
    {
        InitializeComponent();

        Loaded += BalanceChart_Loaded;
    }


    private void BalanceChart_Loaded(
        object sender,
        System.Windows.RoutedEventArgs e)
    {
        if (DataContext is MainViewModel vm)
        {
            vm.PropertyChanged += Vm_PropertyChanged;
        }
    }


    private void Vm_PropertyChanged(
        object? sender,
        System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(MainViewModel.Balance))
        {
            Dispatcher.Invoke(() =>
            {
                DataContext = null;
                DataContext = sender;
            });
        }
    }
}