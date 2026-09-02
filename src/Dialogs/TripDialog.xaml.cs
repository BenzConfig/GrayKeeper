using GrayKeeper.Models;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace GrayKeeper.Dialogs;

public partial class TripDialog : Window
{
    public Trip? Result { get; private set; }

    public TripDialog()
        : this(null)
    {
    }

    public TripDialog(Trip? trip)
    {
        InitializeComponent();

        if (trip == null)
        {
            StartDatePicker.SelectedDate = DateTime.Today;
            EndDatePicker.SelectedDate = DateTime.Today;
            return;
        }

        OrganizationBox.Text = trip.Organization;
        CityBox.Text = trip.City;

        StartDatePicker.SelectedDate = trip.StartDate;
        EndDatePicker.SelectedDate = trip.EndDate;
    }

    private void Create_Click(object sender, RoutedEventArgs e)
    {
        Result = new Trip
        {
            Title = string.IsNullOrWhiteSpace(OrganizationBox.Text)
                ? "Командировка"
                : OrganizationBox.Text,

            Organization = OrganizationBox.Text,
            City = CityBox.Text,
            StartDate = StartDatePicker.SelectedDate ?? DateTime.Today,
            EndDate = EndDatePicker.SelectedDate ?? DateTime.Today
        };

        DialogResult = true;
        Close();
    }

    private void CloseButton_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }

    private void Header_MouseLeftButtonDown( object sender, MouseButtonEventArgs e)
    {
        DragMove();
    }
}