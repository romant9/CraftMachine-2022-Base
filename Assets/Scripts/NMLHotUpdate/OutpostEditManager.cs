using System.Collections.Generic;
using BaseModel;
using Client.Connectivity;
using TWDModel;

public class OutpostEditManager
{
	public delegate void LoadingDoneDelegate();

	private const string DebugString = "OutpostManager: ";

	public static MapMissionParameters OutpostAttackData { get; set; }

	public static PlayerModel Player => GameManager.Instance.playerModel;

	public static void StartOutpostAttack()
	{
		if (OutpostAttackData.IsPvP)
		{
			ShowLoading();
			if (GameManager.Instance.IsConnectedToServer)
			{
				SignalRClient.Instance.RequestCommand("LockPlayer", OutpostAttackData.MissionId, OnPlayerLockResult, waitForResponse: true);
			}
			else
			{
				GameManager.Instance.LoadVisitModel(VisitMode.PVP, OutpostAttackData);
			}
		}
	}

	private static void OnPlayerLockResult(string message)
	{
		LockRespond lockRespond = GameManager.Instance.jsonSerializer.DeserializeObject<LockRespond>(message);
		if (lockRespond != null && lockRespond.IsLocked && lockRespond.Status == LockRespond.LockStatus.Locked)
		{
			GameManager.Instance.LoadVisitModel(VisitMode.PVP, OutpostAttackData);
			return;
		}
		HideLoading();
		AlertPopup.ShowPopup(LocalizationManager.GetText("Popup.Alert.OutpostAttackPlayerOnline.Title"), LocalizationManager.GetText("Popup.Alert.OutpostAttackPlayerOnline.Message"), LocalizationManager.GetText("Button.Ok"));
		if (AnalyticsManager.instance != null)
		{
			AnalyticsManager.instance.CreateEvent("MatchMaking_Error_PlayerOnline").Send();
		}
	}

