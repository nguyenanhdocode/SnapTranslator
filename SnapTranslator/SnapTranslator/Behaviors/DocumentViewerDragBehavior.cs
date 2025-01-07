using Microsoft.Xaml.Behaviors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace SnapTranslator.Behaviors
{
    public class DocumentViewerDragBehavior : Behavior<DocumentViewer>
    {
        private bool isDragging = false;
        private Point lastMousePosition;
        private ScrollViewer? scrollViewer;
        private RoutedEventHandler AssociatedObject_Loaded_Hanler;

        public DocumentViewerDragBehavior()
        {
            AssociatedObject_Loaded_Hanler = (sender, e) =>
            {
                scrollViewer = FindScrollViewer(this.AssociatedObject);

                if (scrollViewer != null)
                {
                    this.AssociatedObject.PreviewMouseDown += DocumentViewer_MouseDown;
                    this.AssociatedObject.PreviewMouseMove += DocumentViewer_MouseMove;
                    this.AssociatedObject.PreviewMouseUp += DocumentViewer_MouseUp;
                }
            };
        }

        protected override void OnAttached()
        {
            base.OnAttached();
            this.AssociatedObject.Loaded += AssociatedObject_Loaded_Hanler;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();

            this.AssociatedObject.Loaded -= AssociatedObject_Loaded_Hanler;

            this.AssociatedObject.PreviewMouseDown -= DocumentViewer_MouseDown;
            this.AssociatedObject.PreviewMouseMove -= DocumentViewer_MouseMove;
            this.AssociatedObject.PreviewMouseUp -= DocumentViewer_MouseUp;
        }

        private ScrollViewer? FindScrollViewer(DependencyObject obj)
        {
            if (obj is ScrollViewer viewer)
                return viewer;

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
            {
                var child = VisualTreeHelper.GetChild(obj, i);
                var result = FindScrollViewer(child);
                if (result != null)
                    return result;
            }

            return null;
        }

        private void DocumentViewer_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left && scrollViewer != null)
            {
                isDragging = true;
                lastMousePosition = e.GetPosition(this.AssociatedObject);
            }
        }

        private void DocumentViewer_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDragging && scrollViewer != null)
            {
                Point currentMousePosition = e.GetPosition(this.AssociatedObject);

                double deltaX = lastMousePosition.X - currentMousePosition.X;
                double deltaY = lastMousePosition.Y - currentMousePosition.Y;

                scrollViewer.ScrollToHorizontalOffset(scrollViewer.HorizontalOffset + deltaX);
                scrollViewer.ScrollToVerticalOffset(scrollViewer.VerticalOffset + deltaY);

                lastMousePosition = currentMousePosition;
            }
        }

        private void DocumentViewer_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                isDragging = false;
            }
        }
    }
}
