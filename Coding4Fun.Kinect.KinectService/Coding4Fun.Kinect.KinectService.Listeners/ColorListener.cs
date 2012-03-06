// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Coding4Fun.Kinect.KinectService.Common;
using Microsoft.Kinect;
using ColorImageFormat = Microsoft.Kinect.ColorImageFormat;
using ColorImageFrame = Microsoft.Kinect.ColorImageFrame;

namespace Coding4Fun.Kinect.KinectService.Listeners
{
	public class ColorListener : KinectListener
	{
		private readonly ImageFormat _format;
		private readonly double _scale = double.NaN;
		private readonly int _fps = -1;
		private int _frameCount;

		private WriteableBitmap _bitmap;
		private byte[] _image;
		private readonly MemoryStream _memoryStream = new MemoryStream();
		private readonly BinaryWriter _binaryWriter;

		public ColorImageFrame ColorImageFrame { get; private set; }

		public ColorListener(KinectSensor kinect, int port, ImageFormat format, double scale, int fps)
		{
			VerifyConstructorArguments(kinect, port);

			if(scale < 0)
				throw new ArgumentException("Scale must be > 0", "scale");

			if(fps < -1 || fps > 30)
				throw new ArgumentException("FPS value must be between 1 and 30, inclusive.");

			_format = format;
			_scale = scale;
			_fps = fps;

			this.Kinect = kinect;
			this.Port = port;
			this.Kinect.ColorFrameReady += Kinect_ColorFrameReady;

			_binaryWriter = new BinaryWriter(_memoryStream);
		}

		public ColorListener(KinectSensor kinect, int port, ImageFormat format, double scale) : this(kinect, port, format, scale, -1)
		{
		}

		public ColorListener(KinectSensor kinect, int port, ImageFormat format) : this(kinect, port, format, double.NaN)
		{
		}

		void Kinect_ColorFrameReady(object sender, ColorImageFrameReadyEventArgs e)
		{
			if(ClientList.Count > 0)
			{
				using(ColorImageFrame frame = e.OpenColorImageFrame())
				{
					this.ColorImageFrame = frame;

					if(frame != null)
					{
						if(_format != ImageFormat.Raw && (frame.Format == ColorImageFormat.RawYuvResolution640x480Fps15 || frame.Format == ColorImageFormat.YuvResolution640x480Fps15))
							throw new ArgumentException("YUV color formats are not supported.  Please use an RGB ColorImageFormat.");

						_memoryStream.Seek(0, SeekOrigin.Begin);

						_binaryWriter.Write((int)_format);

						_binaryWriter.Write(frame);
						_binaryWriter.Write((int)frame.Format);

						if(_image == null || _image.Length < frame.PixelDataLength)
							_image = new byte[frame.PixelDataLength];

						frame.CopyPixelDataTo(_image);
	
						BitmapEncoder encoder = null;

						switch(_format)
						{
							case ImageFormat.Jpeg:
								encoder = new JpegBitmapEncoder();
								break;
							case ImageFormat.Png:
								encoder = new PngBitmapEncoder();
								break;
							case ImageFormat.Raw:
								break;
						}

						if(encoder != null)
						{
							if(_bitmap == null || _bitmap.PixelWidth != frame.Width || _bitmap.PixelHeight != frame.Height)
								_bitmap = new WriteableBitmap(frame.Width, frame.Height, 96, 96, PixelFormats.Bgr32, null);

							_bitmap.WritePixels(new Int32Rect(0, 0, frame.Width, frame.Height), _image, frame.Width * frame.BytesPerPixel, 0);

							if(double.IsNaN(_scale))
								encoder.Frames.Add(BitmapFrame.Create(_bitmap));
							else
							{
								BitmapSource bs = new TransformedBitmap(_bitmap, new ScaleTransform(_scale, _scale));
								encoder.Frames.Add(BitmapFrame.Create(bs));
							}

							encoder.Save(_memoryStream);
						}
						else
							_binaryWriter.Write(_image);

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

		private int GetFps(ColorImageFormat format)
		{
			switch(format)
			{
				case ColorImageFormat.Undefined:
					return 0;
				case ColorImageFormat.RgbResolution640x480Fps30:
					return 30;
				case ColorImageFormat.RgbResolution1280x960Fps12:
					return 12;
				case ColorImageFormat.YuvResolution640x480Fps15:
				case ColorImageFormat.RawYuvResolution640x480Fps15:
					return 15;
				default:
					throw new ArgumentOutOfRangeException("format");
			}
		}
	}
}
