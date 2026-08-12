using System;
using System.Collections.Generic;
using BestHTTP.SignalR;

namespace Client.Connectivity
{
	public class SignalRNotificationManager
	{
		public static List<long> ReconnectingTimesList = new List<long>();

		public const long RecurrentReconnectTimeThreshold = 60000L;

		public const int RecurrectReconnectingCountThreshold = 10;

		private static long LastTimestampTryingToReconnectShown = 0L;

		private static long ShowUIReconnectingThresholdMs = 20000L;

		public static void NotifyReconnecting(Connection connection, DateTime lastReconnectTime)
		{
			long num = DateTime.UtcNow.Ticks / 10000;
			bool flag = false;
			if (connection != null && connection.Transport != null && connection.NegotiationResult != null)
			{
				int num2;
				if (connection.Transport.SupportsKeepAlive && connection.NegotiationResult.KeepAliveTimeout.HasValue)
				{
					TimeSpan value = DateTime.UtcNow - lastReconnectTime;
					TimeSpan? keepAliveTimeout = connection.NegotiationResult.KeepAliveTimeout;
					num2 = ((value >= keepAliveTimeout) ? 1 : 0);
				}
				else
				{
					num2 = 0;
				}
				flag = (byte)num2 != 0;
			}
			if (LastTimestampTryingToReconnectShown <= 0)
			{
				LastTimestampTryingToReconnectShown = num;
			}
			if (!flag && num - LastTimestampTryingToReconnectShown < ShowUIReconnectingThresholdMs)
			{
				return;
			}
			LastTimestampTryingToReconnectShown = 0L;
			ShowReconnectingUI(blockInputWithBackground: true);
			ReconnectingTimesList.Add(num);
			if (ReconnectingTimesList.Count > 10)
			{
				ReconnectingTimesList.RemoveAt(0);
				long num3 = ReconnectingTimesList[0];
				if (num - num3 < 60000 && GameManager.Instance != null)
				{
					if (AnalyticsManager.instance != null)
					{
						AnalyticsManager.instance.CreateEvent("Connectivity_ReloadingDueToRecurrectReconnects").Send();
					}
					GameManager.Instance.ReloadGame();
				}
			}
			if (PlayerInputManager.Instance != null)
			{
				PlayerInputManager.Instance.IsReconnecting = true;
			}
			if (AnalyticsManager.instance != null)
			{
				AnalyticsManager.instance.CreateEvent("Connectivity_Reconnecting").Send();
				SingularityMonoBehaviour<SDKManager>.Instance.Reload("Connectivity_Reconnecting", "");
			}
		}

		private static void ShowReconnectingUI(bool blockInputWithBackground)
		{
			if (SingularityMonoBehaviour<HUDManager>.Instance != null)
			{
				SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.IngameLoading).Open();
				((IngameLoading)SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.IngameLoading)).SetText(LocalizationManager.GetText("Error.Reconnecting"), blockInputWithBackground);
			}
		}

		public static void NotifyReconnected()
		{
			LastTimestampTryingToReconnectShown = 0L;
			if (SingularityMonoBehaviour<HUDManager>.Instance != null)
			{
				SingularityMonoBehaviour<HUDManager>.Instance.CloseIfExists(UIType.IngameLoading);
			}
			if (PlayerInputManager.Instance != null)
			{
				PlayerInputManager.Instance.IsReconnecting = false;
			}
			if (AnalyticsManager.instance != null)
			{
				AnalyticsManager.instance.CreateEvent("Connectivity_Reconnected").Send();
				SingularityMonoBehaviour<SDKManager>.Instance.Reload("Connectivity_Reconnected", "");
			}
		}

		public static void NotifyDisconnected()
		{
			LastTimestampTryingToReconnectShown = 0L;
			if (GameManager.Instance != null)
			{
				GameManager.Instance.ShowConnectionLost();
			}
		}
	}
}
