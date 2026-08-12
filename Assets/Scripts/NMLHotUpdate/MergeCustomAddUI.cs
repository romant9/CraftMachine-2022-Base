using UnityEngine;

public class MergeCustomAddUI : MonoBehaviour
{
	[SerializeField]
	private OptionalItemCard optionalItemCard;

	[SerializeField]
	private GameObject changeButton;

	[SerializeField]
	private GameObject selectObj;

	[SerializeField]
	private UIButton apocalypticButton;

	private int _currentIndex;

	public void Init(int index)
	{
		_currentIndex = index;
		if (apocalypticButton != null)
		{
			apocalypticButton.isEnabled = false;
		}
		Helpers.GameObjectSetActive(optionalItemCard, value: false);
		Helpers.GameObjectSetActive(changeButton, value: false);
		Helpers.GameObjectSetActive(selectObj, GameManager.Instance.playerModel.CustomizedBundleManager.currentSelectIndex == _currentIndex);
	}

	public void ShowReward(IReward reward = null)
	{
		Helpers.GameObjectSetActive(optionalItemCard, value: false);
		Helpers.GameObjectSetActive(changeButton, value: false);
		if (reward != null)
		{
			optionalItemCard.Init(reward);
			Helpers.GameObjectSetActive(optionalItemCard, value: true);
			Helpers.GameObjectSetActive(changeButton, value: true);
		}
	}

	public void OnClickReward()
	{
		if (GameManager.Instance.playerModel.CustomizedBundleManager.currentSelectIndex != _currentIndex)
		{
			GameManager.Instance.playerModel.CustomizedBundleManager.currentSelectIndex = _currentIndex;
			UIEvent.Send("SelectCustomStorageEvent", _currentIndex);
		}
	}
}
