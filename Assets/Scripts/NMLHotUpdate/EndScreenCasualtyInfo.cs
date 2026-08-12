using UnityEngine;

public class EndScreenCasualtyInfo : MonoBehaviour
{
	public UISprite Icon;

	public UILabel Name;

	public UILabel Status;

	public void SetCasualtyInfo(string iconString, string name, string status)
	{
		Icon.spriteName = iconString.Replace("UI/Combat/Survivors/SurvivorPortrait_0", "Character_") + "_portrait";
		Icon.GetAtlasSprite();
		Name.text = name;
		Status.text = status;
	}
}
