// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Kinect;

namespace Coding4Fun.Kinect.KinectService.Listeners
{
	public class SkeletonListener : KinectListener
	{
		private readonly MemoryStream _memoryStream = new MemoryStream();
		private readonly BinaryWriter _binaryWriter;
		private Skeleton[] _skeletons;

		public SkeletonFrame SkeletonFrame { get; private set; }

		public SkeletonListener(KinectSensor kinect, int port)
		{
			VerifyConstructorArguments(kinect, port);

			this.Kinect = kinect;
			this.Port = port;

			this.Kinect.SkeletonFrameReady += Kinect_SkeletonFrameReady;

			_binaryWriter = new BinaryWriter(_memoryStream);
		}

		void Kinect_SkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e)
		{
			if(ClientList.Count > 0)
			{
			    using(SkeletonFrame frame = e.OpenSkeletonFrame())
				{
					this.SkeletonFrame = frame;

					if(frame != null)
					{
						_memoryStream.Seek(0, SeekOrigin.Begin);

						if(_skeletons == null || _skeletons.Length != frame.SkeletonArrayLength)
							_skeletons = new Skeleton[frame.SkeletonArrayLength];

						frame.CopySkeletonDataTo(_skeletons);

						_binaryWriter.Write(frame.FloorClipPlane.Item1);
						_binaryWriter.Write(frame.FloorClipPlane.Item2);
						_binaryWriter.Write(frame.FloorClipPlane.Item3);
						_binaryWriter.Write(frame.FloorClipPlane.Item4);
						_binaryWriter.Write(frame.FrameNumber);
						_binaryWriter.Write(frame.SkeletonArrayLength);
						_binaryWriter.Write(frame.Timestamp);

						foreach(Skeleton s in _skeletons)
						{
							_binaryWriter.Write((int)s.ClippedEdges);
							_binaryWriter.Write(s.Joints.Count);
							foreach(Joint j in s.Joints)
							{
								_binaryWriter.Write((int)j.JointType);
								_binaryWriter.Write(j.Position.X);
								_binaryWriter.Write(j.Position.Y);
								_binaryWriter.Write(j.Position.Z);
								_binaryWriter.Write((int)j.TrackingState);
							}
							_binaryWriter.Write(s.Position.X);
							_binaryWriter.Write(s.Position.Y);
							_binaryWriter.Write(s.Position.Z);
							_binaryWriter.Write(s.TrackingId);
							_binaryWriter.Write((int)s.TrackingState);
						}

						Parallel.For(0, ClientList.Count, index =>
						{
							SocketClient sc = ClientList[index];
						
							sc.Send(BitConverter.GetBytes((int)_memoryStream.Length));
							sc.Send(_memoryStream.ToArray());
						});

						RemoveClients();
					}
				}
			}
		}
	}
}
