using UnityEngine;

public class OutpostObjectiveCard : MonoBehaviour
{
	[SerializeField]
	private UISprite[] SpritesArray;

	[SerializeField]
	private UILabel TextLabel;

	[SerializeField]
	private GameObject CompletedParent;

	private void Awake()
	{
		if (TextLabel != null)
		{
			TextLabel.text = "";
		}
		Helpers.GameObjectSetActive(CompletedParent, value: false);
	}

	public void SetObjectiveStatus(string textContent, bool completed, string spriteNameOverride = "")
	{
		HelpersUI.SetContentToLabel(TextLabel, textContent);
		Helpers.GameObjectSetActive(CompletedParent, completed);
		if (SpritesArray == null)
		{
			return;
		}
		for (int i = 0; i < SpritesArray.Length; i++)
		{
			if (SpritesArray[i] != null)
			{
				if (spriteNameOverride != "")
				{
					HelpersUI.SetSprite(SpritesArray[i], spriteNameOverride, completed);
				}
				else
				{
					Helpers.GameObjectSetActive(SpritesArray[i].gameObject, completed);
				}
			}
		}
	}
}
