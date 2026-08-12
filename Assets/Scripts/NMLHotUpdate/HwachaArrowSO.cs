using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/HwachaArrowSO", order = 1)]
public class HwachaArrowSO : ScriptableObject
{
	public float DisableTrailEmissionTime = 0.01f;

	public float TrailFadeoutTime = 0.4f;

	public float DisableTrailTime = 0.8f;

	[Range(0f, 1f)]
	public float StuckDepthMin = 0.25f;

	[Range(0f, 1f)]
	public float StuckDepthMax = 0.6f;

	public LayerMask CollisionLayerMask;
}
