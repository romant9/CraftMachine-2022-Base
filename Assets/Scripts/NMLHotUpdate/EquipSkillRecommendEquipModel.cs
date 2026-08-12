using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BaseModel;
using Client.Connectivity;
using Driller.Models;
using TWDModel;

public class EquipSkillRecommendEquipModel
{
	public class PlayerDataSubset
	{
		public string Nickname { get; set; }

		public string PlayerEmblem { get; set; }
	}

	public EquipmentSkillSuggestion equipmentSkillSuggestion;

	private CancellationTokenSource _likeSyncCts;

	public Action<EquipSkillRecommendEquipModel> OnRefreshCall;

	private const int DelayTime = 10000;

	public SkillSuggestionLikeStatus LikeStatusNet { get; private set; }

	private SkillSuggestionLikeStatus LikeStatusLocal { get; set; }

	public SkillSuggestionLikeStatus CurrentLikeStatus => LikeStatusLocal ?? LikeStatusNet;

	public string PlayerName { get; private set; }

	public PlayerEmblem PlayerData { get; private set; }

	public string Class => equipmentSkillSuggestion.Class;

	public bool Default => equipmentSkillSuggestion.Default;

	private string Skill1 => equipmentSkillSuggestion.Skill1;

	private string Skill2 => equipmentSkillSuggestion.Skill2;

	private string Skill3 => equipmentSkillSuggestion.Skill3;

	private string Skill4 => equipmentSkillSuggestion.Skill4;

	private string Skill5 => equipmentSkillSuggestion.Skill5;

	private string Skill6 => equipmentSkillSuggestion.Skill6;

	private ModSkillSlot[] ModSkillSlots { get; set; }

	public EquipSkillRecommendEquipModel(EquipmentSkillSuggestion conf)
	{
		equipmentSkillSuggestion = conf;
	}

	private ModSkillSlot[] GetConfigModSkillSlots(PlayerModel player)
	{
		if (ModSkillSlots == null && player != null)
		{
			List<ModSkillSlot> list = new List<ModSkillSlot>();
			string[] array = new string[6] { Skill1, Skill2, Skill3, Skill4, Skill5, Skill6 };
			for (int i = 0; i < array.Length; i++)
			{
				if (!string.IsNullOrEmpty(array[i]))
				{
					ModSkillSlot modSkillSlot = new ModSkillSlot(i);
					ModSkillMode modSkillMode = LoadConfigModSkillMode(array[i], player);
					modSkillSlot.ModSkillMode = modSkillMode;
					list.Add(modSkillSlot);
				}
			}
			ModSkillSlots = list.ToArray();
		}
		return ModSkillSlots;
	}

	public ModSkillSlot[] GetResultModSkillSlots(PlayerModel player)
	{
		if (player == null)
		{
			return ModSkillSlots ?? Array.Empty<ModSkillSlot>();
		}
		GetConfigModSkillSlots(player);
		if (ModSkillSlots == null)
		{
			return Array.Empty<ModSkillSlot>();
		}
		List<ModSkillSlot> list = new List<ModSkillSlot>();
		for (int i = 0; i < ModSkillSlots.Length; i++)
		{
			if (ModSkillSlots[i].ModSkillMode == null || player.ModSkillManager == null)
			{
				list.Add(ModSkillSlots[i]);
				continue;
			}
			ModSkillMode modSkillModeByGroupID = player.ModSkillManager.GetModSkillModeByGroupID(ModSkillSlots[i].ModSkillMode.Type);
			if (modSkillModeByGroupID != null)
			{
				ModSkillSlot modSkillSlot = new ModSkillSlot(ModSkillSlots[i].Index);
				modSkillSlot.ModSkillMode = modSkillModeByGroupID;
				list.Add(modSkillSlot);
			}
			else
			{
				list.Add(ModSkillSlots[i]);
			}
		}
		return list.ToArray();
	}

	public static List<ModSkillMode> LoadConfigModSkillModes(List<string> skillIds, PlayerModel player)
	{
		List<ModSkillMode> list = new List<ModSkillMode>();
		if (skillIds == null)
		{
			return list;
		}
		foreach (string skillId in skillIds)
		{
			ModSkillMode modSkillMode = LoadConfigModSkillMode(skillId, player);
			if (modSkillMode != null)
			{
				list.Add(modSkillMode);
			}
		}
		return list;
	}

	private static ModSkillMode LoadConfigModSkillMode(string skillId, PlayerModel player)
	{
		if (string.IsNullOrEmpty(skillId))
		{
			return null;
		}
		SPTraitsRemoldDefinitions sPTraitsRemoldDefinitions = player?.gameEconomyData?.GetSPTraitsRemodeDefinitionByType(skillId)?.FirstOrDefault();
		if (sPTraitsRemoldDefinitions == null)
		{
			return null;
		}
		ModSkillMode modSkillMode = new ModSkillMode(sPTraitsRemoldDefinitions.ID, sPTraitsRemoldDefinitions.Type, sPTraitsRemoldDefinitions.AvailableClass, ModSkillState.Count, null, ModSkillLockState.Locked);
		if (player.ModSkillManager != null)
		{
			modSkillMode.SetManager(player.ModSkillManager.manager);
		}
		return modSkillMode;
	}

