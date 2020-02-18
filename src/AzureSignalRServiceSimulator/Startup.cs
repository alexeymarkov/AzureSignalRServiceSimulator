namespace AzureSignalRServiceSimulator
{
	using System.Linq;
	using System.Text;
	using System.Threading.Tasks;

	using AzureSignalRServiceSimulator.Extensions;

	using Microsoft.AspNetCore.Authentication.JwtBearer;
	using Microsoft.AspNetCore.Builder;
	using Microsoft.AspNetCore.Hosting;
	using Microsoft.AspNetCore.Http;
	using Microsoft.Extensions.Configuration;
	using Microsoft.Extensions.DependencyInjection;
	using Microsoft.Extensions.Hosting;
	using Microsoft.Extensions.Logging;
	using Microsoft.IdentityModel.Tokens;

	public class Startup
	{
		public Startup(IConfiguration configuration)
		{
			configuration.GetSection("SignalR").Bind(SignalROptions);
			SignalROptions.Validate();
		}

		protected SignalROptions SignalROptions { get; } = new SignalROptions();

		protected HubFactory HubFactory { get; } = new HubFactory();

		// This method gets called by the runtime. Use this method to add services to the container.
		// For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
		public void ConfigureServices(IServiceCollection services)
		{
			services.AddCors(options =>
			{
				options.AddDefaultPolicy(
					builder =>
					{
						builder.WithOrigins(SignalROptions.CORS);
						builder.AllowCredentials();
						builder.AllowAnyHeader();
						builder.AllowAnyMethod();
					});
			});

			services.AddAuthentication(options =>
			{
				// Identity made Cookie authentication the default.
				// However, we want JWT Bearer Auth to be the default.
				options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
				options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
			})
				.AddJwtBearer(options =>
				{
					options.TokenValidationParameters = new TokenValidationParameters()
					{
						ValidateIssuer = false,
						ValidateAudience = false,
						ValidateIssuerSigningKey = true,
						IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SignalROptions.AccessKey))
					};

					// See https://docs.microsoft.com/en-us/aspnet/core/signalr/authn-and-authz?view=aspnetcore-3.1#authenticate-users-connecting-to-a-signalr-hub
					// We have to hook the OnMessageReceived event in order to
					// allow the JWT authentication handler to read the access
					// token from the query string when a WebSocket or
					// Server-Sent Events request comes in.

					// Sending the access token in the query string is required due to
					// a limitation in Browser APIs. We restrict it to only calls to the
					// SignalR hub in this code.
					// See https://docs.microsoft.com/aspnet/core/signalr/security#access-token-logging
					// for more information about security considerations when using
					// the query string to transmit the access token.
					options.Events = new JwtBearerEvents
					{
						OnMessageReceived = context =>
						{
							var accessToken = context.Request.Query["access_token"];

							// If the request is for our hub...
							var path = context.HttpContext.Request.Path;
							if (!string.IsNullOrEmpty(accessToken) &&
								SignalROptions.Hubs.Any(x => path.StartsWithSegments($"/{x}")))
							{
								// Read the token out of the query string
								context.Token = accessToken;
							}

							return Task.CompletedTask;
						}
					};

					options.SaveToken = true;
				});

			services.AddSignalR();

			services.AddHttpContextAccessor();
			services.AddTransient(s => s.GetService<IHttpContextAccessor>().HttpContext.User);

			services.AddControllers();

			HubFactory.Build(SignalROptions.Hubs);
			services.AddSingleton(HubFactory);
			services.AddSingleton(typeof(HubConnectionStore<>));
			services.AddSingleton(typeof(HubGroupStore<>));
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(
			IApplicationBuilder app,
			IWebHostEnvironment env,
			IConfiguration configuration,
			ILogger<Startup> logger)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}

			app.UseCors();

			app.UseRouting();

			app.UseAuthentication();

			app.UseEndpoints(endpoints =>
			{
				endpoints.MapHubNames(HubFactory);

				endpoints.MapControllers();
			});

			// get http url
			var httpEndpoint = configuration["Kestrel:Endpoints:Http:Url"] ?? "https://localhost:5000";

			// get https url
			var httpsEndpoint = configuration["Kestrel:Endpoints:Https:Url"] ?? "https://localhost:5001";

			var connectionString = $"Endpoint={httpsEndpoint ?? httpEndpoint};AccessKey={SignalROptions.AccessKey};Version=1.0;";

			logger.LogInformation($"Azure SignalR Service connection string:");
			logger.LogInformation(connectionString);

			logger.LogInformation($"Following hubs are configured:");
			foreach (var hub in SignalROptions.Hubs)
			{
				logger.LogInformation($"\t{hub}");
			}

			if (SignalROptions.CORS?.Any() ?? false)
			{
				logger.LogInformation($"Following CORS are allowed:");
				foreach (var cors in SignalROptions.CORS)
				{
					logger.LogInformation($"\t{cors}");
				}
			}
		}
	}
}
