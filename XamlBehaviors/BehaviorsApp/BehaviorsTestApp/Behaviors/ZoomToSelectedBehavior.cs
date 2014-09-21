using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;

namespace BehaviorsTestApp.Behaviors
{
    public class ZoomToSelectedBehavior : Behavior<Selector>
    {
        CompositeTransform _transform;

        public Stretch Stretch
        {
            get { return (Stretch)GetValue(StretchProperty); }
            set { SetValue(StretchProperty, value); }
        }

        public static readonly DependencyProperty StretchProperty = DependencyProperty.Register("Stretch", typeof(Stretch), typeof(ZoomToSelectedBehavior), new PropertyMetadata(Stretch.Uniform));

        public StretchDirection StretchDirection
        {
            get { return (StretchDirection)GetValue(StretchDirectionProperty); }
            set { SetValue(StretchDirectionProperty, value); }
        }

        public static readonly DependencyProperty StretchDirectionProperty = DependencyProperty.Register("StretchDirection", typeof(StretchDirection), typeof(ZoomToSelectedBehavior), new PropertyMetadata(StretchDirection.Both));

        public TimeSpan ZoomInDuration
        {
            get { return (TimeSpan)GetValue(ZoomInDurationProperty); }
            set { SetValue(ZoomInDurationProperty, value); }
        }

        public static readonly DependencyProperty ZoomInDurationProperty = DependencyProperty.Register("ZoomInDuration", typeof(TimeSpan), typeof(ZoomToSelectedBehavior), new PropertyMetadata(TimeSpan.FromMilliseconds(1200)));

        public TimeSpan ZoomOutDuration
        {
            get { return (TimeSpan)GetValue(ZoomOutDurationProperty); }
            set { SetValue(ZoomOutDurationProperty, value); }
        }

        public static readonly DependencyProperty ZoomOutDurationProperty = DependencyProperty.Register("ZoomOutDuration", typeof(TimeSpan), typeof(ZoomToSelectedBehavior), new PropertyMetadata(TimeSpan.FromMilliseconds(1200)));

        public EasingFunctionBase ZoomEasingMode
        {
            get { return (EasingFunctionBase)GetValue(ZoomEasingModeProperty); }
            set { SetValue(ZoomEasingModeProperty, value); }
        }

        public static readonly DependencyProperty ZoomEasingModeProperty = DependencyProperty.Register("ZoomEasingMode", typeof(EasingFunctionBase), typeof(ZoomToSelectedBehavior), new PropertyMetadata(new ElasticEase { EasingMode = EasingMode.EaseOut, Oscillations = 1, Springiness = 17 }));

        public bool DisableScrollOnZoom
        {
            get { return (bool)GetValue(DisableScrollOnZoomProperty); }
            set { SetValue(DisableScrollOnZoomProperty, value); }
        }

        public static readonly DependencyProperty DisableScrollOnZoomProperty = DependencyProperty.Register("DisableScrollOnZoom", typeof(bool), typeof(ZoomToSelectedBehavior), new PropertyMetadata(true));


        protected override void OnAttached()
        {
            AssociatedObject.SelectionChanged += AssociatedObjectOnSelectionChanged;

            _transform = new CompositeTransform();

            Debug.Assert(AssociatedObject.RenderTransform == null);

            AssociatedObject.RenderTransform = _transform;

            base.OnAttached();
        }

        protected override void OnDetaching()
        {
            AssociatedObject.SelectionChanged -= AssociatedObjectOnSelectionChanged;
            AssociatedObject.RenderTransform = null;
            base.OnDetaching();
        }

        void ZoomOut()
        {
            if (DisableScrollOnZoom)
            {
                ScrollViewer.SetHorizontalScrollMode(AssociatedObject, ScrollMode.Enabled);
            }

            AnimateTo(1, 1, 0, 0, ZoomOutDuration);
        }

        async Task ZoomIn(FrameworkElement container)
        {
            var scrollviewer = FindAncestor<ScrollViewer>(container);

            if (scrollviewer != null)
            {
                scrollviewer.BringIntoViewOnFocusChange = true;
                await Task.Delay(20);
            }

            var containerTransformToSelector = container.TransformToVisual(AssociatedObject);
            var containerLocalRect = new Rect(0, 0, container.ActualWidth, container.ActualHeight);
            var containerRect = containerTransformToSelector.TransformBounds(containerLocalRect);
            var selectorRect = new Rect(0, 0, AssociatedObject.ActualWidth, AssociatedObject.ActualHeight);

            var sizeRatio = CalculateScale(new Size(selectorRect.Width, selectorRect.Height),
                                                              new Size(containerRect.Width, containerRect.Height));

            double scaleX = sizeRatio.Width;
            double scaleY = sizeRatio.Height;

            double xoffset = (selectorRect.Width / 2) - ((containerRect.Width / 2) * scaleX);
            double translateX = ((selectorRect.Left - containerRect.Left) * scaleX) + xoffset;

            double yoffset = (selectorRect.Height / 2) - ((containerRect.Height / 2) * scaleY);
            double translateY = ((selectorRect.Top - containerRect.Top) * scaleY) + yoffset;

            if (DisableScrollOnZoom)
            {
                ScrollViewer.SetHorizontalScrollMode(AssociatedObject, ScrollMode.Disabled);
            }
            AnimateTo(scaleX, scaleY, translateX, translateY, ZoomInDuration);
        }

