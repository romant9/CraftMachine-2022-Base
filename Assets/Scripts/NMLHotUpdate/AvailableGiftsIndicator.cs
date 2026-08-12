using BaseModel;
using UnityEngine;

public class AvailableGiftsIndicator : MonoBehaviour
{
	[SerializeField]
	private UILabel requestsNumberLabel;

	private void OnEnable()
	{
		if (GameManager.Instance != null)
		{
			GameManager.Instance.playerModel.Changed += OnPlayerChanged;
			UpdateUI();
		}
	}

	private void OnDisable()
	{
		if (GameManager.Instance != null)
		{
			GameManager.Instance.playerModel.Changed -= OnPlayerChanged;
		}
	}

	private void OnPlayerChanged(ModelObject modelObject, string changed, object args)
	{
		if (changed == "guildGiftAvailable" || changed == "guildGiftClaimed")
		{
			UpdateUI();
		}
	}

	public void UpdateUI()
	{
		int num = 0;
		GameManager instance = GameManager.Instance;
		if (instance.guildModel != null && instance.playerModel.PendingGuildGiftsToOpen != null)
		{
			num += instance.playerModel.PendingGuildGiftsToOpen.Count;
		}
		if (num == 0)
		{
			NGUITools.SetActiveChildren(base.gameObject, state: false);
			return;
		}
		NGUITools.SetActiveChildren(base.gameObject, state: true);
		requestsNumberLabel.text = num.ToString();
	}
}
