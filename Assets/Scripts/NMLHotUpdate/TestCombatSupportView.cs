public class TestCombatSupportView : CombatSupportView
{
	private void Start()
	{
		base.gameObject.SetActive(value: false);
	}

	public override void Execute()
	{
		base.gameObject.SetActive(value: true);
	}
}
