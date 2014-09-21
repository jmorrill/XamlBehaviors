using System;
using System.Diagnostics;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Media;

namespace BehaviorsTestApp.Behaviors
{
    public class DistanceValueBehavior : Behavior<FrameworkElement>
    {
        private enum Side
        {
            Top,
            Left
        }

        private FrameworkElement _topAncenstor;
        private FrameworkElement _leftAncenstor;
        private Type             _topAncenstorType;
        private Type             _leftAncenstorType;
        private TimeSpan         _lastLayoutUpdate;
        private TimeSpan         _renderingHookTimeout;
        private Stopwatch        _stopwatch;
        private double           _distanceFromLeftCached;
        private double           _distanceFromTopCached;

        public double DistanceFromTop
        {
            get { return (double)GetValue(DistanceFromTopProperty); }
            set { SetValue(DistanceFromTopProperty, value); }
        }

        public static readonly DependencyProperty DistanceFromTopProperty = DependencyProperty.Register("DistanceFromTop", typeof(double), typeof(DistanceValueBehavior), new PropertyMetadata(0.0));

        public double DistanceFromLeft
        {
            get { return (double)GetValue(DistanceFromLeftProperty); }
            set { SetValue(DistanceFromLeftProperty, value); }
        }

        public static readonly DependencyProperty DistanceFromLeftProperty = DependencyProperty.Register("DistanceFromLeft", typeof(double), typeof(DistanceValueBehavior), new PropertyMetadata(0.0));

        public string LeftAncestorType { get; set; }
        public string TopAncestorType  { get; set; }
        public string TopAncestorName  { get; set; }
        public string LeftAncestorName { get; set; }

        public DistanceValueBehavior()
        {
            _renderingHookTimeout = TimeSpan.FromMilliseconds(2000);
            _stopwatch = new Stopwatch();
            _stopwatch.Start();
        }

        protected override void OnAttached()
        {
            if (!string.IsNullOrEmpty(LeftAncestorType))
            {
                _leftAncenstorType = Type.GetType(LeftAncestorType);
            }

            if (!string.IsNullOrEmpty(TopAncestorType))
            {
                _topAncenstorType = Type.GetType(TopAncestorType);
            }

            AssociatedObject.Loaded        += AssociatedObject_Loaded;
            AssociatedObject.Unloaded      += AssociatedObject_Unloaded;
            AssociatedObject.LayoutUpdated += AssociatedObject_LayoutUpdated;
            EnsureRenderingHook();
        }

        protected override void OnDetaching()
        {
            AssociatedObject.Loaded        -= AssociatedObject_Loaded;
            AssociatedObject.Unloaded      -= AssociatedObject_Unloaded;
            AssociatedObject.LayoutUpdated -= AssociatedObject_LayoutUpdated;

            UnhookRendering();
        }

        private void AssociatedObject_LayoutUpdated(object sender, object e)
        {
            _lastLayoutUpdate = _stopwatch.Elapsed;
        }

        private void SetDistance(Side side)
        {
            FrameworkElement ancestor;

            switch (side)
            {
                case Side.Top:
                    ancestor = _topAncenstor;
                    break;
                case Side.Left:
                    ancestor = _leftAncenstor;
                    break;
                default:
                    throw new ArgumentOutOfRangeException("side");
            }

            Rect ancestorObjectBounds   = LayoutInformation.GetLayoutSlot(ancestor);
            Rect associatedObjectBounds = LayoutInformation.GetLayoutSlot(AssociatedObject);
            GeneralTransform transform  = AssociatedObject.TransformToVisual(ancestor);
            Point associatedObjectPoint = transform.TransformPoint(new Point(associatedObjectBounds.X, associatedObjectBounds.Y));

            double distance;

            switch (side)
            {
                case Side.Top:
                    distance = associatedObjectPoint.Y / ancestorObjectBounds.Height;
                    distance = Math.Round(distance, 5);

                    if (!double.IsInfinity(distance) && !double.IsNaN(distance))
                    {
                        if (Math.Abs(_distanceFromTopCached - distance) > 0.0001)
                        {
                            DistanceFromTop = _distanceFromTopCached = distance;
                        }
                    }
                    break;
                case Side.Left:
                    distance = associatedObjectPoint.X / ancestorObjectBounds.Width;
                    distance = Math.Round(distance, 5);

                    if (!double.IsInfinity(distance) && !double.IsNaN(distance))
                    {
                        if (Math.Abs(_distanceFromLeftCached - distance) > 0.0001)
                        {
                            DistanceFromLeft = _distanceFromLeftCached = distance;
                        }
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException("side");
            }
        }

        private void CompositionTarget_OnRendering(object sender, object o)
        {
            if (_topAncenstor == null && _leftAncenstor == null)
            {
                return;
            }

            if ((_stopwatch.Elapsed > (_lastLayoutUpdate + _renderingHookTimeout)))
            {
                return;
            }

            if (_topAncenstor != null)
            {
                SetDistance(Side.Top);
            }

            if (_leftAncenstor != null)
            {
                SetDistance(Side.Left);
            }
        }

        private void AssociatedObject_Unloaded(object sender, RoutedEventArgs e)
        {
            UnhookRendering();
            _topAncenstor  = null;
            _leftAncenstor = null;
        }

        private void AssociatedObject_Loaded(object sender, RoutedEventArgs e)
        {
            EnsureRenderingHook();

            if (_topAncenstorType != null || !string.IsNullOrEmpty(TopAncestorName))
            {
                _topAncenstor = FindAncestor(_topAncenstorType, AssociatedObject, TopAncestorName);
            }

            if (_leftAncenstorType != null || !string.IsNullOrEmpty(LeftAncestorName))
            {
                _leftAncenstor = FindAncestor(_leftAncenstorType, AssociatedObject, LeftAncestorName);
            }
        }
        private void EnsureRenderingHook()
        {
            UnhookRendering();
            CompositionTarget.Rendering += CompositionTarget_OnRendering;
        }

        private void UnhookRendering()
        {
            CompositionTarget.Rendering -= CompositionTarget_OnRendering;
        }

        private static FrameworkElement FindAncestor(Type t, FrameworkElement child, string name = null)
        {
            var parent = VisualTreeHelper.GetParent(child) as FrameworkElement;

            while (parent != null)
            {
                if (parent.GetType() == t && (string.IsNullOrEmpty(name) || parent.Name == name))
                {
                    break;
                }
                
                if (t == null && (string.IsNullOrEmpty(name) || parent.Name == name))
                {
                    break;
                }

                parent = VisualTreeHelper.GetParent(parent) as FrameworkElement;
            }

            return parent;
        }
    }
}
