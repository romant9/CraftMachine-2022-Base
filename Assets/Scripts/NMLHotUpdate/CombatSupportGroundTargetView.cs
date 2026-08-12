using TWDModel;
using UnityEngine;

public abstract class CombatSupportGroundTargetView : ModelView<CombatSupportModel>
{
	public abstract void Execute(Vector3 position);
}
