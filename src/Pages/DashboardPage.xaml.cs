using GrayKeeper.Controls.Cards;
using GrayKeeper.Controls.Dashboard;
using GrayKeeper.Models;
using GrayKeeper.ViewModels;
using System;
using GrayKeeper.Dialogs;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.ComponentModel;
using System.Windows.Media.Animation;

namespace GrayKeeper.Pages;

public partial class DashboardPage : UserControl
{
    // ============================================================
    // КОНСТРУКТОР
    // ============================================================
    public DashboardPage()
    {
        InitializeComponent();

        DateBox.PreviewTextInput += DateBox_PreviewTextInput;
        DataObject.AddPastingHandler(DateBox, DateBox_Pasting);

        PreviewKeyDown += DashboardPage_PreviewKeyDown;

        DataContextChanged += DashboardPage_DataContextChanged;
    }


    // ============================================================
    // ВВОД ДАННЫХ
    // ============================================================
    private void DateBox_PreviewTextInput(
        object sender,
        TextCompositionEventArgs e)
    {
        const string separators = ".,/<>?";

        if (e.Text.Length == 1 &&
            separators.Contains(e.Text))
        {
            e.Handled = true;

            int start = DateBox.SelectionStart;
            int length = DateBox.SelectionLength;

            DateBox.Text =
                DateBox.Text.Remove(start, length)
                            .Insert(start, ".");

            DateBox.SelectionStart = start + 1;
            DateBox.SelectionLength = 0;
        }
    }

    private void DateBox_Pasting(
    object sender,
    DataObjectPastingEventArgs e)
    {
        if (!e.DataObject.GetDataPresent(typeof(string)))
        {
            e.CancelCommand();
            return;
        }

        var text = e.DataObject.GetData(typeof(string)) as string;

        if (string.IsNullOrEmpty(text))
            return;

        text = text.Replace(',', '.')
                   .Replace('/', '.')
                   .Replace('<', '.')
                   .Replace('>', '.')
                   .Replace('?', '.');

        e.DataObject.SetData(typeof(string), text);
    }


    // ============================================================
    // ОБРАБОТКА КЛАВИАТУРЫ
    // ============================================================
    private void DashboardPage_PreviewKeyDown(
        object sender,
        KeyEventArgs e)
    {
        if (e.Key == Key.Enter)
        {
            AddExpense();

            e.Handled = true;
        }
    }


    // ============================================================
    // РАБОТА С РАСХОДАМИ
    // ============================================================
    private void AddExpense_Click(
        object sender,
        RoutedEventArgs e)
    {
        AddExpense();
    }

    private void AddExpense()
    {
        if (DataContext is not MainViewModel vm)
            return;

        if (!TryParseDate(DateBox.Text, out var date))
        {
            var messageWindow = new MessageWindow(
                "Некорректная дата",
                "Введите дату в формате 01.08.26 или 01.08.2026")
            {
                Owner = Window.GetWindow(this)
            };

            messageWindow.ShowDialog();

            return;
        }

        var entry = new ExpenseEntry
        {
            Date = date,
            Rent = Parse(RentBox.Text),
            Fuel = Parse(FuelBox.Text),
            PerDiem = Parse(PerDiemBox.Text),
            Tickets = Parse(TicketBox.Text),
            Other = Parse(OtherBox.Text),
            Income = Parse(IncomeBox.Text)
        };

        vm.AddExpense(entry);

        ClearFields();
    }


    // ============================================================
    // ОБРАБОТКА КЛИКА МЫШИ
    // ============================================================
    private void DashboardPage_PreviewMouseDown(
        object sender,
        MouseButtonEventArgs e)
    {
        if (e.OriginalSource is DependencyObject source)
        {
            var item = FindParent<ListBoxItem>(source);
            var input = FindParent<TextBox>(source);
            var button = FindParent<Button>(source);

            if (item == null &&
                input == null &&
                button == null)
            {
                Keyboard.ClearFocus();

                if (DataContext is MainViewModel vm)
                    vm.SelectedExpense = null;
            }
        }
    }


    // ============================================================
    // РАБОТА С VISUAL TREE
    // ============================================================
    private static T? FindParent<T>(
        DependencyObject child)
        where T : DependencyObject
    {
        while (child != null)
        {
            if (child is T result)
                return result;

            child = VisualTreeHelper.GetParent(child);
        }

        return null;
    }


