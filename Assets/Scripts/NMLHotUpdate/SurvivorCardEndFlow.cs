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

	private bool portraitRender;

	private ActorModel ExpectingPortraitForActor;

	public void SetSurvivor(SurvivorModel survivor, Color injuryColor, bool isDead, bool isSurvival, bool isEndless)
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
			base.gameObject.SetActive(value: true);
		}
		else
		{
			base.gameObject.SetActive(value: false);
		}
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
