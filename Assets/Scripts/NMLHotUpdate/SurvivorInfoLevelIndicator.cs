using TWDModel;
using UnityEngine;

public class SurvivorInfoLevelIndicator : MonoBehaviour
{
	[SerializeField]
	private Transform container;

	public void SetSurvivor(SurvivorModel survivorModel)
	{
		foreach (Transform item in container)
		{
			item.gameObject.SetActive(value: false);
		}
		if (survivorModel == null)
		{
			return;
		}
		Material material = null;
		string n = "UI_CharacterPreview_" + (survivorModel.SurvivorRarityLevel + 1);
		Transform transform = container.Find(n);
		if (transform != null)
		{
			MeshRenderer componentInChildren = transform.GetComponentInChildren<MeshRenderer>();
			if (componentInChildren != null)
			{
				material = componentInChildren.material;
			}
			transform.gameObject.SetActive(value: true);
		}
		Transform transform2 = container.Find("UI_CharacterPreview_" + survivorModel.SurvivorClass);
		if (transform2 != null)
		{
			MeshRenderer component = transform2.GetComponent<MeshRenderer>();
			if (component != null && material != null)
			{
				component.material = material;
			}
			transform2.gameObject.SetActive(value: true);
		}
	}
}
