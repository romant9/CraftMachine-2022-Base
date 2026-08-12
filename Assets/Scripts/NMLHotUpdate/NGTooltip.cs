using UnityEngine;

public class NGTooltip : HUDElement
{
	private static string toolitpText;

	public override void Update()
	{
		base.Update();
		if (toolitpText != null)
		{
			UITooltip.Show(toolitpText);
			toolitpText = null;
		}
		if (Input.GetMouseButtonDown(0))
		{
			UITooltip.Show(null);
		}
	}

	public static void Show(string text)
	{
		SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.Tooltip).Open();
		toolitpText = text;
	}
}
