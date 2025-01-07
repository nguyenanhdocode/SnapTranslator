using Microsoft.Xaml.Behaviors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Media3D;

namespace SnapTranslator.Behaviors
{
    public class ResizableWindowBehavior : Behavior<UIElement>
    {
        private enum Directions
        {
            Right, Bottom, BottomRight
        }

        public static readonly DependencyProperty RightBarElement =
        DependencyProperty.Register("RightBar"
            , typeof(UIElement)
            , typeof(ResizableWindowBehavior)
            , new PropertyMetadata(null));

        public static readonly DependencyProperty BottomBarElement =
        DependencyProperty.Register("BottomBar"
            , typeof(UIElement)
            , typeof(ResizableWindowBehavior)
            , new PropertyMetadata(null));

        public static readonly DependencyProperty BottomRightElement =
        DependencyProperty.Register("BottomRight"
            , typeof(UIElement)
            , typeof(ResizableWindowBehavior)
            , new PropertyMetadata(null));

        public static readonly DependencyProperty WindowProperty =
        DependencyProperty.Register("Window"
            , typeof(Window)
            , typeof(ResizableWindowBehavior)
            , new PropertyMetadata(null));

        private MouseButtonEventHandler?
              RightBar_OnMouseDown_Handler
            , BottomRight_OnMouseDown_Handler
            , BottomBar_OnMouseDown_Handler

            , RightBar_OnMouseUp_Handler
            , BottomRight_OnMouseUp_Handler
            , BottomBar_OnMouseUp_Handler;

        private MouseEventHandler Window_OnMouseMove_Hander;

        private bool _isResizing;
        private Point _clickPosition;
        private Directions _direction;

        public ResizableWindowBehavior()
        {
            RightBar_OnMouseDown_Handler = (sender, args) =>
            {
                _direction = Directions.Right;
                OnMouseDown(sender, args);
            };

            BottomRight_OnMouseDown_Handler = (sender, args) =>
            {
                _direction = Directions.BottomRight;
                OnMouseDown(sender, args);
            };

            BottomBar_OnMouseDown_Handler = (sender, args) =>
            {
                _direction = Directions.Bottom;
                OnMouseDown(sender, args);
            };

            RightBar_OnMouseUp_Handler = (sender, args) =>
            {
                OnMouseUp(sender, args);
            };

            BottomRight_OnMouseUp_Handler = (sender, args) =>
            {
                OnMouseUp(sender, args);
            };

            BottomBar_OnMouseUp_Handler = (sender, args) =>
            {
                OnMouseUp(sender, args);
            };

            Window_OnMouseMove_Hander = (sender, args) =>
            {
                Window_OnMouseMove(sender, args);
            };
        }

        public UIElement? RightBar
        {
            get
            {
                return (Grid)GetValue(RightBarElement);
            }
            set 
            { 
                SetValue(RightBarElement, value);
            }
        }

        public UIElement? BottomBar
        {
            get
            {
                return (UIElement)GetValue(BottomBarElement);
            }
            set 
            { 
                SetValue(BottomBarElement, value); 
            }
        }

        public UIElement? BottomRight
        {
            get
            {
                return (UIElement)GetValue(BottomRightElement);
            }
            set 
            { 
                SetValue(BottomRightElement, value); 
            }
        }

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

            if (RightBar != null)
            {
                RightBar.MouseLeftButtonDown += RightBar_OnMouseDown_Handler;
                RightBar.MouseLeftButtonUp += RightBar_OnMouseUp_Handler;
            }

            if (BottomRight != null)
            {
                BottomRight.MouseLeftButtonDown += BottomRight_OnMouseDown_Handler;
                BottomRight.MouseLeftButtonUp += BottomRight_OnMouseUp_Handler;
            }

            if (BottomBar != null)
            {
                BottomBar.MouseLeftButtonDown += BottomBar_OnMouseDown_Handler;
                BottomBar.MouseLeftButtonUp += BottomBar_OnMouseUp_Handler;
            }

            Window.MouseMove += Window_OnMouseMove_Hander;
        }

        protected override void OnDetaching()
        {
            if (RightBar != null)
            {
                RightBar.MouseLeftButtonDown -= RightBar_OnMouseDown_Handler;
                RightBar.MouseLeftButtonUp -= RightBar_OnMouseUp_Handler;
            }

            if (BottomRight != null)
            {
                BottomRight.MouseLeftButtonDown -= BottomRight_OnMouseDown_Handler;
                BottomRight.MouseLeftButtonUp -= BottomRight_OnMouseUp_Handler;
            }

            if (BottomBar != null)
            {
                BottomBar.MouseLeftButtonDown -= BottomBar_OnMouseDown_Handler;
                BottomBar.MouseLeftButtonUp -= BottomBar_OnMouseUp_Handler;
            }

            Window.MouseMove -= Window_OnMouseMove_Hander;

            base.OnDetaching();
        }

        private void OnMouseDown(object sender, MouseEventArgs args)
        {
            if (args.LeftButton == MouseButtonState.Pressed)
            {
                _isResizing = true;
                _clickPosition = args.GetPosition(Window);
                Mouse.Capture(sender as IInputElement);
            }
        }

        private void Window_OnMouseMove(object sender, MouseEventArgs args)
        {
            if (_isResizing)
            {
                var currentPosition = args.GetPosition(Window);

                if (_direction == Directions.Right)
                {
                    double newWidth = Window.ActualWidth + (currentPosition.X - _clickPosition.X);
                    if (newWidth > Window.MinWidth)
                    {
                        Window.Width = newWidth;
                        _clickPosition.X = currentPosition.X;
                    }
                }
                else if (_direction == Directions.Bottom)
                {
                    double newHeight = Window.Height + (currentPosition.Y - _clickPosition.Y);
                    if (newHeight > Window.MinHeight)
                    {
                        Window.Height = newHeight;
                        _clickPosition.Y = currentPosition.Y;
                    }
                }
                else if (_direction == Directions.BottomRight)
                {
                    double newHeight = Window.Height + (currentPosition.Y - _clickPosition.Y);
                    double newWidth = Window.Width + (currentPosition.X - _clickPosition.X);
                    if (newHeight > Window.MinHeight)
                    {
                        Window.Height = newHeight;
                        _clickPosition.Y = currentPosition.Y;
                    }
                    if (newWidth > Window.MinWidth)
                    {
                        Window.Width = newWidth;
                        _clickPosition.X = currentPosition.X;
                    }
                }
            }
        }

        private void OnMouseUp(object sender, MouseEventArgs args)
        {
            _isResizing = false;
            Mouse.Capture(null);
        }
    }
}
