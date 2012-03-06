// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.IO;
using Coding4Fun.Kinect.KinectService.Common;
using Windows.Storage.Streams;

#if NETFX_CORE
#else
using System.Net.Sockets;
#endif

namespace Coding4Fun.Kinect.KinectService.MetroClient
{
	public class SkeletonClient : KinectServiceClient
	{
		public event EventHandler<SkeletonFrameReadyEventArgs> SkeletonFrameReady;
		public SkeletonFrameData SkeletonFrame { get; private set; }

		public SkeletonClient()
		{
			this.ReadAsyncProsessor += StreamSkeleton;
		}

		private async void StreamSkeleton()
		{
			try
			{
                var reader = new DataReader( Client.InputStream );
                reader.InputStreamOptions = InputStreamOptions.Partial;

                while ( IsConnected ) {
                    await reader.LoadAsync( 4 );
                    var size = reader.ReadUInt32();

                    await reader.LoadAsync( size );
                    byte[] bytes = new byte[size];
                    reader.ReadBytes( bytes );

                    MemoryStream ms = new MemoryStream( bytes );
					BinaryReader br = new BinaryReader(ms);

					SkeletonFrameData frame = br.ReadSkeletonFrame();

					SkeletonFrameReadyEventArgs args = new SkeletonFrameReadyEventArgs { SkeletonFrame = frame };
					SkeletonFrame = frame;

					Context.Send(delegate
					{
						if(SkeletonFrameReady != null)
							SkeletonFrameReady(this, args);
					}, null);
				}
			}
			catch(IOException)
			{
                Disconnect();
			}
		}
	}
}
