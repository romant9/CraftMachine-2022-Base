using TWDModel;
using UnityEngine;

public class SurvivorCardEndFlow : MonoBehaviour
{
	[SerializeField]
	private UILabel nameLabel;

	[SerializeField]
	private UILabel levelLabel;

	[SerializeField]
	private UISprite classIconSprite;

	[SerializeField]
	private UILabel injuryLabel;

	[SerializeField]
	private UISprite injuryBgSprite;

	[SerializeField]
	private UITexture portraitTexture;

	[Tooltip("Effect used for Dead Survivors")]
	[SerializeField]
	private GameObject deadlyParent;

	[Header("WorldBoss fatigue")]
	[SerializeField]
	private GameObject worldBossTiredParent;

	[SerializeField]
	private GameObject worldBossTiredUpContainer;

	private static readonly Color WorldBossTiredFullColor = new Color(32f / 51f, 67f / 85f, 0.18431373f, 1f);

	private static readonly Color WorldBossTiredTwoColor = new Color(0.96862745f, 0.7607843f, 0.14509805f, 1f);

	private static readonly Color WorldBossTiredOneColor = new Color(0.99215686f, 0.20784314f, 0.20784314f, 1f);

	private bool portraitRender;

	private ActorModel ExpectingPortraitForActor;

	public void SetSurvivor(SurvivorModel survivor, Color injuryColor, bool isDead, bool isSurvival, bool isEndless, bool showWorldBossTired = false)
	{
		if (survivor != null)
		{
			HelpersUI.SetContentToLabel(nameLabel, survivor.Name);
			HelpersUI.SetContentToLabel(levelLabel, survivor.Level.ToString());
			HelpersUI.SetSprite(classIconSprite, HelpersGfx.GetSurvivorClassIconName(survivor));
			if (isSurvival)
			{
				if (survivor.PreviousCombatInjuryType == InjuryType.OutOfAction)
				{
					HelpersUI.SetContentToLabel(injuryLabel, LocalizationManager.GetText("SurvivorStatus.Injury.OutOfAction"));
				}
				else
				{
					HelpersUI.SetContentToLabel(injuryLabel, LocalizationManager.GetText("SurvivorStatus.Injury.Survived"));
				}
			}
			else if (isEndless)
			{
				HelpersUI.SetContentToLabel(injuryLabel, LocalizationManager.GetText("Combat.EndFade.EndlessMode.Survivor{0}", survivor.SurvivedUntilWave));
			}
			else if (isDead)
			{
				HelpersUI.SetContentToLabel(injuryLabel, LocalizationManager.GetText("SurvivorStatus.Dead"));
			}
			else
			{
				HelpersUI.SetContentToLabel(injuryLabel, LocalizationManager.GetText("SurvivorStatus.Injury." + survivor.PreviousCombatInjuryType));
			}
			Helpers.GameObjectSetActive(deadlyParent, isDead);
			if (injuryBgSprite != null)
			{
				injuryBgSprite.color = injuryColor;
			}
			SetPortrait(survivor, portraitTexture);
			SetInjuryAudio(survivor.PreviousCombatInjuryType, isDead);
			UpdateWorldBossTiredInfo(survivor, showWorldBossTired);
			base.gameObject.SetActive(value: true);
		}
		else
		{
			Helpers.GameObjectSetActive(worldBossTiredParent, value: false);
			base.gameObject.SetActive(value: false);
		}
	}

