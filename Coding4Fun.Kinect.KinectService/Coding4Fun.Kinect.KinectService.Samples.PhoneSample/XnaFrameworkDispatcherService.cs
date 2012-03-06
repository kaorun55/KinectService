using System;
using System.Threading;
using System.Windows;
using Microsoft.Xna.Framework;

namespace Coding4Fun.Kinect.KinectService.Samples.PhoneSample
{
	public class XnaFrameworkDispatcherService :  IApplicationService
	{
		private Timer _threadTimer;
		
		public void TimerAction(object state)
		{
			FrameworkDispatcher.Update();
		}

		public void StartService(ApplicationServiceContext context)
		{
			_threadTimer = new Timer(TimerAction, null, TimeSpan.FromMilliseconds(33), TimeSpan.FromMilliseconds(33));
		}

		public void StopService()
		{
			_threadTimer.Dispose();
		}
	}
}
