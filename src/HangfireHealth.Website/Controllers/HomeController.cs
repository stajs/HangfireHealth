using Hangfire;
using Microsoft.AspNet.Mvc;

namespace HangfireHealth.Website.Controllers
{
	public class HomeController : Controller
	{
		public IActionResult Index()
		{
			return View();
		}

		public IActionResult KillServer()
		{
			BackgroundJob.Enqueue<Job>(j => j.LongRunning());

			return RedirectToAction("Index");
		}

		public IActionResult LotsAJobs()
		{
			for (var i = 0; i < 100; i++)
				BackgroundJob.Enqueue<Job>(j => j.ShortRunning());

			return RedirectToAction("Index");
		}
	}
}