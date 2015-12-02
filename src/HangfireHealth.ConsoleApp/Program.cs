using System;
using System.Collections.Generic;
using HangfireHealth.Website;
using Microsoft.AspNet.Hosting;
using Microsoft.Data.Entity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HangfireHealth.ConsoleApp
{
	public class Program
	{
		private static IDisposable _website;

		public static void ConfigureServices(IServiceCollection services)
		{
			services
				.AddEntityFramework()
				.AddSqlServer()
				.AddDbContext<HangfireHealthDbContext>(options => options.UseSqlServer("Server=.\\sqlexpress;Database=HangfireHealth;Trusted_Connection=True;MultipleActiveResultSets=true"));

			services.AddTransient<Job>();
		}

		public static void Main(string[] args)
		{
			var config = new ConfigurationBuilder()
				.AddInMemoryCollection(new [] { new KeyValuePair<string, string>("Server.Urls", "http://localhost:2100") })
            .Build();

			var host = new WebHostBuilder(config, true)
				.UseStartup<Startup>()
				.UseServices(ConfigureServices)
				.UseServer("Microsoft.AspNet.Server.Kestrel");

			var hostingEngine = host.Build();

			_website = hostingEngine.Start();

			Console.WriteLine("hai");
			Console.ReadKey();
		}
	}
}