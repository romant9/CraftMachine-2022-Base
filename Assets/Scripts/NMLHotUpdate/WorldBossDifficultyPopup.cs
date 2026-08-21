using System;
using System.Collections;
using System.Collections.Generic;
using BaseModel;
using Client.Connectivity;
using TWDModel;
using UnityEngine;

public class WorldBossDifficultyPopup : HUDElement
{
	private const string TitleLocalizationKey = "World.Boss.CYCLEDIFFICULTY";

	public const string DifficultyItemPrefabName = "WorldBoss_Difficulty_Item";

	[SerializeField]
	private UILabel titleLabel;

	[SerializeField]
	private UILabel desTxt;

	[SerializeField]
	private UILabel timerTxt;

	[SerializeField]
	private UILabel bottomTxt;

	[SerializeField]
	private GameObject loadingContainer;

	[SerializeField]
	private NUIScrollableList scrollableList;

	private WorldBossDifficultyData selectedDifficulty;

	private WorldBossDifficultySubItem selectedSubItem;

	private WorldBossDifficultyItemItem selectedSubItemView;

	private WorldBossDifficultyData pendingDifficulty;

	private WorldBossDifficultySubItem pendingSubItem;

	private WorldBossDifficultyItemItem pendingSubItemView;

	private WorldBossGuildFullSnapshot worldBossGuildFullSnapshot;

	protected string timeLabelLocalisation = "";

	private const long delayOnCompleteMillisec = -1000L;

	protected long gameModeTimeLeft = -1000L;

	private bool isCanSelectDifficulty;

	private int selectedDifficultyLevel;

	private int maxUnlockedDifficulty;

	private long difficultySelectionCooldownMs;

	private const long difficultySelectionCooldownMilliseconds = 300000L;

	private bool closeWhenCycleStarts;

	private Coroutine restoreScrollCoroutine;

	public override void Open()
	{
		base.Open();
		WorldBossModelManager worldBossModelManager = GameManager.Instance?.playerModel?.WorldBossModelManager;
		closeWhenCycleStarts = worldBossModelManager == null || !worldBossModelManager.IsCycleOpen();
		Helpers.GameObjectSetActive(loadingContainer, value: false);
		GetWorldBossFullSnapshot();
	}

	public void Awake()
	{
		if (SignalRClient.Instance != null)
		{
			SignalRClient.Instance.OnWorldBossFullSnapshotMessage -= OnWorldBossFullSnapshotChanged;
			SignalRClient.Instance.OnWorldBossFullSnapshotMessage += OnWorldBossFullSnapshotChanged;
		}
	}

	private void OnDestroy()
	{
		if (SignalRClient.Instance != null)
		{
			SignalRClient.Instance.OnWorldBossFullSnapshotMessage -= OnWorldBossFullSnapshotChanged;
		}
	}

	private void OnWorldBossFullSnapshotChanged(string message, string type)
	{
		if (IsPopupAlive())
		{
			GetWorldBossFullSnapshot();
		}
	}

	private bool IsPopupAlive()
	{
		if (this != null && base.gameObject != null)
		{
			return base.IsOpen;
		}
		return false;
	}

	private void GetWorldBossFullSnapshot()
	{
		if (GameManager.Instance.playerModel != null && GameManager.Instance.playerModel.WorldBossModelManager != null)
		{
			WorldBossGetSnapshotRequest worldBossGetSnapshotRequest = null;
			if (GameManager.Instance.playerModel.WorldBossModelManager.IsOffSeason())
			{
				worldBossGetSnapshotRequest = new WorldBossGetSnapshotRequest
				{
					GroupId = GameManager.Instance.playerModel.GuildId,
					SeasonId = GameManager.Instance.playerModel.WorldBossModelManager.GetCurrentSeasonId(),
					CycleId = GameManager.Instance.playerModel.WorldBossModelManager.GetNextCycleId()
				};
				timeLabelLocalisation = "World.Boss.NextCycle.Countdown";
				gameModeTimeLeft = GameManager.Instance.playerModel.WorldBossModelManager.GetTimeUntilNextCycleStartMs();
			}
			else
			{
				worldBossGetSnapshotRequest = new WorldBossGetSnapshotRequest
				{
					GroupId = GameManager.Instance.playerModel.GuildId,
					SeasonId = GameManager.Instance.playerModel.WorldBossModelManager.GetCurrentSeasonId(),
					CycleId = GameManager.Instance.playerModel.WorldBossModelManager.GetCurrentCycleId()
				};
				timeLabelLocalisation = "World.Boss.Countdown";
				gameModeTimeLeft = GameManager.Instance.playerModel.WorldBossModelManager.GetTimeUntilCycleEndMs();
			}
			string arg = GameManager.Instance.jsonSerializer.Serialize(worldBossGetSnapshotRequest);
			SignalRClient.Instance.RequestCommand("WorldBossFullSnapshot", arg, OnWorldBossFullSnapshotAsync, waitForResponse: true);
		}
	}

