using UnityEngine;

public class ListPanel : MonoBehaviour
{
	[SerializeField]
	[Tooltip("Is it a vertical or horizontal list panel?")]
	private UIScrollView.Movement orientation;

	[Tooltip("The GameObject that will contain the slots.")]
	public GameObject container;

	[SerializeField]
	[Tooltip("The prefab for each of the slot.")]
	private GameObject slotPrefab;

	[SerializeField]
	protected int pixelsBetweenSlots;

	public int NumberSlots { get; private set; }

	private void Clear()
	{
		for (int i = 0; i < container.transform.childCount; i++)
		{
			NGUITools.Destroy(container.transform.GetChild(i).gameObject);
		}
		container.RemoveAllChildren();
	}

	protected void CreateSlots(int numberSlots)
	{
		if (NumberSlots != numberSlots)
		{
			Clear();
			NumberSlots = numberSlots;
			Vector3 size = slotPrefab.GetComponent<BoxCollider>().size;
			Vector3 zero = Vector3.zero;
			Vector3 zero2 = Vector3.zero;
			if (orientation == UIScrollView.Movement.Horizontal)
			{
				zero.x = (float)(-(NumberSlots - 1)) * size.x / 2f - (float)(pixelsBetweenSlots / 2 * (NumberSlots - 1));
				zero2.x = size.x + (float)pixelsBetweenSlots;
			}
			else
			{
				zero.y = (float)(NumberSlots - 1) * size.y / 2f + (float)(pixelsBetweenSlots / 2 * (NumberSlots - 1));
				zero2.y -= size.y + (float)pixelsBetweenSlots;
			}
			for (int i = 0; i < NumberSlots; i++)
			{
				container.AddChild(slotPrefab).transform.localPosition = zero;
				zero += zero2;
			}
		}
	}

	public Transform GetSlotAt(int index)
	{
		if (container.transform.childCount <= index)
		{
			return null;
		}
		return container.transform.GetChild(index);
	}
}
