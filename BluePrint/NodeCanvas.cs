using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace BluePrint.UI
{
    public class NodeCanvas : Canvas
    {
        internal UIElement? CurrentElement { get; set; }

        private Point? _elementPoint;

        protected override void OnPreviewMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            if (e.Source is Node node)
            {
                CurrentElement = node;
                _elementPoint = e.GetPosition(node);

                return;
            }

            CurrentElement = null;
            _elementPoint = null;
        }

        protected override void OnPreviewMouseMove(MouseEventArgs e)
        {
            Point mouse = e.GetPosition(this);

            if (CurrentElement != null && _elementPoint != null && e.LeftButton == MouseButtonState.Pressed)
            {
                double left = mouse.X - _elementPoint.Value.X;
                double top = mouse.Y - _elementPoint.Value.Y;

                SetLeft(CurrentElement, left);
                SetTop(CurrentElement, top);
            }
        }
    }
}
