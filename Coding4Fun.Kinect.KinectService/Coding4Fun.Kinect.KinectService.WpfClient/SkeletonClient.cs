// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.IO;
using Coding4Fun.Kinect.KinectService.Common;

#if NETFX_CORE
#else
using System.Net.Sockets;
#endif

namespace Coding4Fun.Kinect.KinectService.WpfClient
{
	public class SkeletonClient : KinectServiceClient
	{
		public event EventHandler<SkeletonFrameReadyEventArgs> SkeletonFrameReady;
		public SkeletonFrameData SkeletonFrame { get; private set; }

		public SkeletonClient()
		{
			this.ThreadProcessor = StreamSkeleton;
		}

		private void StreamSkeleton()
		{
			try
			{
				NetworkStream ns = Client.GetStream();
				BinaryReader reader = new BinaryReader(ns);

				while(Client.Connected)
				{
					int size = reader.ReadInt32();
					byte[] data = reader.ReadBytes(size);

					MemoryStream ms = new MemoryStream(data);
					BinaryReader br = new BinaryReader(ms);

					SkeletonFrameData frame = br.ReadSkeletonFrame();

					SkeletonFrameReadyEventArgs args = new SkeletonFrameReadyEventArgs { SkeletonFrame = frame };
					SkeletonFrame = frame;

					Context.Send(delegate
					{
						if(SkeletonFrameReady != null)
							SkeletonFrameReady(this, args);
					}, null);
				}
			}
			catch(IOException)
			{
				Client.Close();
			}
		}
	}
}
