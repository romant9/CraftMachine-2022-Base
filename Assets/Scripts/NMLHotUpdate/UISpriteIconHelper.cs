public class UISpriteIconHelper
{
	private static readonly string missingIconName = "Ui_Icon_Resource_Supplies";

	public static void SetIcons(UISprite[] iconSprites, string[] iconNames)
	{
		if (iconSprites == null || iconNames == null)
		{
			return;
		}
		for (int i = 0; i < iconSprites.Length; i++)
		{
			if (!(iconSprites[i] == null))
			{
				if (i < iconNames.Length && iconNames[i] != null)
				{
					iconSprites[i].spriteName = iconNames[i];
					Helpers.GameObjectSetActive(iconSprites[i], value: true);
				}
				else
				{
					iconSprites[i].spriteName = missingIconName;
					Helpers.GameObjectSetActive(iconSprites[i], value: false);
				}
			}
		}
	}

	public static void SetIcons(UISprite[] iconSprites, string iconsStrings)
	{
		if (iconsStrings != null)
		{
			string[] iconNames = iconsStrings.Split(',');
			SetIcons(iconSprites, iconNames);
		}
	}
}
