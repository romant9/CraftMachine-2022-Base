using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using BaseModel;
using BestHTTP;
using BestHTTP.SignalR;
using BestHTTP.SignalR.Hubs;
using BestHTTP.SignalR.Messages;
using UnityEngine;
using TwdCustomMod;

namespace Client.Connectivity
{
	public class SignalRClient : MonoBehaviour
	{
		public enum SignalRClientLogLevel
		{
			Trace = 0,
			Normal = 1,
			Critical = 2
		}

		protected class Command
		{
			public string Method { get; protected set; }

			public string Arg1 { get; protected set; }

			public string Arg2 { get; protected set; }

			public SignalREventHandler Handler { get; protected set; }

			public int SequenceId { get; set; }

			public long SendTimeTicks { get; set; }

			public bool WaitForResponse { get; set; }

			public Command(string method, string arg1, string arg2, SignalREventHandler handler, bool waitForResponse)
			{
				Method = method;
				Arg1 = arg1;
				Arg2 = arg2;
				Handler = handler;
				WaitForResponse = waitForResponse;
			}

			public Command(string method, string arg1, string arg2, SignalREventHandler handler, ModelCommand command, bool waitForResponse)
			{
				Method = method;
				Arg1 = arg1;
				Arg2 = arg2;
				Handler = handler;
				SequenceId = command.SequenceId;
				WaitForResponse = waitForResponse;
			}
		}

		public static SignalRClientLogLevel LogLevel = SignalRClientLogLevel.Normal;

		public static bool ENABLE_INVOKE_LOG = true;

		public const int MAX_COMMAND_BATCH_SIZE = 1638400;

		public float WaitingForConnectionTimeout = 60f;

		public float InitialConnectTimeout = 15f;

		public float CommandTimeout = 60f;

		public string HubName = "DrillHub";

		public float UserInactivityDisconnectTime = 240f;

		public float ReconnectTimeout = 30f;

		public float ReconnectInterval = 5f;

		public float HighLatencyThreshold = 2f;

		private string currentSessionToken;

		public SignalRClientStatistics Statistics;

		private Connection connection;

		private Hub currentHub;

		private Uri currentUrl;

		private bool disconnectRequested;

		public SignalRClientState state;

		private List<InvokeLogEntry> InvokeLog;

		private List<Command> commandQueue;

		private List<Command> sentCommandQueue;

		private float responseTimeout;

		private DateTime lastConnectedTime;

		private DateTime lastReconnectTime;

		private SignalREventHandler connectionRequestedHandler;

		private static List<string> GroupServiceMethodNames = new List<string> { "CreateGroup", "GetGroups", "GetGroupInfo", "LoadGroups", "SyncGroup", "SearchGroups" }; //TryGetGroupInfo

		public static SignalRClient Instance { get; protected set; }

		public string CurrentSessionToken => currentSessionToken;

		public string CurrentHostPort { get; private set; }

		public string CurrentHubName { get; private set; }

		public SignalRClientState State
		{
			get
			{
				return state;
			}
			private set
			{
				if (state != value)
				{
					Log("State changed (old = " + state.ToString() + ", new = " + value, SignalRClientLogLevel.Trace);
					state = value;
				}
			}
		}

		public bool IsConnected
		{
			get
			{
				if (State != SignalRClientState.Disconnected)
				{
					return State != SignalRClientState.Disconnecting;
				}
				return false;
			}
		}

		public bool IsReconnecting => State == SignalRClientState.Reconnecting;

		public bool HasConnectivityIssues
		{
			get
			{
				if (State != SignalRClientState.Reconnecting)
				{
					return Statistics.AverageRTT > (long)(HighLatencyThreshold * 1000f);
				}
				return true;
			}
		}

		public bool HasError => Statistics.LastErrorType != ErrorType.None;

		public string LastErrorMessage => Statistics.LastError;

		public bool IsWaitingForResponse
		{
			get
			{
				if (sentCommandQueue != null)
				{
					return sentCommandQueue.Count > 0;
				}
				return false;
			}
		}

		private TimeSpan TimeSinceLastConnected => DateTime.UtcNow - lastConnectedTime;

		private TimeSpan TimeSinceLastReconnect => DateTime.UtcNow - lastReconnectTime;

		public bool IsWaitingForBlockingCommandResponse
		{
			get
			{
				if (sentCommandQueue != null && sentCommandQueue.Count > 0 && sentCommandQueue[sentCommandQueue.Count - 1].WaitForResponse)
				{
					return true;
				}
				return false;
			}
		}

		public event ServerMessage OnServerMessage;

		public event ServerMessage OnSocialMessage;

		public event ServerMessage OnHubConnectionMessage;

		public event ServerMessage OnBananaMessage;

		public event ServerMessage OnWorldBossBaseSnapshotMessage;

