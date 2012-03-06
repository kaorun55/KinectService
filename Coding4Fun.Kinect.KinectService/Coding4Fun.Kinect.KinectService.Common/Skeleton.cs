// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

namespace Coding4Fun.Kinect.KinectService.Common
{
	public class Skeleton
	{
		public FrameEdges ClippedEdges { get; set; }
		public Joint[] Joints { get; set; }
		public SkeletonPoint Position { get; set; }
		public int TrackingId { get; set; }
		public SkeletonTrackingState TrackingState { get; set; }
	}
}
