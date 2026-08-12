using UnityEngine;

public class ThingsToDoIndicator : MonoBehaviour
{
	[SerializeField]
	private UILabel numberlLabel;

	public int Number { get; private set; }

	public void SetNumber(int number)
	{
		Number = number;
		if (number <= 0)
		{
			NGUITools.SetActiveChildren(base.gameObject, state: false);
			return;
		}
		NGUITools.SetActiveChildren(base.gameObject, state: true);
		HelpersUI.SetContentToLabel(numberlLabel, number.ToString());
	}
}