		public event ServerMessage OnWorldBossFullSnapshotMessage;

		public event ServerMessage OnBuySubscriptionMessage;

		public event ServerMessage OnGuildBattleHighScoresMessage;

		public event CommandCompletedMessageHandler OnCommandCompletedMessage;

		public override string ToString()
		{
			return "SignalRClient State = " + State.ToString() + ", Statistics = " + Statistics;
		}

		public static void Log(string text, SignalRClientLogLevel logLevel = SignalRClientLogLevel.Normal)
		{
			_ = LogLevel;
		}

		public static void LogError(string text, SignalRClientLogLevel logLevel = SignalRClientLogLevel.Critical)
		{
			if (logLevel >= LogLevel)
			{
				Debug.LogError("### SignalRClient ### -- " + text.Substring(0, Math.Min(1024, text.Length)) + "\nSignalRClient " + Instance);
			}
		}

		protected void SetLastError(string inLastError, ErrorType inLastErrorType = ErrorType.None)
		{
			Statistics.LastError = inLastError;
			Statistics.LastErrorType = inLastErrorType;
			if (inLastErrorType == ErrorType.CommandExecution)
			{
				Statistics.HasCommandExecutionError = true;
			}
			if (inLastError == null)
			{
				Statistics.HasCommandExecutionError = false;
			}
			if (Statistics.LastErrorType != ErrorType.None && !string.IsNullOrEmpty(Statistics.LastError))
			{
				LogError(Statistics.LastError);
			}
		}

		public List<InvokeLogEntry> GetInvokeLog()
		{
			return InvokeLog;
		}

		public IEnumerator OnApplicationPause(bool paused)
		{
			if (!paused)
			{
				yield return null;
				yield return null;
				Log("Resuming...", SignalRClientLogLevel.Trace);
			}
		}

		protected bool OnBestHTTPApplicationQuit()
		{
			if (State != SignalRClientState.Disconnected)
			{
				Disconnect();
				ClearError();
			}
			return true;
		}

		private void Awake()
		{
			if (!this.enabled) return;
			Init();
		}

		public void Init()
		{
			this.enabled = true;
			if (Instance != null)
			{
				Log("Duplicate instance of SignalRClient, destroying owning gameobject!");
				return;
			}
			DebugTWD.LogMycode("SignalR Awake");
			UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
			Instance = this;
			LogLevel = GameConfiguration.Instance.Config.SignalRLogLevel;
			Statistics = new SignalRClientStatistics();
			commandQueue = new List<Command>();
			sentCommandQueue = new List<Command>();
			Statistics.Clear();
			disconnectRequested = false;
			HTTPUpdateDelegator.OnBeforeApplicationQuit = OnBestHTTPApplicationQuit;
			IsInited = true;
		}

		private void NotifyServerMessage(string message, string type)
		{
			this.OnServerMessage?.Invoke(message, type);
		}

		private void Update()
		{
			if (Instance != this)
			{
				Log("Update received on duplicate SignalRClient instance", SignalRClientLogLevel.Trace);
				return;
			}
			if (disconnectRequested)
			{
				Disconnect();
			}
			if (State == SignalRClientState.Connected)
			{
				lastConnectedTime = DateTime.UtcNow;
				IsConnectedState = true;
			}
			else
			{
				IsConnectedState = false;
			}
			if (IsWaitingForResponse && responseTimeout > 0f)
			{
				responseTimeout -= ((Time.timeScale > 0f) ? (Time.deltaTime / Time.timeScale) : 0f);
				if (responseTimeout <= 0f)
				{
					if (HasCommandInSentQueue("Connect"))
					{
						return;
					}
					SetLastError("SignalR command response timeout!", ErrorType.Timeout);
					string text = null;
					if (sentCommandQueue.Count > 0)
					{
						Command command = sentCommandQueue[0];
						text = GenerateCommandDescription(command.Method, command.Arg1, command.Arg2);
					}
					else
					{
						text = "null";
					}
					NotifyServerMessage(text, "timeout");
				}
			}
			if (State == SignalRClientState.Reconnecting)
			{
				double value = (double)Statistics.AverageRTT / 1000.0 + (double)ReconnectInterval;
				if (TimeSinceLastReconnect > TimeSpan.FromSeconds(value))
				{
					TryReconnect();
				}
			}
			float num = ((State == SignalRClientState.Connecting) ? InitialConnectTimeout : WaitingForConnectionTimeout);
			if (State != SignalRClientState.Disconnected && TimeSinceLastConnected > TimeSpan.FromSeconds(num))
			{
				SetLastError("SignalR connection timeout", ErrorType.Connectivity);
				NotifyServerMessage("Connect", "timeout");

				if (IsLoadDataManager)
				{
					lastConnectedTime = DateTime.UtcNow;
					DebugTWD.LogMycode("if (IsLoadDataManager)");
					ClearError();
					TryReconnect();

					if (DataManager.Instance.language != DataManager.Language.Ru)
						MyTools.UpdateLogPanel("Connection timeout. Internet speed is very low. Try Connect again");
					else
						MyTools.UpdateLogPanel("Время соединения истекло. Медленный интернет. Попробуйте подключиться снова.");
					//string message = CustomLocalization.GetText("ConnectionTimeout");
					//MyTools.UpdateLogPanel(message);
					SingularityMonoBehaviour<HUDManager>.Instance.CloseIfExists(UIType.IngameLoading);
				}
			}
		}

