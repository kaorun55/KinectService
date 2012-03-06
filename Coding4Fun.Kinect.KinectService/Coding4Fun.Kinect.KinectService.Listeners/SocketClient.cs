// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Diagnostics;
using System.IO;
using System.Net.Sockets;

namespace Coding4Fun.Kinect.KinectService.Listeners
{
	class SocketClient
	{
		public bool IsConnected
		{
			get { return _connected && _client.Connected ; }
		}

		private readonly TcpClient _client;
		private bool _connected;

		public SocketClient(TcpClient client)
		{
			_client = client;
			_connected = true;
		}

		public bool Send(byte[] data, int length)
		{
			try
			{
				if (IsConnected)
				{
					NetworkStream stream = _client.GetStream();
					stream.BeginWrite(data, 0, length, WriteCompleted, stream);
				}
				else
				{
					_connected = false;
					return false;
				}
			}
			catch(IOException ex)
			{
				Debug.WriteLine(ex.ToString());
				_connected = false;
			}
			catch(ObjectDisposedException ex)
			{
				Debug.WriteLine(ex.ToString());
				_connected = false;
			}

			return true;
		}

		public bool Send(byte[] data)
		{
			return Send(data, data.Length);
		}

		private void WriteCompleted(IAsyncResult ar)
		{
			try
			{
				if(IsConnected)
				{
					NetworkStream ns = (NetworkStream)ar.AsyncState;
					ns.EndWrite(ar);
				}
			}
			catch(IOException ex)
			{
				Debug.WriteLine(ex.ToString());
				_connected = false;
			}
			catch(ObjectDisposedException ex)
			{
				Debug.WriteLine(ex.ToString());
				_connected = false;
			}
		}

		public void Close()
		{
			if(_client.Connected)
				_client.Close();
		}
	}
}
