using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

namespace BehaviorsTestApp.Behaviors
{
    public class AncestorProviderBehavior : Behavior<FrameworkElement>
    {
        public AncestorProviderBehavior()
        {
            AncestorLevel = 1;
        }

        public string AncestorType  { get; set; }
        public string AncestorName  { get; set; }
        public int    AncestorLevel { get; set; }

        public FrameworkElement AncestorElement
        {
            get { return (FrameworkElement)GetValue(AncestorElementProperty); }
            set { SetValue(AncestorElementProperty, value); }
        }

        public static readonly DependencyProperty AncestorElementProperty = DependencyProperty.Register("AncestorElement", typeof(FrameworkElement), typeof(AncestorProviderBehavior), new PropertyMetadata(null));

        protected override void OnAttached()
        {
            AssociatedObject.Loaded   += AssociatedObject_Loaded;
            AssociatedObject.Unloaded += AssociatedObject_Unloaded;

            SetAncestor();
        }

        private void AssociatedObject_Unloaded(object sender, RoutedEventArgs e)
        {
            AncestorElement = null;
        }
        private void AssociatedObject_Loaded(object sender, RoutedEventArgs e)
        {
            SetAncestor();
        }

        protected override void OnDetaching()
        {
            AssociatedObject.Loaded   -= AssociatedObject_Loaded;
            AssociatedObject.Unloaded -= AssociatedObject_Unloaded;
        }

        private void SetAncestor()
        {
            Type ancestorType = Type.GetType(AncestorType);

            AncestorElement = FindAncestor(ancestorType, AssociatedObject, AncestorLevel, AncestorName);
        }

        private static FrameworkElement FindAncestor(Type t, FrameworkElement child, int ancestorLevel, string name = null)
        {
            var parent = VisualTreeHelper.GetParent(child) as FrameworkElement;
            int levelCount = 0;

            while (parent != null)
            {
                if (parent.GetType() == t && (string.IsNullOrEmpty(name) || parent.Name == name))
                {
                    levelCount++;

                    if (levelCount == ancestorLevel)
                    {
                        break;
                    }
                }
                else if (t == null && (string.IsNullOrEmpty(name) || parent.Name == name))
                {
                    levelCount++;

                    if (levelCount == ancestorLevel)
                    {
                        break;
                    }
                }

                parent = VisualTreeHelper.GetParent(parent) as FrameworkElement;
            }

            return parent;
        }
    }
}
