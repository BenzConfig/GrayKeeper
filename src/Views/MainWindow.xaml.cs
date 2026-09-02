using System;
using GrayKeeper.ViewModels;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media.Imaging;

namespace GrayKeeper.Views;

public partial class MainWindow : Window
{
    [DllImport("user32.dll")]
    private static extern void ReleaseCapture();

    [DllImport("user32.dll")]

    private static extern IntPtr SendMessage(
        IntPtr hWnd,
        int Msg,
        IntPtr wParam,
        IntPtr lParam);

    private const int WM_NCLBUTTONDOWN = 0xA1;
    private const int HTCAPTION = 0x2;

    public MainWindow()
    {
        InitializeComponent();

        DataContext = new MainViewModel();

        StateChanged += MainWindow_StateChanged;

        UpdateMaximizeIcon();
    }

    private void DashboardPage_Loaded(object sender, RoutedEventArgs e)
    {
    }

    private void MinimizeButton_Click(object sender, RoutedEventArgs e)
    {
        WindowState = WindowState.Minimized;
    }

    private void MaximizeButton_Click(object sender, RoutedEventArgs e)
    {
        if (WindowState == WindowState.Maximized)
        {
            WindowState = WindowState.Normal;
        }

        else
        {
            WindowState = WindowState.Maximized;
        }
    }

    private void MainWindow_StateChanged(object? sender, EventArgs e)
    {
        UpdateMaximizeIcon();
    }

    private void UpdateMaximizeIcon()
    {
        MaximizeIcon.Source = new BitmapImage(
            new Uri(
                WindowState == WindowState.Maximized
                    ? "/Assets/window/shrink.png"
                    : "/Assets/window/expand.png",
                UriKind.Relative));
    }

    private void CloseButton_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }

    private void Header_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (e.ClickCount == 2)
        {
            WindowState = WindowState == WindowState.Maximized
                ? WindowState.Normal
                : WindowState.Maximized;

            return;
        }


        ReleaseCapture();

        SendMessage(
            new WindowInteropHelper(this).Handle,
            WM_NCLBUTTONDOWN,
            (IntPtr)HTCAPTION,
            IntPtr.Zero);
    }
}