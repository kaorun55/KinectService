// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.IO;
using Coding4Fun.Kinect.KinectService.Common;

namespace Coding4Fun.Kinect.KinectService.PhoneClient
{
	public class SkeletonClient : KinectServiceClient
	{
		public event EventHandler<SkeletonFrameReadyEventArgs> SkeletonFrameReady;
		public SkeletonFrameData SkeletonFrame { get; private set; }

		public SkeletonClient()
		{
			this.FrameReady += SkeletonClient_FrameReady;
		}

		void SkeletonClient_FrameReady(object sender, FrameReadyEventArgs e)
		{
			MemoryStream ms = new MemoryStream(e.Data);
			BinaryReader br = new BinaryReader(ms);

			SkeletonFrameData frame = br.ReadSkeletonFrame();
			
			SkeletonFrameReadyEventArgs args = new SkeletonFrameReadyEventArgs { SkeletonFrame = frame };
			SkeletonFrame = frame;

			if(SkeletonFrameReady != null)
				SkeletonFrameReady(this, args);
		}
	}
}
