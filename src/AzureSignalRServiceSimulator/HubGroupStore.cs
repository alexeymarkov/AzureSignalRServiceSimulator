namespace AzureSignalRServiceSimulator
{
	using System.Collections.Concurrent;
	using System.Collections.Generic;
	using System.Linq;

	using Microsoft.AspNetCore.SignalR;

	public class HubGroupStore<THub> : IHubGroupStore where THub : Hub
	{
		private ConcurrentDictionary<string, List<string>> _connectionGroups = new ConcurrentDictionary<string, List<string>>();

		public void AddConnectionGroup(string connectionId, string groupName)
		{
			_connectionGroups.AddOrUpdate(
				connectionId,
				key => new[] { groupName }.ToList(),
				(key, list) =>
				{
					list.Add(groupName);
					return list;
				});
		}

		public void RemoveConnectionGroup(string connectionId, string groupName)
		{
			while (true)
			{
				if (!_connectionGroups.TryGetValue(connectionId, out var groupNames))
				{
					break;
				}

				var newGroupNames = groupNames.Where(x => x != groupName).ToList();
				if (newGroupNames.Count > 0)
				{
					if (_connectionGroups.TryUpdate(connectionId, newGroupNames, groupNames))
					{
						break;
					}
				}
				else
				{
					if (_connectionGroups.TryRemove(connectionId, out var temp))
					{
						break;
					}
				}
			}
		}

		public IEnumerable<string> GetConnectionGroups(string connectionId)
		{
			return _connectionGroups.TryGetValue(connectionId, out var groupNames) ? groupNames : Enumerable.Empty<string>();
		}
	}
}
