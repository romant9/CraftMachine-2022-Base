using UnityEngine;

public class UIButtonWithLabel : UIButtonExtended
{
	public UILabel firstLabel;

	public UILabel secondLabel;

	[SerializeField]
	private UILabel thirdLabel;

	private bool firstLabelLockState;

	private bool secondLabelLockState;

	private bool thirdLabelLockState;

	public override bool IsVisuallyDisabled
	{
		get
		{
			return base.IsVisuallyDisabled;
		}
		set
		{
			base.IsVisuallyDisabled = value;
			UpdateLabelVisuallyDisabled(firstLabel);
			UpdateLabelVisuallyDisabled(secondLabel);
			UpdateLabelVisuallyDisabled(thirdLabel);
		}
	}

	public void SetContentToLabelOne(string content, bool show = true)
	{
		SetContent(firstLabel, content, show);
	}

	public void SetContentToLabelTwo(string content, bool show = true)
	{
		SetContent(secondLabel, content, show);
	}

	public void SetContentToLabelTwo(string content, Color color, bool show = true)
	{
		secondLabel.color = color;
		SetContent(secondLabel, content, show);
	}

	public void SetContentToLabelOne(string content, Color color, bool show = true)
	{
		firstLabel.color = color;
		SetContent(firstLabel, content, show);
	}

	public void SetContentToColorLabelTwo(string content, Color color, bool show = true)
	{
		secondLabel.color = color;
		UpdateUILabelColor(secondLabel, color);
		SetContent(secondLabel, content, show);
	}

	public void SetContentToColorLabelOne(string content, Color color, bool show = true)
	{
		firstLabel.color = color;
		UpdateUILabelColor(firstLabel, color);
		SetContent(firstLabel, content, show);
	}

	public void SetContentToLabelThree(string content, bool show = true)
	{
		SetContent(thirdLabel, content, show);
	}

	public void SetStateToLabelOne(State state, bool immediate = true, bool lockstate = false)
	{
		UpdateLabelState(firstLabel, state, immediate);
		firstLabelLockState = lockstate;
	}

	public void SetStateToLabelTwo(State state, bool immediate = true, bool lockstate = false)
	{
		UpdateLabelState(secondLabel, state, immediate);
		secondLabelLockState = lockstate;
	}

	public void SetStateToLabelThree(State state, bool immediate = true, bool lockstate = false)
	{
		UpdateLabelState(thirdLabel, state, immediate);
		thirdLabelLockState = lockstate;
	}

	public override void SetState(State state, bool immediate)
	{
		base.SetState(state, immediate);
		if (!firstLabelLockState)
		{
			UpdateLabelState(firstLabel, state, immediate);
		}
		if (!secondLabelLockState)
		{
			UpdateLabelState(secondLabel, state, immediate);
		}
		if (!thirdLabelLockState)
		{
			UpdateLabelState(thirdLabel, state, immediate);
		}
	}

	private void SetContent(UILabel label, string content, bool show = true)
	{
		HelpersUI.SetContentToLabel(label, content, show);
	}

	private void UpdateLabelState(UILabel label, State state, bool instant)
	{
		UILabelColor uILabelColor = null;
		if (label != null && label is UILabelColor)
		{
			uILabelColor = label as UILabelColor;
			if (uILabelColor != null)
			{
				uILabelColor.SetState(state, instant);
			}
		}
	}

	private void UpdateLabelVisuallyDisabled(UILabel label)
	{
		UILabelColor uILabelColor = null;
		if (label != null && label is UILabelColor)
		{
			uILabelColor = label as UILabelColor;
			if (uILabelColor != null)
			{
				uILabelColor.IsVisuallyDisabled = IsVisuallyDisabled;
			}
		}
	}

	private void UpdateUILabelColor(UILabel label, Color color)
	{
		UILabelColor uILabelColor = null;
		if (label != null && label is UILabelColor)
		{
			uILabelColor = label as UILabelColor;
			if (uILabelColor != null)
			{
				uILabelColor.defaultColor = color;
			}
		}
	}
}