	private void UpdateWorldBossTiredInfo(SurvivorModel survivor, bool showWorldBossTired)
	{
		Helpers.GameObjectSetActive(worldBossTiredParent, showWorldBossTired);
		if (!showWorldBossTired || survivor == null)
		{
			return;
		}
		WorldBossModelManager worldBossModelManager = GameManager.Instance?.playerModel?.WorldBossModelManager;
		if (worldBossModelManager == null || string.IsNullOrEmpty(survivor.IdForAnalytics))
		{
			Helpers.GameObjectSetActive(worldBossTiredParent, value: false);
			return;
		}
		int heroChargeLimit = worldBossModelManager.GetHeroChargeLimit();
		int heroCharges = worldBossModelManager.GetHeroCharges(survivor.IdForAnalytics);
		if (heroChargeLimit <= 0 || heroCharges >= heroChargeLimit)
		{
			Helpers.GameObjectSetActive(worldBossTiredUpContainer, value: true);
			SetWorldBossTiredUpIndicators(3, WorldBossTiredFullColor);
			return;
		}
		switch (heroCharges)
		{
		case 2:
			Helpers.GameObjectSetActive(worldBossTiredUpContainer, value: true);
			SetWorldBossTiredUpIndicators(2, WorldBossTiredTwoColor);
			break;
		case 1:
			Helpers.GameObjectSetActive(worldBossTiredUpContainer, value: true);
			SetWorldBossTiredUpIndicators(1, WorldBossTiredOneColor);
			break;
		default:
			SetWorldBossTiredUpIndicators(0, WorldBossTiredOneColor);
			Helpers.GameObjectSetActive(worldBossTiredUpContainer, value: false);
			break;
		}
	}

	private void SetWorldBossTiredUpIndicators(int visibleCount, Color color)
	{
		if (worldBossTiredUpContainer == null)
		{
			return;
		}
		for (int i = 1; i <= 3; i++)
		{
			Transform transform = FindWorldBossTiredIndicatorTransform(i);
			if (transform == null)
			{
				continue;
			}
			bool flag = i <= visibleCount;
			Helpers.GameObjectSetActive(transform.gameObject, flag);
			if (flag)
			{
				UISprite component = transform.GetComponent<UISprite>();
				if (component != null)
				{
					component.color = color;
				}
			}
		}
	}

	private Transform FindWorldBossTiredIndicatorTransform(int index)
	{
		if (worldBossTiredUpContainer == null)
		{
			return null;
		}
		Transform transform = worldBossTiredUpContainer.transform.Find("Tired " + index);
		if (transform == null)
		{
			transform = worldBossTiredUpContainer.transform.Find("Tired" + index);
		}
		return transform;
	}

	private void SetPortrait(ActorModel survivor, UITexture portrait)
	{
		if (survivor == null || !(portrait != null) || !(PortraitManager.Instance != null))
		{
			return;
		}
		PortraitRenderSource info = PortraitRenderSource.fromActorModel(survivor);
		if (PortraitManager.Instance.GetPortrait(info) == null)
		{
			ModularCharacter prefabForActor = ActorView.GetPrefabForActor(survivor);
			if (!portraitRender && prefabForActor != null)
			{
				ExpectingPortraitForActor = survivor;
				PortraitManager.Instance.CreatePortrait(info, prefabForActor, OnMissingPortraitRendered);
			}
			portrait.gameObject.SetActive(value: false);
		}
		else
		{
			portrait.mainTexture = PortraitManager.Instance.GetPortrait(PortraitRenderSource.fromActorModel(survivor));
			portrait.gameObject.SetActive(value: true);
			ExpectingPortraitForActor = null;
		}
	}

	private void OnMissingPortraitRendered(IPortraitRenderSource info)
	{
		if (ExpectingPortraitForActor != null && info != null && ExpectingPortraitForActor.ActorDefinitionID == info.ActorDefinitionId)
		{
			portraitRender = true;
			SetPortrait(ExpectingPortraitForActor, portraitTexture);
		}
	}

	private void SetInjuryAudio(InjuryType injury, bool dead = false)
	{
		TweenSound component = base.gameObject.GetComponent<TweenSound>();
		if (component != null)
		{
			if (dead)
			{
				component.soundEventName = "combat_ui/endflow_injury_death";
			}
			else if (injury == InjuryType.None)
			{
				component.soundEventName = "combat_ui/endflow_injury_none";
			}
			else
			{
				component.soundEventName = "combat_ui/endflow_injury_any";
			}
		}
	}
}
