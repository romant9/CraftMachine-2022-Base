using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TWDModel;

namespace BaseModel
{
	public abstract class ModelManager
	{
		public enum ModelManagerStartState
		{
			Initial = 0,
			Starting = 1,
			Started = 2
		}

		protected ModelObject root;

		protected Dictionary<int, ModelObject> models;

		protected List<int> modelIds;

		protected int nextModelId;

		protected int nextCommandSequenceId;

		protected int nextRandomSeed;

		protected Dictionary<string, GroupModelBase> groupModels = new Dictionary<string, GroupModelBase>();

		protected List<ModelCommand> pauseCommandQueue = new List<ModelCommand>();

		protected long accumulatedPauseTime;

		protected List<ModelCommand> coroutinePendingCommandQueue = new List<ModelCommand>();

		protected bool isDirty;

		protected IModelCommandTransport commandTransport;

		private bool isExecutingCommand;

		public Dictionary<Type, long> ModelTypeHashCache = new Dictionary<Type, long>();

		public ModelManagerMode Mode { get; protected set; }

		public VisitMode VisitMode { get; protected set; }

		public long Time { get; protected set; }

		public IServerService ServerService { get; protected set; }

		public IModelContentService ContentService { get; protected set; }

		public IModelDebug Debug { get; protected set; }

		public IModelAnalytics Analytics { get; protected set; }

		public IModelAnalytics TdAnalytics { get; protected set; }

		public IModelAnalytics TdUserAnalytics { get; protected set; }

		public ModelManagerStartState StartState { get; protected set; }

		public bool Paused { get; protected set; }

		public bool TickModelSuspended { get; set; }

		public bool ModelStateCheckEnabled { get; set; }

		public bool IsStarted => StartState == ModelManagerStartState.Started;

		public bool IsExecutingCommand => isExecutingCommand;

		public ModelRespondCode ResultForEndSurvivorTurnCommand { get; private set; }

		public ModelManager()
		{
			models = new Dictionary<int, ModelObject>();
			modelIds = new List<int>();
			nextModelId = 1;
			nextCommandSequenceId = 1;
			nextRandomSeed = 1;
			StartState = ModelManagerStartState.Initial;
		}

		public abstract IPlayerModel CreateModel();

		public abstract IPlayerModel LoadModel(string data, LoginRequest loginRequest);

		public abstract IPlayerModel GetPlayer();

		public abstract IPlayerModel GetVisitPlayer();

		public abstract string SerializeModel();

		public abstract void SetGameEconomyData(IGameEconomyData data);

		public abstract bool Disconnect(long time);

		public abstract void LoadVisitModel(string modelJson, long time, VisitMode visitMode);

		public virtual bool SetMatchData(string parameters, List<MatchMakingInfo> matchInfos)
		{
			return true;
		}

		public abstract string GetVersion();

		public abstract string GetDebugInfo(string debugInfoKey);

		public abstract ModelManager CreateManager();

		public abstract IMessageSerializer GetMessageSerializer();

		public abstract StorePurchaseInfo GetStorePurchaseInfo(string transactionId);

		public abstract StorePurchaseInfo GetCurrentPurchaseInfo(string transactionId);

		public abstract GroupModelBase CreateGroupModel(string id);

		public abstract void LoadGroupModel(string json, bool forceSync = false);

		public abstract GroupCommandBase ExecuteGroupCommand(GroupCommandBase command);

		public abstract GroupModelBase GetGroupModelInfo(string groupModelJson);

		public abstract IGuildBattleMatchmakingInfoBase GetGuildBattleMatchmakingInfo(string groupModelJson);

		public GroupModelBase GetGroupModel(string id)
		{
			if (groupModels.ContainsKey(id))
			{
				return groupModels[id];
			}
			return null;
		}

		public virtual List<GroupInfo> GetGroupInfo()
		{
			return new List<GroupInfo>();
		}

		public string GetGroupModelsJson()
		{
			return GetMessageSerializer().SerializeObject(groupModels);
		}

