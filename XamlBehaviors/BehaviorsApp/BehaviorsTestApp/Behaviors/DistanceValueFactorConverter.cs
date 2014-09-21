using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace BehaviorsTestApp.Behaviors
{
    public class DistanceValueFactorConverter : IValueConverter
    {
        public double Factor { get; set; }

        public double Center { get; set; }

        public DistanceValueFactorConverter()
        {
            Center = 0.5;
            Factor = 1;
        }

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is double == false)
                return 0.0d;

            var val = (double)value;

            //Adjust for the center point
            val = val - Center;

            val *= Factor;
            return val;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return 0.0;
        }
    }
}
