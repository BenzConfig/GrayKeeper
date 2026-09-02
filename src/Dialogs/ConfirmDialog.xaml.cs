using System.Windows;

namespace GrayKeeper.Dialogs;

public partial class ConfirmDialog : Window
{
    public ConfirmDialog()
    {
        InitializeComponent();
    }


    private void Delete_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = true;
    }
}