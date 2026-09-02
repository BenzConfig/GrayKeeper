using System.Windows;

namespace GrayKeeper.Dialogs;

public partial class MessageWindow : Window
{
    public MessageWindow(
        string title,
        string message,
        Window? owner = null)
    {
        InitializeComponent();

        Owner = owner;

        TitleText.Text = title;
        MessageText.Text = message;
    }

    private void OkButton_Click(
        object sender,
        RoutedEventArgs e)
    {
        DialogResult = true;
    }
}