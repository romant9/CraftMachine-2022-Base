using System;
using UnityEngine;

[RequireComponent(typeof(UIWidget))]
[AddComponentMenu("NGUI/Interaction/Envelop Content")]
public class EnvelopContent : MonoBehaviour
{
	[Tooltip("The widgets used to determine the content bounds should reside underneath this root object")]
	public Transform targetRoot;

	[Tooltip("Value added to the left border (usually negative)")]
	public int padLeft;

	[Tooltip("Value added to the right border (usually positive)")]
	public int padRight;

	[Tooltip("Value added to the bottom border (usually negative)")]
	public int padBottom;

	[Tooltip("Value added to the top border (usually positive)")]
	public int padTop;

	[Tooltip("Minimum desired width, used only if the value is above 0")]
	public int minWidth;

	[Tooltip("Minimum desired height, used only if the value is above 0")]
	public int minHeight;

	[Tooltip("If true, disabled widgets will be ignored and won't be used for bounds calculations")]
	public bool ignoreDisabled = true;

	[NonSerialized]
	private bool mStarted;

	private void Start()
	{
		mStarted = true;
		Execute();
	}

	private void OnEnable()
	{
		if (mStarted)
		{
			Execute();
		}
	}

	[ContextMenu("Execute")]
	public void Execute()
	{
		if (targetRoot == base.transform)
		{
			Debug.LogError("Target Root object cannot be the same object that has Envelop Content. Make it a sibling instead.", context: this);
			return;
		}
		if (NGUITools.IsChild(targetRoot, base.transform))
		{
			Debug.LogError("Target Root object should not be a parent of Envelop Content. Make it a sibling instead.", context: this);
			return;
		}
		Bounds bounds = NGUIMath.CalculateRelativeWidgetBounds(base.transform.parent, targetRoot, !ignoreDisabled);
		float num = bounds.min.x + (float)padLeft;
		float num2 = bounds.min.y + (float)padBottom;
		float num3 = bounds.max.x + (float)padRight;
		float num4 = bounds.max.y + (float)padTop;
		if (minWidth > 0)
		{
			num3 = Mathf.Max(num3, num + (float)minWidth);
		}
		if (minHeight > 0)
		{
			num2 = Mathf.Min(num2, num4 - (float)minHeight);
		}
		int num5 = Mathf.RoundToInt(num3 - num);
		int num6 = Mathf.RoundToInt(num4 - num2);
		if ((num5 & 1) == 1)
		{
			num5++;
		}
		if ((num6 & 1) == 1)
		{
			num6++;
		}
		GetComponent<UIWidget>().SetRect(num, num2, num5, num6);
		BroadcastMessage("UpdateAnchors", SendMessageOptions.DontRequireReceiver);
		NGUITools.UpdateWidgetCollider(base.gameObject);
	}
}
