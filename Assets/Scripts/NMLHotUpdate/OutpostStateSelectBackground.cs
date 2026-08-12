using System.Collections.Generic;
using TWDModel;
using UnityEngine;

public class OutpostStateSelectBackground : OutpostStateBase
{
	[SerializeField]
	private OutpostSelectBackgroundList scrollList;

	[SerializeField]
	private OutpostHelpPopup outpostHelpPopup;

	private List<OutpostBackgroundCard> BackgroundCardsList;

	public override GameObject GetTutorialPanel => outpostHelpPopup.gameObject;

	private void Awake()
	{
		outpostHelpPopup.gameObject.SetActive(!GameManager.Instance.playerModel.HasPublishedOutpost);
	}

	private void OnEnable()
	{
		if (scrollList != null)
		{
			scrollList.SetCards(GameManager.Instance.playerModel.gameEconomyData.OutpostTemplateDefinitions);
		}
		OutpostBackgroundCard outpostBackgroundCard = null;
		for (int i = 0; i < scrollList.GetCards().Count; i++)
		{
			outpostBackgroundCard = scrollList.GetCards()[i] as OutpostBackgroundCard;
			if (outpostBackgroundCard != null)
			{
				outpostBackgroundCard.Button.SetCallback(OnMapCardClicked);
				outpostBackgroundCard.Button.id = i.ToString() ?? "";
			}
		}
	}

	public virtual void Clear()
	{
		OutpostBackgroundCard outpostBackgroundCard = null;
		for (int i = 0; i < scrollList.GetCards().Count; i++)
		{
			outpostBackgroundCard = scrollList.GetCards()[i] as OutpostBackgroundCard;
			if (outpostBackgroundCard != null)
			{
				outpostBackgroundCard.Clear();
				outpostBackgroundCard = null;
			}
		}
	}

	private void OnMapCardClicked(ButtonBase button)
	{
		int result = 0;
		if (int.TryParse(button.id, out result) && scrollList != null && scrollList.GetCards().Count > result)
		{
			OutpostBackgroundCard outpostBackgroundCard = scrollList.GetCards()[result] as OutpostBackgroundCard;
			if (!outpostBackgroundCard.IsBackgroundUnlocked())
			{
				outpostBackgroundCard.BuyBackgroundUnlock();
			}
			else
			{
				SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.IngameLoading).Open();
				RunLocationLoader.LoadLocationModel(GameManager.Instance.playerModel.gameEconomyData.OutpostTemplateDefinitions[result], LoadingDone, LoadingError);
				removeCardCallbacks();
			}
			SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("global/ui_click");
		}
	}

	private void LoadingDone(RunLocationModel runLocationModel)
	{
		SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.IngameLoading).Close();
		RequestStateChange(StateChangeDirection.Next);
	}

	private void LoadingError(RunLocationModel runLocationModel)
	{
		SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.IngameLoading).Close();
	}

	private void removeCardCallbacks()
	{
		OutpostBackgroundCard outpostBackgroundCard = null;
		for (int i = 0; i < scrollList.GetCards().Count; i++)
		{
			outpostBackgroundCard = scrollList.GetCards()[i] as OutpostBackgroundCard;
			if (outpostBackgroundCard != null)
			{
				outpostBackgroundCard.Button.RemoveCallback(OnMapCardClicked);
			}
		}
	}
}
