using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using BehaviorsTestApp.ViewModels;

namespace BehaviorsTestApp
{

    public sealed partial class DistanceFromContainerPage : Page
    {
        public DistanceFromContainerPage()
        {
            this.InitializeComponent();
            DataContext = new DistanceValueViewModel();
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            Frame rootFrame = Window.Current.Content as Frame;

            rootFrame.GoBack();
        }
    }
}
