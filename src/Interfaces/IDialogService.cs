using GrayKeeper.Models;

namespace GrayKeeper.Interfaces;

public interface IDialogService
{
    Trip? ShowTripDialog(Trip? trip = null);

    bool ConfirmDelete();
}