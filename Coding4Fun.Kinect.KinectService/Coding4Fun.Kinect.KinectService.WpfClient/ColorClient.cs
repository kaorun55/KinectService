// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.IO;
using Coding4Fun.Kinect.KinectService.Common;

#if NETFX_CORE
using Windows.UI.Xaml.Media.Imaging;
#else
using System.Net.Sockets;
using System.Windows.Media.Imaging;
#endif

namespace Coding4Fun.Kinect.KinectService.WpfClient
{
	public class ColorClient : KinectServiceClient
	{
		public event EventHandler<ColorFrameReadyEventArgs> ColorFrameReady;
		public ColorFrameData ColorFrame { get; private set; }

		public ColorClient()
		{
			this.ThreadProcessor = ColorThread;
		}

		private void ColorThread()
		{
			try
			{
				NetworkStream ns = Client.GetStream();
				BinaryReader reader = new BinaryReader(ns);

				while(Client.Connected)
				{
					ColorFrameReadyEventArgs args = new ColorFrameReadyEventArgs();
					ColorFrameData cfd = new ColorFrameData();

					int size = reader.ReadInt32();
					byte[] data = reader.ReadBytes(size);

					MemoryStream ms = new MemoryStream(data);
					BinaryReader br = new BinaryReader(ms);

					cfd.Format = (ImageFormat)br.ReadInt32();
					cfd.ImageFrame = br.ReadColorImageFrame();

					MemoryStream msData = new MemoryStream(data, (int)ms.Position, (int)(ms.Length - ms.Position));

					Context.Send(delegate
					{
						if(cfd.Format == ImageFormat.Raw)
							cfd.RawImage = ms.ToArray();
						else
						{
							BitmapImage bi = new BitmapImage();
							bi.BeginInit();
							bi.StreamSource = msData;
							bi.EndInit();

							cfd.BitmapImage = bi;
						}

						ColorFrame = cfd;
						args.ColorFrame = cfd;

						if(ColorFrameReady != null)
							ColorFrameReady(this, args);

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
