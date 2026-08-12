namespace Client.Connectivity
{
	public enum SignalRClientState
	{
		Disconnected = 0,
		Disconnecting = 1,
		Reconnecting = 2,
		Connecting = 3,
		Connected = 4
	}
}