		public void Connect(string url, SignalREventHandler handler)
		{
			Messages = new List<string>();

			Log("Connect requested", SignalRClientLogLevel.Trace);
			disconnectRequested = false;
			if (State != SignalRClientState.Disconnected)
			{
				Disconnect();
			}
			ClearError();
			lastConnectedTime = DateTime.UtcNow;
			ClearQueues();
			connectionRequestedHandler = handler;
			State = SignalRClientState.Connecting;
			CurrentHostPort = url;
			InternalConnect(url, HubName);
		}

		private void ClearQueues()
		{
			commandQueue.Clear();
			sentCommandQueue.Clear();
		}

		public void Disconnect()
		{
			Log("Disconnect");
			State = SignalRClientState.Disconnected;
			DisconnectHubConnection();
			ClearQueues();
			currentSessionToken = null;
		}

		public void ClearError()
		{
			SetLastError(null);
		}

		public void RequestCommand(string method, SignalREventHandler handler, bool waitForResponse)
		{
			RequestCommand(method, null, null, handler, null, waitForResponse);
		}

		public void RequestCommand(string method, string arg1, SignalREventHandler handler, bool waitForResponse)
		{
			RequestCommand(method, arg1, null, handler, null, waitForResponse);
		}

		private string GenerateCommandDescription(string method, string arg1, string arg2)
		{
			if (method == null)
			{
				method = "";
			}
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("(");
			stringBuilder.Append(method);
			if (!string.IsNullOrEmpty(arg1))
			{
				stringBuilder.Append(", ");
				stringBuilder.Append(arg1);
			}
			if (!string.IsNullOrEmpty(arg2))
			{
				stringBuilder.Append(", ");
				stringBuilder.Append(arg2);
			}
			stringBuilder.Append(")");
			return stringBuilder.ToString();
		}

		public void RequestCommand(string method, string arg1, string arg2, SignalREventHandler handler, IModelCommand command, bool waitForResponse)
		{
			Log("RequestCommand " + GenerateCommandDescription(method, arg1, arg2), SignalRClientLogLevel.Trace);
			if (command == null)
			{
				commandQueue.Add(new Command(method, arg1, arg2, handler, waitForResponse));
			}
			else
			{
				commandQueue.Add(new Command(method, arg1, arg2, handler, (ModelCommand)command, waitForResponse));
			}
			if (!IsWaitingForResponse)
			{
				SendNextCommand();
			}
		}

		private bool HasCommandInSentQueue(string methodName)
		{
			if (sentCommandQueue != null)
			{
				string text = methodName.ToLowerInvariant();
				for (int i = 0; i < sentCommandQueue.Count; i++)
				{
					if (sentCommandQueue[i].Method.ToLowerInvariant() == text)
					{
						return true;
					}
				}
			}
			return false;
		}

		[ContextMenu("TryReconnect")]
		public void TryReconnect()
		{
			Log("TryReconnect");
			Statistics.ReconnectCount++;

			CallCountBase.Instance?.Show_Reconnect_Error(Statistics.ReconnectCount);

			if (OfflineManager.IsIgnoreReconnect)
			{
				DebugTWD.LogMycode("if (OfflineManager.IsIgnoreReconnect)");
				Log("Attempting to reconnect");
				lastReconnectTime = DateTime.UtcNow;
				InternalConnect(CurrentHostPort, CurrentHubName);
				DebugTWD.LogWarning("TryReconnect", DebugType.SignalR);
			}
			else
			if (TimeSinceLastConnected < TimeSpan.FromSeconds(ReconnectTimeout) && TimeSinceLastConnected < TimeSpan.FromSeconds(WaitingForConnectionTimeout))
			{
				Log("Attempting to reconnect");
				lastReconnectTime = DateTime.UtcNow;
				InternalConnect(CurrentHostPort, CurrentHubName);
			}
			else
			{
				Log("No time left, disconnect");
				SetLastError("Reconnection attempts failed, disconnected from server", ErrorType.Connectivity);
				Disconnect();
				SignalRNotificationManager.NotifyDisconnected();
			}
		}

