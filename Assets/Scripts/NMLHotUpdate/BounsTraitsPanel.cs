using TWDModel;
using UnityEngine;

public class BounsTraitsPanel : MonoBehaviour
{
	[SerializeField]
	private UISprite mainSprite;

	[SerializeField]
	private UILabel levelLabel;

	[SerializeField]
	private GameObject traitObj;

	private string _message;

	public void Init(int level, string message, string traitId)
	{
		if (level < 0)
		{
			Helpers.GameObjectSetActive(base.gameObject, value: false);
			return;
		}
		_message = message;
		if (levelLabel != null)
		{
			levelLabel.text = level.ToString() ?? "";
		}
		if (mainSprite != null)
		{
			string spriteName = "Ui_Icon_Trait_" + StripTraitLevelIdentifier(traitId);
			mainSprite.spriteName = spriteName;
		}
	}

	public void OnTraitTooltipClicked()
	{
		TooltipManager.OpenTextBoxWithText(traitObj, _message);
	}

	private string StripTraitLevelIdentifier(string traitIdentifier)
	{
		int num = traitIdentifier.LastIndexOf(".");
		if (num >= 0 && traitIdentifier.Substring(num + 1).StartsWith(TraitDefinition.TRAIT_TAG_RARITY_LEVEL))
		{
			return traitIdentifier.Substring(0, num);
		}
		return traitIdentifier;
	}
}
