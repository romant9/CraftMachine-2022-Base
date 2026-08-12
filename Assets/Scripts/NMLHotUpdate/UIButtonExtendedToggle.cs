using UnityEngine;

public class UIButtonExtendedToggle : UIButtonExtended
{
	public delegate void ToggleEvent(bool isOn);

	[SerializeField]
	private GameObject onState;

	[SerializeField]
	private GameObject offState;

	private bool isOn = true;

	public bool IsOn
	{
		get
		{
			return isOn;
		}
		private set
		{
			if (isOn != value)
			{
				isOn = value;
				UpdateState();
				if (this.OnToggleValueChanged != null)
				{
					this.OnToggleValueChanged(IsOn);
				}
			}
		}
	}

	public event ToggleEvent OnToggleValueChanged;

	private new void OnEnable()
	{
		SetToggleState(IsOn);
	}

	private void Toggle()
	{
		IsOn = !IsOn;
	}

	protected override void OnClick()
	{
		Toggle();
		base.OnClick();
	}

	protected virtual void UpdateState()
	{
		Helpers.GameObjectSetActive(onState, IsOn);
		Helpers.GameObjectSetActive(offState, !IsOn);
	}

	public void SetToggleState(bool toggleState)
	{
		IsOn = toggleState;
	}

	public void ForceUpdate()
	{
		UpdateState();
		if (this.OnToggleValueChanged != null)
		{
			this.OnToggleValueChanged(IsOn);
		}
	}
}
