using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace WPFComSettingsViewLib.Converter
{
    internal class BoolToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolValue)
            {
                return boolValue ? Brushes.LightGreen : Brushes.Yellow;
            }
            return Brushes.Transparent;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}
