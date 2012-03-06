// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.ServiceProcess;

namespace Coding4Fun.Kinect.KinectService.WindowsService
{
	public partial class ServiceHost : ServiceBase
	{
		ServiceEngine se = new ServiceEngine();

		public ServiceHost()
		{
			InitializeComponent();
		}

		protected override void OnStart(string[] args)
		{
			se.Start();
		}

		protected override void OnStop()
		{
			se.Stop();
		}
	}
}
