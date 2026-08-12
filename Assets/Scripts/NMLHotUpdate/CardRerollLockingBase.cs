using UnityEngine;

public class CardRerollLockingBase : MonoBehaviourExtended
{
	[SerializeField]
	private GameObject buttonLockCardContainer;

	[SerializeField]
	private GameObject buttonUnlockCardContainer;

	[SerializeField]
	private UIButtonWithLabel buttonLockCard;

	[SerializeField]
	private UIButtonWithLabel buttonUnlockCard;

	private int lootIndex = -1;

	public int LootIndex
	{
		get
		{
			return lootIndex;
		}
		set
		{
			lootIndex = value;
		}
	}

	protected void UpdateButtonsImpl()
	{
		bool flag = false;
		if (GameManager.Instance.playerModel.PhoneCall != null)
		{
			flag = GameManager.Instance.playerModel.PhoneCall.IsLootLockedForReroll(LootIndex);
		}
		bool activeSelf = buttonUnlockCardContainer.activeSelf;
		Helpers.GameObjectSetActive(buttonLockCardContainer, !flag);
		Helpers.GameObjectSetActive(buttonUnlockCardContainer, flag);
		if (buttonLockCard != null)
		{
			buttonLockCard.isEnabled = !flag;
			buttonLockCard.SetClickCallback(OnClickLock);
		}
		if (buttonUnlockCard != null)
		{
			buttonUnlockCard.isEnabled = flag;
			buttonUnlockCard.SetClickCallback(OnClickUnlock);
		}
		if (activeSelf != flag)
		{
			//замок появляется и пропадает
			if (!OfflineManager.IsNoEffects)
			{
				TweenManager.PlayTweenGroup(base.gameObject, 3);
			}
		}
	}

	public override void Clear()
	{
		base.Clear();
		if (buttonLockCard != null)
		{
			buttonLockCard.Clear();
		}
		if (buttonUnlockCard != null)
		{
			buttonUnlockCard.Clear();
		}
	}

	private void OnClickLock(UIButtonExtended button)
	{
		UIEvent.Send("OnLockLootEntry", LootIndex);
	}

	private void OnClickUnlock(UIButtonExtended button)
	{
		UIEvent.Send("OnUnlockLootEntry", LootIndex);
	}
}
