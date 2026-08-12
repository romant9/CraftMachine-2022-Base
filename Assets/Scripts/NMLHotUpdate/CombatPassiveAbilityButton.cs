using System.Collections.Generic;
using UnityEngine;

public class CombatPassiveAbilityButton : MonoBehaviour
{
	private UIButton buttonComponent;

	private string traitID;

	public void Setup(string inTraitID, int traitValue = 0)
	{
		traitID = inTraitID;
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary.Add("Retaliate", "Icon_Skill_Avoid");
		dictionary.Add("FieldMedic", "Icon_Skill_Avoid");
		dictionary.Add("Lucky", "Icon_Skill_Lucky");
		string text = "";
		if (dictionary.ContainsKey(traitID))
		{
			text = dictionary[traitID];
		}
		buttonComponent.normalSprite = text;
		buttonComponent.hoverSprite = text;
		buttonComponent.pressedSprite = text;
		UISprite componentInChildren = buttonComponent.gameObject.GetComponentInChildren<UISprite>();
		if (componentInChildren != null)
		{
			componentInChildren.MakePixelPerfect();
			float r = 1f;
			float g = 1f;
			float b = 1f;
			if (traitValue > 0)
			{
				r = 0f;
				g = 1f;
				b = 0f;
			}
			else if (traitValue < 0)
			{
				r = 1f;
				g = 0f;
				b = 0f;
			}
			Color defaultColor = new Color(r, g, b);
			buttonComponent.defaultColor = defaultColor;
		}
	}

	private void OnEnable()
	{
		buttonComponent = GetComponentInChildren<UIButton>();
	}
}
