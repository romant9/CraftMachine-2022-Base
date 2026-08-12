using TWDModel;
using UnityEngine;

public class UnlockClassPopupSelectedClass : MonoBehaviour
{
	[SerializeField]
	private UITexture classTexture;

	[SerializeField]
	private UILabel classNameLabel;

	[SerializeField]
	private UILabel classDescriptionLabel;

	[SerializeField]
	private UISprite classIcon;

	[SerializeField]
	private UILabel damageDescriptionLabel;

	[SerializeField]
	private UILabel healthDescriptionLabel;

	[SerializeField]
	private UILabel tipLabel;

	[SerializeField]
	private UILabel titleTopRight;

	[SerializeField]
	private GameObject lockedParent;

	[SerializeField]
	private UILabel lockedLabel;

	[SerializeField]
	private GameObject videoButton;

	[SerializeField]
	private GameObject specialVideoButton;

	public SurvivorClass SurvivorClass { get; set; }

	private void OnEnable()
	{
		UpdateUI();
	}

	public void UpdateUI()
	{
		QuestDefinition questDefinition = null;
		bool flag = !GameManager.Instance.playerModel.SurvivorContainer.IsSurvivorClassUnlocked(SurvivorClass);
		Helpers.GameObjectSetActive(lockedParent, flag);
		if (flag)
		{
			HelpersUI.SetContentToLabel(titleTopRight, LocalizationManager.GetText("Popup.UnlockClass.ClassLocked{ClassName}", HelpersLocalization.GetSurvivorClassName(SurvivorClass)));
			questDefinition = QuestUtils.GetUnlockSurvivorClassQuest(GameManager.Instance.modelManager, SurvivorClass);
			if (questDefinition != null)
			{
				MapMissionGroupModel unlockedEpisode = questDefinition.GetUnlockedEpisode(GameManager.Instance.modelManager);
				if (unlockedEpisode != null)
				{
					string text = LocalizationManager.GetText("Popup.StartPhoneCall.UnlockClass{EpisodeName}{ClassName}", HelpersLocalization.GetEpisodeTitle(unlockedEpisode), HelpersLocalization.GetSurvivorClassName(SurvivorClass));
					HelpersUI.SetContentToLabel(lockedLabel, text);
				}
			}
		}
		else
		{
			HelpersUI.SetContentToLabel(titleTopRight, LocalizationManager.GetText("Popup.UnlockClass.ClassUnlocked"));
		}
		HelpersGfx.SetSurvivorClassMaterial(classTexture, SurvivorClass);
		classNameLabel.text = HelpersLocalization.GetSurvivorClassName(SurvivorClass);
		classDescriptionLabel.text = HelpersLocalization.GetSurvivorClassDescription(SurvivorClass);
		classIcon.spriteName = HelpersGfx.GetSurvivorClassIconName(SurvivorClass);
		damageDescriptionLabel.text = LocalizationManager.GetText(getDamageDescriptionTextId());
		healthDescriptionLabel.text = LocalizationManager.GetText(getHealthDescriptionTextId());
		tipLabel.text = LocalizationManager.GetText("Survivor.Tip." + SurvivorClass);
		Helpers.GameObjectSetActive(videoButton, value: false);
		Helpers.GameObjectSetActive(specialVideoButton, value: false);
		if (!string.IsNullOrEmpty(GetClassVideoURL()) && GameManager.Instance.gameEconomyData.ConfigData.ShowClassIntroVideos)
		{
			if (GameManager.Instance.gameEconomyData.ConfigData.CurrentCampaign != PromoCampaignType.None)
			{
				Helpers.GameObjectSetActive(specialVideoButton, value: true);
			}
			else if (!GameConfiguration.Instance.Config.LowViolence)
			{
				Helpers.GameObjectSetActive(videoButton, value: true);
			}
		}
	}

	public void Close()
	{
		UnlockClassPopup componentInParent = GetComponentInParent<UnlockClassPopup>();
		if (componentInParent != null && componentInParent.SingleInfoMode)
		{
			base.gameObject.SetActive(value: false);
			componentInParent.Close();
		}
		else
		{
			base.gameObject.SetActive(value: false);
		}
	}

	private string GetClassVideoURL()
	{
		string result = null;
		switch (SurvivorClass)
		{
		case SurvivorClass.Assault:
			result = GameManager.Instance.gameEconomyData.ConfigData.IntroVideoAssault;
			break;
		case SurvivorClass.Bruiser:
			result = GameManager.Instance.gameEconomyData.ConfigData.IntroVideoBruiser;
			break;
		case SurvivorClass.Warrior:
			result = GameManager.Instance.gameEconomyData.ConfigData.IntroVideoWarrior;
			break;
		case SurvivorClass.Hunter:
			result = GameManager.Instance.gameEconomyData.ConfigData.IntroVideoHunter;
			break;
		case SurvivorClass.Scout:
			result = GameManager.Instance.gameEconomyData.ConfigData.IntroVideoScout;
			break;
		case SurvivorClass.Shooter:
			result = GameManager.Instance.gameEconomyData.ConfigData.IntroVideoShooter;
			break;
		}
		return result;
	}

	public void OnWatch()
	{
		string classVideoURL = GetClassVideoURL();
		if (!string.IsNullOrEmpty(classVideoURL))
		{
			Application.OpenURL(classVideoURL);
		}
	}

	private string getDamageDescriptionTextId()
	{
		switch (SurvivorClass)
		{
		case SurvivorClass.Bruiser:
		case SurvivorClass.Assault:
			return "Survivor.Stat.LowDamage";
		case SurvivorClass.Hunter:
		case SurvivorClass.Warrior:
			return "Survivor.Stat.AverageDamage";
		case SurvivorClass.Shooter:
		case SurvivorClass.Scout:
			return "Survivor.Stat.HighDamage";
		default:
			return null;
		}
	}

	private string getHealthDescriptionTextId()
	{
		switch (SurvivorClass)
		{
		case SurvivorClass.Hunter:
		case SurvivorClass.Scout:
			return "Survivor.Stat.LowHealth";
		case SurvivorClass.Shooter:
		case SurvivorClass.Warrior:
			return "Survivor.Stat.AverageHealth";
		case SurvivorClass.Bruiser:
		case SurvivorClass.Assault:
			return "Survivor.Stat.HighHealth";
		default:
			return null;
		}
	}
}
