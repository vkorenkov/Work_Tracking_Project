using System;
using System.Collections.Generic;
//using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using WorkTrackingLib.Models;

namespace NewWorkTracking.Converters
{
    /// <summary>
    /// Конвертер изменения цвета текста
    /// </summary>
    class RepairsTextColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value.GetType() == typeof(int))
            {
                return ReturnIntColor((int)value);
            }
            else
            {
                return ReturnTextColor((string)value);
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }

        /// <summary>
        /// Метод меняет цвет текста в зависимости от числа в ячейке DataGrid
        /// </summary>
        /// <param name="days"></param>
        /// <returns></returns>
        private SolidColorBrush ReturnIntColor(int days)
        {
            if (days <= 15)
            {
                return Brushes.White;
            }
            else if (days > 15 && days < 30)
            {
                return Brushes.Goldenrod;
            }
            else
            {
                return Brushes.Red;
            }
        }

        /// <summary>
        /// Метод изменения цвета текста статуса ремонта
        /// </summary>
        /// <param name="status"></param>
        /// <returns></returns>
        private SolidColorBrush ReturnTextColor(string status)
        {
            SolidColorBrush brushes = new SolidColorBrush();

            switch (status)
            {
                case "Диагностика":
                    brushes = Brushes.Gold;
                    break;
                case "Подготовка к отправке":
                    brushes = Brushes.Gold; ;
                    break;
                case "Отгружено в ремонт":
                    brushes = Brushes.Goldenrod; ;
                    break;
                case "Вернулось (отказ)":
                    brushes = Brushes.Red;
                    break;
                case "Вернулось (диагностика)":
                    brushes = Brushes.Gold;
                    break;
                case "Вернулось (исправно)":
                    brushes = Brushes.Green;
                    break;
                case "Вернулось (НРП / Разбор на п.к.)":
                    brushes = Brushes.Green;
                    break;
                case "Вернулось (ремонт по средствам замены)":
                    brushes = Brushes.Green;
                    break;
                case "Вернулось (неисправно)":
                    brushes = Brushes.Red;
                    break;
            }

            return brushes;
        }
    }
}
