using System.Collections.Generic;
using TWDModel;
using UnityEngine;

public class UIDataRowsRadioCall : MonoBehaviourExtended
{
	[SerializeField]
	private GameObject parent;

	[SerializeField]
	private UISprite spriteBackground;

	[SerializeField]
	private GameObject linePrefab;

	private GameObject templatePrivate;

	private UIWidget lineWidget;

	private List<UIDataRowRadioCall> AllRows = new List<UIDataRowRadioCall>();

	private int rowsHeight;

	public int Count
	{
		get
		{
			if (AllRows != null)
			{
				return AllRows.Count;
			}
			return 0;
		}
	}

	public void SetDataToIndex(int rowIndex, ItemAmountProbabilityData data)
	{
		UIDataRowRadioCall uIDataRowRadioCall = null;
		if (rowIndex < 0)
		{
			return;
		}
		uIDataRowRadioCall = ((AllRows.Count <= rowIndex || !(AllRows[rowIndex] != null)) ? Helpers.InstantiateToList(linePrefab, parent, AllRows) : AllRows[rowIndex]);
		if (uIDataRowRadioCall != null)
		{
			lineWidget = uIDataRowRadioCall.GetComponent<UIWidget>();
			if (data != null)
			{
				uIDataRowRadioCall.SetData(data);
				uIDataRowRadioCall.Show();
			}
			else
			{
				uIDataRowRadioCall.Hide();
			}
		}
		else
		{
			Debug.LogError($"UIDataRows: Cant set data to {rowIndex} row is NULL!");
		}
	}

	public void PositionRows()
	{
		if (AllRows == null || !(parent != null) || !(lineWidget != null))
		{
			return;
		}
		Vector3 vector = new Vector3(0f, 0f, 0f);
		int num = 0;
		for (int i = 0; i < AllRows.Count; i++)
		{
			vector = parent.transform.localPosition;
			if (AllRows[i] != null && AllRows[i].IsVisible)
			{
				vector.y -= lineWidget.localSize.y * (float)num;
				AllRows[i].transform.localPosition = vector;
				num++;
			}
		}
		if (spriteBackground != null && num > 0)
		{
			rowsHeight = (int)((float)num * lineWidget.localSize.y);
			spriteBackground.height += rowsHeight;
		}
	}

	public int GetRowsHeight()
	{
		return rowsHeight;
	}

	public override void Clear()
	{
		base.Clear();
		if (AllRows != null)
		{
			for (int i = 0; i < AllRows.Count; i++)
			{
				if (AllRows[i] != null)
				{
					AllRows[i].Clear();
					Object.Destroy(AllRows[i]);
				}
			}
		}
		AllRows = new List<UIDataRowRadioCall>();
	}
}
