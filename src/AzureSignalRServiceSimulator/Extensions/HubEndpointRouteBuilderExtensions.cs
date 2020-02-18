namespace AzureSignalRServiceSimulator.Extensions
{
	using Microsoft.AspNetCore.Builder;
	using Microsoft.AspNetCore.Routing;

	public static class HubEndpointRouteBuilderExtensions
	{
		public static void MapHubNames(this IEndpointRouteBuilder endpoints, HubFactory hubFactory)
		{
			foreach (var hubname in hubFactory.HubNames)
			{
				endpoints.MapHubName(hubFactory, hubname);
			}
		}

		public static HubEndpointConventionBuilder MapHubName(this IEndpointRouteBuilder endpoints, HubFactory hubFactory, string hubName)
		{
			var method = typeof(Microsoft.AspNetCore.Builder.HubEndpointRouteBuilderExtensions).GetMethod("MapHub", new[] { typeof(IEndpointRouteBuilder), typeof(string) });
			var generic = method.MakeGenericMethod(hubFactory[hubName]);
			return generic.Invoke(null, new object[] { endpoints, $"/{hubName}" }) as HubEndpointConventionBuilder;

			//return Microsoft.AspNetCore.Builder.HubEndpointRouteBuilderExtensions.MapHub<TemplateHub>(endpoints, $"/{hubName}");
		}
	}
}
