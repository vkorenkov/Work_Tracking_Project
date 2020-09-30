using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace NewWorkTracking.Converters
{
    /// <summary>
    /// Конвертер удаления некорректных символов
    /// </summary>
    class CharCutConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }

        /// <summary>
        /// Метод принимает строку и удаляет из нее ненужные символы
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!string.IsNullOrEmpty(value.ToString()) && value.ToString().Length >= 10)
            {
                char[] symbols = new char[] { ' ', ',', '.', '?', '/', '\\', '-', '=', '_', '+', '!', '@', '#', '$', '%', '^', '&', '*', '(', ')', '<', '>', '~', ';', ':' };

                foreach (var t in symbols)
                {
                    if (value.ToString().Contains(t))
                    {
                        Message.Show("Ошибка заполнения", "Инвентарный номер должен содержать только буквы и цифры", MessageBoxButton.OK);

                        return null;
                    }
                }

                return value;
            }
            else
            {
                Message.Show("Ошибка заполнения", "Инвентарный номер должен содержать от 10 до 12 символов", MessageBoxButton.OK);

                return null;
            }
        }
    }
}
