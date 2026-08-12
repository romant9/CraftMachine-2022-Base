using System;
using System.Collections.Generic;
using BaseModel;
using Client.Connectivity;
using TWDModel;

public class TWDModelCommandTransport : IModelCommandTransport
{
	protected SignalRClient signalr;

	protected MessageSerializer jsonSerializer;

	private Dictionary<Type, bool> HasWaitForResponseAttribute;

	public TWDModelCommandTransport()
	{
		signalr = SignalRClient.Instance;
		jsonSerializer = new MessageSerializer();
		HasWaitForResponseAttribute = new Dictionary<Type, bool>();
	}

	private bool WaitForResponse(Type commandType)
	{
		if (!HasWaitForResponseAttribute.ContainsKey(commandType))
		{
			bool value = false;
			object[] customAttributes = commandType.GetCustomAttributes(inherit: true);
			for (int i = 0; i < customAttributes.Length; i++)
			{
				if (customAttributes[i] is WaitForResponseAttribute)
				{
					value = true;
					break;
				}
			}
			HasWaitForResponseAttribute.Add(commandType, value);
		}
		return HasWaitForResponseAttribute[commandType];
	}

	public bool Send(IModelCommand command)
	{
		string arg = jsonSerializer.Serialize(command);
		if (signalr.IsConnected)
		{
			bool waitForResponse = command != null && WaitForResponse(command.GetType());

			if (signalr.CurrentSessionToken == null)
			{
				if (OfflineManager.IsOfflineMode)
				{
					DebugTWD.LogMycode("if (OfflineManager.IsOfflineMod)");
					var cmd = new OfflineCommandItem(arg, command.GetType().ToString(), command, waitForResponse);
					OfflineManager.Instance.OfflineCommandItems.Add(cmd);
					return true;
				}
				Debug.LogWarning($"Trying to send model command {command.GetType()} when no session has yet been initialized.");
				return false;
			}
			signalr.RequestCommand("Command", arg, command.GetType().ToString(), null, command, waitForResponse);
		}
		return true;
	}
}
