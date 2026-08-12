using System;
using BaseModel;
using TWDModel;
using UnityEngine;

public class BattlePassProgressBar : MonoBehaviour
{
	[SerializeField]
	private UIProgressBar battlePassProgressBar;

	[SerializeField]
	private UIProgressBar dailyKillProgressBar;

	[SerializeField]
	private UILabel nextTierLabel;

	[SerializeField]
	private GameObject maxKillsObject;

	[SerializeField]
	private UILabel counterLabel;

	[SerializeField]
	private UILabel timerLabel;

	[SerializeField]
	private GameObject battlePassContainer;

	[SerializeField]
	private UILabel dailyKillLabel;

	[SerializeField]
	private GameObject freeObject;

	[SerializeField]
	private GameObject premiumObject;

	[SerializeField]
	private GameObject levelUpObject;

	[SerializeField]
	private UIWidget battlePassProgressBarForeground;

	[SerializeField]
	private UIWidget battlePassProgressBarBackground;

	[SerializeField]
	private TweenProgressBar battlePassProgressBarTween;

	[SerializeField]
	private GameObject containerObject;

	[SerializeField]
	private bool animateLevelUp;

	private BattlePassModel battlePass;

	private BeginnerBattlePassInfo beginnerBattlePassInfo;

	private bool hidden;

	private bool battlePassProgressBarInitialized;

	private int currentRequiredCurrency;

	private void Start()
	{
		battlePass = GameManager.Instance.playerModel.BattlePass;
		beginnerBattlePassInfo = GameManager.Instance.playerModel.BeginnerBattlePassInfo;
		battlePass.Changed += OnChange;
		battlePass.BattleCurrency.Changed += OnChange;
		UIEvent.OnUIEvent += OnUIEvent;
		Refresh();
	}

	private void OnEnable()
	{
		if (battlePass != null)
		{
			AnimateLevelUpAndSetProgress();
		}
		SingularityMonoBehaviour<LocalizationManager>.Instance.OnLocalizationLanguageChanged += OnLocalizationLanguageChanged;
	}

	private void OnDisable()
	{
		SingularityMonoBehaviour<LocalizationManager>.Instance.OnLocalizationLanguageChanged -= OnLocalizationLanguageChanged;
	}

	private void OnLocalizationLanguageChanged(string newLanguage)
	{
		Refresh();
	}

	private void OnDestroy()
	{
		if (battlePass != null)
		{
			battlePass.Changed -= OnChange;
			battlePass.BattleCurrency.Changed -= OnChange;
		}
		UIEvent.OnUIEvent -= OnUIEvent;
	}

	private void OnChange(ModelObject model, string changed, object args)
	{
		Refresh();
	}

	private void OnUIEvent(string type, object param)
	{
		if (param is QuestsPopup && type == "OnPopUpOpen")
		{
			SetHidden(hide: false);
		}
		if (param is ShopPopup || param is NewPhonePopup)
		{
			if (type == "OnPopUpOpen")
			{
				SetHidden(hide: true);
			}
			else if (type == "OnPopUpClose")
			{
				SetHidden(hide: false);
			}
		}
	}

	public void Refresh()
	{
		if (battlePass != null)
		{
			bool flag = battlePass.manager.Player.IsInPreBeginnerBattlePassState();
			bool flag2 = !hidden && (battlePass.IsSeasonActive || flag);
			Helpers.GameObjectSetActive(containerObject, flag2);
			if (flag2)
			{
				bool value = battlePass.EarnedFromKillsThisCycle >= battlePass.MaxDailyBCFromKills;
				Helpers.GameObjectSetActive(maxKillsObject, value);
				bool flag3 = flag || (!battlePass.AtMaxTier && battlePass.BattleCurrency.Value >= battlePass.NextTierBCPrice);
				counterLabel.text = (flag3 ? LocalizationManager.GetText("BattlePass.Progress.LevelUp") : $"{battlePass.BattleCurrency.Value}/{battlePass.NextTierBCPrice}");
				Helpers.GameObjectSetActive(levelUpObject, flag3);
				Helpers.GameObjectSetActive(premiumObject, battlePass.PremiumActive);
				Helpers.GameObjectSetActive(freeObject, !battlePass.PremiumActive);
				float num = (float)battlePass.EarnedFromKillsThisCycle / (float)battlePass.MaxDailyBCFromKills;
				dailyKillProgressBar.Set(num);
				dailyKillLabel.text = $"{Mathf.RoundToInt(num * 100f)}%";
				AnimateLevelUpAndSetProgress();
			}
		}
	}

	private void Update()
	{
		if (maxKillsObject.activeSelf)
		{
			TimeSpan timeSpan = TimeSpan.FromMilliseconds(battlePass.KillCapExpiryDateMilliseconds - battlePass.manager.Player.UtcTimeStamp);
			timerLabel.text = $"{(int)timeSpan.TotalHours}:{timeSpan.Minutes:00}:{timeSpan.Seconds:00}";
		}
		if (!battlePass.AtMaxTier && animateLevelUp)
		{
			int num = Mathf.RoundToInt(battlePassProgressBar.value * (float)currentRequiredCurrency);
			counterLabel.text = $"{num}/{currentRequiredCurrency}";
		}
		if (!battlePassProgressBarInitialized)
		{
			battlePassProgressBarBackground?.UpdateAnchors();
			battlePassProgressBarForeground?.UpdateAnchors();
			battlePassProgressBarInitialized = true;
		}
	}

	public void Click()
	{
		SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("global/ui_click");
		UIEvent.Send("OnBattlePassOpened");
		SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.BattlePassPopup)?.Open();
	}

	public void SetHidden(bool hide)
	{
		hidden = hide;
		Refresh();
	}

	private void AnimateLevelUpAndSetProgress()
	{
		PlayerModel player = battlePass.manager.Player;
		float bpFill = Mathf.Min((float)battlePass.BattleCurrency.Value / (float)battlePass.NextTierBCPrice, 1f);
		string nextTier;
		if (player.IsInPreBeginnerBattlePassState())
		{
			nextTier = "1";
		}
		else if (battlePass.AtMaxTier)
		{
			nextTier = LocalizationManager.GetText("BattlePass.Progress.Max");
		}
		else
		{
			nextTier = (battlePass.ReachedTier + 2).ToString();
		}
		battlePassProgressBarTween.Finish();
		TweenManager.FinishTweenGroup(nextTierLabel.gameObject, 0);
		if (animateLevelUp && player.UtcTimeStamp - battlePass.LastTierIncreaseTimestamp < 2000)
		{
			nextTierLabel.text = (battlePass.ReachedTier + 1).ToString();
			battlePassProgressBarTween.From = 0f;
			battlePassProgressBarTween.To = 1f;
			currentRequiredCurrency = battlePass.PreviousTierBCPrice;
			TweenManager.PlayTweenGroup(base.gameObject, battlePassProgressBarTween.tweenGroup, forward: true, delegate
			{
				TweenManager.PlayTweenGroup(base.gameObject, 0, forward: true, delegate
				{
					nextTierLabel.text = nextTier;
					TweenManager.PlayTweenGroup(base.gameObject, 1);
					currentRequiredCurrency = battlePass.NextTierBCPrice;
					battlePassProgressBarTween.To = bpFill;
					TweenManager.PlayTweenGroup(base.gameObject, battlePassProgressBarTween.tweenGroup, forward: true, delegate
					{
					});
				});
			});
		}
		else
		{
			battlePassProgressBar.Set(bpFill);
			nextTierLabel.text = nextTier;
			currentRequiredCurrency = battlePass.NextTierBCPrice;
		}
	}
}