	private static void ShowLoading()
	{
		if (SingularityMonoBehaviour<HUDManager>.Instance != null)
		{
			HUDElement hUDElement = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.IngameLoading);
			if (hUDElement != null)
			{
				hUDElement.Open();
			}
		}
	}

	private static void HideLoading()
	{
		if (SingularityMonoBehaviour<HUDManager>.Instance != null)
		{
			HUDElement hUDElement = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.IngameLoading, null, createIfNotExist: false);
			if (hUDElement != null && hUDElement.IsOpen)
			{
				hUDElement.Close();
			}
		}
	}

	public static bool CallModifyOutpostCommand(string sliceViewId, string hotspotViewId, HotspotState state, WalkerType walkerType, int count, AIMode defensiveMode = AIMode.Aggressive, bool showNotification = true)
	{
		int num = 0;
		if (GetEditLevelModel() != null)
		{
			num = GetEditLevelModel().CanAffordHotspotModifiaction(sliceViewId, hotspotViewId, state, walkerType, count);
			if (num < 1)
			{
				if (Helpers.ExecuteCommand(new ModifyOutpostHotspotCommand(sliceViewId, hotspotViewId, state, walkerType, count, defensiveMode)) == TWDModelResult.OK)
				{
					return true;
				}
				return false;
			}
			if (showNotification)
			{
				HUDNotification.Info("Missing Points: " + num);
			}
		}
		return false;
	}

	public static void CallCreateOutpostCommand(bool autoFill = false)
	{
		if (Helpers.ExecuteCommand(new CreateOutpostCommand()) == TWDModelResult.OK)
		{
			RunLocationModel outpostTemplate = Player.GetOutpostTemplate(Player.OutpostModel.EditLevelModel.BaseRunLocationID);
			if (outpostTemplate != null)
			{
				List<KeyValuePair<string, OutpostHotspotModel>> list = new List<KeyValuePair<string, OutpostHotspotModel>>();
				List<string> sliceViewIds = outpostTemplate.GetSliceViewIds(SlicePosition.First);
				if (sliceViewIds != null && sliceViewIds.Count > 0)
				{
					Helpers.ExecuteCommand(new SetOutpostSliceCommand(SlicePosition.First, sliceViewIds[0], clearPrevious: true));
					OutpostSliceModel sliceModel = outpostTemplate.GetSliceModel(sliceViewIds[0]);
					foreach (TWDModelObject model in sliceModel.Models)
					{
						if (model is OutpostHotspotModel value)
						{
							list.Add(new KeyValuePair<string, OutpostHotspotModel>(sliceModel.ViewId, value));
						}
					}
				}
				sliceViewIds = outpostTemplate.GetSliceViewIds(SlicePosition.Second);
				if (sliceViewIds != null && sliceViewIds.Count > 0)
				{
					Helpers.ExecuteCommand(new SetOutpostSliceCommand(SlicePosition.Second, sliceViewIds[0], clearPrevious: true));
					OutpostSliceModel sliceModel2 = outpostTemplate.GetSliceModel(sliceViewIds[0]);
					foreach (TWDModelObject model2 in sliceModel2.Models)
					{
						if (model2 is OutpostHotspotModel value2)
						{
							list.Add(new KeyValuePair<string, OutpostHotspotModel>(sliceModel2.ViewId, value2));
						}
					}
				}
				sliceViewIds = outpostTemplate.GetSliceViewIds(SlicePosition.Third);
				if (sliceViewIds != null && sliceViewIds.Count > 0)
				{
					Helpers.ExecuteCommand(new SetOutpostSliceCommand(SlicePosition.Third, sliceViewIds[0], clearPrevious: true));
					OutpostSliceModel sliceModel3 = outpostTemplate.GetSliceModel(sliceViewIds[0]);
					foreach (TWDModelObject model3 in sliceModel3.Models)
					{
						if (model3 is OutpostHotspotModel value3)
						{
							list.Add(new KeyValuePair<string, OutpostHotspotModel>(sliceModel3.ViewId, value3));
						}
					}
				}
			}
			AutoAssignDefenders();
			if (autoFill)
			{
				AutoFill();
			}
		}
		else
		{
			Debug.LogError("OutpostManager: Could not create new outpost!");
		}
	}

	public static void AutoAssignDefenders()
	{
		SurvivorContainerModel survivorContainer = GameManager.Instance.playerModel.SurvivorContainer;
		if (survivorContainer == null)
		{
			return;
		}
		int num = 3 - survivorContainer.OutpostDefendingSurvivors.Count;
		if (survivorContainer.Survivors.Count <= 5 || num <= 0)
		{
			return;
		}
		for (int i = 0; i < survivorContainer.Survivors.Count; i++)
		{
			SurvivorModel survivorModel = survivorContainer.Survivors[i];
			if (survivorModel != null && survivorModel.InjuryType == InjuryType.None && !survivorContainer.CombatSurvivors.Contains(survivorModel) && survivorContainer.OutpostDefendingSurvivors.Count < 3)
			{
				Helpers.ExecuteCommand(new SetSurvivorToCombatCommand(survivorModel)
				{
					SurvivorType = SurvivorContainerModel.SurvivorType.Outpost
				});
			}
		}
	}

	public static void AutoFill()
	{
		RunLocationModel outpostTemplate = Player.GetOutpostTemplate(Player.OutpostModel.EditLevelModel.BaseRunLocationID);
		if (outpostTemplate == null)
		{
			return;
		}
		Helpers.ExecuteCommand(new ClearOutpostCommand());
		List<KeyValuePair<string, OutpostHotspotModel>> list = new List<KeyValuePair<string, OutpostHotspotModel>>();
		for (int i = 0; i < Player.OutpostModel.EditLevelModel.ChosenSlices.Count; i++)
		{
			OutpostSliceModel sliceModel = outpostTemplate.GetSliceModel(Player.OutpostModel.EditLevelModel.ChosenSlices[i].ViewId);
			foreach (TWDModelObject model in sliceModel.Models)
			{
				if (model is OutpostHotspotModel value)
				{
					list.Add(new KeyValuePair<string, OutpostHotspotModel>(sliceModel.ViewId, value));
				}
			}
		}
		int num = 0;
		bool flag = false;
		bool flag2 = false;
		ModelRandom random = new ModelRandom((int)GameManager.Instance.playerModel.LifeTime);
		UtilsArray.ShuffleList(list, random);
		for (int j = 0; j < list.Count; j++)
		{
			string key = list[j].Key;
			OutpostHotspotModel value2 = list[j].Value;
			if (Player.OutpostModel.EditLevelModel.FindHotspotInfo(value2.ViewId) != null)
			{
				continue;
			}
			if (value2.Type == HotspotType.Goal && !flag)
			{
				if (CallModifyOutpostCommand(key, value2.ViewId, HotspotState.Flag, WalkerType.WalkerNormal, 1))
				{
					flag = true;
				}
			}
			else if (value2.Type == HotspotType.Goal && !flag2)
			{
				if (CallModifyOutpostCommand(key, value2.ViewId, HotspotState.ResourceContainer, WalkerType.WalkerNormal, 1))
				{
					flag2 = true;
				}
			}
			else if (value2.CanAssignDefender && num < 3 && CallModifyOutpostCommand(key, value2.ViewId, (HotspotState)(num + 2), WalkerType.WalkerNormal, 1))
			{
				num++;
			}
		}
		int startIndex = TryAssignWalkers(list, WalkerType.WalkerArmored);
		TryAssignWalkers(list, WalkerType.WalkerTank, startIndex);
		AutoAssignDefenders();
	}

	private static int TryAssignWalkers(List<KeyValuePair<string, OutpostHotspotModel>> hotspotModels, WalkerType walkerType, int startIndex = 0)
	{
		string walkerId = walkerType.ToString();
		int totalWalkersAssigned = Player.OutpostModel.EditLevelModel.GetTotalWalkersAssigned(walkerType);
		int num = Player.OutpostModel.GetWalkerModel(walkerId).Amount - totalWalkersAssigned;
		if (num <= 0)
		{
			return startIndex;
		}
		for (int i = startIndex; i < hotspotModels.Count; i++)
		{
			string key = hotspotModels[i].Key;
			OutpostHotspotModel value = hotspotModels[i].Value;
			HotspotInfo hotspotInfo = Player.OutpostModel.EditLevelModel.FindHotspotInfo(value.ViewId);
			if (value.CanAssignWalker && hotspotInfo == null && CallModifyOutpostCommand(key, value.ViewId, HotspotState.Walker, walkerType, 1, AIMode.Aggressive, showNotification: false))
			{
				num--;
				if (num <= 0)
				{
					return i + 1;
				}
			}
		}
		return hotspotModels.Count;
	}

	public static bool CallStartEditingOutpost()
	{
		if (Helpers.ExecuteCommand(new StartEditingOutpostCommand()) == TWDModelResult.OK)
		{
			return true;
		}
		return false;
	}

	public static bool CallStopEditingOutpost(bool publishCurrentEdit = false)
	{
		if (Helpers.ExecuteCommand(new StopEditingOutpostCommand((!publishCurrentEdit) ? StopEditingOutpostCommand.ActionType.Discard : StopEditingOutpostCommand.ActionType.Save)) == TWDModelResult.OK)
		{
			if (!publishCurrentEdit)
			{
				return true;
			}
			if (Helpers.ExecuteCommand(new PublishOutpostCommand()) == TWDModelResult.OK)
			{
				if (CampView.Instance != null)
				{
					CampView.Instance.Hud.ShowOutpostMenuSuggetion(show: false);
				}
				return true;
			}
			Debug.LogError("OutpostManager: Publish Outpost Model Error!");
		}
		else
		{
			Debug.LogError("OutpostManager: Could Not Stop Editing!");
		}
		return false;
	}

	private static OutpostLevelModel GetEditLevelModel()
	{
		if (GameManager.Instance == null || (GameManager.Instance.playerModel.OutpostModel == null && GameManager.Instance.playerModel.OutpostModel.EditLevelModel == null))
		{
			Debug.LogError("OutpostManager: Edit Model NULL!");
			return null;
		}
		return GameManager.Instance.playerModel.OutpostModel.EditLevelModel;
	}
}
