// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Coding4Fun.Kinect.KinectService.PhoneClient
{
	public class KinectServiceClient
	{
		private Socket _socket;
		private DnsEndPoint _endPoint;
		private readonly SynchronizationContext _context;

		private byte[] _data;
		private int _totalBytesTransferred;

		private enum State { Size, Data };
		private State _state = State.Size;

		public event EventHandler<FrameReadyEventArgs> FrameReady;
		public event EventHandler<ConnectionEventArgs> OnConnectionCompleted;

		public bool IsConnected { get { return _socket != null && _socket.Connected; } }

		public KinectServiceClient()
		{
			_context = SynchronizationContext.Current;
		}

		public void Connect(string address, int port)
		{
			_totalBytesTransferred = 0;

			_endPoint = new DnsEndPoint(address, port);
			_socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
			
			SocketAsyncEventArgs args = new SocketAsyncEventArgs();
			args.UserToken = _socket;
			args.RemoteEndPoint = _endPoint;
			args.Completed += OnSocketCompleted;
			_socket.ConnectAsync(args);
		}

		private void OnSocketCompleted(object sender, SocketAsyncEventArgs e)
		{
			switch(e.LastOperation)
			{
				case SocketAsyncOperation.Connect:

					Debug.WriteLine("Connected: " + _socket.Connected);

					if(OnConnectionCompleted != null)
						OnConnectionCompleted(this, new ConnectionEventArgs { Connected = _socket.Connected });

					_state = State.Size;
					_data = new byte[sizeof(int)];
					e.SetBuffer(_data, 0, _data.Length);

					TryReceiveAsync(e);

					break;
				case SocketAsyncOperation.Receive:
					switch(_state)
					{
						case State.Size:
							_totalBytesTransferred += e.BytesTransferred;

							if(_totalBytesTransferred < sizeof(int))
							{
								e.SetBuffer(_data, _totalBytesTransferred, sizeof(int) - _totalBytesTransferred);
								TryReceiveAsync(e);
							}
							else
							{
								_state = State.Data;
								int size = BitConverter.ToInt32(_data, 0);
								_data = new byte[size];
								e.SetBuffer(_data, 0, _data.Length);

								_totalBytesTransferred = 0;

								TryReceiveAsync(e);
							}
							break;
						case State.Data:
							_totalBytesTransferred += e.BytesTransferred;

							if(_totalBytesTransferred < _data.Length)
							{
								e.SetBuffer(_data, _totalBytesTransferred, _data.Length - _totalBytesTransferred);
								TryReceiveAsync(e);
							}
							else
							{
								// NOTE: .Post bogs down a WP7 device pretty badly...this *may* cause lag over time, though.  Needs further testing.
								_context.Send(delegate
								{
									if(FrameReady != null)
										FrameReady(this, new FrameReadyEventArgs { Data = _data });
								}, null);

								_totalBytesTransferred = 0;
								_state = State.Size;
								_data = new byte[sizeof(int)];

								e.SetBuffer(_data, 0, _data.Length);
								TryReceiveAsync(e);
							}

							break;
						default:
							throw new ArgumentOutOfRangeException();
					}

					break;
			}
		}

		public void Disconnect()
		{
			if(_socket != null)
				_socket.Close();
		}

		private void TryReceiveAsync(SocketAsyncEventArgs args)
		{
			try
			{
				if(_socket.Connected)
					_socket.ReceiveAsync(args);
			}
			catch(ObjectDisposedException)
			{
				Debug.WriteLine("Attempted to read on closed socket.");
			}
		}
	}

	public class ConnectionEventArgs : EventArgs
	{
		public bool Connected { get; set; }
	}

	public class FrameReadyEventArgs : EventArgs
	{
		public byte[] Data { get; set; }
	}
}
