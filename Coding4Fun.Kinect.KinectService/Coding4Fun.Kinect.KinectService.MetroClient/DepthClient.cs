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
using System.Windows.Media.Imaging;
#endif

namespace Coding4Fun.Kinect.KinectService.MetroClient
{
    public class DepthClient : KinectServiceClient
    {
        public event EventHandler<DepthFrameReadyEventArgs> DepthFrameReady;
        public DepthFrameData DepthFrame
        {
            get;
            private set;
        }

        public DepthClient()
        {
            this.ReadAsyncProsessor += DepthThread;
        }

        private async void DepthThread()
        {
            try {
                short[] depthShort = null;

                var reader = new DataReader( Client.InputStream );
                reader.InputStreamOptions = InputStreamOptions.Partial;

                while ( IsConnected ) {
                    await reader.LoadAsync( 4 );
                    var size = reader.ReadUInt32();

                    await reader.LoadAsync( size );
                    byte[] bytes = new byte[size];
                    reader.ReadBytes( bytes );

                    MemoryStream ms = new MemoryStream( bytes );
                    BinaryReader br = new BinaryReader( ms );

                    DepthFrameData dfd = new DepthFrameData();
                    dfd.PlayerIndexBitmask = br.ReadInt32();
                    dfd.PlayerIndexBitmaskWidth = br.ReadInt32();

                    DepthImageFrame frame = br.ReadDepthImageFrame();
                    dfd.ImageFrame = frame;

                    int dataLength = (int)(ms.Length - ms.Position);

                    if ( depthShort == null || depthShort.Length != dataLength / 2 )
                        depthShort = new short[dataLength / 2];

                    Buffer.BlockCopy( bytes, (int)br.BaseStream.Position, depthShort, 0, dataLength );

                    dfd.DepthData = depthShort;

                    DepthFrame = dfd;

                    DepthFrameReadyEventArgs args = new DepthFrameReadyEventArgs();
                    args.DepthFrame = dfd;

                    Context.Send( delegate
                    {
                        if ( DepthFrameReady != null )
                            DepthFrameReady( this, args );
                    }, null );
                }
            }
            catch ( IOException ) {
                Disconnect();
            }
        }
    }
}
