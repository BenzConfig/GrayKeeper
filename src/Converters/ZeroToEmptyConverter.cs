using System;
using System.Globalization;
using System.Windows.Data;

namespace GrayKeeper.Converters;

public class ZeroToEmptyConverter : IValueConverter
{
    public object Convert(
        object value,
        Type targetType,
        object parameter,
        CultureInfo culture)
    {
        if (value is decimal decimalValue)
        {
            return decimalValue == 0
                ? ""
                : decimalValue.ToString(
                    CultureInfo.InvariantCulture);
        }

        return value?.ToString() ?? "";
    }

    public object ConvertBack(
        object value,
        Type targetType,
        object parameter,
        CultureInfo culture)
    {
        if (value is string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return 0m;

            text = text.Replace(',', '.');

            if (decimal.TryParse(
                text,
                NumberStyles.Any,
                CultureInfo.InvariantCulture,
                out var result))
            {
                return result;
            }
        }

        return 0m;
    }
}