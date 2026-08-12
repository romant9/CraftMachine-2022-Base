using System;
using BaseModel;
using TWDModel;
using UnityEngine;

public class RunLocationLoader
{
	public delegate void LoadingDoneDelegate(RunLocationModel model);

	private const string DebugString = "RunLocationLoader: ";

	private static LoadingDoneDelegate LoadingSuccessCallback;

	private static LoadingDoneDelegate LoadingErrorCallback;

	public static bool IsLoading { get; private set; }

	public static void LoadLocationModel(OutpostTemplateDefinition definition, LoadingDoneDelegate loadingSuccess, LoadingDoneDelegate loadingError)
	{
		if (definition == null)
		{
			return;
		}
		string missionID = definition.MissionID;
		if (missionID != "")
		{
			Helpers.ExecuteCommand(new SetSelectedOutpostTemplateCommand(definition.Id));
			RunLocationModel outpostTemplate = GameManager.Instance.playerModel.GetOutpostTemplate(GameManager.Instance.playerModel.GetSelectedOutpostTemplateMissionId());
			if (outpostTemplate != null)
			{
				loadingSuccess(outpostTemplate);
				return;
			}
			if (!GameManager.Instance.IsConnectedToServer && GameConfiguration.Instance.Config.OnlineLevel == BuildGameConfiguration.OnlineLevelType.Offline)
			{
				MissionData missionData = GameManager.Instance.gameEconomyData.GetMissionData(missionID);
				if (missionData == null)
				{
					loadingError(null);
					Debug.LogError("Offline outpost mission data not found:" + missionID);
					return;
				}
				TextAsset textAsset = UnityUtils.LoadAsset<TextAsset>("run_locations/" + missionData.RunLocationName);
				if (textAsset == null)
				{
					loadingError(null);
					Debug.LogError("Offline outpost template not found:" + missionData.RunLocationName);
					return;
				}
				try
				{
					RunLocationModel runLocation = GameManager.Instance.jsonSerializer.DeserializeObject<RunLocationModel>(textAsset.text);
					GameManager.Instance.modelManager.ApplyRunLocation(VisitMode.ScoutPVE, runLocation, null);
				}
				catch (Exception)
				{
					loadingError(null);
				}
				loadingSuccess(GameManager.Instance.playerModel.GetOutpostTemplate(GameManager.Instance.playerModel.GetSelectedOutpostTemplateMissionId()));
			}
			if (!IsLoading && GameManager.Instance.IsConnectedToServer)
			{
				LoadingSuccessCallback = loadingSuccess;
				LoadingErrorCallback = loadingError;
				IsLoading = true;
				ContentManager.Instance.LoadContent("RunLocation/" + missionID, OnRunLocationContent, 1);
			}
			else
			{
				Debug.LogWarning("RunLocationLoader: Cannot start loading!");
				loadingError(null);
			}
		}
		else
		{
			Debug.LogError("RunLocationLoader: Cant load template with empty missionId!");
		}
	}

	private static void OnRunLocationContent(string transactionId, bool loaded)
	{
		IsLoading = false;
		if (!loaded)
		{
			TriggerCallbacks(null);
		}
		else if (Helpers.ExecuteCommand(new ApplyRunLocationCommand(transactionId, VisitMode.ScoutPVE)) != TWDModelResult.OK)
		{
			Debug.LogError("RunLocationLoader: Failed to apply PvP outpost template matchmaking visit model!");
			TriggerCallbacks(null);
		}
		else
		{
			TriggerCallbacks(GameManager.Instance.playerModel.GetOutpostTemplate(GameManager.Instance.playerModel.GetSelectedOutpostTemplateMissionId()));
		}
	}

	private static void TriggerCallbacks(RunLocationModel model)
	{
		if (model != null)
		{
			if (LoadingSuccessCallback != null)
			{
				LoadingSuccessCallback(model);
			}
		}
		else if (LoadingErrorCallback != null)
		{
			LoadingErrorCallback(null);
		}
		ClearCallbacks(LoadingSuccessCallback, LoadingErrorCallback);
	}

	public static void ClearCallbacks(LoadingDoneDelegate success, LoadingDoneDelegate error)
	{
		if (LoadingSuccessCallback == success)
		{
			LoadingSuccessCallback = null;
		}
		if (LoadingErrorCallback == error)
		{
			LoadingErrorCallback = null;
		}
	}
}
