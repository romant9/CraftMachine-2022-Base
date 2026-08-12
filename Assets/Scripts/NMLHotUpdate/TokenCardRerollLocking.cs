public class TokenCardRerollLocking : CardRerollLockingBase
{
	private void Awake()
	{
		DebugIdString = "SurviorCardRerollLocking";
	}

	public void UpdateLockingButtons()
	{
		UpdateButtonsImpl();
	}
}
