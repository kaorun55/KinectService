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
using System.Windows.Media.Imaging;
#endif

namespace Coding4Fun.Kinect.KinectService.WpfClient
{
	public class DepthClient : KinectServiceClient
	{
		public event EventHandler<DepthFrameReadyEventArgs> DepthFrameReady;
		public DepthFrameData DepthFrame { get; private set; }

		public DepthClient()
		{
			this.ThreadProcessor = DepthThread;
		}

		private void DepthThread()
		{
			try
			{
				NetworkStream ns = Client.GetStream();
				BinaryReader networkReader = new BinaryReader(ns);
				short[] depthShort = null;

				while(Client.Connected)
				{
					DepthFrameReadyEventArgs args = new DepthFrameReadyEventArgs();
					DepthFrameData dfd = new DepthFrameData();

					int size = networkReader.ReadInt32();
					byte[] data = networkReader.ReadBytes(size);

					MemoryStream ms = new MemoryStream(data);
					BinaryReader br = new BinaryReader(ms);

					dfd.PlayerIndexBitmask = br.ReadInt32();
					dfd.PlayerIndexBitmaskWidth = br.ReadInt32();

					DepthImageFrame frame = br.ReadDepthImageFrame();
					dfd.ImageFrame = frame;

					int dataLength = (int)(ms.Length - ms.Position);

					if(depthShort == null || depthShort.Length != dataLength / 2)
						depthShort = new short[dataLength / 2];

					Buffer.BlockCopy(data, (int)br.BaseStream.Position, depthShort, 0, dataLength);

					dfd.DepthData = depthShort;

					DepthFrame = dfd;
					args.DepthFrame = dfd;

					Context.Send(delegate
					{
						if(DepthFrameReady != null)
							DepthFrameReady(this, args);
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
