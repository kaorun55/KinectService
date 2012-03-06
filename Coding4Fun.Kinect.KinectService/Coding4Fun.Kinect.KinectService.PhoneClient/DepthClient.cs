// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.IO;
using Coding4Fun.Kinect.KinectService.Common;

namespace Coding4Fun.Kinect.KinectService.PhoneClient
{
	public class DepthClient : KinectServiceClient
	{
		public event EventHandler<DepthFrameReadyEventArgs> DepthFrameReady;
		public DepthFrameData DepthFrame { get; private set; }

		private short[] _depthShort;

		public DepthClient()
		{
			this.FrameReady += DepthClient_FrameReady;
		}

		void DepthClient_FrameReady(object sender, FrameReadyEventArgs e)
		{
			DepthFrameReadyEventArgs args = new DepthFrameReadyEventArgs();
			DepthFrameData dfd = new DepthFrameData();

			MemoryStream ms = new MemoryStream(e.Data);
			BinaryReader br = new BinaryReader(ms);

			dfd.PlayerIndexBitmask = br.ReadInt32();
			dfd.PlayerIndexBitmaskWidth = br.ReadInt32();

			DepthImageFrame frame = br.ReadDepthImageFrame();
			dfd.ImageFrame = frame;

			int dataLength = (int)(ms.Length - ms.Position);

			if(_depthShort == null || _depthShort.Length != dataLength / 2)
				_depthShort = new short[dataLength / 2];

			Buffer.BlockCopy(e.Data, (int)br.BaseStream.Position, _depthShort, 0, dataLength);

			dfd.DepthData = _depthShort;

			DepthFrame = dfd;
			args.DepthFrame = dfd;

			if(DepthFrameReady != null)
				DepthFrameReady(this, args);
		}
	}
}
