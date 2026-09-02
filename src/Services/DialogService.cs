using GrayKeeper.Dialogs;
using GrayKeeper.Interfaces;
using GrayKeeper.Models;
using System.Windows;

namespace GrayKeeper.Services;

public class DialogService : IDialogService
{
    public Trip? ShowTripDialog(Trip? trip = null)
    {
        var dialog = new TripDialog(trip);
        dialog.Owner = Application.Current.MainWindow;
        var result = dialog.ShowDialog();

        if (result == true)
            return dialog.Result;

        return null;
    }




    public bool ConfirmDelete()
    {
        var dialog = new ConfirmDialog();
        dialog.Owner = Application.Current.MainWindow;

        return dialog.ShowDialog() == true;
    }
}