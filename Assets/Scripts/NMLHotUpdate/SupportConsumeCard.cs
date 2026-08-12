using UnityEngine;

public class SupportConsumeCard : MonoBehaviour
{
	[SerializeField]
	private UISprite supportIcon;

	[SerializeField]
	private UILabel supportNumLabel;

	public void SetContent(string spriteName, int haveNum, int useNum)
	{
		supportIcon.spriteName = spriteName;
		supportNumLabel.text = haveNum + "/" + useNum;
		if (haveNum < useNum)
		{
			supportNumLabel.color = Color.red;
		}
		else
		{
			supportNumLabel.color = Color.white;
		}
	}
}
