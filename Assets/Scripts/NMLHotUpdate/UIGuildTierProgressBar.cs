using System;
using BaseModel;
using Client.Tweener;
using TWDModel;
using UnityEngine;

public class UIGuildTierProgressBar : UIProgressBarExtended
{
	public struct GuildUIData
	{
		public string GuildName;

		public int Tier;

		public int CurrentTotalVictoryPoints;

		public int PreviousTotalVictoryPoints;

		public GuildUIData(string guildName, int tier, int currentTotalVictoryPoints, int previousTotalVictoryPoints)
		{
			GuildName = guildName;
			Tier = tier;
			CurrentTotalVictoryPoints = currentTotalVictoryPoints;
			PreviousTotalVictoryPoints = previousTotalVictoryPoints;
		}
	}

	[Header("Is Enemy")]
	[SerializeField]
	private bool isEnemy;

	[SerializeField]
	protected UILabel guildNameLabel;

	[Header("Tier Info")]
	[SerializeField]
	protected UILabel tierNameLabel;

	[SerializeField]
	protected UISprite tierIconSprite;

	[SerializeField]
	protected UILabel nextTierVictoryPointsLabel;

	[SerializeField]
	protected UILabel tierUpLabel;

	[SerializeField]
	protected int tierUpTweenGroup;

	private FixedPoint progressionCurrent;

	private FixedPoint progressionOld;

	private float genricTweenDuration = 1f;

	private GuildTierDefinition shownTier;

	private GuildTierDefinition shownTierTarget;

	private int victoryPointsShown;

	private Callback progressionCompleteCallback;

	private bool isNumberLabelAnimating;

	private GuildUIData guildData;

	private const string nextTierLocalizationString = "GvG.Hub.NextTier{amount}";

	private void SetTier(int tier)
	{
		shownTier = GameManager.Instance.gameEconomyData.GetGuildTierDefinition(tier);
		shownTierTarget = GuildTierHelper.GetNextGuildTier(shownTier.Tier);
		HelpersUI.SetSprite(tierIconSprite, shownTier.IconSprite);
		HelpersUI.SetContentToLabel(tierNameLabel, SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText(shownTier.NameLocalizationKey));
		if (shownTierTarget == null)
		{
			Helpers.GameObjectSetActive(nextTierVictoryPointsLabel, value: false);
			return;
		}
		HelpersUI.SetContentToLabel(nextTierVictoryPointsLabel, SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText("GvG.Hub.NextTier{amount}", shownTierTarget.VictoryPointsRequired - guildData.CurrentTotalVictoryPoints));
	}

	public override void OnEnable()
	{
		base.OnEnable();
		Setup();
	}

	public void Setup()
	{
		if (isEnemy)
		{
			GuildBattleModel currentBattle = GuildWarHelper.GetCurrentBattle();
			if (currentBattle != null)
			{
				guildData = new GuildUIData(currentBattle.EnemyGuildName, currentBattle.EnemyGuildTier, currentBattle.EndEnemyVictoryPoints, currentBattle.EndEnemyVictoryPoints);
			}
		}
		else
		{
			GuildModel guildModel = GameManager.Instance.playerModel.GuildModel;
			if (guildModel != null)
			{
				guildData = new GuildUIData(guildModel.Name, guildModel.GuildBattleTier, guildModel.CurrentVictoryPoints, guildModel.PreviousVictoryPoints);
			}
		}
		Helpers.GameObjectSetActive(tierUpLabel, value: false);
		UpdateToCurrentProgression();
		if (shownTier == null)
		{
			Helpers.GameObjectSetActive(base.gameObject, value: false);
		}
		else
		{
			UpdateUI();
		}
	}

	public void SubscribeToGuildChanges()
	{
		if (!isEnemy && GameManager.Instance.playerModel.GuildModel != null)
		{
			GameManager.Instance.playerModel.GuildModel.Changed += OnGuildChanged;
		}
	}

	public override void OnDisable()
	{
		base.OnDisable();
		if (GameManager.Instance.playerModel.GuildModel != null)
		{
			GameManager.Instance.playerModel.GuildModel.Changed -= OnGuildChanged;
		}
		Clear();
	}

	public void OnComplete(Callback callback)
	{
		progressionCompleteCallback = (Callback)Delegate.Remove(progressionCompleteCallback, callback);
		progressionCompleteCallback = (Callback)Delegate.Combine(progressionCompleteCallback, callback);
	}

