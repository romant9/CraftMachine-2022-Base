using System;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("NGUI/Interaction/Grid")]
public class UIGrid : UIWidgetContainer
{
	public delegate void OnReposition();

	public Vector2 offset;

	[DoNotObfuscateNGUI]
	public enum Arrangement
	{
		Horizontal = 0,
		Vertical = 1,
		CellSnap = 2
	}

	[DoNotObfuscateNGUI]
	public enum Sorting
	{
		None = 0,
		Alphabetic = 1,
		Horizontal = 2,
		Vertical = 3,
		Custom = 4
	}

	[DoNotObfuscateNGUI]
	public enum Expansion
	{
		Legacy = 0,
		BasedOnPivot = 1
	}

	public Arrangement arrangement;

	public Sorting sorting;

	[Tooltip("Whether the sort order will be inverted")]
	public bool inverted;

	[Tooltip("Final pivot point for the grid's content.")]
	public UIWidget.Pivot pivot;

	[Tooltip("Legacy style expansion positions children to the right and down from the first one. Pivot-based expansion positions children moving away from the pivot instead, and centered if necessary.")]
	public Expansion expansionStyle;

	public int maxPerLine;

	public float cellWidth = 200f;

	public float cellHeight = 200f;

	[Tooltip("Whether the grid will smoothly animate its children into the correct place.")]
	public bool animateSmoothly;

	[Tooltip("If 'true' and Animate Smoothly is also 'true', will check to see if elements have a TweenAlpha on them. If so, elements will appear in their target position instead of animating from the current position.")]
	public bool animateFadeIn;

	public bool hideInactive;

	public bool keepWithinPanel;

	public OnReposition onReposition;

	public Comparison<Transform> onCustomSort;

	[HideInInspector]
	[SerializeField]
	private bool sorted;

	protected bool mReposition;

	protected UIPanel mPanel;

	protected bool mInitDone;

	[NonSerialized]
	private List<SpringPosition> mSprings;

	public bool repositionNow
	{
		set
		{
			if (value)
			{
				mReposition = true;
				base.enabled = true;
			}
		}
	}

	public List<Transform> GetChildList()
	{
		Transform transform = base.transform;
		List<Transform> list = new List<Transform>();
		for (int i = 0; i < transform.childCount; i++)
		{
			Transform child = transform.GetChild(i);
			if ((!hideInactive || ((bool)child && child.gameObject.activeSelf)) && !UIDragDropItem.IsDragged(child.gameObject))
			{
				list.Add(child);
			}
		}
		if (sorting != 0 && arrangement != Arrangement.CellSnap)
		{
			if (sorting == Sorting.Alphabetic)
			{
				if (inverted)
				{
					list.Sort(SortByNameInv);
				}
				else
				{
					list.Sort(SortByName);
				}
			}
			else if (sorting == Sorting.Horizontal)
			{
				if (inverted)
				{
					list.Sort(SortHorizontalInv);
				}
				else
				{
					list.Sort(SortHorizontal);
				}
			}
			else if (sorting == Sorting.Vertical)
			{
				if (inverted)
				{
					list.Sort(SortVerticalInv);
				}
				else
				{
					list.Sort(SortVertical);
				}
			}
			else if (onCustomSort != null)
			{
				list.Sort(onCustomSort);
			}
			else
			{
				Sort(list);
			}
		}
		return list;
	}

	public Transform GetChild(int index)
	{
		List<Transform> childList = GetChildList();
		if (index >= childList.Count)
		{
			return null;
		}
		return childList[index];
	}

	public int GetIndex(Transform trans)
	{
		return GetChildList().IndexOf(trans);
	}

	[Obsolete("Use gameObject.AddChild or transform.parent = gridTransform")]
	public void AddChild(Transform trans)
	{
		if (trans != null)
		{
			trans.parent = base.transform;
			ResetPosition(GetChildList());
		}
	}

	[Obsolete("Use gameObject.AddChild or transform.parent = gridTransform")]
	public void AddChild(Transform trans, bool sort)
	{
		if (trans != null)
		{
			trans.parent = base.transform;
			ResetPosition(GetChildList());
		}
	}

