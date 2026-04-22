using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace WpfMvvmApp.Core;

/// <summary>
/// null → Collapsed, non-null → Visible.
/// </summary>
[ValueConversion(typeof(object), typeof(Visibility))]
public class NullToVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        bool inverse = parameter is string s && s.Equals("inverse", StringComparison.OrdinalIgnoreCase);
        bool isNull  = value is null;
        return (inverse ? isNull : !isNull) ? Visibility.Visible : Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => throw new NotImplementedException();
}
