using UnityEngine;

public class TooltipTextboxHud : TooltipTextbox
{
	[SerializeField]
	private UILabel[] ParamName;

	[SerializeField]
	private UILabel[] ParaValue;

	public void SetParamAndValuesTexts(string[] paramNames, string[] paramValues)
	{
		Transform transform = null;
		transform = loopUILabels(ParamName, paramNames);
		loopUILabels(ParaValue, paramValues);
		if (ContentSize != null && ContentSize.bottomAnchor != null)
		{
			Transform target = ((transform != null) ? transform : Label.transform);
			int absolute = ContentSize.bottomAnchor.absolute;
			ContentSize.bottomAnchor.target = target;
			ContentSize.bottomAnchor.absolute = absolute;
			ContentSize.ResetAndUpdateAnchors();
		}
		Position();
	}

	public override void SetText(string text)
	{
		base.SetText(text);
		if (Label != null)
		{
			Label.text = text;
			str = null;
		}
	}

	private Transform loopUILabels(UILabel[] UILabels, string[] content)
	{
		Transform result = null;
		for (int i = 0; i < UILabels.Length; i++)
		{
			if (UILabels[i] != null)
			{
				if (i < content.Length && content[i] != null && content[i] != "")
				{
					UILabels[i].text = content[i];
					UILabels[i].gameObject.SetActive(value: true);
					result = UILabels[i].gameObject.transform;
				}
				else
				{
					UILabels[i].gameObject.SetActive(value: false);
				}
			}
		}
		return result;
	}
}
