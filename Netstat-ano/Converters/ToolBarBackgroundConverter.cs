using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace Netstat_ano.Converters
{
    public class ToolBarBackgroundConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int count)
            {
                return (count % 3) switch
                {
                    0 => new SolidColorBrush { Color = Color.FromRgb(4, 64, 155), Opacity = 0.1 },
                    1 => new SolidColorBrush { Color = Color.FromRgb(247, 211, 13), Opacity = 0.1 },
                    2 => new SolidColorBrush { Color = Color.FromRgb(27, 27, 27), Opacity = 0.1 },
                    _ => new SolidColorBrush { Color = Color.FromRgb(217, 98, 62), Opacity = 0.1 },
                };
            }
            else
            {
                return new SolidColorBrush { Color = Color.FromRgb(217, 98, 62), Opacity = 0.1 };
            }

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
}
