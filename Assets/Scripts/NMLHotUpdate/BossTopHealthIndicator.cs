using System.Collections;
using System.Collections.Generic;
using TWDModel;
using UnityEngine;

public class BossTopHealthIndicator : HealthIndicator
{
	[SerializeField]
	private UIProgressBar delayHealthBar;

	[SerializeField]
	private UILabel barCountLabel;

	[SerializeField]
	private UILabel barPercentLabel;

	[SerializeField]
	private UISprite bossIcon;

	[SerializeField]
	private GameObject bossInfoPanel;

	[SerializeField]
	private GameObject barStatsPanel;

	[SerializeField]
	[Tooltip("Visual styles rotated each time a segmented HP bar is depleted. Cycles when there are more bars than styles.")]
	private BossHealthBarVisualStyle[] segmentedBarStyles;

	[SerializeField]
	private float delayBarTweenDuration = 0.6f;

	[SerializeField]
	private float shakeDuration = 0.25f;

	[SerializeField]
	[Tooltip("Diagonal shake offset in local space (X = horizontal, Y = vertical).")]
	private Vector3 shakeStrength = new Vector3(2.5f, 2.5f, 0f);

	[SerializeField]
	[Tooltip("Background tint when only one segmented HP bar remains (RGB 0, A 0.5 by default).")]
	private Color lastBarBackgroundColor = new Color(0f, 0f, 0f, 0.5f);

	[SerializeField]
	private UIGrid entryInfoGrid;

	[SerializeField]
	private GameObject entryInfoItemPrefab;

	private int lastSegmentedHPCount = -1;

	private int lastBarStyleIndex = -1;

	private float lastRatio = -1f;

	private int lastHitpoints = -1;

	private bool defaultBackgroundColorsCached;

	private Color healthBarDefaultBackgroundColor = Color.white;

	private Color delayBarDefaultBackgroundColor = Color.white;

	private Coroutine delayBarCoroutine;

	private Coroutine shakeCoroutine;

	private Transform[] shakeChildTargets;

	private Vector3[] shakeChildBaseLocalPositions;

	private List<GuildBossEntryInfoItem> entryInfoItems = new List<GuildBossEntryInfoItem>();

	public override bool IsScreenTopBossBar => true;

	private void Awake()
	{
		CacheShakeChildTargets();
		CacheDefaultBackgroundColors();
	}

	private void CacheShakeChildTargets()
	{
		int childCount = base.transform.childCount;
		shakeChildTargets = new Transform[childCount];
		shakeChildBaseLocalPositions = new Vector3[childCount];
		for (int i = 0; i < childCount; i++)
		{
			shakeChildTargets[i] = base.transform.GetChild(i);
			shakeChildBaseLocalPositions[i] = shakeChildTargets[i].localPosition;
		}
	}

	public void RefreshBossInfo(ActorModel actor)
	{
		if (actor == null)
		{
			return;
		}
		if (NameLabel != null)
		{
			if (actor.Definition.BossType != BossType.Any)
			{
				NameLabel.text = SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText("ActorName." + actor.Definition.BossType);
			}
			else
			{
				NameLabel.text = actor.Name;
			}
		}
		if (bossIcon != null)
		{
			bossIcon.spriteName = actor.Definition.NormalHead;
		}
		if (LevelLabel != null)
		{
			LevelLabel.text = (actor.IsEnvironmental ? string.Empty : ("Lv." + actor.Level));
			LevelLabel.color = Color.white;
		}
		if (ActorClass != null)
		{
			ActorClass.spriteName = HelpersGfx.GetHealthbarClassIconName(actor);
		}
		if (barCountLabel != null)
		{
			if (actor.IsSegmentedHP)
			{
				barCountLabel.text = "×" + actor.SegmentedHPCount;
				Helpers.GameObjectSetActive(barCountLabel.gameObject, value: true);
			}
			else
			{
				barCountLabel.text = string.Empty;
				Helpers.GameObjectSetActive(barCountLabel.gameObject, value: false);
			}
		}
		if (barPercentLabel != null && actor.MaxHitPoints > 0)
		{
			int num = Mathf.Clamp(Mathf.RoundToInt(100f * (float)actor.Hitpoints / (float)actor.MaxHitPoints), 0, 100);
			barPercentLabel.text = num + "%";
		}
		if (entryInfoGrid != null && entryInfoItemPrefab != null && entryInfoItems.Count == 0 && actor.Definition?.TagDisplay != null)
		{
			for (int i = 0; i < actor.Definition.TagDisplay.Count; i++)
			{
				GuildBossEntryInfoItem component = entryInfoGrid.gameObject.AddChild(entryInfoItemPrefab).GetComponent<GuildBossEntryInfoItem>();
				if (!(component == null))
				{
					string[] array = actor.Definition.TagDisplay[i].Split(':');
					if (array.Length >= 2)
					{
						component.UpdateUI(array[0], array[1]);
						entryInfoItems.Add(component);
					}
				}
			}
			entryInfoGrid.repositionNow = true;
		}
		Helpers.GameObjectSetActive(bossInfoPanel, value: true);
		Helpers.GameObjectSetActive(barStatsPanel, value: true);
	}

