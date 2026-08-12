using UnityEngine;

public class TooltipBase : MonoBehaviour
{
	private GameObject TargetGameObject;

	private const string LogName = "TooltipBase: ";

	private Vector3 newPosition = Vector3.zero;

	public TooltipTarget TooltipTarget { get; set; }

	public virtual void Update()
	{
		if (Input.GetMouseButtonDown(0) || Input.GetKeyUp(KeyCode.Escape))
		{
			TooltipManager.HideAll();
		}
	}

	public GameObject GetTarget()
	{
		return TargetGameObject;
	}

	public virtual void Show()
	{
		Activate();
	}

	public virtual void Hide()
	{
		Deactivate();
	}

	public virtual void SetTarget(GameObject target)
	{
		if (target != null)
		{
			if (target.GetComponentInChildren<TooltipTarget>() != null)
			{
				TooltipTarget = target.GetComponentInChildren<TooltipTarget>();
				TargetGameObject = TooltipTarget.gameObject;
			}
			else
			{
				TooltipTarget = null;
				TargetGameObject = target;
			}
		}
		else
		{
			Debug.LogError("TooltipBase: Can't Set NULL Target!");
		}
	}

	public virtual void Overlay()
	{
		if (TargetGameObject != null)
		{
			base.transform.OverlayPosition(TargetGameObject.transform);
			newPosition = base.transform.localPosition;
			newPosition.z = 0f;
			base.transform.localPosition = newPosition;
		}
		else
		{
			Debug.LogError("TooltipBase: Can't Overlay. Target is NULL!");
		}
	}

	public virtual void SetText(string text)
	{
	}

	protected virtual void Activate()
	{
		SetActiveGamebject(base.gameObject, value: true);
	}

	protected virtual void Deactivate()
	{
		SetActiveGamebject(base.gameObject, value: false);
		TargetGameObject = null;
	}

	private void SetActiveGamebject(GameObject obj, bool value)
	{
		if (obj != null && obj.activeSelf != value)
		{
			obj.SetActive(value);
		}
	}
}