		public bool SetModelManagerMode(ModelManagerMode mode)
		{
			Mode = mode;
			return true;
		}

		public virtual void TickModel(long deltaTime)
		{
			if (Paused)
			{
				accumulatedPauseTime += deltaTime;
			}
			else if (!TickModelSuspended)
			{
				Time += deltaTime;
				root.Tick(deltaTime);
				Dictionary<string, GroupModelBase>.Enumerator enumerator = groupModels.GetEnumerator();
				while (enumerator.MoveNext())
				{
					enumerator.Current.Value.Tick(deltaTime);
				}
			}
		}

		public void SetPaused(bool paused)
		{
			if (isExecutingCommand)
			{
				throw new InvalidOperationException("Call to SetPaused is not allowed within ExecuteCommand.");
			}
			if (paused == Paused)
			{
				return;
			}
			Paused = paused;
			if (!paused)
			{
				for (int i = 0; i < pauseCommandQueue.Count; i++)
				{
					ExecuteCommand(pauseCommandQueue[i]);
				}
				pauseCommandQueue.Clear();
				TickModel(accumulatedPauseTime);
				accumulatedPauseTime = 0L;
			}
		}

		public void SetCommandTransport(IModelCommandTransport commandTransport)
		{
			this.commandTransport = commandTransport;
		}

		public int RegisterModel(ModelObject model)
		{
			int num = nextModelId++;
			models.Add(num, model);
			modelIds.Add(num);
			return num;
		}

		public void DeregisterModel(ModelObject model)
		{
			models.Remove(model.ModelId);
			modelIds.Remove(model.ModelId);
		}

		public IModelObject GetModel(int modelId)
		{
			ModelObject value = null;
			models.TryGetValue(modelId, out value);
			return value;
		}

		public T GetModel<T>(int modelId) where T : class, IModelObject
		{
			return GetModel(modelId) as T;
		}

		public IEnumerator ExecuteEndSurvivorTurnCommand(ModelCommand command)
		{
			if (isExecutingCommand)
			{
				throw new InvalidOperationException("Nested calls to ExecuteCommand are not allowed.");
			}
			if (Paused)
			{
				pauseCommandQueue.Add(command);
				ResultForEndSurvivorTurnCommand = ModelRespondCode.Paused;
				yield break;
			}
			isExecutingCommand = true;
			TickModelSuspended = true;
			command.SetParameters(Time, nextCommandSequenceId++);
			OnBeforeCommandExecution(command);
			isDirty = true;
			Debug.Log("ModelManager.ExecuteCommand " + command.GetType().FullName + ", time " + command.Time);
			yield return command.ExecuteForClient(this);
			ResultForEndSurvivorTurnCommand = command.respondCode;
			if (ResultForEndSurvivorTurnCommand == ModelRespondCode.OK)
			{
				OnCommandExecuted(command);
				if (ModelStateCheckEnabled)
				{
					AddModelState(command);
				}
				if (commandTransport != null)
				{
					commandTransport.Send(command);
				}
			}
			else
			{
				Debug.LogError("ModelManager.ExecuteCommand failed: " + command.GetType().FullName);
			}
			isExecutingCommand = false;
			TickModelSuspended = false;
			FlushCoroutinePendingCommands();
		}

		private void FlushCoroutinePendingCommands()
		{
			while (coroutinePendingCommandQueue.Count > 0)
			{
				ModelCommand command = coroutinePendingCommandQueue[0];
				coroutinePendingCommandQueue.RemoveAt(0);
				ExecuteCommand(command);
			}
		}

