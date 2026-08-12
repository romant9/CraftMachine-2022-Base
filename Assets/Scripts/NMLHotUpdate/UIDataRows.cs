using System.Collections.Generic;
using UnityEngine;

public class UIDataRows : MonoBehaviourExtended
{
	[SerializeField]
	private UIDataRow template;

	[SerializeField]
	private GameObject parent;

	[SerializeField]
	private UISprite spriteBackground;

	[SerializeField]
	private int marginHeight = 10;

	private List<UIDataRow> AllRows = new List<UIDataRow>();

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

	public void SetDataToIndex(int rowIndex, string[] textContent, int rarity = -1)
	{
		if (template == null || parent == null)
		{
			Debug.LogError("UIDataRows: template or parent is NULL!");
			return;
		}
		UIDataRow uIDataRow = null;
		if (rowIndex < 0)
		{
			return;
		}
		if (AllRows.Count > rowIndex && AllRows[rowIndex] != null)
		{
			uIDataRow = AllRows[rowIndex];
		}
		else if (rowIndex == 0)
		{
			uIDataRow = template;
			AllRows.Add(uIDataRow);
		}
		else
		{
			uIDataRow = Helpers.InstantiateToList(template.gameObject, parent, AllRows);
			if (uIDataRow != null)
			{
				uIDataRow.name += rowIndex;
			}
		}
		if (uIDataRow != null)
		{
			if (textContent != null)
			{
				uIDataRow.SetDataToLabel(textContent);
				uIDataRow.Show();
			}
			else
			{
				uIDataRow.Hide();
			}
			uIDataRow.UseStarsAsRarityIndicator(rarity);
		}
		else
		{
			Debug.LogError($"UIDataRows: Cant set data to {rowIndex} row is NULL!");
		}
	}

	public void PositionRows()
	{
		if (AllRows == null || !(template != null) || !(template.widget != null))
		{
			return;
		}
		Vector3 vector = new Vector3(0f, 0f, 0f);
		int num = 0;
		for (int i = 1; i < AllRows.Count; i++)
		{
			vector = template.transform.localPosition;
			if (AllRows[i] != null && AllRows[i].IsVisible)
			{
				num++;
				vector.y -= template.widget.localSize.y * (float)num;
				AllRows[i].transform.localPosition = vector;
			}
		}
		if (spriteBackground != null && num > 0)
		{
			rowsHeight = (int)((float)(num + 2) * template.widget.localSize.y);
			spriteBackground.height = rowsHeight + marginHeight;
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
					if (i != 0)
					{
						Object.Destroy(AllRows[i]);
					}
				}
			}
		}
		AllRows = new List<UIDataRow>();
	}
}
