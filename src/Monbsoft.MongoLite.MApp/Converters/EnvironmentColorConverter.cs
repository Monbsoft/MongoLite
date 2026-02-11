using System.Globalization;
using Monbsoft.MongoLite.MApp.Models;

namespace Monbsoft.MongoLite.MApp.Converters;

public class EnvironmentColorConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is EnvironmentType env)
            return Color.FromArgb(env.GetColor());

        return Colors.Gray;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotSupportedException();
}