		public IModelCommandRespond ExecuteCommand(ModelCommand command)
		{
			if (isExecutingCommand)
			{
				coroutinePendingCommandQueue.Add(command);
				Debug.LogDebug("ModelManager.ExecuteCommand queued during coroutine: " + command.GetType().FullName);
				return new ModelCommandRespond(command.SequenceId, -2, "Command queued during coroutine execution");
			}
			if (Paused)
			{
				pauseCommandQueue.Add(command);
				return new ModelCommandRespond(command.SequenceId, -1, "Command added to pause queue");
			}
			isExecutingCommand = true;
			try
			{
				if (Mode == ModelManagerMode.Client)
				{
					command.SetParameters(Time, nextCommandSequenceId++);
				}
				else
				{
					if (command.SequenceId < nextCommandSequenceId)
					{
						return new ModelCommandRespond(command.SequenceId, 0, "Command with sequenceId already executed");
					}
					if (command.SequenceId != nextCommandSequenceId)
					{
						Debug.LogDebug("ModelManager.ExecuteCommand sequenceId mismatch " + command.SequenceId + " " + nextCommandSequenceId);
						nextCommandSequenceId = command.SequenceId;
					}
					nextCommandSequenceId++;
					long num = command.Time - Time;
					if (num != 0L)
					{
						TickModel(num);
					}
				}
				OnBeforeCommandExecution(command);
				isDirty = true;
				IModelCommandRespond modelCommandRespond = command.Execute(this);
				GameManager.Instance?.Show_Command_Error(modelCommandRespond.Code);
				Debug.Log("ModelManager.ExecuteCommand " + command.GetType().FullName + ", time: " + command.Time + ", code: " + modelCommandRespond.Code);
				if (CallCountBase.Instance && modelCommandRespond.Code != 0 && modelCommandRespond.Code != 42)
				{
					Debug.LogWarning("ModelManager.ExecuteCommand " + command.GetType().FullName + ", code: " + modelCommandRespond.Code, DebugType.CommandError);
					CallCountBase.Instance.Show_Command_Error(modelCommandRespond);
					if (OfflineManager.IsLoadDataManager || OfflineManager.IsIgnoreResponseNotOK)
					{
						modelCommandRespond.Code = 0;
					}
				}

				if (modelCommandRespond.Code == 0 || modelCommandRespond.Code == 42)
				{
					OnCommandExecuted(command);
					if (!OfflineManager.IsLoadDataManager)
					{
						if (Mode == ModelManagerMode.Client && ModelStateCheckEnabled)
						{
							AddModelState(command);
						}
						else if (Mode == ModelManagerMode.Server && command.ModelState != null)
						{
							IModelCommandRespond modelCommandRespond2 = CheckModelState(command);
							if (modelCommandRespond2.Code != 0)
							{
								if (!OfflineManager.IsIgnoreResponseNotOK)
								{
									Debug.LogWarning("ModelManager.ExecuteCommand " + command.GetType().FullName + ", code: " + modelCommandRespond2.Code, DebugType.CommandError);
									if (CallCountBase.Instance)
									{
										CallCountBase.Instance.Show_Command_Error(modelCommandRespond2);
									}
									modelCommandRespond = modelCommandRespond2;
									GameManager.Instance?.Show_Command_Error(modelCommandRespond2.Code);
								}
							}
						}
						if (commandTransport != null)
						{
							commandTransport.Send(command);
						}
					}
					else
					{
						if (command is TWDModel.SetChatReadCommand || command is TWDModel.ChatMessageCommand) //TWDModel.SendSearchGuildMetricCommand
						{
							commandTransport ??= new TWDModelCommandTransport();
							commandTransport.Send(command);
						}

						Debug.Log("Fuck off the commandTransport.Send", DebugType.Analytics);
					}
				}
				else if (modelCommandRespond.Code == 37)
				{
					Debug.Log("ModelManager.ExecuteCommand skipped: " + command.GetType().FullName + " " + modelCommandRespond.Message);
					if (!OfflineManager.IsIgnoreResponseNotOK)
					{
						Debug.LogWarning("ModelManager.ExecuteCommand " + command.GetType().FullName + ", code: " + modelCommandRespond.Code, DebugType.CommandError);
						if (CallCountBase.Instance)
						{
							CallCountBase.Instance.Show_Command_Error(modelCommandRespond);
						}
						modelCommandRespond.Code = 0;
					}
				}
				else
				{
					Debug.LogError("ModelManager.ExecuteCommand failed: " + command.GetType().FullName + " " + modelCommandRespond.Code + " " + modelCommandRespond.Message);
				}
				return modelCommandRespond;
			}
			catch (Exception exception)
			{
				string text = FormatException(exception);
				Debug.LogError("ModelManager.ExecuteCommand exception: " + command.GetType().FullName + " " + text);
				return new ModelCommandRespond(command.SequenceId, 1, text);
			}
			finally
			{
				isExecutingCommand = false;
			}
		}

