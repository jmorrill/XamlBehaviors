using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace BehaviorsTestApp.ViewModels
{
    public class DistanceValueViewModel
    {
        public DistanceValueViewModel()
        {
            Items = new ObservableCollection<string>();

            for (int i = 0; i < 300; i++)
            {
                Items.Add(string.Format("This is item {0}", i));
            }
        }

        public ObservableCollection<string> Items { get; private set; } 
    }
}
