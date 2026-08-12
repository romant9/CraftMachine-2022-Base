using TWDModel;
using UnityEngine;

public abstract class CombatModelView : ModelView<TWDModelObject>
{
	public virtual void Kill()
	{
		Object.Destroy(base.gameObject);
	}

	protected virtual void OnDestroy()
	{
		GameManager.Instance.UnregisterViewWithModel(base.Model);
	}
}
