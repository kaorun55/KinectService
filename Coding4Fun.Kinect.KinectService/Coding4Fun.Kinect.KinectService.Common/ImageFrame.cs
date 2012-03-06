// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

namespace Coding4Fun.Kinect.KinectService.Common
{
	public class ImageFrame
	{
		public int BytesPerPixel { get; set; }
		public int FrameNumber { get; set; }
		public int Height { get; set; }
		public int PixelDataLength { get; set; }
		public long Timestamp { get; set; }
		public int Width { get; set; }
	}
}
