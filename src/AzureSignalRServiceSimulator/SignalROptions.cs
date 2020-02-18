namespace AzureSignalRServiceSimulator
{
	using System;

	public class SignalROptions
	{
		public string AccessKey { get; set; }
		public string[] CORS { get; set; }
		public string[] Hubs { get; set; }

		public void Validate()
		{
			if (string.IsNullOrEmpty(AccessKey))
			{
				throw new Exception("AccessKey must be specified and cannot be empty.");
			}

			if (Hubs == null || Hubs.Length == 0)
			{
				throw new Exception("Hubs must be specified and must contain at least one hub name.");
			}
		}
	}
}
