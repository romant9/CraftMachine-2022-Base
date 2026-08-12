using UnityEngine;

public class UIToggleContent : MonoBehaviour
{
	[SerializeField]
	protected bool SetupOnInstantiate = true;

	private UIToggleMenu AddedToSet;

	public UIToggleMenu GetOwningSet => AddedToSet;

	public virtual void Activate()
	{
		base.gameObject.SetActive(value: true);
	}

	public virtual void Deactivate()
	{
		base.gameObject.SetActive(value: false);
	}

	public void Added(UIToggleMenu addedTo)
	{
		Deactivate();
		AddedToSet = addedTo;
	}

	public void Clean()
	{
		AddedToSet = null;
	}
}