	public void RequestLikeStatus()
	{
		if (LikeStatusNet != null)
		{
			OnRefreshCall?.Invoke(this);
		}
		else
		{
			SignalRClient.Instance.RequestCommand("GetSkillSuggestionLikeStatus", equipmentSkillSuggestion.ID, OnLikeStatusReceived, waitForResponse: true);
		}
	}

	private void OnLikeStatusReceived(string message)
	{
		if (!CheckError(message))
		{
			IMessageSerializer messageSerializer = GameManager.Instance.modelManager.GetMessageSerializer();
			SetLikeStatusNet(messageSerializer.DeserializeObject<SkillSuggestionLikeStatus>(message));
			OnRefreshCall?.Invoke(this);
		}
	}

	private void SetLikeStatusNet(SkillSuggestionLikeStatus status)
	{
		LikeStatusNet = status;
		if (LikeStatusNet != null)
		{
			LikeStatusLocal = new SkillSuggestionLikeStatus
			{
				Count = LikeStatusNet.Count,
				HasLiked = LikeStatusNet.HasLiked
			};
		}
		OnRefreshCall?.Invoke(this);
	}

	public void RequestPlayerData()
	{
		if (PlayerData != null || string.IsNullOrEmpty(equipmentSkillSuggestion.UID))
		{
			OnRefreshCall?.Invoke(this);
		}
		else
		{
			SignalRClient.Instance.RequestCommand("GetPlayerDataSubsetByHashedId", equipmentSkillSuggestion.UID, OnPlayerDataReceived, waitForResponse: true);
		}
	}

	private void OnPlayerDataReceived(string message)
	{
		if (!CheckError(message))
		{
			IMessageSerializer messageSerializer = GameManager.Instance.modelManager.GetMessageSerializer();
			PlayerDataSubset playerDataSubset = messageSerializer.DeserializeObject<PlayerDataSubset>(message);
			PlayerName = playerDataSubset?.Nickname;
			if (!string.IsNullOrEmpty(playerDataSubset?.PlayerEmblem))
			{
				PlayerData = messageSerializer.DeserializeObject<PlayerEmblem>(playerDataSubset.PlayerEmblem);
			}
			OnRefreshCall?.Invoke(this);
		}
	}

	public void ToggleLikeLocal()
	{
		if (LikeStatusLocal == null)
		{
			LikeStatusLocal = new SkillSuggestionLikeStatus
			{
				Count = (LikeStatusNet?.Count ?? 0),
				HasLiked = (LikeStatusNet?.HasLiked ?? false)
			};
		}
		LikeStatusLocal.HasLiked = !LikeStatusLocal.HasLiked;
		LikeStatusLocal.Count += (LikeStatusLocal.HasLiked ? 1 : (-1));
		OnRefreshCall?.Invoke(this);
		_likeSyncCts?.Cancel();
		_likeSyncCts?.Dispose();
		_likeSyncCts = new CancellationTokenSource();
		SyncLikeToServerDelayed(_likeSyncCts.Token);
	}

	private async void SyncLikeToServerDelayed(CancellationToken token)
	{
		try
		{
			await Task.Delay(10000, token);
			if (!token.IsCancellationRequested)
			{
				RequestLike();
			}
		}
		catch (OperationCanceledException)
		{
		}
		catch (Exception arg)
		{
			Debug.LogError($"[EquipSkillRecommendEquipModel] Like sync failed: {arg}");
		}
	}

	public void CancelPendingLikeSync()
	{
		_likeSyncCts?.Cancel();
		_likeSyncCts?.Dispose();
		_likeSyncCts = null;
	}

	public bool GetIsChange()
	{
		if (LikeStatusNet == null)
		{
			return false;
		}
		if (LikeStatusNet.HasLiked == LikeStatusLocal.HasLiked)
		{
			return false;
		}
		return true;
	}

	private void RequestLike()
	{
		if (GetIsChange())
		{
			RequestLike(new Dictionary<string, bool> { [equipmentSkillSuggestion.ID] = LikeStatusLocal.HasLiked }, RequestLikeReceived);
		}
	}

	public static void RequestLike(Dictionary<string, bool> dict, SignalREventHandler handler)
	{
		if (dict != null)
		{
			string arg = GameManager.Instance.modelManager.GetMessageSerializer().SerializeObject(dict);
			SignalRClient.Instance.RequestCommand("LikeSkillSuggestion", arg, handler, waitForResponse: true);
		}
	}

	private void RequestLikeReceived(string message)
	{
		if (!CheckError(message))
		{
			Dictionary<string, SkillSuggestionLikeStatus> dictionary = GameManager.Instance.modelManager.GetMessageSerializer().DeserializeObject<Dictionary<string, SkillSuggestionLikeStatus>>(message);
			if (dictionary != null && dictionary.Count > 0)
			{
				SetLikeStatusNet(dictionary.Values.First());
			}
		}
	}

	private bool CheckError(string message)
	{
		SingularityMonoBehaviour<HUDManager>.Instance.CloseIfExists(UIType.IngameLoading);
		if (string.IsNullOrEmpty(message) || message == "null")
		{
			AlertPopup.ShowPopupGetText("Error.Error", "Error.ErrorGeneric", "Button.Ok", null);
			return true;
		}
		return false;
	}

	public static string GetTagText(string tag)
	{
		return "SystemInfo.EquipmentSkillSuggestionTags." + tag;
	}
}
