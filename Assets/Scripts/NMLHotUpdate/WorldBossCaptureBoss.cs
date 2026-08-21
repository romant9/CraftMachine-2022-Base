using TWDModel;
using UnityEngine;

public class WorldBossCaptureBoss : WorldBossCaptureBase
{
	[SerializeField]
	private GameObject LockTipContainer;

	public void CloseLockTip()
	{
		Helpers.GameObjectSetActive(LockTipContainer, value: false);
	}

	public override void OnClick()
	{
		if (data.view.State == WorldBossCapturePointState.Locked)
		{
			Helpers.GameObjectSetActive(LockTipContainer, value: true);
		}
		else if (data.view.State == WorldBossCapturePointState.PvpUnoccupied && data.owner == WorldBossCaptureOwner.BOSS)
		{
			base.OnClick();
			WorldBossStartPopup worldBossStartPopup = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.WorldBossStartPopup) as WorldBossStartPopup;
			if (!(worldBossStartPopup == null))
			{
				worldBossStartPopup.Open();
			}
		}
	}

	public override void UpdateUI()
	{
		base.UpdateUI();
		if (data.view.State == WorldBossCapturePointState.Locked)
		{
			UpdateMyLockTipLabels();
		}
		else if (data.view.State == WorldBossCapturePointState.PvpUnoccupied)
		{
			Transform transform = base.transform.Find("NameBG/WarLabel");
			if (transform != null)
			{
				string text = LocalizationManager.GetText(data.definition.BuildingName);
				HelpersUI.SetContentToLabel(transform.GetComponent<UILabel>(), text);
			}
			Transform transform2 = base.transform.Find("NameBG");
			if (transform2 != null)
			{
				transform2.gameObject.SetActive(value: true);
			}
		}
	}

	private void UpdateMyLockTipLabels()
	{
		if (data?.definition != null && !(LockTipContainer == null))
		{
			Transform transform = LockTipContainer.transform.Find("Bg/Bg2");
			if (!(transform == null))
			{
				UILabel label = transform.Find("Title")?.GetComponent<UILabel>();
				UILabel label2 = transform.Find("Title2 (1)")?.GetComponent<UILabel>();
				HelpersUI.SetContentToLabel(label, LocalizationManager.GetText(data.definition.BuildingName));
				HelpersUI.SetContentToLabel(label2, LocalizationManager.GetText(data.definition.BuildingLockedDesc));
			}
		}
	}
}
