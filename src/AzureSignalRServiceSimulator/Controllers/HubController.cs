namespace AzureSignalRServiceSimulator.Controllers
{
	using System;
	using System.Linq;
	using System.Threading.Tasks;

	using AzureSignalRServiceSimulator.Extensions;

	using Microsoft.AspNetCore.Mvc;
	using Microsoft.AspNetCore.SignalR;

	/// <summary>
	/// Implementation of https://github.com/Azure/azure-signalr/blob/dev/docs/rest-api.md#api
	/// </summary>
	[Route("/api/v1/hubs")]
	[ApiController]
	public class HubController : ControllerBase
	{
		public class SignalRMessage
		{
			public string Target { get; set; }
			public object[] Arguments { get; set; }
		}

		private readonly IServiceProvider _serviceProvider;
		private readonly HubFactory _hubFactory;

		public HubController(
			IServiceProvider serviceProvider,
			HubFactory hubFactory)
		{
			_serviceProvider = serviceProvider;
			_hubFactory = hubFactory;
		}

		[HttpPost("{hubName}")]
		public async Task SendToAll(string hubName, [FromBody]SignalRMessage message)
		{
			await GetClients(hubName).All.SendCoreAsync(message.Target, message.Arguments);
		}

		[HttpPost("{hubName}/groups/{groupName}")]
		public async Task SendToGroup(string hubName, string groupName, [FromBody]SignalRMessage message)
		{
			await GetClients(hubName).Group(groupName).SendCoreAsync(message.Target, message.Arguments);
		}

		[HttpPost("{hubName}/users/{userId}")]
		public async Task SendToUser(string hubName, string userId, [FromBody]SignalRMessage message)
		{
			await GetClients(hubName).User(userId).SendCoreAsync(message.Target, message.Arguments);
		}

		[HttpPut("{hubName}/groups/{groupName}/connections/{connectionId}")]
		[HttpPut("{hubName}/connections/{connectionId}/groups/{groupName}")]
		public async Task AddConnectionToGroup(string hubName, string groupName, string connectionId)
		{
			await GetGroups(hubName).AddToGroupAsync(connectionId, groupName);
			GetHubGroupStore(hubName).AddConnectionGroup(connectionId, groupName);
		}

		[HttpDelete("{hubName}/groups/{groupName}/connections/{connectionId}")]
		[HttpDelete("{hubName}/connections/{connectionId}/groups/{groupName}")]
		public async Task RemoveConnectionFromGroup(string hubName, string groupName, string connectionId)
		{
			await GetGroups(hubName).RemoveFromGroupAsync(connectionId, groupName);
			GetHubGroupStore(hubName).RemoveConnectionGroup(connectionId, groupName);
		}

		[HttpPut("{hubName}/groups/{groupName}/users/{userId}")]
		[HttpPut("{hubName}/users/{userId}/groups/{groupName}")]
		public async Task AddUserToGroup(string hubName, string groupName, string userId)
		{
			foreach (var connectionId in GetHubConnectionStore(hubName).GetUserConnections(userId))
			{
				await AddConnectionToGroup(hubName, groupName, connectionId);
			}
		}

		[HttpDelete("{hubName}/groups/{groupName}/users/{userId}")]
		[HttpDelete("{hubName}/users/{userId}/groups/{groupName}")]
		public async Task RemoveUserFromGroup(string hubName, string groupName, string userId)
		{
			foreach (var connectionId in GetHubConnectionStore(hubName).GetUserConnections(userId))
			{
				await RemoveConnectionFromGroup(hubName, groupName, connectionId);
			}
		}

		[HttpDelete("{hubName}/users/{userId}/groups")]
		public async Task RemoveUserFromAllGroups(string hubName, string userId)
		{
			var groupNames = GetHubConnectionStore(hubName).GetUserConnections(userId).SelectMany(connectionId => GetHubGroupStore(hubName).GetConnectionGroups(connectionId).Select(x => (connectionId, x)));
			foreach (var (connectionId, groupName) in groupNames)
			{
				await AddConnectionToGroup(hubName, groupName, connectionId);
			}
		}

		private IHubConnectionStore GetHubConnectionStore(string hubName) => _serviceProvider.GetGenericService<IHubConnectionStore>(typeof(HubConnectionStore<>), _hubFactory, hubName);

		private IHubClients GetClients(string hubName) => _serviceProvider.GetGenericServiceProperty<IHubClients>(typeof(IHubContext<>), _hubFactory, hubName, "Clients");

		private IGroupManager GetGroups(string hubName) => _serviceProvider.GetGenericServiceProperty<IGroupManager>(typeof(IHubContext<>), _hubFactory, hubName, "Groups");

		private IHubGroupStore GetHubGroupStore(string hubName) => _serviceProvider.GetGenericService<IHubGroupStore>(typeof(HubGroupStore<>), _hubFactory, hubName);
	}
}
