using UnityEngine;

public class RecipesButton : MonoBehaviour
{
	[SerializeField]
	private UIButtonExtended recipeButton;

	private void OnEnable()
	{
		recipeButton.SetClickCallback(OnRecipesButtonClicked);
	}

	private void OnDisable()
	{
		recipeButton.Clear();
	}

	public void OnRecipesButtonClicked(UIButtonExtended button)
	{
		SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.RecipesPopup).Open();
	}
}
