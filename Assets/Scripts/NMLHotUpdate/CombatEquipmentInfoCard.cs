using TWDModel;
using UnityEngine;

public class CombatEquipmentInfoCard : MonoBehaviour
{
	[Tooltip("Equipment name label.")]
	[SerializeField]
	private UILabel nameLabel;

	[Tooltip("Equipment image.")]
	[SerializeField]
	private UITexture equipmentImage;

	[Tooltip("Equipment rarity icon.")]
	[SerializeField]
	private UISprite rarityIcon;

	[Tooltip("Equipment stat label.")]
	[SerializeField]
	private UILabel statLabel;

	[Tooltip("Equipment stat icon.")]
	[SerializeField]
	private UISprite statIcon;

	public void Setup(EquipmentItemModel equipment)
	{
		if (nameLabel != null)
		{
			nameLabel.text = HelpersLocalization.GetEquipmentName(equipment);
		}
		if (rarityIcon != null)
		{
			Color rarityColor = HelpersGfx.GetRarityColor(equipment.RarityLevel);
			rarityIcon.color = rarityColor;
		}
		if (statLabel != null)
		{
			statLabel.text = equipment.Damage.ToString();
		}
		if (equipmentImage != null)
		{
			equipmentImage.mainTexture = HelpersGfx.GetEquipmentIconTexture(equipment);
		}
	}
}
