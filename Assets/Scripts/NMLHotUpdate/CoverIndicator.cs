using System.Collections.Generic;
using TWDModel;
using UnityEngine;

public class CoverIndicator : HUDElementFollowTarget
{
	[SerializeField]
	private UISprite leftIndicator;

	[SerializeField]
	private UISprite rightIndicator;

	[SerializeField]
	private UISprite upIndicator;

	[SerializeField]
	private UISprite downIndicator;

	[SerializeField]
	private UISprite coverIcon;

	private GameObject followTarget;

	private CoverIndicator()
	{
	}

	public void SetCoverDirections(List<CoverDirection> directions, CoverIconState coverState = CoverIconState.None)
	{
		if (directions == null || directions.Count == 0)
		{
			base.gameObject.SetActive(value: false);
			return;
		}
		upIndicator.spriteName = (directions.Contains(CoverDirection.Top) ? "Ui_Cover_Enabled" : "Ui_Cover_Disabled");
		rightIndicator.spriteName = (directions.Contains(CoverDirection.Right) ? "Ui_Cover_Enabled" : "Ui_Cover_Disabled");
		downIndicator.spriteName = (directions.Contains(CoverDirection.Bottom) ? "Ui_Cover_Enabled" : "Ui_Cover_Disabled");
		leftIndicator.spriteName = (directions.Contains(CoverDirection.Left) ? "Ui_Cover_Enabled" : "Ui_Cover_Disabled");
		if (coverState != CoverIconState.None)
		{
			coverIcon.spriteName = HelpersGfx.GetCoverIconName(coverState);
			if (SingularityMonoBehaviour<AudioManager>.Instance != null)
			{
				SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("combat_ui/cover_select");
			}
		}
		base.gameObject.SetActive(value: true);
	}

	public void SetPosition(Vector3 pos)
	{
		if (followTarget == null)
		{
			followTarget = new GameObject("Cover Move Indicator position");
		}
		followTarget.transform.position = pos;
		FollowTarget(followTarget);
	}
}
