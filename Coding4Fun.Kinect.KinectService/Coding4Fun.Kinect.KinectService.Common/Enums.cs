// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;

namespace Coding4Fun.Kinect.KinectService.Common
{
	public enum ImageFormat
	{
		Jpeg,
		Png,
		Raw
	}

	public enum ColorImageFormat
	{
		Undefined,
		RgbResolution640x480Fps30,
		RgbResolution1280x960Fps12,
		YuvResolution640x480Fps15,
		RawYuvResolution640x480Fps15,
	}

	public enum DepthImageFormat
	{
		Undefined,
		Resolution640x480Fps30,
		Resolution320x240Fps30,
		Resolution80x60Fps30,
	}


	[Flags]
	public enum FrameEdges
	{
		Bottom = 8,
		Left = 2,
		None = 0,
		Right = 1,
		Top = 4
	}

	public enum SkeletonTrackingState
	{
		NotTracked,
		PositionOnly,
		Tracked
	}

	public enum JointType
	{
		HipCenter,
		Spine,
		ShoulderCenter,
		Head,
		ShoulderLeft,
		ElbowLeft,
		WristLeft,
		HandLeft,
		ShoulderRight,
		ElbowRight,
		WristRight,
		HandRight,
		HipLeft,
		KneeLeft,
		AnkleLeft,
		FootLeft,
		HipRight,
		KneeRight,
		AnkleRight,
		FootRight
	}

	public enum JointTrackingState
	{
		NotTracked,
		Inferred,
		Tracked
	}
}
