using UnityEngine;

public class HUDElementFollowTarget : MonoBehaviour
{
	private GameObject target;

	private int left;

	private int bottom;

	private int right;

	private int top;

	public void FollowTarget(GameObject target, int left = 0, int bottom = 0, int right = 0, int top = 0)
	{
		if (GetComponent<UIWidget>() == null)
		{
			Debug.LogError("No UIWidget on " + base.name + ". Cannot follow " + target.name);
			return;
		}
		if (target == null)
		{
			Debug.LogError("Target of " + base.name + " deleted, cannot follow.");
			return;
		}
		this.target = target;
		this.left = left;
		this.bottom = bottom;
		this.right = right;
		this.top = top;
		GetComponent<UIWidget>().SetAnchor(target, left, bottom, right, top);
		if (GetComponent<CombatHUDWidgetUpdater>() != null)
		{
			GetComponent<UIWidget>().updateAnchors = UIRect.AnchorUpdate.OnStart;
		}
	}

	public void UpdateFollowTarget()
	{
		FollowTarget(target, left, bottom, right, top);
	}

	public bool HasTarget()
	{
		return target != null;
	}

	public void PinToHudAnchor(GameObject hudAnchor, int left = 0, int bottom = 0, int right = 0, int top = 0)
	{
		UIWidget component = GetComponent<UIWidget>();
		if (component == null)
		{
			Debug.LogError("No UIWidget on " + base.name + ". Cannot pin to " + hudAnchor.name);
			return;
		}
		if (hudAnchor == null)
		{
			Debug.LogError("HUD anchor of " + base.name + " is null, cannot pin.");
			return;
		}
		target = hudAnchor;
		this.left = left;
		this.bottom = bottom;
		this.right = right;
		this.top = top;
		component.SetAnchor(hudAnchor, left, bottom, right, top);
		component.updateAnchors = UIRect.AnchorUpdate.OnStart;
	}
}
