using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace BluePrint.UI
{
    internal class NodeLineAdorner : Adorner
    {
        internal Node HostNode { get; set; }
        internal Node? AttachNode { get; set; }

        private readonly NodeCanvas _canvas;
        private readonly PathGeometry _geometry;
        private readonly PathFigure _figure;
        private readonly BezierSegment _segment;
        private readonly Pen _pen;

        public NodeLineAdorner(NodeCanvas canvas, Node host) : base(canvas)
        {
            _canvas = canvas;

            _figure = new PathFigure();
            _segment = new BezierSegment();
            _figure.Segments.Add(_segment);
            _geometry = new PathGeometry([_figure]);

            _pen = new Pen(Brushes.Purple, 2d)
            {
                LineJoin = PenLineJoin.Round,
            };

            HostNode = host;
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);

            drawingContext.DrawGeometry(null, _pen, _geometry);
            drawingContext.DrawRectangle(Brushes.Transparent, null, new Rect(RenderSize));
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (e.RightButton == MouseButtonState.Pressed)
            {
                OnChanged(e.GetPosition(this));
                InvalidateVisual();
            }
            else
            {
                if (IsMouseCaptured)
                {
                    ReleaseMouseCapture();
                }
            }
        }

        protected override void OnPreviewMouseRightButtonUp(MouseButtonEventArgs e)
        {
            if (IsMouseCaptured)
            {
                ReleaseMouseCapture();
            }

            if (_canvas.InputHitTest(e.GetPosition(this)) is UIElement element)
            {
                Node? node = element.FindParent<Node>();

                if (node != null)
                {
                    AttachNode = node;
                    OnChanged(e.GetPosition(this));
                    InvalidateVisual();
                }
            }

            if (AttachNode != null)
            {
                // 接入后端

                NodeLine? line = new NodeLine()
                {
                    Source = HostNode,
                    SourceId = HostNode.Name,
                    Target = AttachNode,
                    TargetId = AttachNode.Name,
                };
                _canvas.Children.Add(line);
            }

            AdornerLayer? adornerLayer = AdornerLayer.GetAdornerLayer(_canvas);

            if (adornerLayer != null)
            {
                adornerLayer.Remove(this);
            }
        }

        private void OnChanged(Point mouse)
        {
            Point start = HostNode.Left;
            Point end = mouse;

            Point p1 = new Point(start.X - 30d, start.Y);
            Point p2 = new Point(end.X - 30d, end.Y);

            if (HostNode.Right.X < mouse.X)
            {
                start = HostNode.Right;
                p1.X = start.X + 30d;
            }
            else if (HostNode.Left.X > mouse.X)
            {
                end = mouse;
                p2.X = end.X + 30d;
            }

            _figure.StartPoint = start;
            _segment.Point1 = p1;
            _segment.Point2 = p2;
            _segment.Point3 = end;
        }
    }
}
