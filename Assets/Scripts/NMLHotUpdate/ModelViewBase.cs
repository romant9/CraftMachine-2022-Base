using UnityEngine;

public class ModelViewBase : MonoBehaviour
{
	[UniqueIdentifier]
	public string ViewId;

	public virtual bool AutoGenerateViewID => false;
}
