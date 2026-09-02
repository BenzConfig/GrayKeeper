using GrayKeeper.Models;
using GrayKeeper.ViewModels;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace GrayKeeper.Controls.Cards;

public partial class ExpenseItemCard : UserControl
{
    private static ExpenseItemCard? _selectedCard;

    private static readonly Brush NormalBorderBrush =
        new SolidColorBrush(Color.FromRgb(51, 51, 51));

    private static readonly Brush SelectedBorderBrush =
        new SolidColorBrush(Color.FromRgb(100, 100, 100));

    private MainViewModel? _viewModel;

    public ExpenseItemCard()
    {
        InitializeComponent();

        Loaded += ExpenseCard_Loaded;
        DataContextChanged += ExpenseCard_DataContextChanged;
        PreviewMouseLeftButtonDown += ExpenseCard_PreviewMouseLeftButtonDown;
    }

    private void ExpenseCard_Loaded(
        object sender,
        RoutedEventArgs e)
    {
        BeginAnimation(
            OpacityProperty,
            new DoubleAnimation
            {
                From = 0,
                To = 1,
                Duration = TimeSpan.FromMilliseconds(180)
            });

        SubscribeToViewModel();

        UpdateBorder();
    }

    private void ExpenseCard_DataContextChanged(
        object sender,
        DependencyPropertyChangedEventArgs e)
    {
        UnsubscribeFromViewModel();

        if (_selectedCard == this)
            _selectedCard = null;

        CardBorder.BorderBrush = NormalBorderBrush;

        SubscribeToViewModel();

        UpdateBorder();
    }

    private void SubscribeToViewModel()
    {
        var window = Window.GetWindow(this);

        if (window?.DataContext is MainViewModel vm)
        {
            _viewModel = vm;
            _viewModel.PropertyChanged += ViewModel_PropertyChanged;
        }
    }

    private void UnsubscribeFromViewModel()
    {
        if (_viewModel != null)
        {
            _viewModel.PropertyChanged -= ViewModel_PropertyChanged;
            _viewModel = null;
        }
    }

    private void ViewModel_PropertyChanged(
        object? sender,
        PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(MainViewModel.SelectedExpense))
        {
            Dispatcher.Invoke(UpdateBorder);
        }
    }

    private void ExpenseCard_PreviewMouseLeftButtonDown(
        object sender,
        MouseButtonEventArgs e)
    {
        if (DataContext is not ExpenseEntry expense)
            return;

        var window = Window.GetWindow(this);

        if (window?.DataContext is not MainViewModel vm)
            return;

        // Снимаем обводку с предыдущей карточки
        if (_selectedCard != null &&
            _selectedCard != this)
        {
            _selectedCard.CardBorder.BorderBrush =
                NormalBorderBrush;
        }

        // Выбираем расход
        vm.SelectedExpense = expense;

        // Подсвечиваем текущую карточку
        CardBorder.BorderBrush = SelectedBorderBrush;

        _selectedCard = this;

        // e.Handled НЕ устанавливаем.
        // Фокус и существующая логика продолжают работать.
    }

    private void UpdateBorder()
    {
        if (DataContext is not ExpenseEntry expense)
        {
            CardBorder.BorderBrush = NormalBorderBrush;
            return;
        }

        var window = Window.GetWindow(this);

        if (window?.DataContext is MainViewModel vm &&
            ReferenceEquals(vm.SelectedExpense, expense))
        {
            CardBorder.BorderBrush = SelectedBorderBrush;
            _selectedCard = this;
        }
        else
        {
            CardBorder.BorderBrush = NormalBorderBrush;

            if (_selectedCard == this)
                _selectedCard = null;
        }
    }
}