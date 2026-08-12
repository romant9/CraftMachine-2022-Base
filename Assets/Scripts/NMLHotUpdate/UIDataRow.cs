using UnityEngine;

public class UIDataRow : MonoBehaviourExtended
{
	[Tooltip("Needed to know the size height of this object")]
	[SerializeField]
	private UIWidget cachedWidget;

	[SerializeField]
	public UILabel[] labelArray;

	[SerializeField]
	public UISprite[] spriteArray;

	public bool IsVisible
	{
		get
		{
			if (base.gameObject != null)
			{
				return base.gameObject.activeSelf;
			}
			return false;
		}
	}

	public UIWidget widget
	{
		get
		{
			if (cachedWidget == null)
			{
				cachedWidget = GetComponent<UIWidget>();
				if (cachedWidget == null)
				{
					Debug.LogError("Cant find UI widget on UIDataRow! Needed to calculate the size of the object!");
				}
			}
			return cachedWidget;
		}
	}

	public void Show()
	{
		Helpers.GameObjectSetActive(base.gameObject, value: true);
	}

	public void Hide()
	{
		Helpers.GameObjectSetActive(base.gameObject, value: false);
	}

	public void SetDataToLabel(string[] content)
	{
		if (labelArray == null || content == null)
		{
			return;
		}
		for (int i = 0; i < labelArray.Length; i++)
		{
			if (content.Length > i && content[i] != null)
			{
				HelpersUI.SetContentToLabel(labelArray[i], content[i]);
			}
			else
			{
				HelpersUI.SetContentToLabel(labelArray[i], "", setActive: false);
			}
		}
	}

	public void SetDataToSprites(string[] content)
	{
		if (spriteArray == null || content == null)
		{
			return;
		}
		for (int i = 0; i < spriteArray.Length; i++)
		{
			if (content.Length > i && content[i] != null)
			{
				HelpersUI.SetSprite(spriteArray[i], content[i]);
			}
			else
			{
				Helpers.GameObjectSetActive(spriteArray[i], value: false);
			}
		}
	}

	public void UseStarsAsRarityIndicator(int rarity)
	{
		for (int i = 0; i < spriteArray.Length; i++)
		{
			if (spriteArray[i] != null)
			{
				if (rarity >= i)
				{
					Helpers.GameObjectSetActive(spriteArray[i], value: true);
				}
				else
				{
					Helpers.GameObjectSetActive(spriteArray[i], value: false);
				}
			}
		}
	}
}