	public override void SyncBossBarFromModel(ActorModel actor, HealthBarUpdateMode mode, float visualRatio = -1f)
	{
		if (actor == null || HealthBar == null || actor.MaxHitPoints <= 0)
		{
			return;
		}
		float num = ((visualRatio >= 0f) ? Mathf.Clamp01(visualRatio) : Mathf.Clamp01((float)actor.Hitpoints / (float)actor.MaxHitPoints));
		int num2 = (actor.IsSegmentedHP ? actor.SegmentedHPCount : 0);
		bool flag = actor.IsSegmentedHP && lastSegmentedHPCount > num2 && num2 > 0;
		bool flag2 = actor.IsSegmentedHP && lastSegmentedHPCount >= 0 && lastSegmentedHPCount < num2;
		bool flag3 = lastRatio >= 0f && num > lastRatio + 0.0001f;
		RefreshBossInfo(actor);
		int barStyleIndex = GetBarStyleIndex(actor);
		if (mode == HealthBarUpdateMode.Instant || barStyleIndex != lastBarStyleIndex || flag || flag2)
		{
			ApplySegmentedBarStyle(barStyleIndex);
		}
		ApplyLastBarBackgroundTint(actor);
		switch (mode)
		{
		case HealthBarUpdateMode.Instant:
			ApplyForegroundRatio(num);
			ApplyDelayRatio(num, instant: true);
			break;
		case HealthBarUpdateMode.Damage:
		{
			if (flag)
			{
				OnSegmentedBarDepleted(num);
				break;
			}
			float num3 = ((lastRatio >= 0f) ? lastRatio : delayHealthBar.value);
			ApplyForegroundRatio(num);
			TweenDelayBarFromTo(num3, num);
			PlayDamageShake();
			break;
		}
		case HealthBarUpdateMode.Heal:
			if (flag2)
			{
				OnSegmentedBarRestored(num);
			}
			else if (flag3)
			{
				TweenForegroundTo(num);
				TweenDelayBarFromTo((delayHealthBar != null) ? delayHealthBar.value : num, num);
			}
			else if (delayBarCoroutine == null)
			{
				ApplyForegroundRatio(num);
				ApplyDelayRatio(num, instant: true);
			}
			break;
		}
		lastSegmentedHPCount = num2;
		lastRatio = num;
		lastHitpoints = actor.Hitpoints;
	}

	public override void ShowBossDefeated()
	{
		ApplyForegroundRatio(0f);
		ApplyDelayRatio(0f, instant: true);
		if (barCountLabel != null)
		{
			barCountLabel.text = "×0";
		}
		if (barPercentLabel != null)
		{
			barPercentLabel.text = "0%";
		}
		lastSegmentedHPCount = 0;
		lastBarStyleIndex = -1;
		lastRatio = 0f;
		lastHitpoints = 0;
	}

	public void OnSegmentedBarDepleted(float ratio)
	{
		ApplyForegroundRatio(ratio);
		ApplyDelayRatio(ratio, instant: true);
		PlayDamageShake();
	}

	public void OnSegmentedBarRestored(float ratio)
	{
		ApplyForegroundRatio(ratio);
		ApplyDelayRatio(ratio, instant: true);
	}

	private int GetBarStyleIndex(ActorModel actor)
	{
		if (segmentedBarStyles == null || segmentedBarStyles.Length == 0)
		{
			return 0;
		}
		if (actor == null || !actor.IsSegmentedHP || actor.SegmentedHPCount <= 0)
		{
			return 0;
		}
		return (actor.SegmentedHPMax - actor.SegmentedHPCount) % segmentedBarStyles.Length;
	}