	private void OnWorldBossFullSnapshotAsync(string responseJson)
	{
		Helpers.GameObjectSetActive(loadingContainer, value: false);
		if (SignalRClient.Instance.HasError || string.IsNullOrEmpty(responseJson))
		{
			SignalRClient.Instance.ClearError();
			return;
		}
		worldBossGuildFullSnapshot = GameManager.Instance.jsonSerializer.Deserialize<WorldBossGuildFullSnapshot>(responseJson);
		if (worldBossGuildFullSnapshot != null)
		{
			selectedDifficultyLevel = worldBossGuildFullSnapshot.GuildFullState?.Difficulty ?? 0;
			maxUnlockedDifficulty = worldBossGuildFullSnapshot.MaxUnlockedDifficulty;
			difficultySelectionCooldownMs = worldBossGuildFullSnapshot.GuildFullState?.DifficultySelectedAtUtcMs ?? 0;
			GameManager.Instance.modelManager.SetWorldBossGuildFullSnapshot(worldBossGuildFullSnapshot);
			if (IsPopupAlive())
			{
				CheckCanSelectDifficulty();
				RebuildDifficultyList();
				UpdateUI();
			}
		}
	}

	public override void UpdateUI()
	{
		if (!IsPopupAlive())
		{
			return;
		}
		base.UpdateUI();
		int currentSeasonId = GameManager.Instance.playerModel.WorldBossModelManager.GetCurrentSeasonId();
		if (titleLabel != null)
		{
			HelpersUI.SetContentToLabel(titleLabel, LocalizationManager.GetText("World.Boss.CYCLEDIFFICULTY"));
		}
		if (desTxt != null)
		{
			WorldBossDifficultyDefinition worldBossDifficultyDefinition = GameManager.Instance.gameEconomyData.FindWorldBossDifficultyDefinition(currentSeasonId, selectedDifficultyLevel);
			string text = LocalizationManager.GetText(worldBossDifficultyDefinition.Localization);
			if (worldBossDifficultyDefinition != null)
			{
				if (GameManager.Instance.playerModel.WorldBossModelManager.IsOffSeason())
				{
					HelpersUI.SetContentToLabel(desTxt, LocalizationManager.GetText("World.Boss.NextCycleDiffic", text));
				}
				else
				{
					HelpersUI.SetContentToLabel(desTxt, LocalizationManager.GetText("World.Boss.ThisCycleDiffic", text));
				}
			}
		}
		if (bottomTxt != null)
		{
			WorldBossDifficultyDefinition worldBossDifficultyDefinition2 = GameManager.Instance.gameEconomyData.FindWorldBossDifficultyDefinition(currentSeasonId, maxUnlockedDifficulty);
			if (worldBossDifficultyDefinition2 != null)
			{
				Helpers.GameObjectSetActive(bottomTxt.gameObject, value: true);
				string text2 = LocalizationManager.GetText(worldBossDifficultyDefinition2.Localization);
				HelpersUI.SetContentToLabel(bottomTxt, LocalizationManager.GetText("World.Boss.NextDifficUnlockNeed", text2, worldBossDifficultyDefinition2.PassScore));
			}
			else
			{
				Helpers.GameObjectSetActive(bottomTxt.gameObject, value: false);
			}
		}
		UpdateSelectionVisuals();
	}