		public void OnMethodResult(Hub hub, ClientMessage originalMessage, ResultMessage result)
		{
			string text = "";
			if (originalMessage.Method == "ExecuteCommands" && sentCommandQueue.Count > 0)
			{
				string[] array = originalMessage.Args[0] as string[];
				string[] array2 = originalMessage.Args[1] as string[];
				List<Command> list = new List<Command>();
				for (int i = 0; i < array.Length; i++)
				{
					for (int j = 0; j < sentCommandQueue.Count; j++)
					{
						Command command = sentCommandQueue[j];
						if (!(command.Method == "Command") || !(command.Arg1 == array[i]) || !(command.Arg2 == array2[i]))
						{
							continue;
						}
						if (result.ReturnValue is IDictionary<string, object> dictionary)
						{
							try
							{
								int responseCode = Convert.ToInt32(dictionary["Code"]);
								int sequenceId = Convert.ToInt32(dictionary["SequenceId"]);
								string message = (dictionary.ContainsKey("Message") ? (dictionary["Message"] as string) : null);
								if (OfflineManager.IsIgnoreResponseNotOK)
								{
									DebugTWD.LogMycode("if (OfflineManager.IsIgnoreResponseNotOK)");
									DebugTWD.LogWarning($"ResponseCode is: {responseCode}, ({dictionary["Code"]})", DebugType.SignalR);
									if (responseCode == 1 || responseCode == 25 || responseCode == 26 || responseCode == 27 || responseCode == 17)
									{
										responseCode = 0;
										message = null;
									}
								}
								command.Handler?.Invoke(message);
								NotifyCommandCompleted(responseCode, sequenceId);
								if (!IsLoadDataManager)
								{
									DebugTWD.LogMycode("if (IsLoadDataManager) ... StartPhoneCallCommand or RerollPhoneCallCommand");
									if (command.Arg2 == "TWDModel.StartPhoneCallCommand")
									{
										GameManager.Instance.PhoneCallResponseReceived = true;
										UIEvent.Send("StartPhoneCallCommandResponseReceived");
									}
									else if (command.Arg2 == "TWDModel.RerollPhoneCallCommand")
									{
										GameManager.Instance.PhoneCallResponseReceived = true;
										UIEvent.Send("RerollPhoneCallCommandResponseReceived");
									}
									else if (command.Arg2 == "TWDModel.RouletteDrawCommand")
									{
										UIEvent.Send("RouletteDrawCommandResponseReceived");
									}
									else if (command.Arg2 == "TWDModel.RouletteMultiDrawCommand")
									{
										UIEvent.Send("RouletteMultiDrawCommandResponseReceived");
									}
								}
							}
							catch (Exception ex)
							{
								SetLastError("Failed to parse return value, exception message = '" + ex.Message + "'!", ErrorType.CommandExecution);
								return;
							}
						}
						list.Add(command);
						text = text + ((i > 0) ? ", " : "") + command.Arg2;
						Statistics.SetLastRTT((DateTime.UtcNow.Ticks - command.SendTimeTicks) / 10000);
					}
				}
				for (int k = 0; k < list.Count; k++)
				{
					sentCommandQueue.Remove(list[k]);
				}
			}
			Log("OnMethodResult(" + text + ") with " + originalMessage.Method + " RPC call");
			if (!IsWaitingForResponse && commandQueue.Count > 0)
			{
				SendNextCommand();
			}
		}

		public void OnMethodFailed(Hub hub, ClientMessage originalMessage, FailureMessage error)
		{
			Log("OnMethodFailed(" + originalMessage.Method + ") , args = " + originalMessage.Args?.ToString() + ". Error message = " + error.ErrorMessage);
			SetLastError(error.ErrorMessage, ErrorType.CommandExecution);
		}

		private void NotifyCommandCompleted(int responseCode, int sequenceId)
		{
			this.OnCommandCompletedMessage?.Invoke(responseCode, sequenceId);
		}

		public void CompleteCommand(string message, string result)
		{
			Log("CompleteCommand(" + message + ", " + result + ")", SignalRClientLogLevel.Trace);
			if (sentCommandQueue.Count > 0)
			{
				int num = -1;
				Command command = null;
				for (int i = 0; i < sentCommandQueue.Count; i++)
				{
					if (sentCommandQueue[i].Method.Equals(message, StringComparison.InvariantCultureIgnoreCase))
					{
						command = sentCommandQueue[i];
						num = i;
						break;
					}
				}
				if (num >= 0)
				{
					sentCommandQueue.RemoveAt(num);
				}
				if (command != null)
				{
					Statistics.SetLastRTT((DateTime.UtcNow.Ticks - command.SendTimeTicks) / 10000);
					command.Handler?.Invoke(result);
				}
			}
			else if (result != null)
			{
				LogError("Unknown message received: " + result);
			}
			if (!IsWaitingForResponse && commandQueue.Count > 0)
			{
				SendNextCommand();
			}
		}

