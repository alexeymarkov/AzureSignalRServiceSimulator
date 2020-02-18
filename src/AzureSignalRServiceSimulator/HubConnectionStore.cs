namespace AzureSignalRServiceSimulator
{
	using System.Collections.Concurrent;
	using System.Collections.Generic;
	using System.Linq;

	using Microsoft.AspNetCore.SignalR;

	public class HubConnectionStore<THub> : IHubConnectionStore where THub : Hub
	{
		private ConcurrentDictionary<string, List<string>> _userConnections = new ConcurrentDictionary<string, List<string>>();

		public void AddUserConnection(string userId, string connectionId)
		{
			_userConnections.AddOrUpdate(
				userId,
				key => new[] { connectionId }.ToList(),
				(key, list) =>
				{
					list.Add(connectionId);
					return list;
				});
		}

		public void RemoveUserConnection(string userId, string connectionId)
		{
			while (true)
			{
				if (!_userConnections.TryGetValue(userId, out var connections))
				{
					break;
				}

				var newConnections = connections.Where(x => x != connectionId).ToList();
				if (newConnections.Count > 0)
				{
					if (_userConnections.TryUpdate(userId, newConnections, connections))
					{
						break;
					}
				}
				else
				{
					if (_userConnections.TryRemove(userId, out var temp))
					{
						break;
					}
				}
			}
		}

		public IEnumerable<string> GetUserConnections(string userId)
		{
			return _userConnections.TryGetValue(userId, out var connections) ? connections : Enumerable.Empty<string>();
		}
	}
}
