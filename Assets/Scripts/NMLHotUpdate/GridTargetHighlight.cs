using Client.Constants;
using UnityEngine;

public class GridTargetHighlight : CacheableObject
{
	[Tooltip("The indicator mesh which we should change the color.")]
	private Renderer[] renderObjects;

	private int activeIndex;

	private void Awake()
	{
		renderObjects = GetComponentsInChildren<Renderer>(includeInactive: true);
		for (int i = 0; i < base.transform.childCount; i++)
		{
			base.transform.GetChild(i).gameObject.SetActive(i == activeIndex);
		}
	}

	public void SetIndicatorIndex(int index)
	{
		int childCount = base.transform.childCount;
		activeIndex = Mathf.Clamp(index, 0, childCount - 1);
		for (int i = 0; i < base.transform.childCount; i++)
		{
			base.transform.GetChild(i).gameObject.SetActive(i == activeIndex);
		}
	}

	public void SetIndicatorColor(Color color)
	{
		if (renderObjects != null)
		{
			for (int i = 0; i < renderObjects.Length; i++)
			{
				renderObjects[i].material.SetColor(MaterialParameters.TintColor, color);
			}
		}
	}
}