	private void ApplySegmentedBarStyle(int styleIndex)
	{
		if (segmentedBarStyles == null || segmentedBarStyles.Length == 0)
		{
			return;
		}
		styleIndex = Mathf.Clamp(styleIndex, 0, segmentedBarStyles.Length - 1);
		BossHealthBarVisualStyle bossHealthBarVisualStyle = segmentedBarStyles[styleIndex];
		ApplyProgressBarStyle(HealthBar, bossHealthBarVisualStyle.foregroundSprite, bossHealthBarVisualStyle.backgroundSprite, bossHealthBarVisualStyle.overrideForegroundColor, bossHealthBarVisualStyle.foregroundColor);
		if (delayHealthBar != null)
		{
			if (!string.IsNullOrEmpty(bossHealthBarVisualStyle.delayForegroundSprite) || !string.IsNullOrEmpty(bossHealthBarVisualStyle.delayBackgroundSprite))
			{
				bool overrideForegroundColor = bossHealthBarVisualStyle.overrideDelayForegroundColor || (bossHealthBarVisualStyle.overrideForegroundColor && !string.IsNullOrEmpty(bossHealthBarVisualStyle.delayForegroundSprite));
				Color foregroundColor = (bossHealthBarVisualStyle.overrideDelayForegroundColor ? bossHealthBarVisualStyle.delayForegroundColor : bossHealthBarVisualStyle.foregroundColor);
				ApplyProgressBarStyle(delayHealthBar, bossHealthBarVisualStyle.delayForegroundSprite, bossHealthBarVisualStyle.delayBackgroundSprite, overrideForegroundColor, foregroundColor);
			}
			else if (bossHealthBarVisualStyle.overrideDelayForegroundColor && delayHealthBar.foregroundWidget != null)
			{
				delayHealthBar.foregroundWidget.color = bossHealthBarVisualStyle.delayForegroundColor;
			}
		}
		lastBarStyleIndex = styleIndex;
	}

	private void CacheDefaultBackgroundColors()
	{
		if (!defaultBackgroundColorsCached)
		{
			if (HealthBar != null && HealthBar.backgroundWidget != null)
			{
				healthBarDefaultBackgroundColor = HealthBar.backgroundWidget.color;
			}
			if (delayHealthBar != null && delayHealthBar.backgroundWidget != null)
			{
				delayBarDefaultBackgroundColor = delayHealthBar.backgroundWidget.color;
			}
			defaultBackgroundColorsCached = true;
		}
	}

	private void ApplyLastBarBackgroundTint(ActorModel actor)
	{
		CacheDefaultBackgroundColors();
		int num;
		Color color;
		if (actor != null && actor.IsSegmentedHP)
		{
			num = ((actor.SegmentedHPCount == 1) ? 1 : 0);
			if (num != 0)
			{
				color = lastBarBackgroundColor;
				goto IL_002e;
			}
		}
		else
		{
			num = 0;
		}
		color = healthBarDefaultBackgroundColor;
		goto IL_002e;
		IL_002e:
		Color color2 = color;
		Color color3 = ((num != 0) ? lastBarBackgroundColor : delayBarDefaultBackgroundColor);
		if (HealthBar != null && HealthBar.backgroundWidget != null)
		{
			HealthBar.backgroundWidget.color = color2;
		}
		if (delayHealthBar != null && delayHealthBar.backgroundWidget != null)
		{
			delayHealthBar.backgroundWidget.color = color3;
		}
	}

	private static void ApplyProgressBarStyle(UIProgressBar bar, string foregroundSprite, string backgroundSprite, bool overrideForegroundColor, Color foregroundColor)
	{
		if (!(bar == null))
		{
			if (bar.foregroundWidget is UISprite uISprite && !string.IsNullOrEmpty(foregroundSprite))
			{
				uISprite.spriteName = foregroundSprite;
			}
			if (bar.backgroundWidget is UISprite uISprite2 && !string.IsNullOrEmpty(backgroundSprite))
			{
				uISprite2.spriteName = backgroundSprite;
			}
			if (overrideForegroundColor && bar.foregroundWidget != null)
			{
				bar.foregroundWidget.color = foregroundColor;
			}
		}
	}

	private void ApplyForegroundRatio(float ratio)
	{
		if (HealthBar != null)
		{
			HealthBar.value = ratio;
		}
	}

	private void SetDelayBarValue(float ratio)
	{
		if (!(delayHealthBar == null))
		{
			delayHealthBar.Start();
			delayHealthBar.value = ratio;
			delayHealthBar.ForceUpdate();
		}
	}

