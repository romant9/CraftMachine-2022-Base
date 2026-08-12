using UnityEngine;

public class TraitPill : MonoBehaviour
{
	[SerializeField]
	private UILabel traitNameLabel;

	[SerializeField]
	private UISprite traitRaritySprite;

	public string Name { get; set; }

	public string Description { get; set; }

	public int RarityLevel { get; set; }

	public void UpdateUI()
	{
		traitNameLabel.text = Name;
		BoxCollider component = GetComponent<BoxCollider>();
		Vector3 size = component.size;
		Vector3 center = component.center;
		size.x = traitNameLabel.localSize.x + 20f;
		center.x = size.x / 2f;
		component.size = size;
		component.center = center;
	}

	private void OnClick()
	{
		NGTooltip.Show(Description);
	}
}
