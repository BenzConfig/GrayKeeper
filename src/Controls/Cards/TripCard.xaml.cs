using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace GrayKeeper.Controls.Cards;

public partial class TripCard : UserControl
{
    public TripCard()
    {
        InitializeComponent();
    }

    private void CardRoot_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
    {
        if (!IsSelected)
        {
            CardRoot.BorderBrush =
                (Brush)Application.Current.Resources["TripBorderHoverBrush"];
        }
    }

    private void CardRoot_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
    {
        if (!IsSelected)
        {
            CardRoot.BorderBrush =
                (Brush)Application.Current.Resources["TripBorderNormalBrush"];
        }
    }

    public static readonly DependencyProperty IsSelectedProperty = DependencyProperty.Register(
        nameof(IsSelected),
        typeof(bool),
        typeof(TripCard),
        new PropertyMetadata(false, OnSelectedChanged)
        );

    public bool IsSelected
    {
        get => (bool)GetValue(IsSelectedProperty);
        set => SetValue(IsSelectedProperty, value);
    }

    private static void OnSelectedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is TripCard card)
        {
            card.UpdateSelection();
        }
    }

    private void UpdateSelection()
    {
        CardRoot.BorderBrush =
            (Brush)Application.Current.Resources[
                IsSelected
                    ? "TripBorderSelectedBrush"
                    : "TripBorderNormalBrush"];
    }

}