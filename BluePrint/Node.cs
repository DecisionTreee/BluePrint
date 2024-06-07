using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;

namespace BluePrint.UI
{
    public class Node : ContentControl
    {
        internal Point Left { get; private set; }
        internal Point Right { get; private set; }
        internal Point? DragStartPoint { get; set; } = null;
        internal List<NodeLine> Lines { get; set; }

        internal event EventHandler? Changed;

        public bool IsSelected
        {
            get { return (bool)GetValue(IsSelectedProperty); }
            set { SetValue(IsSelectedProperty, value); }
        }
        public static readonly DependencyProperty IsSelectedProperty = DependencyProperty.Register("IsSelected", typeof(bool), typeof(Node));

        private NodeCanvas? _canvas;

        static Node()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Node), new FrameworkPropertyMetadata(typeof(Node)));
        }

        public Node()
        {
            Lines = new List<NodeLine>();

            LayoutUpdated += OnLayoutUpdated;
            Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            NodeCanvas? canvas = this.FindParent<NodeCanvas>();

            if (canvas != null)
            {
                _canvas = canvas;
            }

        }

        private void OnLayoutUpdated(object? sender, EventArgs e)
        {
            bool isMoving = false;

            double left = double.IsNaN(Canvas.GetLeft(this)) ? 0d : Canvas.GetLeft(this);
            double right = left + ActualWidth;
            double top = double.IsNaN(Canvas.GetTop(this)) ? 0d : Canvas.GetTop(this);
            double bottom = top + ActualHeight;

            double x = left + ActualWidth / 2d;
            double y = top + ActualHeight / 2d;

            Point leftPoint = new Point(left, y);
            Point rightPoint = new Point(right, y);

            if (Left != leftPoint)
            {
                Left = leftPoint;
                isMoving = true;
            }
            if (Right != rightPoint)
            {
                Right = rightPoint;
                isMoving = true;
            }

            if (isMoving)
            {
                Changed?.Invoke(this, EventArgs.Empty);
            }
        }

        protected override void OnPreviewMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            IsSelected = !IsSelected;
        }

        protected override void OnPreviewMouseRightButtonDown(MouseButtonEventArgs e)
        {
            if (_canvas != null)
            {
                DragStartPoint = new Point?(e.GetPosition(_canvas));
                e.Handled = true;
            }
        }

        protected override void OnPreviewMouseMove(MouseEventArgs e)
        {
            if (_canvas != null)
            {
                if (e.RightButton != MouseButtonState.Pressed)
                {
                    DragStartPoint = null;
                }

                if (e.LeftButton == MouseButtonState.Pressed)
                {
                    IsSelected = true;
                }

                if (DragStartPoint.HasValue)
                {
                    AdornerLayer? adornerLayer = AdornerLayer.GetAdornerLayer(_canvas);

                    if (adornerLayer != null)
                    {
                        NodeLineAdorner? lineAdorner = new NodeLineAdorner(_canvas, this);

                        if (lineAdorner != null)
                        {
                            adornerLayer.Add(lineAdorner);
                            e.Handled |= true;
                        }
                    }
                }
            }
        }
    }
}
