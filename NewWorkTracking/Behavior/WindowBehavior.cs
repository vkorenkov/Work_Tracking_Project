using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interactivity;

namespace NewWorkTracking.Behavior
{
    class WindowBehavior : Behavior<Window>
    {
        // Пременная диалогового окна
        public static Window window;

        protected override void OnAttached()
        {
            base.OnAttached();

            // Подписка на событие удержания левой кнопки мыши на верхней панели
            this.AssociatedObject.MouseLeftButtonDown += TopPanel_MouseLeftButtonDown;
        }

        // Обработчик события удержания левой кнопки мыши на верхней панели
        private void TopPanel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is Window)
            {
                window = sender as Window;
            }

            // Метод перетаскивания окна
            window.DragMove();
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
        }
    }
}
