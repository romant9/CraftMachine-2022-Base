using TWDModel;
using UnityEngine;

public abstract class PlaceableRunLocationItem : MonoBehaviour, IRunLocationItem
{
	public GridCoordinate EditorGridCoordinate
	{
		get
		{
			GridView activeInstance = GridView.ActiveInstance;
			if (!(activeInstance != null))
			{
				return GridCoordinate.Invalid;
			}
			return activeInstance.GetConfiguredCoordinate(base.transform.position);
		}
	}

	public Vector3 EditorGridCoordinatePosition
	{
		get
		{
			GridView activeInstance = GridView.ActiveInstance;
			if (!(activeInstance != null))
			{
				return new Vector3(0f, 0f, 0f);
			}
			return activeInstance.GetConfiguredPosition(activeInstance.GetConfiguredCoordinate(base.transform.position));
		}
	}

	public virtual bool ShouldReturnModel => true;

	private void OnDrawGizmosSelected()
	{
		GridView activeInstance = GridView.ActiveInstance;
		if (activeInstance != null)
		{
			Gizmos.DrawWireCube(activeInstance.GetConfiguredPosition(EditorGridCoordinate), new Vector3(activeInstance.ConfiguredCellSize.X * 0.9f, 0.1f, activeInstance.ConfiguredCellSize.Y * 0.9f));
		}
	}

	public abstract TWDModelObject Apply(IRunLocationItemContainer runLocation, IRunLocationErrorContext errors);
}
