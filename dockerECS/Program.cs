using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace dockerECS
{
	public class Program
	{
		public static IConfigurationRoot Configuration;
		public static void Main(string[] args)
		{
			using (var host = CreateHostBuilder(args).Build())
			{
				var logger = host.Services.GetRequiredService<ILogger<Program>>();
				logger.LogInformation("dockerECS Application has started.");

				string con = Configuration.GetSection("DatabaseConfig:DatabaseConnectionString").Value;
				using (var connection = new SqlConnection(con))
				{
					var sql = string.Format(@"SELECT Name FROM Name");
					try
					{
						List<string> names = (connection.QueryAsync<string>(sql).Result.ToList());
						names.ForEach(i =>
						{
							logger.LogInformation($"Name: {i}");
							Task.Run(() => Task.Delay(2000)).Wait();
						});
					}
					catch (Exception ex)
					{
						logger.LogError($"Error: {ex}");
					}
				}
				logger.LogInformation("dockerESC Application has stopped.");
			}
		}

		public static IHostBuilder CreateHostBuilder(string[] args) =>
			Host.CreateDefaultBuilder(args)
				.ConfigureAppConfiguration((context, builder) =>
				{
					//Add Configurations from appsettings.json
					Configuration = new ConfigurationBuilder()
									.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
									.AddJsonFile($"appsettings.{context.HostingEnvironment.EnvironmentName}.json", optional: true)
									.AddEnvironmentVariables()
									.Build();
				})
				.ConfigureServices((hostContext, services) =>
				{
					//services.AddHostedService<Worker>();
				});
	}
}