		protected void SendNextCommand()
		{
			if (IsWaitingForResponse)
			{
				return;
			}
			List<string> list = null;
			List<string> list2 = null;
			int num = 0;
			while (commandQueue.Count > 0)
			{
				Command command = commandQueue[0];
				if (list != null && list.Count > 0 && command.Method != "Command")
				{
					break;
				}
				commandQueue.RemoveAt(0);
				long lastSendTimeTicks = (command.SendTimeTicks = DateTime.UtcNow.Ticks);
				Statistics.LastSendTimeTicks = lastSendTimeTicks;
				responseTimeout = GetTimeout(command.Method);
				Log($"Sending command with timeout {responseTimeout}s: {command.Method}({command.Arg1}, {command.Arg2})");
				if (command.Method == "Command")
				{
					if (list == null)
					{
						list = new List<string>();
					}
					if (list2 == null)
					{
						list2 = new List<string>();
					}
					if (command.Arg1 != null && command.Arg2 != null)
					{
						list.Add(command.Arg1);
						list2.Add(command.Arg2);
						num += command.Arg1.Length + command.Arg2.Length;
					}
				}
				else
				{
					RemoteInvoke(command.Method, command.Arg1, command.Arg2);
				}
				sentCommandQueue.Add(command);
				if (command.WaitForResponse || num > 1638400)
				{
					break;
				}
			}
			if (list != null && list.Count > 0)
			{
				ExecuteCommands(OnMethodResult, OnMethodFailed, list, list2);
			}
		}

		protected float GetTimeout(string method)
		{
			if (method == "Command")
			{
				float num = CommandTimeout;
				if (GameManager.Instance != null && GameManager.Instance.gameEconomyData != null && GameManager.Instance.gameEconomyData.ConfigData != null)
				{
					num = GameManager.Instance.gameEconomyData.ConfigData.ReloadTimer;
				}
				if (!(num > 0f))
				{
					return CommandTimeout;
				}
				return num;
			}
			if (method == "Reconnect")
			{
				return ReconnectTimeout;
			}
			return WaitingForConnectionTimeout;
		}

		private void InternalConnect(string hostPort, string hubName)
		{
			Log("Connect(" + hostPort + ", " + hubName + ")");
			CurrentHostPort = hostPort;
			CurrentHubName = hubName;
			HTTPManager.UseAlternateSSLDefaultValue = true;
			currentUrl = new Uri(hostPort + "/signalr");

			if (IsLoadDataManager && OfflineManager.Instance.IsGedLoaded && (OfflineManager.Instance.IsReconnectPlayerState || OfflineManager.Instance.IsReconnectByCode))
			{
				DebugTWD.Log("InternalConnect : " + hostPort + "  " + hubName + " " + currentUrl, DebugType.SignalR);
				DebugTWD.LogMycode("if (IsLoadDataManager && OfflineManager.Instance.IsGedLoaded && (OfflineManager.Instance.IsReconnectPlayerState || OfflineManager.Instance.IsReconnectByCode))");
				IsOnlyGedData = false;
				IsOnlyGetImagesData = false;
			}
			if (connection != null)
			{
				DisconnectHubConnection();
			}
			ConnectHubConnection(hubName);
		}

		private void DisconnectHubConnection()
		{
			if (!IsLoadDataManager)
			{
				if (CommandHelper.Instance && CommandHelper.Instance.IsUseCustomID) IsOnlyGedData = true;
				IsOnlyGetImagesData = true;
				IsOnlyGetPlayersData = true;
			}
			Log("DisconnectHubConnection", SignalRClientLogLevel.Trace);
			disconnectRequested = false;
			if (currentHub != null)
			{
				if (IsOnlyGetPlayersData)
				{
					currentHub.Off("LoadPlayer");
					currentHub.Off("LoadRemoteContent");
				}
				if (IsOnlyGetImagesData)
				{
					currentHub.Off("ModelMessage");
					currentHub.Off("LoadContent");
				}
				if (IsOnlyGedData)
				{
					currentHub.Off("LoadGed");
				}
				currentHub.Off("RequestDisconnect");
				if (!IsLoadDataManager)
				{
					currentHub.Off("Warning");
				}
			}
			if (connection != null)
			{
				connection.OnConnected -= OnConnected;
				connection.OnReconnecting -= OnReconnecting;
				connection.OnReconnected -= OnConnected;
				connection.OnClosed -= OnDisconnected;
				connection.OnError -= OnError;
				connection.Close();
			}
			connection = null;
			currentHub = null;
		}

