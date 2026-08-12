using System.Collections.Generic;
using TWDModel;
using UnityEngine;

public class StorageRewardListCard : UIListCard<MergeBundleData>
{
	[SerializeField]
	private OptionalItemCard optionalItemCard;

	[SerializeField]
	private UIButton apocalypticButton;

	private CustomRewardStatus _status;

	public override void UpdateUI()
	{
		base.UpdateUI();
		if (base.Item != null)
		{
			CheckStatus();
			if (optionalItemCard != null)
			{
				optionalItemCard.Init(base.Item.Reward);
				optionalItemCard.SetSelectStatus(_status);
			}
			if (apocalypticButton != null)
			{
				apocalypticButton.isEnabled = false;
			}
		}
	}

	private void CheckStatus()
	{
		if (base.Item == null)
		{
			return;
		}
		CustomizedBundleManager customizedBundleManager = GameManager.Instance.playerModel.CustomizedBundleManager;
		if (customizedBundleManager == null)
		{
			return;
		}
		IReward selectRewardByIndex = customizedBundleManager.GetSelectRewardByIndex(base.Item.CustomBundleDefinition.Identifier, base.Item.CurrentSelectIndex);
		List<IReward> selectReward = customizedBundleManager.GetSelectReward(base.Item.CustomBundleDefinition.Identifier);
		_status = CustomRewardStatus.Normal;
		if (selectReward != null)
		{
			if (selectRewardByIndex != null)
			{
				if (customizedBundleManager.CheckTypeEqual(base.Item.CustomBundleDefinition, selectRewardByIndex, base.Item.Reward, isNeedExclusion: false))
				{
					_status = CustomRewardStatus.Selected;
					return;
				}
				for (int i = 0; i < selectReward.Count; i++)
				{
					if (_status != CustomRewardStatus.Selected && selectReward[i] != null && customizedBundleManager.CheckTypeEqual(base.Item.CustomBundleDefinition, selectReward[i], base.Item.Reward, isNeedExclusion: true))
					{
						_status = CustomRewardStatus.CannotSelect;
					}
				}
				return;
			}
			for (int j = 0; j < selectReward.Count; j++)
			{
				if (_status != CustomRewardStatus.Selected && selectReward[j] != null && customizedBundleManager.CheckTypeEqual(base.Item.CustomBundleDefinition, selectReward[j], base.Item.Reward, isNeedExclusion: true))
				{
					_status = CustomRewardStatus.CannotSelect;
				}
			}
		}
		else
		{
			_status = CustomRewardStatus.Normal;
		}
	}

	public void OnClickReward()
	{
		if (base.Item != null && _status != CustomRewardStatus.CannotSelect)
		{
			TWDModelResult tWDModelResult = TWDModelResult.Error;
			if (_status == CustomRewardStatus.Normal)
			{
				tWDModelResult = Helpers.ExecuteCommand(new CustomizedBundleClaimRewardCommand(base.Item.CustomBundleDefinition.Identifier, base.Item.CurrentSelectIndex, base.Item.Reward));
			}
			if (_status == CustomRewardStatus.Selected)
			{
				tWDModelResult = Helpers.ExecuteCommand(new CustomizedBundleClaimRewardCommand(base.Item.CustomBundleDefinition.Identifier, base.Item.CurrentSelectIndex, null));
			}
			if (tWDModelResult == TWDModelResult.OK)
			{
				UIEvent.Send("SelectCustomRewardEvent");
			}
		}
	}

	private void OnEnable()
	{
		UIEvent.OnUIEvent += OnUIEventHandler;
	}

	private void OnDisable()
	{
		UIEvent.OnUIEvent -= OnUIEventHandler;
	}

	private void OnUIEventHandler(string type, object parameter)
	{
		if (type == "SelectCustomRewardEvent")
		{
			UpdateUI();
		}
	}
}
