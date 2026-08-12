public class UIButtonToggle : UIButtonWithLabel
{
	private bool Toggled;

	private const string LogName = "UIToggleButton: ";

	public bool IsToggled => Toggled;

	public override void SetState(State state, bool immediate)
	{
		base.SetState(GetCurrentState(state), immediate);
	}

	public void SetToggled(bool toggled, bool updateState = true)
	{
		Toggled = toggled;
		if (updateState)
		{
			UpdateState();
		}
	}

	public void ForceClick()
	{
		OnClick();
	}

	protected override void OnClick()
	{
		if (Toggled)
		{
			Toggled = false;
		}
		else
		{
			Toggled = true;
		}
		OnClickToggleEvent?.Invoke(Toggled);
		UpdateState();
		base.OnClick();
	}

	private void UpdateState()
	{
		SetState(GetCurrentState(), true);
	}

	private State GetCurrentState(State state = State.Pressed)
	{
		if (!isEnabled)
		{
			return State.Disabled;
		}
		if (!Toggled)
		{
			return state == State.Hover && OfflineManager.IsLoadDataManager ? State.Hover : State.Normal;
		}
		return State.Pressed;
	}



	#region myparams
	public delegate void OnClickToggle(bool IsToggle);
	public event OnClickToggle OnClickToggleEvent;
	#endregion
}
