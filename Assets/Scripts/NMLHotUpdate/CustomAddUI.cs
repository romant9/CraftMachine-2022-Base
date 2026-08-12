using TWDModel;
using UnityEngine;

public class CustomAddUI : MonoBehaviour
{
	[SerializeField]
	private OptionalItemCard optionalItemCard;

	[SerializeField]
	private GameObject changeButton;

	[SerializeField]
	private UIButton apocalypticButton;

	private CustomBundleDefinition customBundleDefinition;

	private int _index;

	public void Init(CustomBundleDefinition definition, int index)
	{
		customBundleDefinition = definition;
		_index = index;
		if (apocalypticButton != null)
		{
			apocalypticButton.isEnabled = false;
		}
		Helpers.GameObjectSetActive(optionalItemCard, value: false);
		Helpers.GameObjectSetActive(changeButton, value: false);
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
		MergeBundlePopup mergeBundlePopup = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.MergeBundlePopup) as MergeBundlePopup;
		if (mergeBundlePopup != null)
		{
			mergeBundlePopup.Bind(customBundleDefinition, _index);
			mergeBundlePopup.Open();
		}
	}
}
