using Microsoft.Xaml.Behaviors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SnapTranslator.Behaviors
{
    public class DraggleWindowBehavior : Behavior<UIElement>
    {

        public static readonly DependencyProperty WindowProperty =
        DependencyProperty.Register("Window"
            , typeof(Window)
            , typeof(DraggleWindowBehavior)
            , new PropertyMetadata(null));

        private bool _isDragging;
        private Point _clickPosition;

        public Window Window
        {
            get
            {
                if (GetValue(WindowProperty) == null)
                    throw new ArgumentNullException("Window must be not null.");

                return (Window)GetValue(WindowProperty);
            }
            set
            {
                SetValue(WindowProperty, value);
            }
        }

        protected override void OnAttached()
        {
            base.OnAttached();
            this.AssociatedObject.MouseLeftButtonDown += OnMouseDown;
            this.AssociatedObject.MouseMove += OnMouseMove;
            this.AssociatedObject.MouseLeftButtonUp += OnMouseUp;
        }

        protected override void OnDetaching()
        {
            this.AssociatedObject.MouseLeftButtonDown -= OnMouseDown;
            this.AssociatedObject.MouseMove -= OnMouseMove;
            this.AssociatedObject.MouseLeftButtonUp -= OnMouseUp;
            base.OnDetaching();
        }

        private void OnMouseDown(object sender, MouseEventArgs args)
        {
            _isDragging = true;
            _clickPosition = args.GetPosition(Window);
            Mouse.Capture(args.OriginalSource as IInputElement);
        }

        private void OnMouseMove(object sender, MouseEventArgs args)
        {
            if (_isDragging)
            {
                var pos = args.GetPosition(Window);
                var offset = pos - _clickPosition;

                double screenWidth = SystemParameters.PrimaryScreenWidth;
                double screenHeight = SystemParameters.PrimaryScreenHeight;
                double newLeft = Window.Left + offset.X;
                double newTop = Window.Top + offset.Y;

                if (newLeft < 0)
                    newLeft = 0;
                if (newTop < 0)
                    newTop = 0;
                if (newLeft + Window.ActualWidth > screenWidth)
                    newLeft = screenWidth - Window.ActualWidth;
                if (newTop + Window.Height > screenHeight)
                    newTop = screenHeight - Window.Height;

                Window.Left = (int)newLeft;
                Window.Top = (int)newTop;
            }
        }

        private void OnMouseUp(object sender, MouseEventArgs args)
        {
            if (_isDragging)
            {
                _isDragging = false;
                Mouse.Capture(null);
            }
        }
    }
}
