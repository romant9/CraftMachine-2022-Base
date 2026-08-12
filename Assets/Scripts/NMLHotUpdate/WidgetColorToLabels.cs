using System.Collections.Generic;
using UnityEngine;

public class WidgetColorToLabels : MonoBehaviourExtended
{
	[Tooltip("Only read at Awake")]
	[SerializeField]
	public UILabel[] ExcludeList;

	private UIWidget widget;

	private bool ForceUpdateOnce;

	private Color cachedColor = Color.white;

	private List<UILabel> ListOfLabels = new List<UILabel>();

	private void Awake()
	{
		DebugIdString = "WidgetColorToLabels";
		Init();
	}

	private void Update()
	{
		if (!(widget != null) || ListOfLabels == null || ListOfLabels.Count <= 0 || (!(cachedColor != widget.color) && !ForceUpdateOnce))
		{
			return;
		}
		cachedColor = widget.color;
		ForceUpdateOnce = false;
		for (int i = 0; i < ListOfLabels.Count; i++)
		{
			if (ListOfLabels[i] != null && !ExcludeListContains(ListOfLabels[i]))
			{
				ListOfLabels[i].color = cachedColor;
			}
		}
	}

	public void Init(bool triggerUpdate = true)
	{
		GameObject gameObject = null;
		UIButton component = GetComponent<UIButton>();
		gameObject = ((!(component != null)) ? base.gameObject : component.tweenTarget);
		if (gameObject != null)
		{
			widget = gameObject.GetComponent<UIWidget>();
			if (widget != null)
			{
				UILabel[] componentsInChildren = GetComponentsInChildren<UILabel>();
				for (int i = 0; i < componentsInChildren.Length; i++)
				{
					if (componentsInChildren[i] != null && !ExcludeListContains(componentsInChildren[i]))
					{
						ListOfLabels.Add(componentsInChildren[i]);
					}
				}
				componentsInChildren = new UILabel[0];
			}
			else
			{
				DebugLogWarning("Cant copy color. No widget found on GameObject: " + gameObject.name);
			}
		}
		else
		{
			DebugLogWarning("Cant copy color. Target GameObject is NULL!");
		}
		ForceUpdateOnce = triggerUpdate;
	}

	private bool ExcludeListContains(UILabel label)
	{
		if (ExcludeList != null && ExcludeList.Length != 0)
		{
			for (int i = 0; i < ExcludeList.Length; i++)
			{
				if (ExcludeList[i] != null && ExcludeList[i] == label)
				{
					return true;
				}
			}
		}
		return false;
	}
}
