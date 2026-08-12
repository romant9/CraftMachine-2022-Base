using UnityEngine;

public class CombatEndWidget : ListWidgetBase
{
	public enum Types
	{
		None = 0,
		VictoryBanner = 1,
		DefeatBanner = 2,
		WalkersDispatched = 3,
		MissionReward = 4,
		TeamStatus = 5,
		Stars = 6,
		OutpostObjectives = 7,
		BonusStar = 8,
		TextMessage = 9,
		OutpostSpecialBanner = 10,
		FleeBanner = 11,
		Equipment = 12,
		LeaderTraitSupplyBonus = 13,
		DrawBanner = 14,
		GuildBattleRpReward = 15,
		GuildBattleVpReward = 16,
		GuildBattlePvPEnemyFound = 17,
		ConsumableReward = 18,
		EndlessModeMissionScore = 19,
		EndlessModeBanner = 20,
		EndlessModeExpertModeMissionScore = 21,
		BCGained = 22,
		WeeklyChallengeActivity = 23
	}

	protected UIWidget widget;

	private int SortValue;

	[SerializeField]
	private string widgetSoundEvent = "combat_ui/endflow_widget";

	[SerializeField]
	private bool hasTweenSoundComponents;

	[SerializeField]
	private GameObject particle;

	[SerializeField]
	private UILabel titleLabel;

	public Types CurrentType { get; set; }

	public override void Awake()
	{
		base.Awake();
		widget = GetComponent<UIWidget>();
		Helpers.GameObjectSetActive(particle, value: false);
	}

	public override void Activate()
	{
		PlayWidgetSound();
		base.Activate();
		SetAlpha(1f);
		Helpers.GameObjectSetActive(particle, value: true);
		TweenManager.PlayTweenGroup(base.gameObject, 0);
	}

	public override void Deactivate()
	{
		base.Deactivate();
		SetAlpha(0f);
		Helpers.GameObjectSetActive(particle, value: false);
	}

	public override void AddedToList()
	{
		base.AddedToList();
	}

	public virtual void SetSortValue(int value)
	{
		SortValue = value;
	}

	public override int GetSortValue()
	{
		return SortValue;
	}

	protected virtual void SetAlpha(float value)
	{
		if (widget != null)
		{
			widget.alpha = value;
		}
		else
		{
			DebugLogWarning("Cant set alpha. Widget is null.");
		}
	}

	private void PlayWidgetSound()
	{
		if (!string.IsNullOrEmpty(widgetSoundEvent) && SingularityMonoBehaviour<AudioManager>.Instance != null)
		{
			SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent(widgetSoundEvent);
		}
		if (!hasTweenSoundComponents)
		{
			return;
		}
		TweenSound[] componentsInChildren = GetComponentsInChildren<TweenSound>();
		if (componentsInChildren != null)
		{
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				componentsInChildren[i].enabled = true;
			}
		}
	}

	public void SetContent(string content)
	{
		titleLabel.text = content;
	}
}
