using UnityEngine;

public class GoldRadioCallDetailPopupOtherItem : MonoBehaviour
{
	[SerializeField]
	private UILabel labelTitle;

	[SerializeField]
	private UILabel labelContent;

	public void SetInfo(string name, string type)
	{
		labelTitle.text = type;
		labelContent.text = name;
	}
}
