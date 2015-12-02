using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HangfireHealth.Website
{
	public class Job
	{
		private static Random _random = new Random();

		public void LongRunning()
		{
			var now = DateTime.Now;

			// I think the default timeout is 5 minutes, so go a bit beyond that.
			var sixMinutes = now.AddMinutes(6);

			// Spin.
			while (now < sixMinutes)
			{
				now = DateTime.Now;
			}
		}

		public void ShortRunning()
		{
			Thread.Sleep(_random.Next(1, 4) * 1000);
		}
	}
}