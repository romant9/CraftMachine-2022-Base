using TWDModel;
using UnityEngine;

public class UnlockClassPopupClassItem : MonoBehaviour
{
	[SerializeField]
	private SurvivorClass survivorClass;

	[SerializeField]
	private GameObject lockedContainer;

	[SerializeField]
	private UILabel unlockClassLabel;

	[SerializeField]
	private UILabel classNameLabel;

	[SerializeField]
	private UITexture classTexture;

	private void OnEnable()
	{
		UpdateUI();
	}

	public void UpdateUI()
	{
		classNameLabel.text = HelpersLocalization.GetSurvivorClassName(survivorClass);
		lockedContainer.SetActive(!IsUnlocked());
		if (IsUnlocked())
		{
			classTexture.color = Color.white;
			return;
		}
		QuestDefinition unlockSurvivorClassQuest = QuestUtils.GetUnlockSurvivorClassQuest(GameManager.Instance.modelManager, survivorClass);
		if (unlockSurvivorClassQuest == null)
		{
			unlockClassLabel.text = "";
			return;
		}
		MapMissionGroupModel unlockedEpisode = unlockSurvivorClassQuest.GetUnlockedEpisode(GameManager.Instance.modelManager);
		unlockClassLabel.text = LocalizationManager.GetText("Popup.StartPhoneCall.UnlockClass{EpisodeName}{ClassName}", HelpersLocalization.GetEpisodeTitle(unlockedEpisode), HelpersLocalization.GetSurvivorClassName(survivorClass));
	}

	public void OnSelect()
	{
		if (IsUnlocked())
		{
			SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("global/ui_click");
			GetComponentInParent<UnlockClassPopup>().SelectClass(survivorClass);
		}
		else
		{
			SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("global/invalid_action");
		}
	}

	private bool IsUnlocked()
	{
		return GameManager.Instance.playerModel.SurvivorContainer.IsSurvivorClassUnlocked(survivorClass);
	}
}
