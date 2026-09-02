using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace GrayKeeper.Behaviors;

public static class PopupAnimationBehavior
{

    public static readonly DependencyProperty EnableAnimationProperty = DependencyProperty.RegisterAttached(
        "EnableAnimation",
        typeof(bool),
        typeof(PopupAnimationBehavior),
        new PropertyMetadata(false, OnChanged)
    );

    public static void SetEnableAnimation(DependencyObject element, bool value)
    {
        element.SetValue(EnableAnimationProperty, value);
    }

    public static bool GetEnableAnimation(DependencyObject element)
    {
        return (bool)element.GetValue(EnableAnimationProperty);
    }

    private static void OnChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {

        if (d is Popup popup && (bool)e.NewValue)
        {
            popup.Opened += Popup_Opened;
        }

    }

    private static void Popup_Opened(object? sender, EventArgs e)
    {

        if (sender is not Popup popup)
            return;


        if (popup.Child == null)
            return;


        popup.Opacity = 0;


        var scale = new ScaleTransform(
            0.96,
            0.96);


        var translate = new TranslateTransform(
            0,
            -4);


        popup.Child.RenderTransform = new TransformGroup
        {
            Children =
        {
            scale,
            translate
        }
        };


        var storyboard = new Storyboard();



        // Прозрачность
        var opacity = new DoubleAnimation
        {
            From = 0,
            To = 1,
            Duration = TimeSpan.FromMilliseconds(120)
        };


        // Увеличение по X
        var scaleX = new DoubleAnimation
        {
            From = 0.96,
            To = 1,
            Duration = TimeSpan.FromMilliseconds(120)
        };


        // Увеличение по Y
        var scaleY = new DoubleAnimation
        {
            From = 0.96,
            To = 1,
            Duration = TimeSpan.FromMilliseconds(120)
        };


        // Смещение вниз
        var move = new DoubleAnimation
        {
            From = -4,
            To = 0,
            Duration = TimeSpan.FromMilliseconds(120)
        };



        Storyboard.SetTarget(opacity, popup.Child);

        Storyboard.SetTarget(scaleX, scale);

        Storyboard.SetTarget(scaleY, scale);

        Storyboard.SetTarget(move, translate);



        Storyboard.SetTargetProperty(
            opacity,
            new PropertyPath("Opacity"));


        Storyboard.SetTargetProperty(
            scaleX,
            new PropertyPath("ScaleX"));


        Storyboard.SetTargetProperty(
            scaleY,
            new PropertyPath("ScaleY"));


        Storyboard.SetTargetProperty(
            move,
            new PropertyPath("Y"));



        storyboard.Children.Add(opacity);

        storyboard.Children.Add(scaleX);

        storyboard.Children.Add(scaleY);

        storyboard.Children.Add(move);



        storyboard.Begin();

    }

}