	public bool RemoveChild(Transform t)
	{
		List<Transform> childList = GetChildList();
		if (childList.Remove(t))
		{
			ResetPosition(childList);
			return true;
		}
		return false;
	}

	protected virtual void Init()
	{
		mInitDone = true;
		mPanel = NGUITools.FindInParents<UIPanel>(base.gameObject);
	}

	protected virtual void Start()
	{
		if (!mInitDone)
		{
			Init();
		}
		bool flag = animateSmoothly;
		animateSmoothly = false;
		Reposition();
		animateSmoothly = flag;
		base.enabled = false;
	}

	protected virtual void Update()
	{
		if (mSprings != null && mSprings.Count != 0)
		{
			bool flag = false;
			foreach (SpringPosition mSpring in mSprings)
			{
				if ((bool)mSpring && mSpring.enabled)
				{
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				mSprings.Clear();
				base.enabled = false;
			}
			if (keepWithinPanel)
			{
				ConstrainWithinPanel();
			}
			if (onReposition != null)
			{
				onReposition();
			}
		}
		else
		{
			Reposition();
			base.enabled = false;
		}
	}

	private void OnValidate()
	{
		if (!Application.isPlaying && NGUITools.GetActive(this))
		{
			Reposition();
		}
	}

	public static int SortByName(Transform a, Transform b)
	{
		return string.Compare(a.name, b.name);
	}

	public static int SortByNameInv(Transform a, Transform b)
	{
		return string.Compare(b.name, a.name);
	}

	public static int SortHorizontal(Transform a, Transform b)
	{
		return a.localPosition.x.CompareTo(b.localPosition.x);
	}

	public static int SortHorizontalInv(Transform a, Transform b)
	{
		return b.localPosition.x.CompareTo(a.localPosition.x);
	}

	public static int SortVertical(Transform a, Transform b)
	{
		return b.localPosition.y.CompareTo(a.localPosition.y);
	}

	public static int SortVerticalInv(Transform a, Transform b)
	{
		return a.localPosition.y.CompareTo(b.localPosition.y);
	}

	protected virtual void Sort(List<Transform> list)
	{
	}

	[ContextMenu("Execute")]
	public virtual void Reposition()
	{
		if (Application.isPlaying && !mInitDone && NGUITools.GetActive(base.gameObject))
		{
			Init();
		}
		if (sorted)
		{
			sorted = false;
			if (sorting == Sorting.None)
			{
				sorting = Sorting.Alphabetic;
			}
			NGUITools.SetDirty(this);
		}
		List<Transform> childList = GetChildList();
		ResetPosition(childList);
		if (keepWithinPanel)
		{
			ConstrainWithinPanel();
		}
		if (onReposition != null)
		{
			onReposition();
		}
	}

	public void ConstrainWithinPanel()
	{
		if (mPanel != null)
		{
			mPanel.ConstrainTargetToBounds(base.transform, immediate: true);
			UIScrollView component = mPanel.GetComponent<UIScrollView>();
			if (component != null)
			{
				component.UpdateScrollbars(recalculateBounds: true);
			}
		}
	}

	protected virtual void ResetPosition(List<Transform> list)
	{
		mReposition = false;
		if (mSprings != null)
		{
			mSprings.Clear();
		}
		int num = 0;
		int num2 = 0;
		int num3 = 0;
		int num4 = 0;
		bool flag = animateSmoothly && base.gameObject.activeInHierarchy && Application.isPlaying;
		float num5 = 0f;
		int i = 0;
		for (int count = list.Count; i < count; i++)
		{
			Transform transform = list[i];
			Vector3 vector = transform.localPosition;
			Vector3 offset3 = offset;
			float z = vector.z;
			if (arrangement == Arrangement.CellSnap)
			{
				if (cellWidth > 0f)
				{
					vector.x = Mathf.Round(vector.x / cellWidth) * cellWidth;
				}
				if (cellHeight > 0f)
				{
					vector.y = Mathf.Round(vector.y / cellHeight) * cellHeight;
				}
			}
			else
			{
				vector = ((arrangement == Arrangement.Horizontal) ? new Vector3(cellWidth * (float)num, (0f - cellHeight) * (float)num2, z) + offset3 : new Vector3(cellWidth * (float)num2, (0f - cellHeight) * (float)num, z));
				if (expansionStyle == Expansion.BasedOnPivot)
				{
					if (pivot == UIWidget.Pivot.Bottom || pivot == UIWidget.Pivot.BottomLeft || pivot == UIWidget.Pivot.BottomRight)
					{
						vector.y = 0f - vector.y;
					}
					if (pivot == UIWidget.Pivot.Right || pivot == UIWidget.Pivot.BottomRight || pivot == UIWidget.Pivot.TopRight)
					{
						vector.x = 0f - vector.x;
					}
					if (num5 != 0f)
					{
						if (arrangement == Arrangement.Horizontal)
						{
							if (pivot == UIWidget.Pivot.Top || pivot == UIWidget.Pivot.Bottom)
							{
								vector.x += num5;
							}
						}
						else if (arrangement == Arrangement.Vertical && (pivot == UIWidget.Pivot.Left || pivot == UIWidget.Pivot.Right))
						{
							vector.y -= num5;
						}
					}
				}
			}
			if (flag && animateFadeIn)
			{
				TweenAlpha component = transform.GetComponent<TweenAlpha>();
				if (component != null && component.enabled && component.value == 0f && component.to == 1f)
				{
					flag = false;
				}
			}
			if (flag)
			{
				SpringPosition component2 = transform.gameObject.GetComponent<SpringPosition>();
				if (component2 != null)
				{
					component2.Finish();
				}
			}
			if (flag)
			{
				SpringPosition springPosition = SpringPosition.Begin(transform.gameObject, vector, 15f);
				springPosition.ignoreTimeScale = true;
				if (mSprings == null)
				{
					mSprings = new List<SpringPosition>();
				}
				mSprings.Add(springPosition);
			}
			else
			{
				transform.localPosition = vector;
			}
			num3 = Mathf.Max(num3, num);
			num4 = Mathf.Max(num4, num2);
			if (++num >= maxPerLine && maxPerLine > 0)
			{
				num = 0;
				num2++;
				int num6 = list.Count - i;
				if (num6 < maxPerLine)
				{
					num5 = Mathf.Round((float)(maxPerLine - num6 + 1) * 0.5f * ((arrangement == Arrangement.Horizontal) ? cellWidth : cellHeight));
				}
			}
		}
		if (pivot != 0)
		{
			Vector2 pivotOffset = NGUIMath.GetPivotOffset(pivot);
			float num7;
			float num8;
			if (arrangement == Arrangement.Horizontal)
			{
				num7 = Mathf.Lerp(0f, (float)num3 * cellWidth, pivotOffset.x);
				num8 = Mathf.Lerp((float)(-num4) * cellHeight, 0f, pivotOffset.y);
			}
			else
			{
				num7 = Mathf.Lerp(0f, (float)num4 * cellWidth, pivotOffset.x);
				num8 = Mathf.Lerp((float)(-num3) * cellHeight, 0f, pivotOffset.y);
			}
			if (expansionStyle == Expansion.BasedOnPivot && arrangement != Arrangement.CellSnap)
			{
				if (pivot == UIWidget.Pivot.Bottom || pivot == UIWidget.Pivot.BottomLeft || pivot == UIWidget.Pivot.BottomRight)
				{
					num8 = 0f;
				}
				if (pivot == UIWidget.Pivot.Right || pivot == UIWidget.Pivot.BottomRight || pivot == UIWidget.Pivot.TopRight)
				{
					num7 = 0f;
				}
			}
			foreach (Transform item in list)
			{
				SpringPosition springPosition2 = (flag ? item.GetComponent<SpringPosition>() : null);
				if (springPosition2 != null && springPosition2.enabled)
				{
					springPosition2.target.x -= num7;
					springPosition2.target.y -= num8;
					continue;
				}
				Vector3 localPosition = item.localPosition;
				localPosition.x -= num7;
				localPosition.y -= num8;
				item.localPosition = localPosition;
			}
		}
		if (mSprings != null && mSprings.Count != 0)
		{
			base.enabled = true;
		}
	}
}
