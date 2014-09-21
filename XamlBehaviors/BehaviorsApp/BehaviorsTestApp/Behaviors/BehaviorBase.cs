using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Microsoft.Xaml.Interactivity;

namespace BehaviorsTestApp.Behaviors
{
    /// <summary>
    /// Stole from some dudes blog, where-tf is the link?
    /// </summary>
    public abstract class Behavior<T> : DependencyObject, IBehavior where T : DependencyObject
    {
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public T AssociatedObject { get; set; }

        protected virtual void OnAttached()
        {
        }

        protected virtual void OnDetaching()
        {
        }

        public void Attach(Windows.UI.Xaml.DependencyObject associatedObject)
        {
            this.AssociatedObject = (T)associatedObject;
            OnAttached();
        }

        public void Detach()
        {
            OnDetaching();
        }

        DependencyObject IBehavior.AssociatedObject
        {
            get { return this.AssociatedObject; }
        }
    }
}