		protected void ConnectHubConnection(string hubName)
		{
			if (!IsLoadDataManager)
			{
				if (CommandHelper.Instance && CommandHelper.Instance.IsUseCustomID) IsOnlyGedData = true;
				IsOnlyGetImagesData = true;
				IsOnlyGetPlayersData = true;
			}
			Log("ConnectHubConnection(" + currentUrl?.ToString() + ", " + hubName + ")", SignalRClientLogLevel.Trace);
			if (currentHub == null)
			{
				currentHub = new Hub(hubName);
				if (IsOnlyGetPlayersData)
				{
					currentHub.On("LoadPlayer", OnLoadPlayer);
					currentHub.On("LoadRemoteContent", OnLoadRemoteContent);
				}
				if (IsOnlyGetImagesData)
				{
					currentHub.On("ModelMessage", OnModelMessage);
					currentHub.On("LoadContent", OnLoadContent);
				}
				if (IsOnlyGedData)
				{
					currentHub.On("LoadGed", OnLoadGed);
				}
				currentHub.On("RequestDisconnect", OnHubRequestDisconnect);
				if (!IsLoadDataManager)
				{
					currentHub.On("Warning", OnHubWarning);
				}
			}
			connection = new Connection(currentUrl, currentHub);
			connection.OnConnected += OnConnected;
			connection.OnReconnecting += OnReconnecting;
			connection.OnReconnected += OnConnected;
			connection.OnClosed += OnDisconnected;
			connection.OnError += OnError;
			connection.Open();
			DebugTWD.Log("Open SignalR Connection: ");
		}

		public void ExecuteCommands(OnMethodResultDelegate onResult, OnMethodFailedDelegate onFailed, List<string> commandJsons, List<string> commandTypes)
		{
			Log("ExecuteCommands, count = " + commandJsons.Count, SignalRClientLogLevel.Trace);
			if (currentHub == null)
			{
				LogError("BestSignalR.ExecuteCommands null hub!");
				return;
			}
			currentHub.Call("ExecuteCommands", onResult, onFailed, commandJsons.ToArray(), commandTypes.ToArray());
		}

		public void RemoteInvoke(string methodName, string arg1, string arg2)
		{
			if (currentHub == null)
			{
				LogError("BestSignalR.Invoke null hub " + methodName + " " + arg2);
			}
			else if (arg2 != null)
			{
				currentHub.Call(methodName, arg1, arg2);
			}
			else if (arg1 != null)
			{
				currentHub.Call(methodName, arg1);
			}
			else
			{
				currentHub.Call(methodName);
			}
		}

		protected void OnConnected(Connection connection)
		{
			Log("OnConnected");
			if (State == SignalRClientState.Reconnecting)
			{
				for (int num = sentCommandQueue.Count - 1; num >= 0; num--)
				{
					commandQueue.Insert(0, sentCommandQueue[num]);
				}
				sentCommandQueue.Clear();
				commandQueue.Insert(0, new Command("Reconnect", string.IsNullOrEmpty(currentSessionToken) ? string.Empty : currentSessionToken, null, OnReconnect, waitForResponse: true));
				SendNextCommand();
			}
			State = SignalRClientState.Connected;
			if (connectionRequestedHandler != null)
			{
				connectionRequestedHandler("connected");
				connectionRequestedHandler = null;
			}
			this.OnHubConnectionMessage?.Invoke(null, "connected");
		}

		private void StartReconnect(Connection connection)
		{
			if (OfflineManager.IsIgnoreReconnect)
			{
				//State = SignalRClientState.Disconnected;
				CallCountBase.Instance.AddReconnectError();
				DebugTWD.LogError("Ignore StartReconnect", DebugType.SignalR);
			}
			else
			{
				SignalRNotificationManager.NotifyReconnecting(connection, lastReconnectTime);
				State = SignalRClientState.Reconnecting;
			}
		}

		protected void OnReconnecting(Connection connection)
		{
			Log("OnReconnecting");
			StartReconnect(connection);
		}

		protected void OnDisconnected(Connection connection)
		{
			Log("OnDisconnected");
			disconnectRequested = false;
			this.OnHubConnectionMessage?.Invoke(null, "disconnected");
			if (State != SignalRClientState.Disconnecting && State != SignalRClientState.Disconnected)
			{
				StartReconnect(connection);
			}
			else
			{
				State = SignalRClientState.Disconnected;
			}
		}

		protected void OnError(Connection connection, string error)
		{
			Log("OnError(" + error + ")");
			this.OnHubConnectionMessage?.Invoke(null, "error");
			if (State == SignalRClientState.Connecting)
			{
				connectionRequestedHandler?.Invoke(error);
			}
			if (error.ToLower().Contains("missing connection token") || (error.ToLower().Contains("request finished with error") && connection.Transport.Type == TransportTypes.LongPoll))
			{
				connection.Close();
				ConnectHubConnection(HubName);
				SetLastError("Received error '" + error + "', disconnected!", ErrorType.Connectivity);
				NotifyServerMessage("Could not connect.", "disconnect");
			}
		}

