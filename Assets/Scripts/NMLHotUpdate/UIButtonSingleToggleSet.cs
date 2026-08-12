using System.Collections.Generic;
using System.Linq;

public class UIButtonSingleToggleSet : UIButtonToggleSetBase
{
    public List<UIButtonToggle> Buttons;

    private void OnEnable()
	{
        List<UIButtonToggle> buttons = Buttons;
		for (int i = 0; i < buttons.Count; i++)
		{
			buttons[i].SetClickCallback(OnClick);
		}
		if (!IsCustomBehaviour)
		{
			Buttons[0].SetToggled(toggled: true);
			OnClick(Buttons[0]);
		}
	}

	private void OnDisable()
	{
        List<UIButtonToggle> buttons = Buttons;
		for (int i = 0; i < buttons.Count; i++)
		{
			buttons[i].RemoveClickCallback(OnClick);
		}
	}

	public void OnClick(UIButtonExtended button)
	{
		UIButtonToggle uIButtonToggle = button as UIButtonToggle;
		if (!IsCustomBehaviour)
		{
			if (!uIButtonToggle.IsToggled)
			{
				uIButtonToggle.SetToggled(toggled: true);
				return;
			}
		}
		else
		{
			if (!uIButtonToggle.IsToggled)
			{
				uIButtonToggle.SetToggled(toggled: false);
				return;
			}
			else
			{
				uIButtonToggle.SetToggled(toggled: true);
			}
		}
		List<UIButtonToggle> buttons = Buttons;
		foreach (UIButtonToggle uIButtonToggle2 in buttons)
		{
			if (!(uIButtonToggle2 == uIButtonToggle))
			{
				uIButtonToggle2.SetToggled(toggled: false);
			}
		}
		if (OnStateUpdate != null)
		{
			OnStateUpdate(GetState());
		}
	}

	public override void ResetToDefault()
	{
		if (!IsCustomBehaviour)
		{
			Buttons[0].SetToggled(toggled: true);
			OnClick(Buttons[0]);
		}
		else
		{
			Buttons[0].SetToggled(toggled: false);
		}
	}

	public override bool DefaultIsSelected()
	{
		return !IsCustomBehaviour ? Buttons[0].IsToggled : !Buttons[0].IsToggled;
	}

	public override bool[] GetState()
	{
		return Buttons.Select((UIButtonToggle x) => x.IsToggled).ToArray();
	}



	#region myparams
	public bool IsCustomBehaviour;
	#endregion
}
