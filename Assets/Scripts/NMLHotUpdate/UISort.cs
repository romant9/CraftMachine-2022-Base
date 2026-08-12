using System;
using UnityEngine;

public class UISort : MonoBehaviour
{
	private const float OFFSET_MULTIPLIER = 100f;

	private UIWidget[] widgetsInDepthOrder;

	public void Start()
	{
		widgetsInDepthOrder = base.gameObject.GetComponentsInChildren<UIWidget>();
		Array.Sort(widgetsInDepthOrder, delegate(UIWidget a, UIWidget b)
		{
			if (a.depth < b.depth)
			{
				return -1;
			}
			return (a.depth > b.depth) ? 1 : 0;
		});
	}

	public void Update()
	{
		for (int i = 0; i < widgetsInDepthOrder.Length; i++)
		{
			widgetsInDepthOrder[i].depth = (int)((0f - base.gameObject.transform.localPosition.y) * 100f) + i;
		}
	}
}
