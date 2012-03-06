// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.IO;
using Coding4Fun.Kinect.KinectService.Common;

#if NETFX_CORE
#else
using System.Net.Sockets;
#endif

namespace Coding4Fun.Kinect.KinectService.WpfClient
{
	public class AudioClient : KinectServiceClient
	{
		public event EventHandler<AudioFrameReadyEventArgs> AudioFrameReady;
		public AudioFrameData AudioFrame { get; private set; }

		public AudioClient()
		{
			this.ThreadProcessor = AudioThread;
		}

		private void AudioThread()
		{
			try
			{
				NetworkStream ns = Client.GetStream();
				BinaryReader reader = new BinaryReader(ns);

				while(Client.Connected)
				{
					int size = reader.ReadInt32();
					byte[] bytes = reader.ReadBytes(size);

					AudioFrameReadyEventArgs args = new AudioFrameReadyEventArgs();
					AudioFrameData afd = new AudioFrameData();

					afd.AudioData = bytes;
					args.AudioFrame = afd;

					Context.Send(delegate
					{
						if(AudioFrameReady != null)
							AudioFrameReady(this, args);
					}, null);
				}
			}
			catch(IOException)
			{
				Client.Close();
			}
		}
	}
}