	public void PlayProgressionUpdate()
	{
		if (base.gameObject != null && base.gameObject.activeInHierarchy)
		{
			Helpers.GameObjectSetActive(tierUpLabel, value: false);
			UpdateToOldProgression();
			AnimatePointsLabel();
			AnimateProgressBar();
		}
	}

	private void AnimateProgressBar()
	{
		int tier = shownTier.Tier;
		int tier2 = guildData.Tier;
		if (tier > tier2)
		{
			TweenToProgress(1f, progressBar.value, genricTweenDuration);
			return;
		}
		victoryPointsShown = guildData.CurrentTotalVictoryPoints;
		float progressionPercentage = GetProgressionPercentage();
		TweenToProgress(progressionPercentage, progressBar.value, genricTweenDuration, Easing.All.CubicEaseOut);
	}

	private void AnimatePointsLabel()
	{
		if (!isNumberLabelAnimating)
		{
			float duration = genricTweenDuration * (float)(shownTier.Tier - guildData.Tier + 1);
			HelpersUI.AnimateLabel(progressBarLabel, int.Parse(progressBarLabel.text), guildData.CurrentTotalVictoryPoints, duration);
			isNumberLabelAnimating = true;
		}
	}

	public override void UpdateUI()
	{
		base.UpdateUI();
		SetProgress(GetProgressionPercentage());
		if (!isNumberLabelAnimating)
		{
			SetTextToLabel(victoryPointsShown.ToString());
		}
		HelpersUI.SetContentToLabel(guildNameLabel, GameManager.Instance.GetFilteredText(guildData.GuildName));
		HelpersUI.SetSprite(tierIconSprite, shownTier.IconSprite);
		HelpersUI.SetContentToLabel(tierNameLabel, SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText(shownTier.NameLocalizationKey));
		if (shownTierTarget == null)
		{
			Helpers.GameObjectSetActive(nextTierVictoryPointsLabel, value: false);
			return;
		}
		HelpersUI.SetContentToLabel(nextTierVictoryPointsLabel, SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText("GvG.Hub.NextTier{amount}", shownTierTarget.VictoryPointsRequired - guildData.CurrentTotalVictoryPoints));
	}

	public void UpdateToOldProgression()
	{
		SetPreviousValues();
		base.CurrentTweener = null;
		isNumberLabelAnimating = false;
		UpdateUI();
	}

	public void UpdateToCurrentProgression()
	{
		SetCurrentValues();
		base.CurrentTweener = null;
		isNumberLabelAnimating = false;
		UpdateUI();
	}

	public override void Clear()
	{
		base.Clear();
	}

	protected override void OnEasingComplete()
	{
		base.OnEasingComplete();
		if (progressBar.value >= 0.99f && shownTierTarget != null)
		{
			OnTierCompleted();
			return;
		}
		UpdateUI();
		if (progressionCompleteCallback != null)
		{
			progressionCompleteCallback();
		}
	}

	private float GetProgressionPercentage()
	{
		if (shownTierTarget == null)
		{
			return 1f;
		}
		return Mathf.Clamp01((float)(victoryPointsShown - shownTier.VictoryPointsRequired) / (float)(shownTierTarget.VictoryPointsRequired - shownTier.VictoryPointsRequired));
	}

	private void OnTierCompleted()
	{
		Helpers.GameObjectSetActive(tierUpLabel, value: true);
		TweenManager.PlayTweenGroup(base.gameObject, tierUpTweenGroup);
		SetTier(shownTierTarget.Tier);
		SetProgress(0f);
		AnimateProgressBar();
		UIEvent.Send("OnGuildTierIncreased");
	}

	private void SetCurrentValues()
	{
		SetTier(guildData.Tier);
		victoryPointsShown = guildData.CurrentTotalVictoryPoints;
	}

	private void SetPreviousValues()
	{
		SetTier(guildData.Tier);
		victoryPointsShown = guildData.PreviousTotalVictoryPoints;
		if (victoryPointsShown < shownTier.VictoryPointsRequired)
		{
			SetTier(GameManager.Instance.gameEconomyData.GetGuildTierForVictoryPoints(victoryPointsShown).Tier);
		}
	}

	public void OnGuildChanged(GroupModelBase model, string changed, object args)
	{
		if (changed == "VictoryPointsChanged")
		{
			PlayProgressionUpdate();
		}
	}
}