    // ============================================================
    // ПАРСИНГ ДАТЫ
    // ============================================================
    private bool TryParseDate(string text,out DateTime date)
    {
        text = text.Replace(',', '.')
                   .Replace('/', '.')
                   .Replace('<', '.')
                   .Replace('>', '.')
                   .Replace('?', '.');

        string[] formats =
        {
            "dd.MM.yyyy",
            "d.MM.yyyy",
            "dd.MM.yy",
            "d.MM.yy"
        };

        return DateTime.TryParseExact(
            text,
            formats,
            System.Globalization.CultureInfo.InvariantCulture,
            System.Globalization.DateTimeStyles.None,
            out date
        );
    }


    // ============================================================
    // ПАРСИНГ ЧИСЛОВЫХ ЗНАЧЕНИЙ
    // ============================================================
    private decimal Parse(string text)
    {
        text = text.Replace(',', '.');

        decimal.TryParse(
            text,
            System.Globalization.NumberStyles.Any,
            System.Globalization.CultureInfo.InvariantCulture,
            out var value
        );

        return value;
    }


    // ============================================================
    // ОЧИСТКА ПОЛЕЙ
    // ============================================================
    private void ClearFields()
    {
        DateBox.Text = "";
        RentBox.Text = "";
        FuelBox.Text = "";
        PerDiemBox.Text = "";
        TicketBox.Text = "";
        OtherBox.Text = "";
        IncomeBox.Text = "";
    }

    // ============================================================
    // АНИМАЦИЯ
    // ============================================================
    private MainViewModel? _dashboardViewModel;

    private void DashboardPage_DataContextChanged(
        object sender,
        DependencyPropertyChangedEventArgs e)
    {
        if (_dashboardViewModel != null)
        {
            _dashboardViewModel.PropertyChanged -=
                ViewModel_PropertyChanged;
        }

        _dashboardViewModel =
            e.NewValue as MainViewModel;

        if (_dashboardViewModel != null)
        {
            _dashboardViewModel.PropertyChanged +=
                ViewModel_PropertyChanged;

            AnimateExpenseButtons(
                _dashboardViewModel.SelectedExpense != null);
        }
    }

