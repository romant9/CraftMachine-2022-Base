using TWDModel;
using UnityEngine;

public class ItemListTabButton : MonoBehaviour
{
	[SerializeField]
	private UISprite SelectBG;

	[SerializeField]
	private UISprite NormalBG;

	[SerializeField]
	private UISprite Sprite;

	private TypeDefinition typeDefinition;

	public void Setup(TypeDefinition data)
	{
		typeDefinition = data;
		UpdateUI();
	}

	private void UpdateUI()
	{
		if (typeDefinition != null)
		{
			Sprite.spriteName = typeDefinition.TypeIcon;
		}
	}

	public void OnButtonClick()
	{
		UIEvent.Send("ItemListPopupTabClickEvent", typeDefinition);
	}

	public void SetSelectState(bool select)
	{
		Helpers.GameObjectSetActive(SelectBG, value: false);
		Helpers.GameObjectSetActive(NormalBG, value: false);
		if (select)
		{
			Helpers.GameObjectSetActive(SelectBG, value: true);
		}
		else
		{
			Helpers.GameObjectSetActive(NormalBG, value: true);
		}
	}

	public void FreshSelectData(TypeDefinition selectData)
	{
		if (selectData != null)
		{
			SetSelectState(typeDefinition == selectData);
		}
	}
}
