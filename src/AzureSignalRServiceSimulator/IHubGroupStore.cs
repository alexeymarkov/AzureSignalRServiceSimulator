namespace AzureSignalRServiceSimulator
{
	using System.Collections.Generic;

	public interface IHubGroupStore
	{
		void AddConnectionGroup(string connectionId, string groupName);
		IEnumerable<string> GetConnectionGroups(string connectionId);
		void RemoveConnectionGroup(string connectionId, string groupName);
	}
}