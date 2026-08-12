using UnityEngine;

public class TutorialArrow : HUDElementFollowTarget
{
	private void Start()
	{
		SetActive(active: false);
	}

	public void Show(GameObject parent, bool downwards = true)
	{
		base.gameObject.SetActive(value: true);
		Vector3 zero = Vector3.zero;
		if (!downwards)
		{
			zero.x = 180f;
		}
		base.transform.localRotation = Quaternion.Euler(zero);
		FollowTarget(parent.gameObject);
	}

	public void SetActive(bool active)
	{
		base.gameObject.SetActive(active);
	}
}
