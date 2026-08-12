using TWDModel;
using UnityEngine;

public class SurvivorFavouriteButton : MonoBehaviour
{
	[SerializeField]
	private SurvivorInfoPopup survivorInfoPopup;

	[SerializeField]
	private GameObject On;

	private void OnEnable()
	{
		UIEvent.OnUIEvent += OnUIEventHandler;
		UpdateVisibility();
	}

	private void OnDisable()
	{
		UIEvent.OnUIEvent -= OnUIEventHandler;
	}

	private void OnUIEventHandler(string type, object parameter)
	{
		if (type == "OnNewSurvivorSelected" || type == "OnSurvivorInfoOpen")
		{
			UpdateVisibility();
		}
	}

	public void OnFavouriteButtonClicked()
	{
		if (survivorInfoPopup.survivorModel != null && Helpers.ExecuteCommand(new ToggleFavouriteForSurvivor(survivorInfoPopup.survivorModel)) == TWDModelResult.OK)
		{
			UpdateVisibility();
			survivorInfoPopup.UpdateUI();
			UIEvent.Send("OnSurvivorFavouriteToggled");
		}
	}

	private void UpdateVisibility()
	{
		On.SetActive(survivorInfoPopup.survivorModel?.IsFavourite ?? false);
	}
}
