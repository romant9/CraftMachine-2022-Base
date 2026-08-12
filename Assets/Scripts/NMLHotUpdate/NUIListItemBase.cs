using UnityEngine;

[RequireComponent(typeof(UIDragScrollView))]
public class NUIListItemBase : MonoBehaviourExtended
{
	private BoxCollider boxColliderRef;

	private int index = -1;

	protected static Vector3[] corners = new Vector3[4];

	public virtual BoxCollider boxCollider
	{
		get
		{
			if (boxColliderRef == null)
			{
				boxColliderRef = GetComponent<BoxCollider>();
			}
			return boxColliderRef;
		}
		set
		{
			boxColliderRef = value;
		}
	}

	public virtual void SetPosition(Vector3 newPosition)
	{
		base.transform.localPosition = newPosition;
	}

	public virtual void SetPosition(Vector3 newPosition, int newIndex = -1)
	{
		base.transform.localPosition = newPosition;
		SetIndex(newIndex);
	}

	public virtual void SetIndex(int newIndex)
	{
		index = newIndex;
	}

	public Vector3 GetLocalSize(bool useLocalScale = false)
	{
		if (boxCollider != null)
		{
			if (useLocalScale && base.transform != null)
			{
				Vector3 size = boxCollider.size;
				size.x *= base.transform.localScale.x;
				size.y *= base.transform.localScale.y;
				size.z *= base.transform.localScale.z;
				return size;
			}
			return boxCollider.size;
		}
		return Vector3.zero;
	}

	public Vector3 GetLocalSizeHalf(bool useLocalScale = false)
	{
		return GetLocalSize(useLocalScale) * 0.5f;
	}

	public virtual Vector3[] GetLocalCorners(bool useLocalScale = false)
	{
		float num = base.transform.localPosition.x - GetLocalSizeHalf(useLocalScale).x;
		float num2 = base.transform.localPosition.y - GetLocalSizeHalf(useLocalScale).y;
		float x = num + GetLocalSize(useLocalScale).x;
		float y = num2 + GetLocalSize(useLocalScale).y;
		corners[0] = new Vector3(num, num2);
		corners[1] = new Vector3(num, y);
		corners[2] = new Vector3(x, y);
		corners[3] = new Vector3(x, num2);
		return corners;
	}

	public virtual void UpdateUI()
	{
	}

	public virtual void AddedToParent(NUIScrollableList parentList)
	{
	}

	public virtual void RemovedFromParent(NUIScrollableList parentList)
	{
	}

	public virtual int GetSortValue()
	{
		return 0;
	}

	public virtual int GetIndexValue()
	{
		return index;
	}
}
