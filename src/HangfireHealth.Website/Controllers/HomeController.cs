using Microsoft.AspNet.Mvc;

namespace HangfireHealth.Website.Controllers
{
	public class HomeController : Controller
	{
		public IActionResult Index()
		{
			return View();
		}
	}
}
