using TWDModel;
using UnityEngine;

public class GuildEmblemIcon : MonoBehaviour
{
	[Header("Set at Awake() if null")]
	public UISprite Sprite;

	private void Awake()
	{
		if (Sprite == null)
		{
			Sprite = GetComponent<UISprite>();
		}
	}

	public void UpdateUI(GuildTierDefinition definition)
	{
		if (definition != null)
		{
			HelpersUI.SetSprite(Sprite, definition.IconSprite);
		}
	}
}
