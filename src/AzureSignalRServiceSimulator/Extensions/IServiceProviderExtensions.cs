namespace AzureSignalRServiceSimulator.Extensions
{
	using System;

	public static class IServiceProviderExtensions
	{
		public static object GetGenericService(this IServiceProvider serviceProvider, Type genericType, HubFactory hubFactory, string hubName)
		{
			var hubGroupStoreType = genericType.MakeGenericType(hubFactory[hubName]);
			return serviceProvider.GetService(hubGroupStoreType);
		}

		public static TService GetGenericService<TService>(this IServiceProvider serviceProvider, Type genericType, HubFactory hubFactory, string hubName)
			where TService : class
		{
			return serviceProvider.GetGenericService(genericType, hubFactory, hubName) as TService;
		}

		public static TProperty GetGenericServiceProperty<TProperty>(this IServiceProvider serviceProvider, Type genericType, HubFactory hubFactory, string hubName, string propertyName)
			 where TProperty : class
		{
			var service = serviceProvider.GetGenericService(genericType, hubFactory, hubName);
			return service.GetType().GetProperty(propertyName).GetValue(service) as TProperty;
		}
	}
}
