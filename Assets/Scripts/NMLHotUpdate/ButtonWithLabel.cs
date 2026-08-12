using UnityEngine;

public class ButtonWithLabel : ButtonBase
{
	[Header("Override")]
	[SerializeField]
	private UILabel label;

	private Transform tempObj;

	public string text
	{
		get
		{
			if (UILabelInternal != null)
			{
				return UILabelInternal.text;
			}
			Debug.LogError("ButtonWithLabel: No Label Found!");
			return "";
		}
		set
		{
			if (UILabelInternal != null)
			{
				UILabelInternal.text = value;
			}
			else
			{
				Debug.LogError("ButtonWithLabel: No Label Found!");
			}
		}
	}

	private UILabel UILabelInternal
	{
		get
		{
			if (label == null)
			{
				tempObj = base.transform.Find("Label");
				if (tempObj != null)
				{
					label = tempObj.gameObject.GetComponent<UILabel>();
					tempObj = null;
				}
			}
			return label;
		}
	}

	public override void Clear()
	{
		base.Clear();
		tempObj = null;
		label = null;
	}
}
