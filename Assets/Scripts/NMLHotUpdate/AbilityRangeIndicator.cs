using Client.Constants;
using UnityEngine;

public class AbilityRangeIndicator : MonoBehaviour
{
	[Tooltip("The indicator mesh which we should change the color.")]
	[SerializeField]
	private Renderer renderObject;

	public void SetIndicatorColor(Color color)
	{
		if (renderObject != null)
		{
			renderObject.material.SetColor(MaterialParameters.TintColor, color);
		}
	}
}
