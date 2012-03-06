using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using Microsoft.Kinect;

namespace Coding4Fun.Kinect.KinectService.Listeners
{
	public class KinectListener
	{
		protected KinectSensor Kinect { get; set; }

		protected int Port { get; set; }

		internal event EventHandler<ConnectionEventArgs> OnConnectionCompleted;
		internal List<SocketClient> ClientList { get; set; }

		private TcpListener _listener;

		public void Start()
		{
			ClientList = new List<SocketClient>();

			_listener = new TcpListener(IPAddress.Any, Port);
			_listener.Start(10);
			_listener.BeginAcceptTcpClient(OnConnection, null);
		}

		public void Stop()
		{
			foreach (SocketClient client in ClientList)
				client.Close();
		}

		private void OnConnection(IAsyncResult ar)
		{
			TcpClient client = _listener.EndAcceptTcpClient(ar);
			SocketClient sc = new SocketClient(client);
			ClientList.Add(sc);
			_listener.BeginAcceptTcpClient(OnConnection, null);

			if(OnConnectionCompleted != null)
				OnConnectionCompleted(this, new ConnectionEventArgs { TcpClient = client, SocketClient = sc });
		}

		protected void RemoveClients()
		{
			lock(ClientList)
			{
				for(int i = 0; i < ClientList.Count; i++)
				{
					if (!ClientList[i].IsConnected)
						ClientList.Remove(ClientList[i]);
				}
			}
		}

		protected void VerifyConstructorArguments(KinectSensor kinect, int port)
		{
			if(kinect == null)
				throw new ArgumentException("A valid KinectSensor object must be provided.", "kinect");

			if(port < 1 || port > 65535)
				throw new ArgumentException("Ports must be between 0 and 65535, inclusive", "port");
		}
	}
}
