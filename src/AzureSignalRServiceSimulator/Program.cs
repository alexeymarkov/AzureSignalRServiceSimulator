namespace AzureSignalRServiceSimulator
{
	using Microsoft.AspNetCore.Hosting;
	using Microsoft.Extensions.Configuration;
	using Microsoft.Extensions.Hosting;

	using Serilog;

	public class Program
	{
		public static void Main(string[] args)
		{
			CreateHostBuilder(args).Build().Run();
		}

		public static IHostBuilder CreateHostBuilder(string[] args) =>
			Host.CreateDefaultBuilder(args)
				.ConfigureWebHostDefaults(webBuilder =>
				{
					webBuilder
						.UseSerilog()
						.UseStartup<Startup>()
						.ConfigureAppConfiguration((context, builder) =>
						{
							var config = builder.Build();

							var configFile = config.GetValue<string>("ConfigFile");
							if (!string.IsNullOrEmpty(configFile))
							{
								builder.AddJsonFile(configFile, optional: false);
							}

							config = builder.Build();

							Log.Logger = new LoggerConfiguration()
								.ReadFrom.Configuration(config)
								.CreateLogger();
						});
				});
	}
}
