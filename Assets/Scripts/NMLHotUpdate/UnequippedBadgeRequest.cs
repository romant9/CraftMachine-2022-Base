using UnityEngine;

public class UnequippedBadgeRequest : MonoBehaviour
{
	[SerializeField]
	private int index;

	public void RequestUnequippedBadge()
	{
		UIEvent.Send("OnClickBadgeIconRemove", index);
	}
}