		public IModelCommandRespond ExecuteCommandDebug(ModelCommand command)
		{
			if (isExecutingCommand)
			{
				throw new InvalidOperationException("Nested calls to ExecuteCommand are not allowed.");
			}
			isExecutingCommand = true;
			try
			{
				OnBeforeCommandExecution(command);
				isDirty = true;
				Debug.Log("ModelManager.ExecuteCommand " + command.GetType().FullName + ", time " + command.Time);
				IModelCommandRespond modelCommandRespond = command.Execute(this);
				if (modelCommandRespond.Code == 0)
				{
					OnCommandExecuted(command);
				}
				else if (modelCommandRespond.Code == 37)
				{
					Debug.Log("ModelManager.ExecuteCommand skipped: " + command.GetType().FullName + " " + modelCommandRespond.Message);
				}
				else
				{
					Debug.LogError("ModelManager.ExecuteCommand failed: " + command.GetType().FullName + " " + modelCommandRespond.Message);
				}
				return modelCommandRespond;
			}
			catch (Exception exception)
			{
				string text = FormatException(exception);
				Debug.LogError("ModelManager.ExecuteCommand exception: " + command.GetType().FullName + " " + text);
				return new ModelCommandRespond(command.SequenceId, 1, text);
			}
			finally
			{
				isExecutingCommand = false;
			}
		}

		protected virtual void OnBeforeCommandExecution(ModelCommand command)
		{
		}

		protected virtual void OnCommandExecuted(ModelCommand command)
		{
		}

		protected virtual void AddModelState(ModelCommand command)
		{
			command.ModelState = new Dictionary<string, string>();
			command.ModelState.Add("ModelCount", GetDebugModelsCount().ToString());
			command.ModelState.Add("ModelHashCode", GetDebugModelsHashCode().ToString());
		}

		protected virtual IModelCommandRespond CheckModelState(ModelCommand command)
		{
			if (GetDebugModelsHashCode() != long.Parse(command.ModelState["ModelHashCode"]))
			{
				return new ModelCommandRespond(command.SequenceId, 1, "ModelHashCode mismatch");
			}
			return new ModelCommandRespond(command.SequenceId, 0, string.Empty);
		}

		public int GetDebugModelsCount()
		{
			return models.Count;
		}

		public long GetDebugModelsHashCode()
		{
			long num = models.Count;
			if (models.Count != 0)
			{
				int count = modelIds.Count;
				for (int i = modelIds.Count - count; i < modelIds.Count; i++)
				{
					Type type = models[modelIds[i]].GetType();
					long num2 = 0L;
					if (ModelTypeHashCache.ContainsKey(type))
					{
						num2 = ModelTypeHashCache[type];
					}
					else
					{
						string text = type.Name;
						int num3 = text.LastIndexOf(".");
						if (num3 >= 0 && num3 < text.Length - 1)
						{
							text = text.Substring(num3 + 1);
						}
						num2 = ModelHelpers.MD5SumLong(text);
						ModelTypeHashCache.Add(type, num2);
					}
					long num4 = num2 * modelIds[i];
					num ^= num4;
				}
			}
			return num;
		}

		private IEnumerable<string> getDebugModelsHashCodeEnumerable()
		{
			yield break;
		}

		public void ExportDebugModelsHashCodeToFile()
		{
			using StreamWriter streamWriter = new StreamWriter(new FileStream("", FileMode.Create, FileAccess.Write));
			foreach (string item in getDebugModelsHashCodeEnumerable())
			{
				streamWriter.Write(item);
			}
			streamWriter.Flush();
		}

