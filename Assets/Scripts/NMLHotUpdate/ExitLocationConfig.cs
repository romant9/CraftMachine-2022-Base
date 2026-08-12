using UnityEngine;

public class ExitLocationConfig : MonoBehaviour
{
	private void Start()
	{
	}

	private void OnDrawGizmos()
	{
		if (base.transform.parent == null || base.transform.parent.GetComponent<CombatExitView>() == null)
		{
			Gizmos.DrawIcon(base.transform.position, "Icon_House_Red");
		}
		else
		{
			Gizmos.DrawIcon(base.transform.position, "Icon_House");
		}
	}

	private void OnDrawGizmosSelected()
	{
		GridView activeInstance = GridView.ActiveInstance;
		if (activeInstance != null)
		{
			Gizmos.DrawWireCube(activeInstance.GetConfiguredPosition(activeInstance.GetConfiguredCoordinate(base.transform.position)), new Vector3(activeInstance.ConfiguredCellSize.X * 0.9f, 0.1f, activeInstance.ConfiguredCellSize.Y * 0.9f));
		}
	}
}
