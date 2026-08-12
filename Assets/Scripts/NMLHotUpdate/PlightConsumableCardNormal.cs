using TWDModel;
using UnityEngine;

public class PlightConsumableCardNormal : UIListCard<DifficultyIncrementalDebuff>
{
	[SerializeField]
	private UILabel descriptionLabel;

	[SerializeField]
	private UITexture icon;

	public override void UpdateUI()
	{
		base.UpdateUI();
		if (base.Item == null)
		{
			return;
		}
		if (descriptionLabel != null)
		{
			descriptionLabel.text = HelpersLocalization.GetChallengeDebuffDescription(base.Item);
		}
		if (icon != null)
		{
			Object obj = UnityUtils.LoadFromAssetBundle(base.Item?.Image, "itemgraphics");
			if (obj != null)
			{
				icon.mainTexture = (Texture)obj;
			}
		}
	}
}
