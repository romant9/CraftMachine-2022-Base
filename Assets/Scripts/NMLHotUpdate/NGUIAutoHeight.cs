using System;
using UnityEngine;

[ExecuteInEditMode]
public class NGUIAutoHeight : MonoBehaviour
{
	public UIGrid grid;

	public UITable table;

	public UISprite background;

	private UIWidget widget;

	public int paddingBottom;

	private void Awake()
	{
		widget = GetComponent<UIWidget>();
	}

	private void OnEnable()
	{
		if (grid != null)
		{
			UIGrid uIGrid = grid;
			uIGrid.onReposition = (UIGrid.OnReposition)Delegate.Combine(uIGrid.onReposition, new UIGrid.OnReposition(OnReposition));
		}
		if (table != null)
		{
			UITable uITable = table;
			uITable.onReposition = (UITable.OnReposition)Delegate.Combine(uITable.onReposition, new UITable.OnReposition(OnReposition));
		}
	}

	private void OnDisable()
	{
		if (grid != null)
		{
			UIGrid uIGrid = grid;
			uIGrid.onReposition = (UIGrid.OnReposition)Delegate.Remove(uIGrid.onReposition, new UIGrid.OnReposition(OnReposition));
		}
		if (table != null)
		{
			UITable uITable = table;
			uITable.onReposition = (UITable.OnReposition)Delegate.Remove(uITable.onReposition, new UITable.OnReposition(OnReposition));
		}
	}

	private void Start()
	{
		if (grid != null)
		{
			grid.Reposition();
		}
		else if (table != null)
		{
			table.Reposition();
		}
	}

	[ContextMenu("Execute")]
	private void Execute()
	{
		if (grid != null)
		{
			grid.Reposition();
		}
		else if (table != null)
		{
			table.Reposition();
		}
		widget = GetComponent<UIWidget>();
		OnReposition();
	}

	private void OnTransformChildrenChanged()
	{
		Execute();
	}

	public void OnReposition()
	{
		Transform transform = ((grid != null) ? grid.transform : ((table != null) ? table.transform : null));
		if (transform == null)
		{
			transform = base.transform;
		}
		Bounds bounds = NGUIMath.CalculateRelativeWidgetBounds(transform);
		int num = ((!(transform == base.transform)) ? paddingBottom : 0);
		int num2 = Mathf.RoundToInt(bounds.size.y) + num;
		if (background != null)
		{
			if (background.height != num2)
			{
				background.height = num2;
			}
		}
		else if (widget != null && widget.height != num2)
		{
			widget.height = num2;
		}
	}
}
