// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

namespace Coding4Fun.Kinect.KinectService.Common
{
	public class DepthFrameData
	{
		public int PlayerIndexBitmask { get; set; }
		public int PlayerIndexBitmaskWidth { get; set; }
		public DepthImageFrame ImageFrame { get; set; }
		public short[] DepthData { get; set; }
	}
}