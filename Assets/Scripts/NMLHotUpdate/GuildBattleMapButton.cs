using System;
using System.Collections.Generic;
using System.Linq;
using TWDModel;
using UnityEngine;

public class GuildBattleMapButton : MapGridItem
{
	private enum State
	{
		None = 0,
		Locked = 1,
		Unlocked = 2,
		Complete = 3
	}

	public UILabel[] SectorName;

	[Space(10f)]
	public GameObject LineParent;

	[Header("Parents For States")]
	public GameObject ParentLocked;

	public GameObject ParentUnlocked;

	public GameObject ParentComplete;

	[Header("Tints For States")]
	public Color TintLocked;

	public Color TintUnlocked;

	public Color TintComplete;

	[NonSerialized]
	public UITexture[] UITextures;

	[Header("Rewards")]
	public GameObject RewardParent;

	public UILabel VPRewardLabel;

	public UISprite VPRewardIcon;

	public GuildBattleRewardBonus BonusReward;

	public GuildBattleRewardCurrencyBonus BonusCurrencyReward;

	[Header("Difficulty")]
	public UILabel DifficultyLabel;

	[Header("Enemy")]
	public UILabel EnemyLabel;

	[Header("ActivityIndicator")]
	[SerializeField]
	private GuildBattleActivityIndicator activityDot;

	[Header("Player created indicators")]
	[SerializeField]
	private GameObject playerEmblemPrefab;

	[Header("StateChangeEffects")]
	[SerializeField]
	private GameObject sectorUnlockEffect;

	[SerializeField]
	private GameObject sectorCompleteEffect;

	[SerializeField]
	private GameObject collectAnimVP;

	private UIDragCamera dragCamera;

	private UIButtonExtended button;

	private GuildBattleMapButtonLines[] linesArray;

	private Vector3[] linesPosition;

	private GuildBattleMapLineAssets lineAssets;

	[Header("Sector started but not complete effect")]
	[SerializeField]
	private GameObject sectorStartedButNotCompletedEffectContainer;

	private GameObject[] StatesParents;

	private Color[] StatesTints;

	private State currentState;

	private bool isVisible = true;

	private int sectorCompleteAnimationTweenGroup = 3;

	private UIWidget widget;

	private BoxCollider boxCollider;

	private Dictionary<string, PlayerEmblemIcon> emblemsPerPlayer = new Dictionary<string, PlayerEmblemIcon>();

	public GuildBattleMapSectorModel Model { get; set; }

	public UIDragCamera DragCamera
	{
		get
		{
			if (dragCamera == null)
			{
				dragCamera = GetComponent<UIDragCamera>();
			}
			if (dragCamera == null && base.gameObject != null)
			{
				dragCamera = base.gameObject.AddComponent<UIDragCamera>();
			}
			return dragCamera;
		}
	}

	public UIButtonExtended Button
	{
		get
		{
			if (button == null)
			{
				button = GetComponent<UIButtonExtended>();
			}
			return button;
		}
	}

	public void Awake()
	{
		StatesParents = new GameObject[4] { ParentLocked, ParentLocked, ParentUnlocked, ParentComplete };
		StatesTints = new Color[4] { TintLocked, TintLocked, TintUnlocked, TintComplete };
		widget = GetComponent<UIWidget>();
		boxCollider = GetComponent<BoxCollider>();
		Helpers.GameObjectSetActive(sectorCompleteEffect, value: false);
		Helpers.GameObjectSetActive(sectorUnlockEffect, value: false);
	}

	public bool IsUnlocked()
	{
		if (currentState != State.Unlocked)
		{
			if (HelpersModel.IsUnlockPVP) return true;
			return currentState == State.Complete;
		}
		return true;
	}

