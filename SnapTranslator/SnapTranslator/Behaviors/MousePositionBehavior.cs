using Microsoft.Xaml.Behaviors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace SnapTranslator.Behaviors
{
    public class MousePositionBehavior : Behavior<UIElement>
    {
        public static readonly DependencyProperty MousePositionProperty =
        DependencyProperty.Register("MousePosition"
            , typeof(Point)
            , typeof(MousePositionBehavior)
            , new PropertyMetadata(default(Point)));

        public Point MousePosition
        {
            get => (Point)GetValue(MousePositionProperty);
            set => SetValue(MousePositionProperty, value);
        }

        protected override void OnAttached()
        {
            base.OnAttached();
            this.AssociatedObject.MouseMove += OnMouseMove;
        }

        protected override void OnDetaching()
        {
            this.AssociatedObject.MouseMove -= OnMouseMove;
            base.OnDetaching();
        }

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            MousePosition = e.GetPosition(sender as IInputElement);
        }
    }
}
