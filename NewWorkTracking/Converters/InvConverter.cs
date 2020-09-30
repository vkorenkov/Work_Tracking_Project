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
    /// Конвертер проверки вводимых символов
    /// </summary>
    class InvConverter : IValueConverter
    {
        /// <summary>
        /// Метод проверяет вводимые символы (Допускается ввод только цифр)
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                foreach (var t in value.ToString())
                {
                    if (!char.IsDigit(t))
                    {
                        value = value.ToString().Remove(value.ToString().Length - 1);

                        Message.Show("Ошибка ввода", "Номер заявки/основания должен содержать толькко цифры", MessageBoxButton.OK);

                        break;
                    }
                }
            }

            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {                      
            return value;
        }
    }
}
