// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;


namespace Coding4Fun.Kinect.KinectService.WindowsService
{
	[RunInstaller(true)]
	public partial class ServiceHostInstaller : System.Configuration.Install.Installer
	{
		public ServiceHostInstaller()
		{
			InitializeComponent();
		}
	}
}