    private void ViewModel_PropertyChanged(
        object? sender,
        PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(MainViewModel.SelectedExpense))
        {
            AnimateExpenseButtons(
                _dashboardViewModel?.SelectedExpense != null);
        }
    }

    private void AnimateExpenseButtons(bool isSelected)
    {
        if (isSelected)
        {
            // Кнопки должны стать доступными
            EditExpenseButton.IsHitTestVisible = true;
            DeleteExpenseButton.IsHitTestVisible = true;

            // -----------------------------
            // КНОПКА "+"
            // -----------------------------

            var addFadeOut = new DoubleAnimation
            {
                From = AddExpenseButton.Opacity,
                To = 0,
                Duration = TimeSpan.FromMilliseconds(160)
            };

            var addScaleOut = new DoubleAnimation
            {
                From = AddButtonScale.ScaleX,
                To = 0.6,
                Duration = TimeSpan.FromMilliseconds(160),
                EasingFunction = new BackEase
                {
                    Amplitude = 0.8,
                    EasingMode = EasingMode.EaseIn
                }
            };

            AddExpenseButton.BeginAnimation(
                OpacityProperty,
                addFadeOut);

            AddButtonScale.BeginAnimation(
                ScaleTransform.ScaleXProperty,
                addScaleOut);

            AddButtonScale.BeginAnimation(
                ScaleTransform.ScaleYProperty,
                addScaleOut);


            // -----------------------------
            // КНОПКА "РЕДАКТИРОВАТЬ"
            // -----------------------------

            var editFadeIn = new DoubleAnimation
            {
                From = 0,
                To = 1,
                Duration = TimeSpan.FromMilliseconds(180)
            };

            var editScaleIn = new DoubleAnimation
            {
                From = 0.6,
                To = 1,
                Duration = TimeSpan.FromMilliseconds(220),
                EasingFunction = new BackEase
                {
                    Amplitude = 1.2,
                    EasingMode = EasingMode.EaseOut
                }
            };

            EditExpenseButton.BeginAnimation(
                OpacityProperty,
                editFadeIn);

            EditButtonScale.BeginAnimation(
                ScaleTransform.ScaleXProperty,
                editScaleIn);

            EditButtonScale.BeginAnimation(
                ScaleTransform.ScaleYProperty,
                editScaleIn);


            // -----------------------------
            // КНОПКА "УДАЛИТЬ"
            // -----------------------------

            var deleteFadeIn = new DoubleAnimation
            {
                From = 0,
                To = 1,
                Duration = TimeSpan.FromMilliseconds(180)
            };

            var deleteScaleIn = new DoubleAnimation
            {
                From = 0.6,
                To = 1,
                Duration = TimeSpan.FromMilliseconds(220),
                EasingFunction = new BackEase
                {
                    Amplitude = 1.2,
                    EasingMode = EasingMode.EaseOut
                }
            };

            var deleteMoveIn = new DoubleAnimation
            {
                From = -50,
                To = 0,
                Duration = TimeSpan.FromMilliseconds(220),
                EasingFunction = new BackEase
                {
                    Amplitude = 1.1,
                    EasingMode = EasingMode.EaseOut
                }
            };

            DeleteExpenseButton.BeginAnimation(
                OpacityProperty,
                deleteFadeIn);

            DeleteButtonScale.BeginAnimation(
                ScaleTransform.ScaleXProperty,
                deleteScaleIn);

            DeleteButtonScale.BeginAnimation(
                ScaleTransform.ScaleYProperty,
                deleteScaleIn);

            DeleteButtonTranslate.BeginAnimation(
                TranslateTransform.XProperty,
                deleteMoveIn);
        }
        else
        {
            // -----------------------------
            // КНОПКА "РЕДАКТИРОВАТЬ"
            // -----------------------------

            var editFadeOut = new DoubleAnimation
            {
                From = 1,
                To = 0,
                Duration = TimeSpan.FromMilliseconds(150)
            };

            var editScaleOut = new DoubleAnimation
            {
                From = 1,
                To = 0.6,
                Duration = TimeSpan.FromMilliseconds(180),
                EasingFunction = new BackEase
                {
                    Amplitude = 0.8,
                    EasingMode = EasingMode.EaseIn
                }
            };

            EditExpenseButton.BeginAnimation(
                OpacityProperty,
                editFadeOut);

            EditButtonScale.BeginAnimation(
                ScaleTransform.ScaleXProperty,
                editScaleOut);

            EditButtonScale.BeginAnimation(
                ScaleTransform.ScaleYProperty,
                editScaleOut);


            // -----------------------------
            // КНОПКА "УДАЛИТЬ"
            // -----------------------------

            var deleteFadeOut = new DoubleAnimation
            {
                From = 1,
                To = 0,
                Duration = TimeSpan.FromMilliseconds(150)
            };

            var deleteScaleOut = new DoubleAnimation
            {
                From = 1,
                To = 0.6,
                Duration = TimeSpan.FromMilliseconds(180),
                EasingFunction = new BackEase
                {
                    Amplitude = 0.8,
                    EasingMode = EasingMode.EaseIn
                }
            };

            var deleteMoveOut = new DoubleAnimation
            {
                From = 0,
                To = -50,
                Duration = TimeSpan.FromMilliseconds(180),
                EasingFunction = new BackEase
                {
                    Amplitude = 0.8,
                    EasingMode = EasingMode.EaseIn
                }
            };

            DeleteExpenseButton.BeginAnimation(
                OpacityProperty,
                deleteFadeOut);

            DeleteButtonScale.BeginAnimation(
                ScaleTransform.ScaleXProperty,
                deleteScaleOut);

            DeleteButtonScale.BeginAnimation(
                ScaleTransform.ScaleYProperty,
                deleteScaleOut);

            DeleteButtonTranslate.BeginAnimation(
                TranslateTransform.XProperty,
                deleteMoveOut);


            // -----------------------------
            // КНОПКА "+"
            // -----------------------------

            var addFadeIn = new DoubleAnimation
            {
                From = 0,
                To = 1,
                Duration = TimeSpan.FromMilliseconds(180)
            };

            var addScaleIn = new DoubleAnimation
            {
                From = 0.6,
                To = 1,
                Duration = TimeSpan.FromMilliseconds(220),
                EasingFunction = new BackEase
                {
                    Amplitude = 1.1,
                    EasingMode = EasingMode.EaseOut
                }
            };

            addScaleIn.Completed += (_, _) =>
            {
                EditExpenseButton.IsHitTestVisible = false;
                DeleteExpenseButton.IsHitTestVisible = false;
            };

            AddExpenseButton.BeginAnimation(
                OpacityProperty,
                addFadeIn);

            AddButtonScale.BeginAnimation(
                ScaleTransform.ScaleXProperty,
                addScaleIn);

            AddButtonScale.BeginAnimation(
                ScaleTransform.ScaleYProperty,
                addScaleIn);
        }
    }

    private void EditExpense_Click(
    object sender,
    RoutedEventArgs e)
    {
        AddExpense();
    }



}