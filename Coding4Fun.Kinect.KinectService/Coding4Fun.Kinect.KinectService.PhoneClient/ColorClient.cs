// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.IO;
using System.Windows.Media.Imaging;
using Coding4Fun.Kinect.KinectService.Common;

namespace Coding4Fun.Kinect.KinectService.PhoneClient
{
	public class ColorClient : KinectServiceClient
	{
		public event EventHandler<ColorFrameReadyEventArgs> ColorFrameReady;

		public ColorFrameData ColorFrame { get; private set; }

		public ColorClient()
		{
			this.FrameReady += ColorClient_FrameReady;
		}

		void ColorClient_FrameReady(object sender, FrameReadyEventArgs e)
		{
			MemoryStream ms = new MemoryStream(e.Data);
			BinaryReader br = new BinaryReader(ms);

			ColorFrameReadyEventArgs args = new ColorFrameReadyEventArgs();
			ColorFrameData cfd = new ColorFrameData
			{
				Format = (ImageFormat)br.ReadInt32(),
				ImageFrame = br.ReadColorImageFrame()
			};

			if(cfd.Format == ImageFormat.Raw)
			    cfd.RawImage = br.ReadBytes(e.Data.Length - sizeof(bool));
			else
			{
				BitmapImage bi = new BitmapImage();
				bi.SetSource(new MemoryStream(e.Data, (int)ms.Position, (int)(ms.Length - ms.Position)));
				cfd.BitmapImage = bi;
			}

			ColorFrame = cfd;
			args.ColorFrame = cfd;

			if(ColorFrameReady != null)
				ColorFrameReady(this, args);
		}
	}
}
