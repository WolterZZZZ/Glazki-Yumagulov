using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace Юмагулов_Глазки_save
{
    public class DiscountColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Безопасно проверяем значение на null и конвертируем в число
            if (value != null)
            {
                if (decimal.TryParse(value.ToString(), out decimal discount))
                {
                    // Если скидка больше или равна 25% — красим в светло-зеленый
                    if (discount >= 25)
                    {
                        return (SolidColorBrush)new BrushConverter().ConvertFrom("#CCFFCC");
                    }
                }
            }

            // Во всех остальных случаях оставляем белый фон
            return Brushes.White;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}