		private void OnHubRequestDisconnect(Hub hub, MethodCallMessage methodCall)
		{
			string text = methodCall.Arguments[0] as string;
			Log("Server requested disconnect");
			AnalyticsManager.instance.CreateEvent("Connectivity_DisconnectRequested").AddProperty("Message", text).Send();
			disconnectRequested = true;
			this.OnServerMessage?.Invoke(text, "disconnect");
		}

		private void OnHubWarning(Hub hub, MethodCallMessage methodCall)
		{
			string message = methodCall.Arguments[0] as string;
			Log("Server issued warning");
			this.OnServerMessage?.Invoke(message, "warning");
		}

		private void OnReconnect(string message)
		{
			if (string.IsNullOrEmpty(message))
			{
				SignalRNotificationManager.NotifyReconnected();
			}
			else
			{
				SetLastError(message, ErrorType.CommandExecution);
			}
		}
		//Self referencing loop detected with type 'TWDModel.ModSkills.ModSkillSlot'. Path 'GvGSeasonModel.GuildWarModel.CurrentBattle.EnemyGuildData.PlayerInfoSnapshot.b34df98a14ac4cc1a7ba99febc864542.SelectedSurvivors[3].MockWeapon.ModSkillSlots[0].ModSkillMode.EquipmentItemModel.ModSkillSlots'.
		private void OnModelMessage(Hub hub, MethodCallMessage methodCall)
		{
			string text = ((methodCall.Arguments.Length >= 1) ? (methodCall.Arguments[0] as string) : null);
			if (string.IsNullOrEmpty(text))
			{
				LogError("SignalRClient no method name defined in response");
				SetLastError("Fatal error", ErrorType.Connectivity);
				return;
			}
			string text2 = ((methodCall.Arguments.Length >= 2) ? (methodCall.Arguments[1] as string) : null);
			string text3 = ((methodCall.Arguments.Length >= 3) ? (methodCall.Arguments[2] as string) : null);
			Log("OnModelMessage [" + text + "] - " + text2 + " (" + text3 + ")");
			if (text.StartsWith("Social") || GroupServiceMethodNames.Contains(text))
			{
				Log("Social [" + text + "] - " + text2 + " (" + text3 + ")");
				if (!string.IsNullOrEmpty(text3))
				{
					LogError("Unable to use group services: " + text3);
					SetLastError(text3, ErrorType.GuildsOffline);
				}
				if (text.StartsWith("Social"))
				{
					if (IsLoadDataManager)
					{
						if (text == "SocialGroupLoaded")
						{
							if (GWTeamUtils.Instance.SetHomeGuild(text2))
							{
								DebugTWD.Log("Load Home Guild", DebugType.SignalR);
							}
						}
						else if (text == "SocialCommand")
						{
							DebugTWD.Log("SocialCommand is " + text2, DebugType.SignalR);

							this.OnSocialMessage?.Invoke(text2, text); //комментировать
							//UIEvent.Send("SocialChatNewMessage");
						}
						else
						{
							DebugTWD.LogMycode("if (IsLoadDataManager)"); //"SocialGroupLoaded" "SocialCommand"
							DebugTWD.LogWarning("Ignore OnSocialMessage. Message is: " + text + " | " + text2, DebugType.SignalR);
						}
						return;
					}
					this.OnSocialMessage?.Invoke(text2, text);
					return;
				}
			}
			else
			{
				if (text.StartsWith("Banana"))
				{
					Log("Banana [" + text + "] - " + text2 + " (" + text3 + ")");
					this.OnBananaMessage?.Invoke(text2, text);
					return;
				}
				if (text.StartsWith("WorldBossBaseSnapshotChanged"))
				{
					Log("WorldBossBaseSnapshot [" + text + "] - " + text2 + " (" + text3 + ")");
					this.OnWorldBossBaseSnapshotMessage?.Invoke(text2, text);
					return;
				}
				if (text.StartsWith("WorldBossFullSnapshotChanged"))
				{
					Log("WorldBossFullSnapshot [" + text + "] - " + text2 + " (" + text3 + ")");
					this.OnWorldBossFullSnapshotMessage?.Invoke(text2, text);
					return;
				}
				if (text.StartsWith("BuySubscription"))
				{
					Log("BuySubscription [" + text + "] - " + text2 + " (" + text3 + ")");
					this.OnBuySubscriptionMessage?.Invoke(text2, text);
					return;
				}
				if (text.StartsWith("GuildBattleHighScoresChanged"))
				{
					Log("GuildBattleHighScoresChanged [" + text + "] - " + text2 + " (" + text3 + ")");
					this.OnGuildBattleHighScoresMessage?.Invoke(text2, text);
					return;
				}
				if (!string.IsNullOrEmpty(text3))
				{
					LogError("SignalRClient " + text + " failed: " + text3);
					SetLastError(text3, ErrorType.CommandExecution);
					if (text == "Reconnect")
					{
						commandQueue.Clear();
					}
				}
			}
			if (!HasCommandInSentQueue(text.ToLower()))
			{
				LogError("SignalRClient Received response for " + text + " which is not in the sent queue!");
				SetLastError("Received response for " + text + " which is not in the sent queue!");
				DebugTWD.LogWarning("Выполняем OnServerMessage: Received response for " + text + " which is not in the sent queue!", DebugType.SignalR);

				this.OnServerMessage?.Invoke(text3, "error");
			}
			else
			{
				NotifyCommandCompleted(0, -1);
				CompleteCommand(text, text2);
				if (IsLoadDataManager && !string.IsNullOrEmpty(text2))
				{
					DebugTWD.LogMycode("if (IsLoadDataManager && !string.IsNullOrEmpty(text2))");
					DebugTWD.Log("OnModelMessage: " + text + " | " + text2, DebugType.SignalR);
					Messages.Add(text2);
					string id = null;
					string identification = null;
					var array1 = text2.Split(',');
					if (array1.Length > 0)
					{
						var array2 = array1[0].Split(':');
						if (array2.Length > 1)
						{
							identification = array2[0];
							id = array2[1];
						}
					}
					if (identification != null && identification.Trim('\"') == "Identification" && id == "null")
					{
						if (DataManager.Instance.language != DataManager.Language.Ru)
						{
							MyTools.UpdateLogPanel("Couldn't response player's data. Please change Game Data Server");
						}
						else
						{
							MyTools.UpdateLogPanel("Не могу получить ответ с игрового сервера. Поменяйте игровой сервер.");
						}
						DebugTWD.LogError("Не могу получить ответ с игрового сервера. Поменяйте игровой сервер");

						//string message = CustomLocalization.GetText("NoResponseServer");
						//MyTools.UpdateLogPanel(message);
						//DebugTWD.LogError(message, DebugType.SignalR);
					}
				}
				else
				{
					DebugTWD.Log("OnModelMessage is null", DebugType.SignalR);
				}
				this.OnServerMessage?.Invoke(text3, "error");
			}
		}

