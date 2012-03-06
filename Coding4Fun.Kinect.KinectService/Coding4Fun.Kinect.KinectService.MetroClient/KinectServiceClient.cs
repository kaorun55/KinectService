// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Threading;

#if NETFX_CORE
using Windows.UI.Xaml.Media.Imaging;
using Windows.Networking.Sockets;
using Windows.Networking;
using System.Threading.Tasks;
using Windows.Foundation;
#else
using System.Net.Sockets;
#endif

namespace Coding4Fun.Kinect.KinectService.MetroClient
{
    public class KinectServiceClient
    {
        public event EventHandler<ConnectionEventArgs> OnConnectionCompleted;

        public delegate void ReadAsync();
        public event ReadAsync ReadAsyncProsessor;

        protected bool isConnected = false;

        protected StreamSocket Client
        {
            get;
            set;
        }
        protected SynchronizationContext Context
        {
            get;
            set;
        }

        public bool IsConnected
        {
            get
            {
                return Client != null && isConnected;
            }
        }

        public KinectServiceClient()
        {
            Context = SynchronizationContext.Current;
        }

        public void Connect( string address, int port )
		{
            try {
                Disconnect();

                Client = new StreamSocket();
                Client.ConnectAsync( new HostName( address ), port.ToString(), SocketProtectionLevel.PlainSocket );
                isConnected = true;

                if ( OnConnectionCompleted != null ) {
                    OnConnectionCompleted( this, new ConnectionEventArgs
                    {
                        Connected = IsConnected
                    } );
                }

                if ( !IsConnected ) {
                    return;
                }

                if ( ReadAsyncProsessor != null ) {
                    ReadAsyncProsessor();
                }
            }
            catch ( Exception ) {
                isConnected = false;
            }
		}

        public void Disconnect()
        {
            if ( Client != null ) {
                isConnected = false;

                Client.Dispose();
                Client = null;
            }
        }
    }

    public class ConnectionEventArgs : EventArgs
    {
        public bool Connected
        {
            get;
            set;
        }
    }
}
