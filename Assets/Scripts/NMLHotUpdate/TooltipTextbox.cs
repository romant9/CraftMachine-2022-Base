using UnityEngine;

public class TooltipTextbox : TooltipBox
{
	[Tooltip("Target of the text content")]
	[SerializeField]
	protected UILabel Label;

	protected string str;

	[SerializeField]
	protected UISprite bgEra;

	public override void Show()
	{
		base.Show();
		if (!(ContentSize.localSize.y > 200f) || string.IsNullOrEmpty(str))
		{
			return;
		}
		if (OfflineManager.IsLoadDataManager)
		{
			DebugTWD.LogMycode("if (OfflineManager.IsLoadDataManager) return");
			return;
		}
		if (GameManager.Instance.playerModel.Combat == null)
		{
			OnClickHide();
			ToolTipTextBoxAuto toolTipTextBoxAuto = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.ToolTipTextboxAutoPopup) as ToolTipTextBoxAuto;
			if (toolTipTextBoxAuto != null)
			{
				toolTipTextBoxAuto.OpenForAuto(str);
			}
		}
		else
		{
			Debug.LogError("combat !=null" + GameManager.Instance.playerModel.Combat.SceneName);
		}
	}

	public override void Hide()
	{
		if (bgEra == null)
		{
			base.Hide();
		}
	}

	public void OnClickHide()
	{
		base.Hide();
	}

	public override void SetText(string text)
	{
		base.SetText(text);
		if (Label != null)
		{
			Label.text = text;
			str = text;
		}
	}


	#region myparams
	bool isWaitForTouchUp;
	#endregion

	#region mycode

	private void OnEnable()
	{
		if (Input.touchCount > 0)
		{
			isWaitForTouchUp = true;
		}
	}
	public void LateUpdate()
	{
		if (OfflineManager.IsLoadDataManager)
		{
			if (isWaitForTouchUp)
			{
				if (Input.touchCount == 0)
				{
					isWaitForTouchUp = false;
				}
				return;
			}
			if (Input.GetMouseButtonDown(0) || Input.GetKeyUp(KeyCode.Escape) || Input.touchCount > 0)
			{
				base.Hide();
			}
		}
		else
		{
			base.Update();
		}
	}
	#endregion
}