		private void OnLoadGed(Hub hub, MethodCallMessage methodCall)
		{
			DebugTWD.Log("OnLoadGed begin 0", DebugType.SignalR);

			string url = methodCall.Arguments[0] as string;
			string checksum = methodCall.Arguments[1] as string;
			GameManager.Instance.OnLoadGed(url, checksum);
		}

		private void OnLoadPlayer(Hub hub, MethodCallMessage methodCall)
		{
			long time = long.Parse(methodCall.Arguments[0].ToString());
			string json = GZip.DeflateBase64String(methodCall.Arguments[1] as string);
			string checksum = methodCall.Arguments[2] as string;
			GameManager.Instance.OnLoadPlayer(time, json, checksum);
		}

		private void OnLoadContent(Hub hub, MethodCallMessage methodCall)
		{
			string transactionId = methodCall.Arguments[0] as string;
			string content = methodCall.Arguments[1] as string;
			string checksum = methodCall.Arguments[2] as string;
			ContentManager.Instance.OnLoadContent(transactionId, content, checksum);
		}

		private void OnLoadRemoteContent(Hub hub, MethodCallMessage methodCall)
		{
		}

		public void SetDirectUrl(string url)
		{
			if (!string.IsNullOrEmpty(url))
			{
				CurrentHostPort = url;
			}
		}

		public void SetSessionToken(string sessionToken)
		{
			currentSessionToken = sessionToken;
		}

		public bool IsTokenAwailable => !string.IsNullOrEmpty(currentSessionToken);

		#region myparams
		public bool IsInited { get; private set; }
		public bool IsOnlyGetPlayersData { get; set; }
		public bool IsOnlyGedData { get; set; }
		public bool IsOnlyGetImagesData { get; set; }
		public bool IsConnectedState;
		public List<string> Messages { get; private set; }
		private bool IsLoadDataManager => OfflineManager.IsLoadDataManager;
		private bool IsLoadingGuild;

		#endregion

		#region mycode
		public string GetDirectUrl()
		{
			return CurrentHostPort;
		}

		public void SetLoadingStatus(bool isLoading)
		{
			IsLoadingGuild = isLoading;
		}

		public string GetSessionToken()
		{
			return currentSessionToken;
		}

		private void OnEnable()
		{
			if (!IsInited)
			{
				Init();
			}
		}
		#endregion
	}
}
