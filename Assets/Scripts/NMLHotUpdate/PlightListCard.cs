using TWDModel;
using UnityEngine;

public class PlightListCard : UIListCard<DifficultyIncrementalDebuff>
{
	[SerializeField]
	private GameObject tip;

	[SerializeField]
	private UITexture icon;

	public override void UpdateUI()
	{
		base.UpdateUI();
		if (icon != null)
		{
			Object obj = UnityUtils.LoadFromAssetBundle(base.Item?.Image, "itemgraphics");
			if (obj != null)
			{
				icon.mainTexture = (Texture)obj;
			}
		}
	}

	public void OnTraitTooltipClicked()
	{
		TooltipManager.OpenTextBoxWithText(tip, HelpersLocalization.GetChallengeDebuffDescription(base.Item));
	}
}
