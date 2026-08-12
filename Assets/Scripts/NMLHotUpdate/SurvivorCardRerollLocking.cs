using TWDModel;

public class SurvivorCardRerollLocking : CardRerollLockingBase
{
	private void Awake()
	{
		DebugIdString = "SurviorCardRerollLocking";
	}

	public void UpdateWithModel(SurvivorModel survivorModel)
	{
		if (IsNotNull(survivorModel, "UpdateWithModel"))
		{
			UpdateButtonsImpl();
		}
	}
}
