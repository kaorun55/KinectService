// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using Coding4Fun.Kinect.KinectService.Common;

namespace Coding4Fun.Kinect.KinectService.PhoneClient
{
	public class AudioClient : KinectServiceClient
	{
		public event EventHandler<AudioFrameReadyEventArgs> AudioFrameReady;
		public AudioFrameData AudioFrame { get; private set; }

		public AudioClient()
		{
			this.FrameReady += AudioClient_FrameReady;
		}

		void AudioClient_FrameReady(object sender, FrameReadyEventArgs e)
		{
			AudioFrameReadyEventArgs args = new AudioFrameReadyEventArgs();
			AudioFrameData afd = new AudioFrameData();
			afd.AudioData = e.Data;

			args.AudioFrame = afd;
			AudioFrame = afd;

			if(AudioFrameReady != null)
				AudioFrameReady(this, args);
		}
	}
}
