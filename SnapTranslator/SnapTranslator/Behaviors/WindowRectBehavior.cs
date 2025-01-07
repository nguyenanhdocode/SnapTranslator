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
    public class WindowRectBehavior : Behavior<Window>
    {
        public static readonly DependencyProperty WindowPositionProperty =
        DependencyProperty.Register("WindowRect"
            , typeof(Rect)
            , typeof(WindowRectBehavior)
            , new PropertyMetadata(default(Rect)));

        public Rect WindowRect
        {
            get => (Rect)GetValue(WindowPositionProperty);
            set => SetValue(WindowPositionProperty, value);
        }

        protected override void OnAttached()
        {
            base.OnAttached();
            this.AssociatedObject.MouseMove += OnWindowMove;
        }

        protected override void OnDetaching()
        {
            this.AssociatedObject.MouseMove -= OnWindowMove;
            base.OnDetaching();
        }

        private void OnWindowMove(object sender, MouseEventArgs e)
        {
            var pos = new Point(this.AssociatedObject.Left, this.AssociatedObject.Top);
            var size = new Size(this.AssociatedObject.ActualWidth, this.AssociatedObject.ActualHeight);

            WindowRect = new Rect(pos, size);
        }
    }
}
