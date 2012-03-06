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
    public class AudioClient : KinectServiceClient
    {
        public event EventHandler<AudioFrameReadyEventArgs> AudioFrameReady;
        public AudioFrameData AudioFrame
        {
            get;
            private set;
        }

        public AudioClient()
        {
            this.ReadAsyncProsessor += AudioThread;
        }

        private async void AudioThread()
        {
            try {
                var reader = new DataReader( Client.InputStream );
                reader.InputStreamOptions = InputStreamOptions.Partial;

                while ( IsConnected ) {
                    await reader.LoadAsync( 4 );
                    var size = reader.ReadUInt32();

                    await reader.LoadAsync( size );
                    byte[] bytes = new byte[size];
                    reader.ReadBytes( bytes );

                    AudioFrameReadyEventArgs args = new AudioFrameReadyEventArgs();
                    AudioFrameData afd = new AudioFrameData();

                    afd.AudioData = bytes;
                    args.AudioFrame = afd;

                    Context.Send( delegate
                    {
                        if ( AudioFrameReady != null )
                            AudioFrameReady( this, args );
                    }, null );
                }
            }
            catch ( IOException ) {
                Disconnect();
            }
        }
    }
}
