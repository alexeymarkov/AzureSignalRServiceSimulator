namespace AzureSignalRServiceSimulator
{
	using System;
	using System.Threading.Tasks;

	using AzureSignalRServiceSimulator.Extensions;

	using Microsoft.AspNetCore.SignalR;

	public abstract class BaseHub : Hub
	{
		private readonly IServiceProvider _serviceProvider;
		private readonly HubFactory _hubFactory;

		protected BaseHub(IServiceProvider serviceProvider, HubFactory hubFactory)
		{
			_serviceProvider = serviceProvider;
			_hubFactory = hubFactory;
		}

		protected string HubName => GetType().Name;

		public override Task OnConnectedAsync()
		{
			HubConnectionStore.AddUserConnection(Context.UserIdentifier, Context.ConnectionId);
			return base.OnConnectedAsync();
		}

		public override Task OnDisconnectedAsync(Exception exception)
		{
			HubConnectionStore.RemoveUserConnection(Context.UserIdentifier, Context.ConnectionId);
			return base.OnDisconnectedAsync(exception);
		}

		private IHubConnectionStore HubConnectionStore => _serviceProvider.GetGenericService<IHubConnectionStore>(typeof(HubConnectionStore<>), _hubFactory, HubName);
	}
}
