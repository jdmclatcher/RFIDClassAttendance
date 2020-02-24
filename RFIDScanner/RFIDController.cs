using System;
using System.Threading;
using Unosquare.RaspberryIO;
using Unosquare.WiringPi;
using System.Text;
using Unosquare.RaspberryIO.Peripherals;
using System.ComponentModel;
using System.Timers;

namespace RFIDScanner
{
	class RFIDController : RFIDControllerMfrc522
	{
		private static System.Timers.Timer detectionTimer;

		public RFIDController()
		{
			detectionTimer = new System.Timers.Timer(500);
			detectionTimer.Elapsed += TimedDetection;
			detectionTimer.AutoReset = true;
			detectionTimer.Enabled = true;
		}

		private void TimedDetection(Object source, ElapsedEventArgs e)
		{
			if (DetectCard().ToString() == "AllOk")
			{
				CardDetected?.Invoke(this, e);
			}
		}

		public event EventHandler CardDetected;
	}
}
