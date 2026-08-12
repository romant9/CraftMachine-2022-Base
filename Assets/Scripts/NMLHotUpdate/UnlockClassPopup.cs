using TWDModel;
using UnityEngine;

public class UnlockClassPopup : HUDElement
{
	[SerializeField]
	private UnlockClassPopupSelectedClass selectedClassPanel;

	[SerializeField]
	private GameObject mainContainer;

	public StoryTellerModel StoryTellerModel { get; set; }

	public SurvivorClass ForceOpenSurvivorClass { get; set; }

	public bool SingleInfoMode { get; protected set; }

	private void OnEnable()
	{
		selectedClassPanel.gameObject.SetActive(value: false);
		mainContainer.SetActive(value: true);
	}

	public static void OpenInfoAboutClass(SurvivorClass survivorClass)
	{
		if (survivorClass != SurvivorClass.None)
		{
			UnlockClassPopup unlockClassPopup = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.UnlockClassPopup) as UnlockClassPopup;
			if (unlockClassPopup != null)
			{
				unlockClassPopup.OpenSingleInfo(survivorClass);
			}
		}
	}

	public void OpenSingleInfo(SurvivorClass survivorClass)
	{
		ForceOpenSurvivorClass = survivorClass;
		SingleInfoMode = true;
		Open();
	}

	public override void Open()
	{
		base.Open();
		if (ForceOpenSurvivorClass != SurvivorClass.None)
		{
			SelectClass(ForceOpenSurvivorClass);
		}
	}

	public void SelectClass(SurvivorClass survivorClass)
	{
		if (SingleInfoMode)
		{
			mainContainer.SetActive(value: false);
		}
		selectedClassPanel.gameObject.SetActive(value: true);
		selectedClassPanel.SurvivorClass = survivorClass;
		selectedClassPanel.UpdateUI();
	}

	public override void Close()
	{
		base.Close();
		SingleInfoMode = false;
		ForceOpenSurvivorClass = SurvivorClass.None;
		if (StoryTellerModel != null)
		{
			StoryTellerFlow.StartFlow(StoryTellerModel);
			StoryTellerModel = null;
		}
	}
}
