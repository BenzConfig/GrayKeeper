using GrayKeeper.Views;
using System.Windows;
using System.Windows.Controls;
using ClosedXML.Excel;
using GrayKeeper.Models;
using GrayKeeper.ViewModels;
using Microsoft.Win32;
using System;
using System.Linq;


namespace GrayKeeper.Controls.Dashboard;

public partial class LeftSidebar : UserControl
{
    public LeftSidebar()
    {
        InitializeComponent();
    }

    private void ExportButton_Click( object sender, RoutedEventArgs e)
    {
        if (DataContext is not MainViewModel vm)
            return;

        vm.ExportCurrentTrip();
    }

    private void AboutButton_Click(object sender, RoutedEventArgs e)
    {
        var aboutWindow = new AboutWindow
        {
            Owner = Window.GetWindow(this)
        };

        aboutWindow.ShowDialog();
    }

}