	private static string FormatTimeLeft(long timeLeft)
	{
		if (timeLeft <= 0)
		{
			return "0";
		}
		return Helpers.FormatTimeNoZero(timeLeft);
	}

	private static string FormatMinutesSeconds(long milliSeconds)
	{
		milliSeconds = Math.Max(0L, milliSeconds);
		int num = (int)(milliSeconds / 1000);
		int num2 = num / 60;
		int num3 = num % 60;
		return $"{num2:00}:{num3:00}";
	}

	public override void Update()
	{
		base.Update();
		if (closeWhenCycleStarts && WorldBossOffSeasonPopup.ClosePreBattlePopupsIfCycleStarted())
		{
			return;
		}
		if (gameModeTimeLeft > -1000)
		{
			gameModeTimeLeft -= (long)(Time.deltaTime * 1000f);
			if (timerTxt != null)
			{
				HelpersUI.SetContentToLabel(timerTxt, string.Concat(LocalizationManager.GetText(timeLabelLocalisation, FormatTimeLeft(gameModeTimeLeft))));
			}
		}
		if (gameModeTimeLeft <= -1000)
		{
			gameModeTimeLeft = -1001L;
		}
	}

	private List<WorldBossDifficultyData> GetDifficultyList()
	{
		List<WorldBossDifficultyData> list = new List<WorldBossDifficultyData>();
		GameEconomyData gameEconomyData = GameManager.Instance?.gameEconomyData;
		if (gameEconomyData?.WorldBossDifficultyDefinitions == null)
		{
			return list;
		}
		Dictionary<int, List<WorldBossDifficultyDefinition>> dictionary = new Dictionary<int, List<WorldBossDifficultyDefinition>>();
		for (int i = 0; i < gameEconomyData.WorldBossDifficultyDefinitions.Length; i++)
		{
			WorldBossDifficultyDefinition worldBossDifficultyDefinition = gameEconomyData.WorldBossDifficultyDefinitions[i];
			if (worldBossDifficultyDefinition != null)
			{
				if (!dictionary.TryGetValue(worldBossDifficultyDefinition.DifficultyClass, out var value))
				{
					value = new List<WorldBossDifficultyDefinition>();
					dictionary.Add(worldBossDifficultyDefinition.DifficultyClass, value);
				}
				value.Add(worldBossDifficultyDefinition);
			}
		}
		List<int> list2 = new List<int>(dictionary.Keys);
		list2.Sort();
		for (int j = 0; j < list2.Count; j++)
		{
			int num = list2[j];
			List<WorldBossDifficultyDefinition> list3 = dictionary[num];
			list3.Sort((WorldBossDifficultyDefinition a, WorldBossDifficultyDefinition b) => a.Difficulty.CompareTo(b.Difficulty));
			WorldBossDifficultySubItem[] array = new WorldBossDifficultySubItem[list3.Count];
			for (int num2 = 0; num2 < list3.Count; num2++)
			{
				WorldBossDifficultyDefinition difficultyDefinition = list3[num2];
				array[num2] = new WorldBossDifficultySubItem
				{
					des = ToRomanNumeral(num2 + 1),
					maxDifficulty = maxUnlockedDifficulty,
					difficultyDefinition = difficultyDefinition
				};
			}
			list.Add(new WorldBossDifficultyData
			{
				difficultyName = LocalizationManager.GetText(GetDifficultyClassLocalizationKey(num)),
				items = array
			});
		}
		return list;
	}

	private static string GetDifficultyClassLocalizationKey(int difficultyClass)
	{
		return "World.Boss.Difficulty." + difficultyClass;
	}

	private static string ToRomanNumeral(int number)
	{
		if (number <= 0)
		{
			return string.Empty;
		}
		string[] array = new string[10] { "", "I", "II", "III", "IV", "V", "VI", "VII", "VIII", "IX" };
		return (new string[10] { "", "X", "XX", "XXX", "XL", "L", "LX", "LXX", "LXXX", "XC" })[number % 100 / 10] + array[number % 10];
	}