		public virtual void StartModel(long time, bool isDev = false)
		{
			StartState = ModelManagerStartState.Starting;
			root.SetManager(this);
			root.Start();
			if (OfflineManager.IsLoadDataManager)
			{
				Debug.Log("Fuck off the Tick and Validation", DebugType.Analytics);
				StartState = ModelManagerStartState.Started;
			}
			else
			{
				root.Tick(0L);
				StartState = ModelManagerStartState.Started;
				root.Validate();
			}
		}

		public bool SetServerService(IServerService service)
		{
			ServerService = service;
			return true;
		}

		public void SetContentService(IModelContentService contentService)
		{
			ContentService = contentService;
		}

		public void Save(SaveType saveType)
		{
			if (ServerService != null)
			{
				ServerService.Save(saveType);
			}
			isDirty = false;
		}

		protected string FormatException(Exception exception)
		{
			return exception.Message + exception.StackTrace;
		}

		public bool SetModelDebug(IModelDebug modelDebug)
		{
			Debug = modelDebug;
			return true;
		}

		public bool SetModelAnalytics(IModelAnalytics modelAnalytics)
		{
			Analytics = modelAnalytics;
			return true;
		}

		public void SetTdModelAnalytics(IModelAnalytics modelAnalytics)
		{
			TdAnalytics = modelAnalytics;
		}

		public void SetTdUserModelAnalytics(IModelAnalytics modelAnalytics)
		{
			TdUserAnalytics = modelAnalytics;
		}

		public virtual int GetRandomSeed()
		{
			return nextRandomSeed++;
		}

		public virtual List<string> GetSupportedModelFixes()
		{
			return new List<string>();
		}

		public virtual bool ApplyModelFix(string key, string parameter)
		{
			return false;
		}

		public virtual object RunModelTestMethod(string methodName, string parameter = null)
		{
			return false;
		}

		public virtual List<string> GetSupportGetMethods()
		{
			return new List<string>();
		}

		public virtual object RunSupportGetMethod(string methodName, string parameter = null)
		{
			return false;
		}

		public virtual List<string> GetSupportGiveMethods()
		{
			return new List<string>();
		}

		public virtual object RunSupportGiveMethod(string methodName, string parameter, string modelVersion, string supportEntityGUID)
		{
			return false;
		}

		public virtual Dictionary<string, string> GetPlayerStateForModelAnalytics(string type = null)
		{
			return new Dictionary<string, string>();
		}

		public abstract MatchMakingInfo GetMatchMakingInfo(IPlayerModel playerModel = null);

		public abstract MatchMakingSearchParameters GetMatchMakingSearchParameters();

		public abstract long GetMarkForDeletionUnixSeconds();

		public virtual bool ExecuteLoadQueueMessage(string payload)
		{
			return true;
		}

		public virtual bool ExecuteSaveQueueMessage(string payload)
		{
			return true;
		}

		public virtual bool ExecuteBananaQueueMessage(string payload, ref BananaBuyBundleRPCCommand bananaBuyBundleRpcCommand)
		{
			return false;
		}

		public virtual string GetModelSnapshotExtraData()
		{
			return null;
		}

		public virtual string GetRollDiceSnapshotData()
		{
			return null;
		}

		public virtual string BuildSearchGroupQuery(Dictionary<string, string> clientParameters)
		{
			return string.Empty;
		}

		public virtual string GetBattlePassRealBundleId(string bundleId, string PurchaseSource = "")
		{
			return bundleId;
		}

		public virtual bool BananaBuyBundle(string bundleId, double paidPrice, long supportEntityTimestamp, string PurchaseSource = "")
		{
			return false;
		}

		public virtual bool BuySubscription(string subscriptionId, int platform, long expiryTimeMillis, int giveExtraReward)
		{
			return false;
		}

		public virtual bool SubscriptionSync()
		{
			return false;
		}
	}
}