	public void UpdateUI()
	{
		if (Model == null && Button != null)
		{
			return;
		}
		SubscribeForGuildModelEvents();
		bool num = GuildWarHelper.IsBattleOnGoing();
		State state = State.None;
		if (!num)
		{
			state = State.Unlocked;
		}
		else if (!Model.IsCompleted())
		{
			state = ((!Model.CanBeUnlocked(GameManager.Instance.playerModel.GuildModel.GuildWarModel.CurrentBattle.CurrentMapModel)) ? State.Locked : State.Unlocked);
		}
		else
		{
			state = State.Complete;
			if (GameManager.Instance.playerModel.GvGSeasonModelPlayer.GuildWarModelPlayer.GuildBattleModel.PersonalSectorRewards.ContainsKey(Model.SectorId))
			{
				SingularityMonoBehaviour<GuildWarManager>.Instance.ClaimSectorReward(Model.SectorId);
			}
		}
		if (currentState != state)
		{
			currentState = state;
			SetParentAndTintForState(currentState);
			CheckCompletionAnimation();
		}
		if (currentState == State.Unlocked || currentState == State.Complete)
		{
			UpdateSectorActivity();
		}
		sectorStartedButNotCompletedEffectContainer.SetActive(Model.IsStartedButNotComplete());
		SetTextsForState(currentState);

		if (OfflineManager.IsNoEffects)
		{
			var particles = this.GetComponentsInChildren<ParticleSystem>(true);
			if (particles != null)
			{
				foreach (var part in particles)
				{
					part.gameObject.SetActive(false);
				}
			}
		}
	}

	private void SubscribeForGuildModelEvents()
	{
		GuildBattleModel currentBattle = GuildWarHelper.GetCurrentBattle();
		if (currentBattle != null && currentBattle.CurrentMapModel != null)
		{
			currentBattle.CurrentMapModel.Changed -= OnBattleMapModelChange;
			currentBattle.CurrentMapModel.Changed += OnBattleMapModelChange;
			currentBattle.Changed -= OnBattleMapModelChange;
			currentBattle.Changed += OnBattleMapModelChange;
		}
		GuildWarModel guildWarModel = GuildWarHelper.GetGuildWarModel();
		if (guildWarModel != null)
		{
			guildWarModel.Changed -= OnBattleMapModelChange;
			guildWarModel.Changed += OnBattleMapModelChange;
		}
	}