	private void SelectDefaultDifficulty(List<WorldBossDifficultyData> difficultyList)
	{
		selectedDifficulty = null;
		selectedSubItem = null;
		if (selectedDifficultyLevel > 0)
		{
			for (int i = 0; i < difficultyList.Count; i++)
			{
				WorldBossDifficultyData worldBossDifficultyData = difficultyList[i];
				if (worldBossDifficultyData?.items == null)
				{
					continue;
				}
				for (int j = 0; j < worldBossDifficultyData.items.Length; j++)
				{
					WorldBossDifficultySubItem worldBossDifficultySubItem = worldBossDifficultyData.items[j];
					if (worldBossDifficultySubItem?.difficultyDefinition != null && worldBossDifficultySubItem.difficultyDefinition.Difficulty == selectedDifficultyLevel)
					{
						selectedDifficulty = worldBossDifficultyData;
						selectedSubItem = worldBossDifficultySubItem;
						return;
					}
				}
			}
		}
		selectedDifficulty = ((difficultyList.Count > 0) ? difficultyList[0] : null);
		selectedSubItem = ((selectedDifficulty?.items != null && selectedDifficulty.items.Length != 0) ? selectedDifficulty.items[0] : null);
	}

	private void RebuildDifficultyList()
	{
		if (scrollableList == null)
		{
			return;
		}
		bool flag = scrollableList.currentItemsCount > 0;
		Vector3 savedPanelLocalPosition = (flag ? scrollableList.GetCurrentScrollPanelLocalPosition() : Vector3.zero);
		scrollableList.Clear();
		List<WorldBossDifficultyData> difficultyList = GetDifficultyList();
		for (int i = 0; i < difficultyList.Count; i++)
		{
			WorldBossDifficultyItem worldBossDifficultyItem = scrollableList.InstantiateAdd("WorldBoss_Difficulty_Item") as WorldBossDifficultyItem;
			if (!(worldBossDifficultyItem == null))
			{
				worldBossDifficultyItem.SetData(difficultyList[i]);
				worldBossDifficultyItem.SetSubItemClickCallback(OnSubItemClicked);
			}
		}
		SelectDefaultDifficulty(difficultyList);
		scrollableList.SortAndRepositionItems();
		if (flag)
		{
			if (restoreScrollCoroutine != null)
			{
				StopCoroutine(restoreScrollCoroutine);
			}
			restoreScrollCoroutine = StartCoroutine(RestoreScrollPanelLocalPositionAfterLayout(savedPanelLocalPosition));
		}
		else
		{
			StartCoroutine(ResetScrollPositionNextFrame());
		}
	}

	private IEnumerator ResetScrollPositionNextFrame()
	{
		yield return null;
		if (scrollableList != null)
		{
			scrollableList.ResetScrollPosition();
		}
		UpdateSelectionVisuals();
	}

	private IEnumerator RestoreScrollPanelLocalPositionAfterLayout(Vector3 savedPanelLocalPosition)
	{
		yield return null;
		restoreScrollCoroutine = null;
		if (!(this == null) && !(scrollableList == null))
		{
			scrollableList.RestoreScrollPanelLocalPosition(savedPanelLocalPosition);
			UpdateSelectionVisuals();
		}
	}

	private void CheckCanSelectDifficulty()
	{
		isCanSelectDifficulty = false;
		if (worldBossGuildFullSnapshot == null)
		{
			return;
		}
		PlayerModel playerModel = GameManager.Instance?.playerModel;
		if (playerModel != null && playerModel.IsGuildMember)
		{
			WorldBossModelManager worldBossModelManager = playerModel.WorldBossModelManager;
			if (worldBossModelManager != null && worldBossModelManager.IsCurrentCycleDifficultySelectionOpen())
			{
				GuildMemberInfo guildMemberInfo = playerModel.GuildModel?.GetMemberInfo(playerModel.HashedId);
				isCanSelectDifficulty = guildMemberInfo != null && guildMemberInfo.Role >= GuildMemberRole.Elder;
			}
		}
	}

