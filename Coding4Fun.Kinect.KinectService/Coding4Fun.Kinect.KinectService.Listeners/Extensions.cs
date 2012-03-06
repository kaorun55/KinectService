// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.IO;
using Microsoft.Kinect;

namespace Coding4Fun.Kinect.KinectService.Listeners
{
	static class Extensions
	{
		public static void Write(this BinaryWriter bw, ImageFrame frame)
		{
			bw.Write(frame.BytesPerPixel);
			bw.Write(frame.FrameNumber);
			bw.Write(frame.Height);
			bw.Write(frame.PixelDataLength);
			bw.Write(frame.Timestamp);
			bw.Write(frame.Width);
		}
	}
}
