using UnityEngine;

[ExecuteInEditMode]
public class WaveIndicatorGroup : MonoBehaviour
{
	public Vector2 UVScrollSpeed;

	public float SpaceBetweenIndicators;

	public ActorSpawnPointView ConnectedSpawnPointView;

	public int SpawnPointIndex;

	private void Awake()
	{
		UvScroll[] componentsInChildren = base.transform.GetComponentsInChildren<UvScroll>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].uvScrollSpeed = UVScrollSpeed;
		}
	}
}
