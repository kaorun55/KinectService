// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Diagnostics;
using System.Net;
using Coding4Fun.Kinect.KinectService.Common;
using Coding4Fun.Kinect.KinectService.Listeners;
using Coding4Fun.Kinect.KinectService.WindowsService.Properties;
using Microsoft.Kinect;
using ColorImageFormat = Microsoft.Kinect.ColorImageFormat;
using DepthImageFormat = Microsoft.Kinect.DepthImageFormat;

namespace Coding4Fun.Kinect.KinectService.WindowsService
{
	class ServiceEngine
	{
		private static readonly int ColorPort = Settings.Default.ColorPort;
		private static readonly int AudioPort = Settings.Default.AudioPort;
		private static readonly int DepthPort = Settings.Default.DepthPort;
		private static readonly int SkeletonPort = Settings.Default.SkeletonPort;

		private DepthListener _depthListener;
		private ColorListener _colorListener;
		private SkeletonListener _skeletonListener;

		private KinectSensor _kinect;
		private AudioListener _audioListener;

		public void Start()
		{
			if(KinectSensor.KinectSensors.Count == 0)
			{
				Debug.WriteLine("No Kinects found.");
				Environment.Exit(-1);
			}

			_kinect = KinectSensor.KinectSensors[0];

			_kinect.ColorStream.Enable(ColorImageFormat.RgbResolution640x480Fps30);
			_kinect.DepthStream.Enable(DepthImageFormat.Resolution320x240Fps30);
			_kinect.SkeletonStream.Enable();

			_kinect.Start();

			_colorListener = new ColorListener(_kinect, ColorPort, ImageFormat.Jpeg);
			_colorListener.Start();

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

			Debug.WriteLine("Waiting for client on..." + s);
		}

		public void Stop()
		{
			_depthListener.Stop();
			_colorListener.Stop();
			_skeletonListener.Stop();
			_audioListener.Stop();
		}
	}
}
