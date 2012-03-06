// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Kinect;

namespace Coding4Fun.Kinect.KinectService.Listeners
{
	public class DepthListener : KinectListener
	{
		private readonly int _fps = -1;
		private int _frameCount;
		private readonly MemoryStream _memoryStream = new MemoryStream();
		private readonly BinaryWriter binaryWriter;
		private short[] _depth;
		private byte[] _depthBytes;

		public DepthImageFrame DepthImageFrame { get; private set; }

		public DepthListener(KinectSensor kinect, int port, int fps)
		{
			VerifyConstructorArguments(kinect, port);

			if(fps < -1 || fps > 30)
				throw new ArgumentException("FPS value must be between 1 and 30, inclusive.");

			_fps = fps;

			this.Kinect = kinect;
			this.Port = port;
			this.Kinect.DepthFrameReady += Kinect_DepthFrameReady;

			binaryWriter = new BinaryWriter(_memoryStream);
		}

		public DepthListener(KinectSensor kinect, int port) : this(kinect, port, -1)
		{
		}

		void Kinect_DepthFrameReady(object sender, DepthImageFrameReadyEventArgs e)
		{
			if(ClientList.Count > 0)
			{
			    using(DepthImageFrame frame = e.OpenDepthImageFrame())
				{
					this.DepthImageFrame = frame;

					if(frame != null)
					{
						_memoryStream.Seek(0, SeekOrigin.Begin);

						binaryWriter.Write(DepthImageFrame.PlayerIndexBitmask);
						binaryWriter.Write(DepthImageFrame.PlayerIndexBitmaskWidth);

						binaryWriter.Write(frame);
						binaryWriter.Write((int)frame.Format);

						if(_depth == null || _depth.Length != frame.PixelDataLength)
							_depth = new short[frame.PixelDataLength];
						frame.CopyPixelDataTo(_depth);

						if(_depthBytes == null || _depthBytes.Length != _depth.Length * 2)
							_depthBytes = new byte[_depth.Length * 2];

						Buffer.BlockCopy(_depth, 0, _depthBytes, 0, _depthBytes.Length);

						binaryWriter.Write(_depthBytes);

						_frameCount++;

						if(_fps == -1 || (_frameCount > 0 && (_frameCount % (GetFps(frame.Format) / _fps)) == 0))
						{
							Parallel.For(0, ClientList.Count, index =>
							{
								SocketClient sc = ClientList[index];
								byte[] data = _memoryStream.ToArray();

								sc.Send(BitConverter.GetBytes(data.Length));
								sc.Send(data);
							});
						}
						RemoveClients();
					}
				}
			}
		}

		private int GetFps(DepthImageFormat format)
		{
			switch(format)
			{
				case DepthImageFormat.Undefined:
					return 0;
				case DepthImageFormat.Resolution640x480Fps30:
				case DepthImageFormat.Resolution320x240Fps30:
				case DepthImageFormat.Resolution80x60Fps30:
					return 30;
				default:
					throw new ArgumentOutOfRangeException("format");
			}
		}
	}
}
