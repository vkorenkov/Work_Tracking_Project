using NewWorkTracking.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Interactivity;


namespace NewWorkTracking.Behavior
{
    /// <summary>
    /// Класс поведения таблицы DataGrid
    /// </summary>
    class DataGridBehavior : Behavior<DataGrid>
    {
        protected override void OnAttached()
        {
            base.OnAttached();

            // Подписка на событие изменения выбора dataGrid
            AssociatedObject.SelectionChanged += AssociatedObject_SelectionChanged;
        }

        // Обрботчик события изсенения выбора DataGrid
        private void AssociatedObject_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is DataGrid grid)
            {
                // Проверка на пустой объект
                if (grid.SelectedItem != null)
                {
                    grid.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        //grid.ScrollIntoView(grid.SelectedItem, null);
                    }));
                }

                // Формирование коллекции выбранных объектов
                AllWorksViewModel.SelectionChanged(grid.SelectedItems);
            }
        }
    }
}
