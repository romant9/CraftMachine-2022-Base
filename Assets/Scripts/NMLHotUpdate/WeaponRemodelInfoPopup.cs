using UnityEngine;

public class WeaponRemodelInfoPopup : HUDElement
{
	private static string targetURL = "https://www.thewalkingdeadnomansland.com/news/remodeling-feature-explained";

	public void JumpButtonClick()
	{
		Application.OpenURL(targetURL);
	}
}
