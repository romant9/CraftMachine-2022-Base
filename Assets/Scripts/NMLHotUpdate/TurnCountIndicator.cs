public class TurnCountIndicator : HUDElementFollowTarget
{
	public UILabel turnCountLabel;

	public void SetTurnCount(int count)
	{
		if (count < 0)
		{
			base.gameObject.SetActive(value: false);
			return;
		}
		base.gameObject.SetActive(value: true);
		turnCountLabel.text = count.ToString();
	}
}
