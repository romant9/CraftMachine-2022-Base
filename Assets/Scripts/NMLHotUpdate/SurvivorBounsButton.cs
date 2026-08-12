using UnityEngine;

public class SurvivorBounsButton : MonoBehaviour
{
	[SerializeField]
	private SurvivorInfoPopup survivorInfoPopup;

	[SerializeField]
	private UISprite sprite;

	[SerializeField]
	private Color normalColor;

	[SerializeField]
	private Color equipColor;

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
		switch (type)
		{
		case "OnNewSurvivorSelected":
		case "OnSurvivorInfoOpen":
		case "BounsEquip":
			UpdateVisibility();
			break;
		}
	}

	private void UpdateVisibility()
	{
		sprite.applyGradient = IsEquip();
	}

	private bool IsEquip()
	{
		if (survivorInfoPopup == null)
		{
			return false;
		}
		return survivorInfoPopup.survivorModel?.UsingBounsModel?.UsingSurvivor != null;
	}
}
