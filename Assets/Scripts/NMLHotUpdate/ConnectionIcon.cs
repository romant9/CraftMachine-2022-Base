using Client.Connectivity;
using UnityEngine;

public class ConnectionIcon : MonoBehaviour
{
	[Header("Tints")]
	public Color DisconnectedColor = Color.white;

	public Color DisconnectingColor = Color.white;

	public Color ReconnectingColor = Color.white;

	public Color ConnectingColor = Color.white;

	public Color ConnectedColor = Color.white;

	[Space(10f)]
	public Color OfflineModeColor = Color.white;

	[Header("Sprite")]
	public UISprite sprite;

	private bool init;

	private SignalRClientState currentState;

	private void Update()
	{
		if (init && SignalRClient.Instance.State == currentState)
		{
			return;
		}
		currentState = SignalRClient.Instance.State;
		if (sprite != null)
		{
			Color color = Color.white;
			switch (currentState)
			{
			case SignalRClientState.Disconnected:
				color = DisconnectedColor;
				break;
			case SignalRClientState.Disconnecting:
				color = DisconnectingColor;
				break;
			case SignalRClientState.Reconnecting:
				color = ReconnectingColor;
				break;
			case SignalRClientState.Connecting:
				color = ConnectingColor;
				break;
			case SignalRClientState.Connected:
				color = ConnectedColor;
				break;
			}
			if (GameConfiguration.Instance.Config.OnlineLevel == BuildGameConfiguration.OnlineLevelType.Offline)
			{
				color = OfflineModeColor;
			}
			sprite.color = color;
			init = true;
		}
	}
}
