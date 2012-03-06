// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Threading;

#if NETFX_CORE
using Windows.UI.Xaml.Media.Imaging;
#else
using System.Net.Sockets;
#endif

namespace Coding4Fun.Kinect.KinectService.WpfClient
{
	public class KinectServiceClient
	{
		public event EventHandler<ConnectionEventArgs> OnConnectionCompleted;

		protected ThreadStart ThreadProcessor { get; set; }
		protected TcpClient Client { get; set; }
		protected SynchronizationContext Context { get; set; }

		public bool IsConnected { get { return Client != null && Client.Connected; } }

		public KinectServiceClient()
		{
			Context = SynchronizationContext.Current;
		}

		public void Connect(string address, int port)
		{
			Client = new TcpClient();
			Client.BeginConnect(address, port, OnSocketConnectCompleted, null);
		}

		private void OnSocketConnectCompleted(IAsyncResult ar)
		{
			bool connected = Client.Connected;

			if(OnConnectionCompleted != null)
				OnConnectionCompleted(this, new ConnectionEventArgs { Connected = connected });

			if(!connected)
				return;

			Thread thread = new Thread(ThreadProcessor) {IsBackground = true};
			thread.Start();
		}

		public void Disconnect()
		{
			if(Client != null)
				Client.Close();
		}
	}

	public class ConnectionEventArgs : EventArgs
	{
		public bool Connected { get; set; }
	}
}
