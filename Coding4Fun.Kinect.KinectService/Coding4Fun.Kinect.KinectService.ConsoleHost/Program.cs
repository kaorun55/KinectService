// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Net;
using Coding4Fun.Kinect.KinectService.Common;
using Coding4Fun.Kinect.KinectService.ConsoleHost.Properties;
using Coding4Fun.Kinect.KinectService.Listeners;
using Microsoft.Kinect;
using ColorImageFormat = Microsoft.Kinect.ColorImageFormat;
using DepthImageFormat = Microsoft.Kinect.DepthImageFormat;

namespace Coding4Fun.Kinect.KinectService.ConsoleHost
{
	class Program
	{
		private static readonly int VideoPort = Settings.Default.ColorPort;
		private static readonly int AudioPort = Settings.Default.AudioPort;
		private static readonly int DepthPort = Settings.Default.DepthPort;
		private static readonly int SkeletonPort = Settings.Default.SkeletonPort;

		private static DepthListener _depthListener;
		private static ColorListener _videoListener;
		private static SkeletonListener _skeletonListener;
		private static AudioListener _audioListener;

		private static KinectSensor _kinect;

		static void Main()
		{
			while(KinectSensor.KinectSensors.Count == 0)
			{
				Console.WriteLine("Please insert a Kinect sensor and press any key to continue.");
				Console.ReadKey();
			}

			_kinect = KinectSensor.KinectSensors[0];

			_kinect.ColorStream.Enable(ColorImageFormat.RgbResolution640x480Fps30);
			_kinect.DepthStream.Enable(DepthImageFormat.Resolution320x240Fps30);
			_kinect.SkeletonStream.Enable();

			_kinect.Start();

			_videoListener = new ColorListener(_kinect, VideoPort, ImageFormat.Jpeg);
			_videoListener.Start();

			_depthListener = new DepthListener(_kinect, DepthPort);
			_depthListener.Start();

			_skeletonListener = new SkeletonListener(_kinect, SkeletonPort);
			_skeletonListener.Start();

			_audioListener = new AudioListener(_kinect, AudioPort);
			_audioListener.Start();

			IPAddress[] addresses = Dns.GetHostAddresses(Dns.GetHostName());
			string s = string.Empty;
			foreach(IPAddress address in addresses)
				s += Environment.NewLine + address;

			Console.WriteLine("Waiting for client on..." + s);

			Console.WriteLine("Press any key to exit...");
			Console.ReadKey();

			if(_depthListener != null)
				_depthListener.Stop();

			if(_videoListener != null)
				_videoListener.Stop();

			if(_skeletonListener != null)
				_skeletonListener.Stop();

			if(_audioListener != null)
				_audioListener.Stop();
		}
	}
}
