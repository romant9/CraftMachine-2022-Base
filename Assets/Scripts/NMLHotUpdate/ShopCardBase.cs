using UnityEngine;

public class ShopCardBase<T> : NUIListItem<T> where T : class
{
	[Header("Button for Toolip")]
	[SerializeField]
	private UIButtonExtended tooltipButton;

	public virtual void OnEnable()
	{
		AddListeners();
	}

	public virtual void OnDisable()
	{
		RemoveListeners();
	}

	public virtual void AddListeners()
	{
		if (tooltipButton != null)
		{
			tooltipButton.SetClickCallback(OnClickedTooltipButton);
		}
	}

	public virtual void RemoveListeners()
	{
		if (tooltipButton != null)
		{
			tooltipButton.RemoveClickCallback(OnClickedTooltipButton);
		}
	}

	protected virtual void OnClickedTooltipButton(UIButtonExtended button)
	{
	}
}
