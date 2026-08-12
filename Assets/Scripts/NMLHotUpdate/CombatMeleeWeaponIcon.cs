using System.Collections.Generic;
using TWDModel;
using UnityEngine;

public class CombatMeleeWeaponIcon : MonoBehaviour
{
	private AbilityModel ability;

	public UISprite Icon;

	public AbilityModel Ability
	{
		get
		{
			return ability;
		}
		set
		{
			ability = value;
			Vector3 localScale = Icon.transform.localScale;
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary.Add("WeaponAbilitySword", "Icon_Hud_Katana");
			dictionary.Add("WeaponAbilityKnife", "Icon_Hud_knife");
			dictionary.Add("WeaponAbilityBaseballBat", "Icon_Hud_Bat");
			string text = "";
			if (dictionary.ContainsKey(ability.Definition.Identifier))
			{
				text = dictionary[ability.Definition.Identifier];
			}
			base.gameObject.SetActive(!string.IsNullOrEmpty(text));
			Icon.spriteName = text;
			Icon.MakePixelPerfect();
			Icon.transform.localScale = localScale;
		}
	}
}
