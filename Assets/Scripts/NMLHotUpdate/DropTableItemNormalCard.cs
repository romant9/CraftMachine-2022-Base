using TWDModel;
using UnityEngine;

public class DropTableItemNormalCard : NUIListItem<DropTableItem>
{
	[SerializeField]
	private UILabel labelHeader;

	[SerializeField]
	private UILabel labelDescription;

	[SerializeField]
	private UIDataRows infoRows;

	[SerializeField]
	private BoxCollider tableCollider;

	public override void SetData(DropTableItem data)
	{
		base.SetData(data);
		if (labelHeader != null)
		{
			labelHeader.text = data.DropName;
		}
		if (labelDescription != null)
		{
			labelDescription.text = data.Description;
		}
		if (!(infoRows != null) || data == null || data.Probabilities == null)
		{
			return;
		}
		for (int i = 0; i < data.Probabilities.Count; i++)
		{
			FixedPoint fixedPoint = data.Probabilities[i].Probability * 100.0;
			if (fixedPoint > 0L)
			{
				infoRows.SetDataToIndex(i, new string[3]
				{
					data.Probabilities[i].Name,
					data.Probabilities[i].Amount,
					$"{(float)fixedPoint:0.##}%"
				}, data.Probabilities[i].Rarity);
			}
		}
		infoRows.PositionRows();
		if (tableCollider != null)
		{
			tableCollider.size = new Vector3(tableCollider.size.x, tableCollider.size.y + (float)infoRows.GetRowsHeight(), tableCollider.size.z);
			tableCollider.center = new Vector3(tableCollider.center.x, tableCollider.center.y - (float)(infoRows.GetRowsHeight() / 2), tableCollider.center.z);
		}
	}

	public override Vector3[] GetLocalCorners(bool useLocalScale = false)
	{
		float num = base.transform.localPosition.x + tableCollider.center.x - GetLocalSizeHalf(useLocalScale).x;
		float num2 = base.transform.localPosition.y + tableCollider.center.y - GetLocalSizeHalf(useLocalScale).y;
		float x = num + GetLocalSize(useLocalScale).x;
		float y = num2 + GetLocalSize(useLocalScale).y;
		NUIListItemBase.corners[0] = new Vector3(num, num2);
		NUIListItemBase.corners[1] = new Vector3(num, y);
		NUIListItemBase.corners[2] = new Vector3(x, y);
		NUIListItemBase.corners[3] = new Vector3(x, num2);
		return NUIListItemBase.corners;
	}

	public override void SetPosition(Vector3 newPosition)
	{
		if (newPosition != Vector3.zero)
		{
			newPosition.y -= boxCollider.center.y;
		}
		base.SetPosition(newPosition);
	}
}
