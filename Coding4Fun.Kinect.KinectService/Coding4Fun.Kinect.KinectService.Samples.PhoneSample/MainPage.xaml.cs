// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Coding4Fun.Kinect.KinectService.Common;
using Coding4Fun.Kinect.KinectService.PhoneClient;
using Microsoft.Phone.Controls;
using Microsoft.Xna.Framework.Audio;

namespace Coding4Fun.Kinect.KinectService.Samples.PhoneSample
{
	public partial class MainPage : PhoneApplicationPage
	{
		private readonly DynamicSoundEffectInstance _kinectSound = new DynamicSoundEffectInstance(16000, AudioChannels.Mono);

		private WriteableBitmap _outputBitmap;

		private ColorClient _colorClient;
		private DepthClient _depthClient;
		private SkeletonClient _skeletonClient;
		private AudioClient _audioClient;

		// Constructor
		public MainPage()
		{
			InitializeComponent();

			_colorClient = new ColorClient();
			_colorClient.ColorFrameReady += client_ColorFrameReady;

			_depthClient = new DepthClient();
			_depthClient.DepthFrameReady += client_DepthFrameReady;

			_skeletonClient = new SkeletonClient();
			_skeletonClient.SkeletonFrameReady += client_SkeletonFrameReady;

			_audioClient = new AudioClient();
			_audioClient.AudioFrameReady += client_AudioFrameReady;
		}

		private void Start_Click(object sender, RoutedEventArgs e)
		{
			if(!_colorClient.IsConnected)
				_colorClient.Connect(ServerIp.Text, 4530);
			else
				_colorClient.Disconnect();
		}

		void client_ColorFrameReady(object sender, ColorFrameReadyEventArgs e)
		{
			if(e.ColorFrame.BitmapImage != null)
				this.Color.Source = e.ColorFrame.BitmapImage;
		}

		private void StartDepth_Click(object sender, RoutedEventArgs e)
		{
			if(!_depthClient.IsConnected)
				_depthClient.Connect(ServerIp.Text, 4531);
			else
				_depthClient.Disconnect();
		}

		void client_DepthFrameReady(object sender, DepthFrameReadyEventArgs e)
		{
			if(_outputBitmap == null)
			{
				this._outputBitmap = new WriteableBitmap(
					e.DepthFrame.ImageFrame.Width, 
					e.DepthFrame.ImageFrame.Height);

				this.Depth.Source = this._outputBitmap;
			}

			byte[] convertedDepthBits = this.ConvertDepthFrame(e.DepthFrame.DepthData, e);

			Buffer.BlockCopy(convertedDepthBits, 0, _outputBitmap.Pixels, 0, convertedDepthBits.Length);
			_outputBitmap.Invalidate();
		}

		private byte[] ConvertDepthFrame(short[] depthFrame, DepthFrameReadyEventArgs args)
		{
			int[] intensityShiftByPlayerR = { 1, 2, 0, 2, 0, 0, 2, 0 };
			int[] intensityShiftByPlayerG = { 1, 2, 2, 0, 2, 0, 0, 1 };
			int[] intensityShiftByPlayerB = { 1, 0, 2, 2, 0, 2, 0, 2 };

			const int RedIndex = 1;
			const int GreenIndex = 2;
			const int BlueIndex = 3;

			byte[] depthFrame32 = new byte[args.DepthFrame.ImageFrame.Width * args.DepthFrame.ImageFrame.Height * 4];

			for (int i16 = 0, i32 = 0; i16 < depthFrame.Length && i32 < depthFrame.Length * 4; i16++, i32 += 4)
			{
				int player = depthFrame[i16] & args.DepthFrame.PlayerIndexBitmask;
				int realDepth = depthFrame[i16] >> args.DepthFrame.PlayerIndexBitmaskWidth;
				
				// transform 13-bit depth information into an 8-bit intensity appropriate
				// for display (we disregard information in most significant bit)
				byte intensity = (byte)(~(realDepth >> 4));

				if (player == 0 && realDepth == 0)
				{
					// white 
					depthFrame32[i32 + RedIndex] = 255;
					depthFrame32[i32 + GreenIndex] = 255;
					depthFrame32[i32 + BlueIndex] = 255;
				}
				else
				{
					// tint the intensity by dividing by per-player values
					depthFrame32[i32 + RedIndex] = (byte)(intensity >> intensityShiftByPlayerR[player]);
					depthFrame32[i32 + GreenIndex] = (byte)(intensity >> intensityShiftByPlayerG[player]);
					depthFrame32[i32 + BlueIndex] = (byte)(intensity >> intensityShiftByPlayerB[player]);
				}
			}

			return depthFrame32;
		}

		private void StartAudio_Click(object sender, RoutedEventArgs e)
		{
			if(!_audioClient.IsConnected)
				_audioClient.Connect(ServerIp.Text, 4533);
			else
				_audioClient.Disconnect();
		}

		void client_AudioFrameReady(object sender, AudioFrameReadyEventArgs e)
		{
			_kinectSound.SubmitBuffer(e.AudioFrame.AudioData);

			if(_kinectSound.State != SoundState.Playing)
				_kinectSound.Play();
		}

		private void StartSkeleton_Click(object sender, RoutedEventArgs e)
		{
			if(!_skeletonClient.IsConnected)
				_skeletonClient.Connect(ServerIp.Text, 4532);
			else
				_skeletonClient.Disconnect();
		}

		private void client_SkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e)
		{
			Skeleton skeleton = (from s in e.SkeletonFrame.Skeletons
									 where s.TrackingState == SkeletonTrackingState.Tracked
									 select s).FirstOrDefault();

			if(skeleton == null)
				return;

			SetEllipsePosition(headEllipse, skeleton.Joints[(int)JointType.Head]);
			SetEllipsePosition(leftEllipse, skeleton.Joints[(int)JointType.HandLeft]);
			SetEllipsePosition(rightEllipse, skeleton.Joints[(int)JointType.HandRight]);
		}

		private void SetEllipsePosition(FrameworkElement ellipse, Joint joint)
		{
			var scaledJoint = ScaleTo(joint, (int)Skeleton.Width, (int)Skeleton.Height, .5f, .5f);

			Canvas.SetLeft(ellipse, scaledJoint.Position.X);
			Canvas.SetTop(ellipse, scaledJoint.Position.Y);            
		}

		private Joint ScaleTo(Joint joint, int width, int height, float skeletonMaxX, float skeletonMaxY)
		{
			SkeletonPoint pos = new SkeletonPoint()
			{
				X = Scale(width, skeletonMaxX, joint.Position.X),
				Y = Scale(height, skeletonMaxY, -joint.Position.Y),
				Z = joint.Position.Z,
			};

			Joint j = new Joint()
			{
				TrackingState = joint.TrackingState,
				Position = pos
			};

			return j;
		}

		private float Scale(int maxPixel, float maxSkeleton, float position)
		{
			float value = ((((maxPixel / maxSkeleton) / 2) * position) + (maxPixel/2));
			if(value > maxPixel)
				return maxPixel;
			if(value < 0)
				return 0;
			return value;
		}
	}
}