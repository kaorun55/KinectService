﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Coding4Fun.Kinect.KinectService.MetroClient;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Coding4Fun.Kinect.KinectService.Samples.MetroSample
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class BlankPage : Page
    {
        ColorClient color = new ColorClient();
        int counter = 0;
        public BlankPage()
        {
            this.InitializeComponent();

            color.ColorFrameReady += color_ColorFrameReady;
            color.Connect("10.0.1.11", 4530);
        }

        void color_ColorFrameReady( object sender, Coding4Fun.Kinect.KinectService.Common.ColorFrameReadyEventArgs e )
        {
            counter++;
            textBlock.Text = counter.ToString();
            imageRgb.Source = e.ColorFrame.BitmapImage;
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }
    }
}
