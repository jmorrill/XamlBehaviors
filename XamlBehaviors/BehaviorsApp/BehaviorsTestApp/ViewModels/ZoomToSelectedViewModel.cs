using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BehaviorsTestApp.ViewModels
{
    public class ZoomToSelectedViewModel
    {
        public ZoomToSelectedViewModel()
        {
            Images = new ObservableCollection<string>();

            for (int i = 0; i < 30; i++)
            {
                Images.Add("Images/waterfall.jpg");
            }
        }

        public ObservableCollection<string> Images { get; private set; } 
    }
}
