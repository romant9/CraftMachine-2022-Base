using System.Linq;

public class UIButtonMultiToggleSet : UIButtonToggleSetBase
{
	public UIButtonToggle[] Buttons;

	public UIButtonToggle MasterButton;

	private void OnEnable()
	{
		UIButtonToggle[] buttons = Buttons;
		if (IsLoadDataManager)
		{
			if (!IsAwake)
			{
				IsAwake = true;

				for (int i = 0; i < buttons.Length; i++)
				{
					buttons[i].SetClickCallback(OnClick);
				}
				MasterButton.SetClickCallback(OnMasterButtonClick);
				MasterButton.SetToggled(toggled: true);
				OnMasterButtonClick(MasterButton);
			}
			else
			{
				if (lastState[0] == true)
				{
					MasterButton.SetToggled(toggled: true);
					OnMasterButtonClick(MasterButton);
				}
				else
				{
					for (int i = 1; i < lastState.Length; i++)
					{
						if (lastState[i] == true)
						{
							buttons[i - 1].SetToggled(toggled: true);
							OnClick(buttons[i - 1]);
						}
					}
				}
			}
		}
		else
		{
			for (int i = 0; i < buttons.Length; i++)
			{
				buttons[i].SetClickCallback(OnClick);
			}
			MasterButton.SetClickCallback(OnMasterButtonClick);
			MasterButton.SetToggled(toggled: true);
			OnMasterButtonClick(MasterButton);
		}
	}

	private void OnDisable()
	{
		if (IsLoadDataManager)
		{
			lastState = GetState();
			return;
		}
		UIButtonToggle[] buttons = Buttons;
		for (int i = 0; i < buttons.Length; i++)
		{
			buttons[i].RemoveClickCallback(OnClick);
		}
		MasterButton.RemoveClickCallback(OnMasterButtonClick);
	}

	private void OnMasterButtonClick(UIButtonExtended masterButton)
	{
		if (!MasterButton.IsToggled)
		{
			MasterButton.SetToggled(toggled: true);
			return;
		}
		UIButtonToggle[] buttons = Buttons;
		for (int i = 0; i < buttons.Length; i++)
		{
			buttons[i].SetToggled(toggled: false);
		}
		NotifyStatusUpdate();
	}

	private void OnClick(UIButtonExtended button)
	{
		if ((button as UIButtonToggle).IsToggled)
		{
			MasterButton.SetToggled(toggled: false);
		}
		else if (!Buttons.Any((UIButtonToggle x) => x.IsToggled))
		{
			MasterButton.SetToggled(toggled: true);
		}
		NotifyStatusUpdate();
	}

	private void NotifyStatusUpdate()
	{
		if (OnStateUpdate != null)
		{
			OnStateUpdate(GetState());
		}
	}

	public override void ResetToDefault()
	{
		MasterButton.SetToggled(toggled: true);
		OnMasterButtonClick(MasterButton);
	}

	public override bool DefaultIsSelected()
	{
		return MasterButton.IsToggled;
	}

	public override bool[] GetState()
	{
		bool[] array = new bool[Buttons.Length + 1];
		array[0] = MasterButton.IsToggled;
		Buttons.Select((UIButtonToggle x) => x.IsToggled).ToArray().CopyTo(array, 1);
		return array;
	}



	#region myparams
	public bool IsAwake;
	private bool IsLoadDataManager => OfflineManager.IsLoadDataManager;
	private bool[] lastState;
	#endregion
}
