using UnityEngine;

public class RemoveMaterialsWinterTint : MonoBehaviour
{
	[Tooltip("Which platform type to check against")]
	[SerializeField]
	private PlatformFlag PlatformCondition;

	[Tooltip("Replaces material tint color")]
	[SerializeField]
	private Color replaceTintColor = Color.white;

	[SerializeField]
	private Material[] materials;

	private void Awake()
	{
		if (materials != null && PlatformInfo.HasFlag(PlatformCondition))
		{
			for (int i = 0; i < materials.Length && !(materials[i] == null) && !(materials[i].color == replaceTintColor); i++)
			{
				materials[i].color = replaceTintColor;
			}
		}
	}
}
