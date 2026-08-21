using BaseModel;
using TWDModel;
using UnityEngine;

public class WorldBossCapturePVE : WorldBossCaptureBase
{
	[SerializeField]
	private GameObject MyLockContainer;

	[SerializeField]
	private GameObject OtherLockContainer;

	[SerializeField]
	private GameObject MyUnlockAndNowContainer;

	[SerializeField]
	private UILabel MyUnlockAndNowNameLabel;

	[SerializeField]
	private GameObject OtherLockAndNowContainer;

	[SerializeField]
	private GameObject MyUnlockContainer;

	[SerializeField]
	private GameObject OtherUnlockContainer;

	[SerializeField]
	private GameObject MyLockTipContainer;

	[SerializeField]
	private UILabel MyLockTipTitleLabel;

	[SerializeField]
	private UILabel MyLockTipDesLabel;

	[SerializeField]
	private GameObject OtherLockAndUnlockTipContainer;

	[SerializeField]
	private UILabel OtherLockTipTitleLabel;

	[SerializeField]
	private UILabel OtherLockTipDesLabel;

	public override void OnClick()
	{
		base.OnClick();
		if (data.owner == WorldBossCaptureOwner.MyPVE)
		{
			if (data.view.State != WorldBossCapturePointState.Locked)
			{
				string text = data?.definition?.CapturePoint;
				if (!string.IsNullOrEmpty(text))
				{
					WorldBossPVEDetailBackPopup.OpenPopup(text);
				}
			}
			else
			{
				UpdateMyLockTipLabels();
				Helpers.GameObjectSetActive(MyLockTipContainer, value: true);
			}
		}
		else if (data.owner == WorldBossCaptureOwner.OtherPVE)
		{
			Helpers.GameObjectSetActive(OtherLockAndUnlockTipContainer, value: true);
		}
	}

	public void CloseTipContainer()
	{
		Helpers.GameObjectSetActive(MyLockTipContainer, value: false);
		Helpers.GameObjectSetActive(OtherLockAndUnlockTipContainer, value: false);
	}

	public override void UpdateUI()
	{
		Helpers.GameObjectSetActive(MyLockContainer, value: false);
		Helpers.GameObjectSetActive(OtherLockContainer, value: false);
		Helpers.GameObjectSetActive(MyUnlockAndNowContainer, value: false);
		Helpers.GameObjectSetActive(OtherLockAndNowContainer, value: false);
		Helpers.GameObjectSetActive(MyUnlockContainer, value: false);
		Helpers.GameObjectSetActive(OtherUnlockContainer, value: false);
		Helpers.GameObjectSetActive(MyLockTipContainer, value: false);
		Helpers.GameObjectSetActive(OtherLockAndUnlockTipContainer, value: false);
		base.UpdateUI();
		if (data == null || data.definition == null || data.view == null)
		{
			return;
		}
		if (data.owner == WorldBossCaptureOwner.MyPVE)
		{
			if (MyLockTipTitleLabel != null)
			{
				MyLockTipTitleLabel.text = LocalizationManager.GetText(data.definition.BuildingName);
			}
			if (MyLockTipDesLabel != null)
			{
				MyLockTipDesLabel.text = LocalizationManager.GetText(data.definition.BuildingLockedDesc);
			}
			if (data.view.State == WorldBossCapturePointState.PveCleared)
			{
				Helpers.GameObjectSetActive(MyUnlockContainer, value: true);
			}
			else if (data.view.State == WorldBossCapturePointState.PveInProgress)
			{
				Helpers.GameObjectSetActive(MyUnlockAndNowContainer, value: true);
				Helpers.GameObjectSetActive(MyUnlockAndNowNameLabel, value: true);
				if (MyUnlockAndNowNameLabel != null)
				{
					MyUnlockAndNowNameLabel.text = LocalizationManager.GetText(data.definition.BuildingName);
				}
			}
			else if (data.view.State == WorldBossCapturePointState.Locked)
			{
				Helpers.GameObjectSetActive(MyLockContainer, value: true);
				UpdateMyLockTipLabels();
			}
		}
		else if (data.owner == WorldBossCaptureOwner.OtherPVE)
		{
			if (OtherLockTipTitleLabel != null)
			{
				OtherLockTipTitleLabel.text = LocalizationManager.GetText(data.definition.BuildingName);
			}
			if (OtherLockTipDesLabel != null)
			{
				OtherLockTipDesLabel.text = LocalizationManager.GetText("World.Boss.EnemyPVEBuildingDesc", GetColoredOpponentGroupName());
			}
			if (data.view.State == WorldBossCapturePointState.PveCleared)
			{
				Helpers.GameObjectSetActive(OtherUnlockContainer, value: true);
			}
			else if (data.view.State == WorldBossCapturePointState.PveInProgress)
			{
				Helpers.GameObjectSetActive(OtherLockAndNowContainer, value: true);
			}
			else if (data.view.State == WorldBossCapturePointState.Locked)
			{
				Helpers.GameObjectSetActive(OtherLockContainer, value: true);
			}
		}
	}

	private static string GetColoredOpponentGroupName()
	{
		WorldBossMatchSnapshot worldBossMatchSnapshot = GameManager.Instance?.playerModel?.WorldBossModelManager?.WorldBossGuildFullSnapshot?.Match;
		if (worldBossMatchSnapshot == null)
		{
			return string.Empty;
		}
		string text = GameManager.Instance?.playerModel?.GuildId;
		bool flag = !string.IsNullOrEmpty(text) && text == worldBossMatchSnapshot.GroupIdA;
		string text2 = (flag ? (worldBossMatchSnapshot.GroupNameB ?? string.Empty) : (worldBossMatchSnapshot.GroupNameA ?? string.Empty));
		if (string.IsNullOrEmpty(text2))
		{
			return string.Empty;
		}
		string arg = (flag ? "923d3d" : "3d6392");
		return $"[{arg}]{text2}[-]";
	}

	private void UpdateMyLockTipLabels()
	{
		if (data?.definition != null && !(MyLockTipContainer == null))
		{
			Transform transform = MyLockTipContainer.transform.Find("Bg/Bg2");
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
