namespace AzureSignalRServiceSimulator
{
	using System.Collections.Generic;

	public interface IHubConnectionStore
	{
		void AddUserConnection(string userId, string connectionId);
		IEnumerable<string> GetUserConnections(string userId);
		void RemoveUserConnection(string userId, string connectionId);
	}
}