using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace BluePrint.UI
{
    public class NodeLine : Shape
    {
        internal Node Source { get; set; } = default!;
        internal Node Target { get; set; } = default!;

        public string SourceId
        {
            get { return (string)GetValue(SourceIdProperty); }
            set { SetValue(SourceIdProperty, value); }
        }
        public static readonly DependencyProperty SourceIdProperty = DependencyProperty.Register("SourceId", typeof(string), typeof(NodeLine));

        public string TargetId
        {
            get { return (string)GetValue(TargetIdProperty); }
            set { SetValue(TargetIdProperty, value); }
        }
        public static readonly DependencyProperty TargetIdProperty = DependencyProperty.Register("TargetId", typeof(string), typeof(NodeLine));

        private NodeCanvas? _canvas;
        private readonly PathFigure _figure;
        private readonly BezierSegment _segment;

        protected override Geometry DefiningGeometry { get; }

        public NodeLine()
        {
            _figure = new PathFigure();
            _segment = new BezierSegment();
            _figure.Segments.Add(_segment);

            DefiningGeometry = new PathGeometry([_figure]);
            StrokeThickness = 2d;
            Stroke = Brushes.Purple;

            Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            NodeCanvas? canvas = this.FindParent<NodeCanvas>();

            if (canvas != null)
            {
                _canvas = canvas;

                if (Source == null || Target == null)
                {
                    object? sourceObj = _canvas.FindName(SourceId);
                    object? targetObj = _canvas.FindName(TargetId);

                    if (sourceObj is Node source && targetObj is Node target)
                    {
                        Source = source;
                        Target = target;
                    }
                }

                if (Source != null && Target != null)
                {
                    Source.Changed += OnChanged;
                    Target.Changed += OnChanged;

                    Source.Lines.Add(this);
                    Target.Lines.Add(this);
                }

                OnChanged(this, EventArgs.Empty);
            }
        }

        private void OnChanged(object? sender, EventArgs e)
        {
            Point start = Source.Left;
            Point end = Target.Left;

            Point p1 = new Point(start.X - 30d, start.Y);
            Point p2 = new Point(end.X - 30d, end.Y);

            if (Source.Right.X < Target.Left.X)
            {
                start = Source.Right;
                p1.X = start.X + 30d;
            }
            else if (Source.Left.X > Target.Right.X)
            {
                end = Target.Right;
                p2.X = end.X + 30d;
            }

            _figure.StartPoint = start;
            _segment.Point1 = p1;
            _segment.Point2 = p2;
            _segment.Point3 = end;
        }
    }
}
