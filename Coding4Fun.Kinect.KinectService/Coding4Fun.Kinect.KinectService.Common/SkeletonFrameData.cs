// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;

namespace Coding4Fun.Kinect.KinectService.Common
{
	public class SkeletonFrameData
	{
		public Tuple<float,float,float,float> FloorClipPlane { get; set; }
		public int FrameNumber { get; set; }
		public int SkeletonArrayLength { get; set; }
		public long Timestamp { get; set; }
		public Skeleton[] Skeletons { get; set; }
	}
}
