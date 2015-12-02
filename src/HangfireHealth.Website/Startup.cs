using System;
using System.Messaging;
using Hangfire;
using Hangfire.Dashboard;
using Hangfire.SqlServer;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.FileProviders;
using Microsoft.AspNet.Hosting;
using Microsoft.Data.Entity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace HangfireHealth.Website
{
	public class Startup
	{
		private readonly IHostingEnvironment _env;
		private string _websitePath;

		public Startup(IHostingEnvironment env)
		{
			_env = env;
			_websitePath = _env.WebRootPath.Replace("ConsoleApp", "Website");
			Console.WriteLine("_websitePath: " + _websitePath);
		}

		private void ConfigureHangfire(IApplicationBuilder app)
		{
			const string queueName = @".\private$\hangfire_health";

			if (!MessageQueue.Exists(queueName))
			{
				var queue = MessageQueue.Create(queueName, true);
				queue.SetPermissions("Administrators", MessageQueueAccessRights.FullControl);
			}

			var options = new SqlServerStorageOptions
			{
				PrepareSchemaIfNecessary = true
			};

			GlobalConfiguration.Configuration
				.UseSqlServerStorage("Server=.\\sqlexpress;Database=HangfireHealth;Trusted_Connection=True;MultipleActiveResultSets=true", options)
				.UseMsmqQueues(queueName)
				.UseDIActivator(app.ApplicationServices)
				.UseDashboardMetric(DashboardMetrics.ScheduledCount)
				.UseDashboardMetric(DashboardMetrics.RetriesCount)
				.UseDashboardMetric(DashboardMetrics.ProcessingCount)
				.UseDashboardMetric(DashboardMetrics.SucceededCount)
				.UseDashboardMetric(DashboardMetrics.FailedCount)
				.UseDashboardMetric(DashboardMetrics.AwaitingCount);

			app.UseHangfireServer();
			app.UseHangfireDashboard("/hangfire", new DashboardOptions
			{
				AuthorizationFilters = new[] { new HangfireAuthorizationFilter() }
			});
		}

		public void ConfigureServices(IServiceCollection services)
		{
			services
				.AddMvc()
				.AddRazorOptions(options => { options.FileProvider = new PhysicalFileProvider(_websitePath); });
		}

		public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
		{
			try
			{
				using (var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
				{
					serviceScope.ServiceProvider.GetService<HangfireHealthDbContext>().Database.Migrate();
				}
			}
			catch { }
			
			ConfigureHangfire(app);

			app.UseMvcWithDefaultRoute();
			app.UseDeveloperExceptionPage();
		}

		public static void Main(string[] args) => WebApplication.Run<Startup>(args);
	}
}