	private void OnSubItemClicked(WorldBossDifficultyData difficulty, WorldBossDifficultySubItem subItem, WorldBossDifficultyItemItem subItemView)
	{
		if (subItem?.difficultyDefinition == null)
		{
			return;
		}
		pendingDifficulty = difficulty;
		pendingSubItem = subItem;
		pendingSubItemView = subItemView;
		string text = LocalizationManager.GetText("World.Boss.DifficChoose");
		string text2 = LocalizationManager.GetText(subItem.difficultyDefinition.Localization);
		string text3 = LocalizationManager.GetText("World.Boss.DifficChangeAsk", text2);
		WorldBossModelManager worldBossModelManager = GameManager.Instance?.playerModel?.WorldBossModelManager;
		if (worldBossModelManager == null || !worldBossModelManager.IsCurrentCycleDifficultySelectionOpen())
		{
			HUDNotification.Info(LocalizationManager.GetText("World.Boss.ChangeDiffcEnd.Tips"));
			return;
		}
		CheckCanSelectDifficulty();
		if (!isCanSelectDifficulty)
		{
			subItemView?.ShowTime(LocalizationManager.GetText("Generic.Locked"));
		}
		else
		{
			if (maxUnlockedDifficulty < pendingSubItem.difficultyDefinition.Difficulty)
			{
				return;
			}
			long num = GameManager.Instance.playerModel.UtcTimeStamp - difficultySelectionCooldownMs;
			if (num < 300000)
			{
				FormatMinutesSeconds(300000 - num);
				subItemView?.ShowTime(LocalizationManager.GetText("World.Boss.ChangeDiffc.CD"));
				return;
			}
			WorldBossDifficultyConfirmPopup worldBossDifficultyConfirmPopup = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.WorldBossDifficultyConfirmPopup) as WorldBossDifficultyConfirmPopup;
			if (!(worldBossDifficultyConfirmPopup == null))
			{
				worldBossDifficultyConfirmPopup.Setup(text, text3, OnSubItemConfirmed, OnSubItemCancelled, pendingSubItem.difficultyDefinition.Difficulty);
				worldBossDifficultyConfirmPopup.Open();
			}
		}
	}

	private void OnSubItemConfirmed()
	{
		if (Helpers.ExecuteCommand(new WorldBossSelectDifficultyCommand(GameManager.Instance.playerModel.WorldBossModelManager.GetCurrentSeasonId(), GameManager.Instance.playerModel.WorldBossModelManager.GetNextCycleId(), pendingSubItem.difficultyDefinition.Difficulty)) == TWDModelResult.OK)
		{
			string text = LocalizationManager.GetText(pendingSubItem.difficultyDefinition.Localization);
			string text2 = LocalizationManager.GetText("World.Boss.ChangeDiffc.title");
			string text3 = LocalizationManager.GetText("World.Boss.ChangeDiffc.desc", text);
			GameManager.Instance.GuildManager.SendChatMessage(text2 + text3);
			difficultySelectionCooldownMs = GameManager.Instance.playerModel.UtcTimeStamp;
			selectedDifficulty = pendingDifficulty;
			selectedSubItem = pendingSubItem;
			selectedSubItemView = pendingSubItemView;
			pendingDifficulty = null;
			pendingSubItem = null;
			pendingSubItemView = null;
			UpdateSelectionVisuals();
		}
	}

	private void OnSubItemCancelled()
	{
		pendingDifficulty = null;
		pendingSubItem = null;
		pendingSubItemView = null;
	}

	private void UpdateSelectionVisuals()
	{
		if (scrollableList == null || scrollableList.currentItemsList == null)
		{
			return;
		}
		for (int i = 0; i < scrollableList.currentItemsList.Count; i++)
		{
			WorldBossDifficultyItem worldBossDifficultyItem = scrollableList.currentItemsList[i] as WorldBossDifficultyItem;
			if (worldBossDifficultyItem != null)
			{
				worldBossDifficultyItem.UpdateSubItemSelection(selectedDifficulty, selectedSubItem);
			}
		}
	}
}
