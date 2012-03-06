// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Kinect;

namespace Coding4Fun.Kinect.KinectService.Listeners
{
	public class AudioListener : KinectListener
	{
		private bool _running;

		public AudioListener(KinectSensor kinect, int port)
		{
			VerifyConstructorArguments(kinect, port);

			this.Kinect = kinect;
			this.Port = port;

			Thread t = new Thread(AudioThread) { IsBackground = true };
			t.Start();
		}

		private void AudioThread()
		{
			_running = true;

			byte[] buffer = new byte[4096];

			Stream kinectAudioStream = Kinect.AudioSource.Start();
			
			while(this._running)
			{
				int count = kinectAudioStream.Read(buffer, 0, buffer.Length);

				Parallel.For(0, ClientList.Count, index =>
				{
					SocketClient sc = ClientList[index];

					sc.Send(BitConverter.GetBytes(count));
					sc.Send(buffer, count);
				});

				RemoveClients();
			}
		}
	}
}