	public void Select()
	{
		GameObject parent = OfflineManager.IsLoadDataManager ? HUDManager.Instance.UIContainerTopCameras : null;
		GuildBattleSelectMissionPopup guildBattleSelectMissionPopup = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.GuildBattleSelectMissionPopup, parent) as GuildBattleSelectMissionPopup;
		if (guildBattleSelectMissionPopup != null)
		{
			DebugTWD.Log("OnClick Sector Button " + Model.SectorId, DebugType.OnClick);
			guildBattleSelectMissionPopup.SectorId = Model.SectorId;
			guildBattleSelectMissionPopup.Open();
		}
	}

	public void ZoomSelect(bool zoomIn, bool instant = false)
	{
		Helpers.GameObjectSetActive(LineParent, !zoomIn);
		isVisible = !zoomIn;
		TweenAlpha tweenAlpha = Helpers.AddComponent<TweenAlpha>(base.gameObject);
		if (tweenAlpha != null)
		{
			tweenAlpha.from = (zoomIn ? 1f : 0f);
			tweenAlpha.to = ((!zoomIn) ? 1f : 0f);
			if (widget != null && widget.alpha == tweenAlpha.to)
			{
				return;
			}
			tweenAlpha.duration = 0.3f;
			tweenAlpha.ResetToBeginning();
			tweenAlpha.PlayForward();
		}
		if (isVisible)
		{
			CheckCompletionAnimation();
		}
	}

	public void SetLineData(GuildBattleMapLineAssets lineAssets)
	{
		this.lineAssets = lineAssets;
	}

	public override void Clear()
	{
		base.Clear();
		GuildBattleModel currentBattle = GuildWarHelper.GetCurrentBattle();
		if (currentBattle != null && currentBattle.CurrentMapModel != null)
		{
			currentBattle.CurrentMapModel.Changed -= OnBattleMapModelChange;
			currentBattle.Changed -= OnBattleMapModelChange;
		}
		GuildWarModel guildWarModel = GuildWarHelper.GetGuildWarModel();
		if (guildWarModel != null)
		{
			guildWarModel.Changed -= OnBattleMapModelChange;
		}
		if (button != null)
		{
			button.Clear();
		}
		if (base.OwnerGrid != null)
		{
			base.OwnerGrid.RemoveOnPositionCallback(OnPositionDone);
		}
		for (int i = 0; i < ((linesArray != null) ? linesArray.Length : 0); i++)
		{
			if (linesArray[i] != null)
			{
				Helpers.DestroyOrCache(linesArray[i]);
			}
			linesArray[i] = null;
		}
		currentState = State.None;
		lineAssets = null;
		Model = null;
		for (int j = 0; j < ((UITextures != null) ? UITextures.Length : 0); j++)
		{
			UITextures[j] = null;
		}
		if (emblemsPerPlayer == null)
		{
			return;
		}
		foreach (KeyValuePair<string, PlayerEmblemIcon> item in emblemsPerPlayer)
		{
			if (!(item.Value == null))
			{
				Helpers.DestroyOrCache(item.Value);
			}
		}
		emblemsPerPlayer.Clear();
	}

	public override void AddedToGrid(MapGrid grid)
	{
		base.AddedToGrid(grid);
		if (base.OwnerGrid != null)
		{
			base.OwnerGrid.AddOnPositionCallback(OnPositionDone);
		}
	}

	[ContextMenu("UpdateConnectingLines")]
	public void UpdateConnectingLines()
	{
		if (Model == null || Model.Prerequisites == null || base.OwnerGrid == null)
		{
			return;
		}
		bool flag = GuildWarHelper.IsBattleOnGoing();
		bool flag2 = Model.CanBeUnlocked(GuildWarHelper.GetCurrentMapModel());
		bool flag3 = false;
		GuildBattleMapButtonLines guildBattleMapButtonLines = null;
		GuildBattleSectorDefinition guildBattleSectorDefinition = null;
		Vector3 position = base.transform.position;
		bool allPrerequisitesMustBeCompleted = Model.MissionSectorDefinition.AllPrerequisitesMustBeCompleted;
		int num = Model.Prerequisites.Length + (allPrerequisitesMustBeCompleted ? 1 : 0);
		if (linesArray == null)
		{
			linesArray = new GuildBattleMapButtonLines[num];
		}
		if (linesPosition == null)
		{
			linesPosition = new Vector3[2];
		}
		if (allPrerequisitesMustBeCompleted)
		{
			for (int i = 0; i < Model.Prerequisites.Length; i++)
			{
				Vector3 position2 = (base.OwnerGrid.GetAt(Model.Prerequisites[i].MapIconConfig.x, Model.Prerequisites[i].MapIconConfig.y) as GuildBattleMapButton).transform.position;
				position += position2;
			}
			position /= (float)num;
		}
		for (int j = 0; j < num; j++)
		{
			if (linesArray[j] == null)
			{
				linesArray[j] = Helpers.InstantiateWithComponent<GuildBattleMapButtonLines>(lineAssets.LineRendererPrefab, LineParent);
				linesArray[j].transform.localPosition = Helpers.staticVector3Zero;
				linesArray[j].transform.localScale = Helpers.staticVector3One;
			}
			if (j < num)
			{
				guildBattleMapButtonLines = linesArray[j];
			}
			guildBattleSectorDefinition = ((Model.Prerequisites.Length <= j) ? Model.MissionSectorDefinition : Model.Prerequisites[j]);
			if (guildBattleSectorDefinition == null || guildBattleSectorDefinition.MapIconConfig == null)
			{
				continue;
			}
			GuildBattleMapButton guildBattleMapButton = base.OwnerGrid.GetAt(guildBattleSectorDefinition.MapIconConfig.x, guildBattleSectorDefinition.MapIconConfig.y) as GuildBattleMapButton;
			if (guildBattleMapButtonLines != null && guildBattleMapButton != null)
			{
				linesPosition[0] = (allPrerequisitesMustBeCompleted ? guildBattleMapButtonLines.transform.InverseTransformPoint(position) : Helpers.staticVector3Zero);
				linesPosition[1] = guildBattleMapButtonLines.transform.InverseTransformPoint(guildBattleMapButton.transform.position);
				guildBattleMapButtonLines.LineRenderer.useWorldSpace = false;
				guildBattleMapButtonLines.LineRenderer.SetPositions(linesPosition);
				flag3 = guildBattleMapButton.Model.IsCompleted();
				GuildBattleMapButtonLines.State newState = GuildBattleMapButtonLines.State.NoneUnlocked;
				if (!flag)
				{
					newState = GuildBattleMapButtonLines.State.AllUnlocked;
				}
				else if (flag2 && (flag3 || guildBattleMapButton.Model == Model))
				{
					newState = GuildBattleMapButtonLines.State.AllUnlocked;
				}
				else if (flag3)
				{
					newState = GuildBattleMapButtonLines.State.SomeUnlocked;
				}
				guildBattleMapButtonLines.ChangeState(newState, lineAssets);
				if (OfflineManager.IsLoadDataManager)
				{
					guildBattleMapButtonLines.LineRenderer.sortingOrder = -1;
				}
			}
		}
	}

	public void InitEmblems(GuildBattleModel currentBattle)
	{
		foreach (KeyValuePair<string, GuildBattleModel.GuildBattleIndicatorData> item in currentBattle.GuildBattleEmblemDataPerPlayer)
		{
			UpdatePlayerEmblems(item.Value);
		}
	}

	public void UpdatePlayerEmblems(GuildBattleModel.GuildBattleIndicatorData data)
	{
		if (data == null)
		{
			return;
		}
		bool flag = false;
		if (emblemsPerPlayer.TryGetValue(data.PlayerHashedId, out var value))
		{
			if (data.SectorId != Model.SectorId)
			{
				emblemsPerPlayer.Remove(data.PlayerHashedId);
				Helpers.DestroyOrCache(value);
				return;
			}
		}
		else if (data.SectorId == Model.SectorId)
		{
			value = Helpers.InstantiateToParent(playerEmblemPrefab, base.gameObject).GetComponent<PlayerEmblemIcon>();
			emblemsPerPlayer.Add(data.PlayerHashedId, value);
			GuildModel guildModel = (GameManager.Instance.playerModel.IsGuildMember ? GameManager.Instance.playerModel.GuildModel : null);
			GuildMemberInfo guildMemberInfo = guildModel?.GetMemberInfo(data.PlayerHashedId);
			GuildBattleParticipantInfo guildBattleParticipantInfo = guildModel?.GuildWarModel.CurrentBattle.GetCurrentGuildBattlePlayerInfo(data.PlayerHashedId);
			if (guildMemberInfo != null)
			{
				value.SetEmblem(guildMemberInfo.PlayerEmblem);
			}
			else if (guildBattleParticipantInfo != null)
			{
				value.SetEmblem(guildMemberInfo.PlayerEmblem);
			}
			flag = true;
		}
		Vector3 vector = new Vector3(data.X, data.Y, 0f);
		if (boxCollider != null)
		{
			vector.x = Mathf.Clamp(vector.x, (0f - boxCollider.size.x) * 0.5f, boxCollider.size.x * 0.5f);
			vector.y = Mathf.Clamp(vector.y, (0f - boxCollider.size.y) * 0.5f, boxCollider.size.y * 0.5f);
		}
		if (value != null && (flag || value.transform.localPosition != vector))
		{
			value.transform.localPosition = vector;
			if (flag)
			{
				TweenManager.PlayTweenGroup(value.gameObject, 1);
			}
		}
	}

	private void OnBattleMapModelChange(TWDGroupModelChild model, string changed, object args)
	{
		switch (changed)
		{
		case "GuildBattleNonPvpCompletionAdded":
		case "GuildBattlePvpCompletionAdded":
			UpdateUI();
			UpdateConnectingLines();
			break;
		case "GuildBattleLiveDataUpdated":
		case "GuildBattleEnded":
			UpdateSectorActivity();
			break;
		case "GuildBattleMapIndicatorsUpdated":
		{
			GuildBattleModel.GuildBattleIndicatorData data = args as GuildBattleModel.GuildBattleIndicatorData;
			UpdatePlayerEmblems(data);
			break;
		}
		}
	}

	private void UpdateSectorActivity()
	{
		if (activityDot != null)
		{
			GuildBattleModel currentBattle = GuildWarHelper.GetCurrentBattle();
			if (currentBattle != null)
			{
				activityDot.SectorActivityIndicatorCheck(currentBattle, Model);
			}
		}
	}

	private void SetTextsForState(State state)
	{
		if (Model == null)
		{
			return;
		}
		for (int i = 0; i < SectorName.Length; i++)
		{
			HelpersUI.SetContentToLabel(SectorName[i], HelpersLocalization.GetGuildBattleSectorName(Model));
		}
		int totalDefeatedCount = 0;
		int num = Model.EnemiesDefeatedCount(out totalDefeatedCount);
		string content = $"{totalDefeatedCount}/{num}";
		HelpersUI.SetContentToLabel(EnemyLabel, content);
		int playerSpecificDifficulty = GvGModelHelper.GetPlayerSpecificDifficulty(GameManager.Instance.playerModel);
		int num2 = playerSpecificDifficulty + GameManager.Instance.gameEconomyData.GetGuildBattleSectorMissionDifficulty(Model.SectorId, 0);
		int num3 = playerSpecificDifficulty + GameManager.Instance.gameEconomyData.GetGuildBattleSectorMissionDifficulty(Model.SectorId, 3);
		string text = string.Format("{0}: {1} - {2}", LocalizationManager.GetText("Popup.SurvivalDifficulty.Difficulty"), num2, num3);
		DifficultyLabel.text = text;
		bool flag = state != State.Complete;
		bool flag2 = GuildWarHelper.IsBattleOnGoing();
		if (Helpers.GameObjectSetActive(RewardParent, flag && flag2) && GameManager.Instance.playerModel.GuildWarModel != null && GameManager.Instance.playerModel.GuildWarModel.CurrentBattle != null)
		{
			int num4 = GameManager.Instance.playerModel.GuildWarModel.CurrentBattle.GetGuildSectorBattleVictoryPoints(Model.SectorId);
			RewardGuildBattleVP bonusVPRewardFromSector = GuildWarHelper.GetCurrentBattle().GetBonusVPRewardFromSector(Model.SectorId);
			if (bonusVPRewardFromSector != null)
			{
				num4 += bonusVPRewardFromSector.Amount;
			}
			HelpersUI.SetContentToLabel(VPRewardLabel, num4.ToString());
			HelpersUI.SetSprite(VPRewardIcon, "Ui_Icon_Resource_Vp");
			BonusReward.Model = Model;
			BonusReward.UpdateUI();
			BonusCurrencyReward.Model = Model;
			BonusCurrencyReward.UpdateUI();
		}
	}

	private void SetParentAndTintForState(State state)
	{
		for (int i = 0; i < ((StatesParents != null) ? StatesParents.Length : 0); i++)
		{
			bool flag = state == (State)i;
			Helpers.GameObjectSetActive(StatesParents[i], flag);
			if (!flag)
			{
				continue;
			}
			Color color = ((StatesTints == null || StatesTints.Length <= i) ? Color.white : StatesTints[i]);
			for (int j = 0; j < ((UITextures != null) ? UITextures.Length : 0); j++)
			{
				if (!(UITextures[j] == null))
				{
					UITextures[j].color = color;
				}
			}
		}
	}

	private void CheckCompletionAnimation()
	{
		GuildBattleMapPopup guildBattleMapPopup = SingularityMonoBehaviour<HUDManager>.Instance.GetNoCreation(UIType.GuildBattleMapPopup) as GuildBattleMapPopup;
		if (((guildBattleMapPopup != null) ? guildBattleMapPopup.GetInitState() : GuildBattleMissionButton.InitState.None) != GuildBattleMissionButton.InitState.ReturnFromCombat && isVisible && GuildWarHelper.IsBattleOnGoing())
		{
			GuildBattleProgressSnapshot currentCompletionSnapshot = GameManager.Instance.playerModel.GvGSeasonModelPlayer.GuildWarModelPlayer.GuildBattleModel.CurrentCompletionSnapshot;
			if (Model.Prerequisites == null && currentState == State.Unlocked)
			{
				Helpers.ExecuteCommandDelayed(new GuildBattleMapSectorStateSeenCommand(Model.SectorId.ToString(), (int)currentState));
			}
			else if (currentState == State.Complete && currentCompletionSnapshot.GetSectorStateSeenValue(Model.SectorId.ToString()) != (int)currentState)
			{
				Helpers.ExecuteCommandDelayed(new GuildBattleMapSectorStateSeenCommand(Model.SectorId.ToString(), (int)currentState));
				if (!OfflineManager.IsNoEffects)
					PlaySectorCompleteAnimation();
			}
			else if (currentState == State.Unlocked && currentCompletionSnapshot.GetSectorStateSeenValue(Model.SectorId.ToString()) != (int)currentState)
			{
				Helpers.ExecuteCommandDelayed(new GuildBattleMapSectorStateSeenCommand(Model.SectorId.ToString(), (int)currentState));
				if (!OfflineManager.IsNoEffects)
					PlaySectorUnlockedAnimation();
			}
		}
	}

	private void PlaySectorCompleteAnimation()
	{
		Helpers.GameObjectSetActive(sectorCompleteEffect, value: true);
		SingularityMonoBehaviour<GuildWarManager>.Instance.ShowSectorCompleteVPReward(Model.SectorId, collectAnimVP);
		TweenManager.PlayTweenGroup(base.gameObject, sectorCompleteAnimationTweenGroup);
	}

	private void PlaySectorUnlockedAnimation()
	{
		Helpers.GameObjectSetActive(sectorUnlockEffect, value: true);
		TweenManager.PlayTweenGroup(base.gameObject, sectorCompleteAnimationTweenGroup);
	}

	private void OnPositionDone()
	{
		UpdateUI();
		UpdateConnectingLines();
	}

	private void OnDrag(Vector2 delta)
	{
		dragCamera.OnDrag(delta);
	}
}
