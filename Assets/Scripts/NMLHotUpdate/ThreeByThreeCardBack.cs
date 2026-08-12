using UnityEngine;

public class ThreeByThreeCardBack : MonoBehaviour
{
	[Tooltip("ResourceCard")]
	[SerializeField]
	public GameObject ResourceCard;

	[Tooltip("EquipmentCard")]
	[SerializeField]
	public GameObject EquipmentCard;

	[Tooltip("CardLabel")]
	[SerializeField]
	public UILabel CardLabel;

	public void SetOpenable(bool canOpen)
	{
		if (CardLabel != null)
		{
			if (canOpen)
			{
				CardLabel.text = SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText("Popups.Rewards.CardBackOpen");
			}
			else
			{
				CardLabel.text = SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText("Popups.Rewards.CardBackBuyMore");
			}
		}
	}
}