        async private void AssociatedObjectOnSelectionChanged(object sender, SelectionChangedEventArgs selectionChangedEventArgs)
        {
            if (AssociatedObject.SelectedItem == null)
            {
                ZoomOut();
            }
            else
            {
                var container = AssociatedObject.ContainerFromItem(AssociatedObject.SelectedValue) as FrameworkElement;

                if (container != null)
                {
                    await ZoomIn(container);
                }
            }
        }

        private void AnimateTo(double scaleX, double scaleY, double translateX, double translateY, TimeSpan duration)
        {
            var scaleXAnimation = new DoubleAnimation();
            scaleXAnimation.Duration = new Duration(duration);
            scaleXAnimation.To = scaleX;
            Storyboard.SetTargetProperty(scaleXAnimation, "(CompositeTransform.ScaleX)");
            Storyboard.SetTarget(scaleXAnimation, _transform);

            var scaleYAnimation = new DoubleAnimation();
            scaleYAnimation.Duration = new Duration(duration);
            scaleYAnimation.To = scaleY;
            Storyboard.SetTargetProperty(scaleYAnimation, "(CompositeTransform.ScaleY)");
            Storyboard.SetTarget(scaleYAnimation, _transform);

            var translateXAnimation = new DoubleAnimation();
            translateXAnimation.Duration = new Duration(duration);
            translateXAnimation.To = translateX;
            Storyboard.SetTargetProperty(translateXAnimation, "(CompositeTransform.TranslateX)");
            Storyboard.SetTarget(translateXAnimation, _transform);

            var translateYAnimation = new DoubleAnimation();
            translateYAnimation.Duration = new Duration(duration);
            translateYAnimation.To = translateY;
            Storyboard.SetTargetProperty(translateYAnimation, "(CompositeTransform.TranslateY)");
            Storyboard.SetTarget(translateYAnimation, _transform);

            var storyboard = new Storyboard();
            storyboard.Children.Add(scaleXAnimation);
            storyboard.Children.Add(scaleYAnimation);
            storyboard.Children.Add(translateXAnimation);
            storyboard.Children.Add(translateYAnimation);

            scaleXAnimation.EasingFunction = ZoomEasingMode;
            scaleYAnimation.EasingFunction = ZoomEasingMode;
            translateXAnimation.EasingFunction = ZoomEasingMode;
            translateYAnimation.EasingFunction = ZoomEasingMode;

            storyboard.Begin();
        }

        Size CalculateScale(Size availableSize, Size elementSize)
        {
            double scaleX = 1.0;
            double scaleY = 1.0;

            bool isConstrainedWidth = !double.IsInfinity(availableSize.Width);
            bool isConstrainedHeight = !double.IsInfinity(availableSize.Height);

            if ((Stretch == Stretch.Uniform || Stretch == Stretch.UniformToFill || Stretch == Stretch.Fill)
                && (isConstrainedWidth || isConstrainedHeight))
            {
                scaleX = elementSize.Width == 0 ? 0.0f : availableSize.Width / elementSize.Width;
                scaleY = elementSize.Height == 0 ? 0.0f : availableSize.Height / elementSize.Height;

                if (!isConstrainedWidth) scaleX = scaleY;
                else if (!isConstrainedHeight) scaleY = scaleX;
                else
                {
                    switch (Stretch)
                    {
                        case Stretch.Uniform:
                            {
                                double minscale = scaleX < scaleY ? scaleX : scaleY;
                                scaleX = scaleY = minscale;
                            }
                            break;
                        case Stretch.UniformToFill:
                            {
                                double maxscale = scaleX > scaleY ? scaleX : scaleY;
                                scaleX = scaleY = maxscale;
                            }
                            break;
                    }
                }

                switch (StretchDirection)
                {
                    case StretchDirection.UpOnly:
                        if (scaleX < 1.0f) scaleX = 1.0f;
                        if (scaleY < 1.0f) scaleY = 1.0f;
                        break;

                    case StretchDirection.DownOnly:
                        if (scaleX > 1.0f) scaleX = 1.0f;
                        if (scaleY > 1.0f) scaleY = 1.0f;
                        break;
                    case StretchDirection.Both:
                        break;
                    default:
                        break;
                }
            }

            return new Size(scaleX, scaleY);
        }

        private static T FindAncestor<T>(FrameworkElement child) where T : DependencyObject
        {
            DependencyObject parent = VisualTreeHelper.GetParent(child);

            while (parent != null && ((parent is T) == false))
            {
                parent = VisualTreeHelper.GetParent(parent);
            }

            return (parent as T);
        }
    }
}
