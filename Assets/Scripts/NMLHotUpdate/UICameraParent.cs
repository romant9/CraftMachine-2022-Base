using System.Collections;

public class UICameraParent : MonoBehaviourExtended
{
	private bool RunUpdateBlock;

	private void Awake()
	{
		DebugIdString = "DisableUICamera";
		RunUpdateBlock = false;
	}

	public void UpdateState()
	{
		if (base.gameObject != null)
		{
			if (!base.gameObject.activeSelf)
			{
				ActivateIfChilden();
			}
			else if (!RunUpdateBlock)
			{
				StartCoroutine(DelayedUpdate());
			}
		}
	}

	private IEnumerator DelayedUpdate()
	{
		RunUpdateBlock = true;
		yield return null;
		ActivateIfChilden();
		RunUpdateBlock = false;
	}

	private void ActivateIfChilden()
	{
		if (base.gameObject != null)
		{
			base.gameObject.SetActive(base.transform.childCount > 0);
		}
	}
}
