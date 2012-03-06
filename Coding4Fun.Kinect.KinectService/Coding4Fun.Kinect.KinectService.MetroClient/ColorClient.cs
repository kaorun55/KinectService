// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.IO;
using Coding4Fun.Kinect.KinectService.Common;

#if NETFX_CORE
using Windows.UI.Xaml.Media.Imaging;
using Windows.Storage.Streams;
using Windows.Graphics.Imaging;
#else
using System.Net.Sockets;
using System.Windows.Media.Imaging;
#endif

namespace Coding4Fun.Kinect.KinectService.MetroClient
{
	public class ColorClient : KinectServiceClient
	{
		public event EventHandler<ColorFrameReadyEventArgs> ColorFrameReady;
		public ColorFrameData ColorFrame { get; private set; }

		public ColorClient()
		{
			this.ReadAsyncProsessor += ColorThread;
		}

		private async void ColorThread()
		{
			try
			{
                var reader = new DataReader( Client.InputStream );
                reader.InputStreamOptions = InputStreamOptions.Partial;
                reader.ByteOrder = ByteOrder.LittleEndian;

                while ( IsConnected ) {
                    await reader.LoadAsync( 4 );
                    var size = reader.ReadUInt32();

                    await reader.LoadAsync( size );
                    byte[] bytes = new byte[size];
                    reader.ReadBytes( bytes );

                    MemoryStream ms = new MemoryStream( bytes );
					BinaryReader br = new BinaryReader(ms);

                    ColorFrameReadyEventArgs args = new ColorFrameReadyEventArgs();
                    ColorFrameData cfd = new ColorFrameData();

                    cfd.Format = (ImageFormat)br.ReadInt32();
					cfd.ImageFrame = br.ReadColorImageFrame();

                    MemoryStream msData = new MemoryStream( bytes, (int)ms.Position, (int)(ms.Length - ms.Position) );
                    if (cfd.Format == ImageFormat.Raw)
                    {
                        cfd.RawImage = ms.ToArray();
                    }
                    else
                    {
                        InMemoryRandomAccessStream ras = new InMemoryRandomAccessStream();
                        DataWriter dw = new DataWriter(ras.GetOutputStreamAt(0));
                        dw.WriteBytes(msData.ToArray());
                        await dw.StoreAsync();   

                        // Set to the image
                        BitmapImage bImg = new BitmapImage();
                        bImg.SetSource(ras);
                        cfd.BitmapImage = bImg;
                    }

					ColorFrame = cfd;
					args.ColorFrame = cfd;

                    if (ColorFrameReady != null)
                    {
                        ColorFrameReady(this, args);
                    }
				}
			}
			catch(IOException)
			{
				Disconnect();
			}
		}
	}
}
