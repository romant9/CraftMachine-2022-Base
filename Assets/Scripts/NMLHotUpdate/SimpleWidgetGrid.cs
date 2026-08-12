using UnityEngine;

public class SimpleWidgetGrid : MonoBehaviour
{
	public bool PositionAtStart;

	public bool ActiveSelf = true;

	public bool ActiveInHierarchy = true;

	public UIWidget[] Widgets;

	private Vector3 NewPosition = Vector3.zero;

	private void Start()
	{
		if (PositionAtStart)
		{
			UpdateGridChildren();
		}
	}

	public void UpdateGridChildren()
	{
		UIWidget uIWidget = null;
		for (int i = 0; i < Widgets.Length; i++)
		{
			if (Widgets[i] != null && (!ActiveSelf || Widgets[i].gameObject.activeSelf) && (!ActiveInHierarchy || Widgets[i].gameObject.activeInHierarchy))
			{
				if (uIWidget != null)
				{
					NewPosition = uIWidget.transform.localPosition;
					NewPosition.y -= (float)uIWidget.height * 0.5f;
					NewPosition.y -= (float)Widgets[i].height * 0.5f;
					Widgets[i].transform.localPosition = NewPosition;
				}
				else
				{
					Widgets[i].transform.localPosition = Vector3.zero;
				}
				uIWidget = Widgets[i];
			}
		}
	}
}