	private void ApplyDelayRatio(float ratio, bool instant)
	{
		if (!(delayHealthBar == null))
		{
			StopDelayBarTween();
			SetDelayBarValue(ratio);
		}
	}

	private void StopDelayBarTween()
	{
		if (delayBarCoroutine != null)
		{
			StopCoroutine(delayBarCoroutine);
			delayBarCoroutine = null;
		}
	}

	private void TweenForegroundTo(float ratio)
	{
		if (!(HealthBar == null))
		{
			HealthBar.value = ratio;
		}
	}

	private void TweenDelayBarFromTo(float from, float to)
	{
		if (!(delayHealthBar == null))
		{
			from = Mathf.Clamp01(from);
			to = Mathf.Clamp01(to);
			if (from <= to + 1E-06f)
			{
				ApplyDelayRatio(to, instant: true);
				return;
			}
			StopDelayBarTween();
			SetDelayBarValue(from);
			delayBarCoroutine = StartCoroutine(TweenDelayBarCoroutine(from, to));
		}
	}

	private IEnumerator TweenDelayBarCoroutine(float from, float to)
	{
		float elapsed = 0f;
		while (elapsed < delayBarTweenDuration)
		{
			elapsed += Time.unscaledDeltaTime;
			float num = Mathf.Clamp01(elapsed / delayBarTweenDuration);
			float t = 1f - Mathf.Pow(1f - num, 3f);
			SetDelayBarValue(Mathf.Lerp(from, to, t));
			yield return null;
		}
		SetDelayBarValue(to);
		delayBarCoroutine = null;
	}

	private void PlayDamageShake()
	{
		if (shakeChildTargets == null || shakeChildTargets.Length == 0)
		{
			CacheShakeChildTargets();
		}
		if (shakeChildTargets != null && shakeChildTargets.Length != 0)
		{
			if (shakeCoroutine != null)
			{
				StopCoroutine(shakeCoroutine);
				ResetShakeChildPositions();
			}
			shakeCoroutine = StartCoroutine(ShakeChildrenOnce());
		}
	}

	private void ResetShakeChildPositions()
	{
		if (shakeChildTargets == null)
		{
			return;
		}
		for (int i = 0; i < shakeChildTargets.Length; i++)
		{
			if (shakeChildTargets[i] != null)
			{
				shakeChildTargets[i].localPosition = shakeChildBaseLocalPositions[i];
			}
		}
	}

	private IEnumerator ShakeChildrenOnce()
	{
		float num = ((Random.value > 0.5f) ? 1f : (-1f));
		Vector3 offset = new Vector3(Mathf.Abs(shakeStrength.x) * num, Mathf.Abs(shakeStrength.y) * num, 0f);
		float halfDuration = shakeDuration * 0.5f;
		float elapsed = 0f;
		while (elapsed < halfDuration)
		{
			elapsed += Time.unscaledDeltaTime;
			float num2 = Mathf.SmoothStep(0f, 1f, elapsed / halfDuration);
			ApplyShakeOffset(offset * num2);
			yield return null;
		}
		elapsed = 0f;
		while (elapsed < halfDuration)
		{
			elapsed += Time.unscaledDeltaTime;
			float num3 = Mathf.SmoothStep(0f, 1f, elapsed / halfDuration);
			ApplyShakeOffset(offset * (1f - num3));
			yield return null;
		}
		ResetShakeChildPositions();
		shakeCoroutine = null;
	}

	private void ApplyShakeOffset(Vector3 offset)
	{
		for (int i = 0; i < shakeChildTargets.Length; i++)
		{
			if (shakeChildTargets[i] != null)
			{
				shakeChildTargets[i].localPosition = shakeChildBaseLocalPositions[i] + offset;
			}
		}
	}

	private void OnDisable()
	{
		StopDelayBarTween();
		if (shakeCoroutine != null)
		{
			StopCoroutine(shakeCoroutine);
			shakeCoroutine = null;
		}
		ResetShakeChildPositions();
	}

	public void OnBossHealthBarClicked()
	{
		if (BindActor != null)
		{
			ActorDetailInfoPopup actorDetailInfoPopup = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.ActorDetailInfoPopup) as ActorDetailInfoPopup;
			if (actorDetailInfoPopup != null)
			{
				actorDetailInfoPopup.OpenForModel(BindActor);
			}
			else
			{
				Debug.LogWarning("BossTopHealthIndicator: Could not show actor info - ActorDetailInfoPopup is NULL");
			}
		}
	}
}
