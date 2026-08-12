using TWDModel;
using UnityEngine;

public abstract class RunLocationItem : MonoBehaviour, IRunLocationItem
{
	public virtual bool ShouldReturnModel => true;

	public abstract TWDModelObject Apply(IRunLocationItemContainer runLocation, IRunLocationErrorContext errors);
}
