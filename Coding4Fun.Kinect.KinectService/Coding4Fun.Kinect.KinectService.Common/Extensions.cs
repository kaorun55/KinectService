// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.IO;

namespace Coding4Fun.Kinect.KinectService.Common
{
	public static class Extensions
	{
		public static SkeletonFrameData ReadSkeletonFrame(this BinaryReader br)
		{
			SkeletonFrameData frame = new SkeletonFrameData();

			frame.FloorClipPlane = new Tuple<float,float,float,float>(br.ReadSingle(), br.ReadSingle(), br.ReadSingle(), br.ReadSingle());
			frame.FrameNumber = br.ReadInt32();
			frame.SkeletonArrayLength = br.ReadInt32();
			frame.Timestamp = br.ReadInt64();

			frame.Skeletons = new Skeleton[frame.SkeletonArrayLength];
			for(int i = 0; i < frame.Skeletons.Length; i++)
			{
				frame.Skeletons[i] = new Skeleton();
				frame.Skeletons[i].ClippedEdges = (FrameEdges)br.ReadInt32();
				int jointCount = br.ReadInt32();
				frame.Skeletons[i].Joints = new Joint[jointCount];
				for(int jx = 0; jx < jointCount; jx++)
				{
					frame.Skeletons[i].Joints[jx].JointType = (JointType)br.ReadInt32();
					frame.Skeletons[i].Joints[jx].Position = new SkeletonPoint { X = br.ReadSingle(), Y = br.ReadSingle(), Z = br.ReadSingle() };
					frame.Skeletons[i].Joints[jx].TrackingState = (JointTrackingState)br.ReadInt32();
				}
				frame.Skeletons[i].Position = new SkeletonPoint { X = br.ReadSingle(), Y = br.ReadSingle(), Z = br.ReadSingle() };
				frame.Skeletons[i].TrackingId = br.ReadInt32();
				frame.Skeletons[i].TrackingState = (SkeletonTrackingState)br.ReadInt32();
			}

			return frame;
		}

		public static ColorImageFrame ReadColorImageFrame(this BinaryReader br)
		{
			ColorImageFrame frame = new ColorImageFrame();
			frame = (ColorImageFrame)ReadImageFrame(frame, br);
			frame.Format = (ColorImageFormat)br.ReadInt32();
			return frame;
		}

		public static DepthImageFrame ReadDepthImageFrame(this BinaryReader br)
		{
			DepthImageFrame frame = new DepthImageFrame();
			frame = (DepthImageFrame)ReadImageFrame(frame, br);
			frame.Format = (DepthImageFormat)br.ReadInt32();
			return frame;
		}

		private static ImageFrame ReadImageFrame(ImageFrame frame, BinaryReader br)
		{
			frame.BytesPerPixel = br.ReadInt32();
			frame.FrameNumber  = br.ReadInt32();
			frame.Height = br.ReadInt32();
			frame.PixelDataLength = br.ReadInt32();
			frame.Timestamp = br.ReadInt64();
			frame.Width = br.ReadInt32();

			return frame;
		}
	}
}