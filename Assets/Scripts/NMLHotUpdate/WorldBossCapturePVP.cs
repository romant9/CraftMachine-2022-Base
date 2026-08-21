using System.Collections.Generic;
using BaseModel;
using TWDModel;
using UnityEngine;

public class WorldBossCapturePVP : WorldBossCaptureBase
{
	[SerializeField]
	private GameObject UnlocakContainer;

	[SerializeField]
	private GameObject LockTipContainer;

	[SerializeField]
	private GameObject ShieldContainer;

	[SerializeField]
	private GameObject UnShieldContainer;

	[SerializeField]
	private UILabel LockTipTitleLabel;

	[SerializeField]
	private UILabel LockTipDescriptionLabel;

	private const int HumanIconCount = 9;

	private const string HumanIconNamePrefix = "Icon";

	private string ShieldedText = "World.Boss.Shielded";

	private string AtWarText = "World.Boss.AtWar";

	private static readonly Color BlueColor = Helpers.HexToColor("#206dde");

	private static readonly Color RedColor = Helpers.HexToColor("#d32d11");

	private static readonly Color BlueHumanIconColor = Helpers.HexToColor("#1853A9");

	private static readonly Color RedHumanIconColor = Helpers.HexToColor("#A0220D");

	private static readonly Color GrayColor = Helpers.HexToColor("#5F5F5F");

	private bool isShieldCountdownActive;

	private long shieldEndUtcMs = -1L;

	private WorldBossModelManager worldBossModelManager;

	public void CloseLockTip()
	{
		Helpers.GameObjectSetActive(LockTipContainer, value: false);
	}

	public override void OnClick()
	{
		base.OnClick();
		ResolveContainers();
		string text = data?.definition?.CapturePoint;
		if (!string.IsNullOrEmpty(text))
		{
			WorldBossModelManager worldBossModelManager = GameManager.Instance.playerModel.WorldBossModelManager;
			if (worldBossModelManager != null && !worldBossModelManager.IsCapturePointUnlockedForMyGroup(text))
			{
				Helpers.GameObjectSetActive(LockTipContainer, value: true);
			}
			else if (UnlocakContainer != null && UnlocakContainer.activeSelf)
			{
				WorldBossPVPDetailPopup.OpenPopup(text);
			}
			else
			{
				Helpers.GameObjectSetActive(LockTipContainer, value: true);
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

	private void Update()
	{
		if (isShieldCountdownActive && shieldEndUtcMs > 0)
		{
			UpdateShieldCountdownLabel();
		}
	}

	public override void UpdateUI()
	{
		base.UpdateUI();
		isShieldCountdownActive = false;
		shieldEndUtcMs = -1L;
		if (data.owner != WorldBossCaptureOwner.PVP)
		{
			return;
		}
		ShieldedText = LocalizationManager.GetText("World.Boss.Shielded");
		AtWarText = LocalizationManager.GetText("World.Boss.AtWar");
		WorldBossModelManager worldBossModelManager = GameManager.Instance.playerModel.WorldBossModelManager;
		if (worldBossModelManager == null || data?.definition == null)
		{
			return;
		}
		ResolveContainers();
		if (!(UnlocakContainer == null) && worldBossModelManager.GetAllCapturePointStates().TryGetValue(data.definition.CapturePoint, out var value))
		{
			bool flag = value.State == WorldBossCapturePointState.PvpOccupiedByOwn || value.State == WorldBossCapturePointState.PvpOccupiedByEnemy;
			bool flag2 = worldBossModelManager.GetCapturePointOwnershipCountdownMs(data.definition.CapturePoint) > 0;
			if (!value.MyUnlocked)
			{
				Helpers.GameObjectSetActive(UnlocakContainer, value: false);
				UpdateMyLockTipLabels();
			}
			else if (flag2)
			{
				ApplyAtWarOccupiedUi(worldBossModelManager, data.definition.CapturePoint, GetOwnershipJudgementGroupId(worldBossModelManager, data.definition.CapturePoint, value));
			}
			else if (flag)
			{
				Helpers.GameObjectSetActive(UnlocakContainer, value: true);
				Helpers.GameObjectSetActive(ShieldContainer, value: true);
				Helpers.GameObjectSetActive(UnShieldContainer, value: false);
				Color groupColor = GetGroupColor(value.GroupId);
				SetSpriteColor((ShieldContainer != null) ? ShieldContainer.transform : null, groupColor);
				SetSpriteColor(UnlocakContainer.transform.Find("NameBG"), groupColor);
				SetLabelText(FindChildLabel(UnlocakContainer.transform, "NameBG/WarLabel"), LocalizationManager.GetText(data.definition.BuildingName));
				SetLabelText(FindChildLabel(UnlocakContainer.transform, "WarBG/WarLabel"), ShieldedText);
				shieldEndUtcMs = value.ProtectionEndUtcMs;
				isShieldCountdownActive = shieldEndUtcMs > 0;
				UpdateShieldCountdownLabel();
				UpdateHumanIconBar(worldBossModelManager, data.definition.CapturePoint);
			}
			else if (!value.OpponentUnlocked)
			{
				Helpers.GameObjectSetActive(UnlocakContainer, value: true);
				Helpers.GameObjectSetActive(ShieldContainer, value: false);
				Helpers.GameObjectSetActive(UnShieldContainer, value: true);
				SetSpriteColor(UnlocakContainer.transform.Find("NameBG"), GrayColor);
				SetLabelText(FindChildLabel(UnlocakContainer.transform, "NameBG/WarLabel"), LocalizationManager.GetText(data.definition.BuildingName));
				SetLabelText(FindChildLabel(UnlocakContainer.transform, "WarBG/WarLabel"), AtWarText);
				UpdateHumanIconBar(worldBossModelManager, data.definition.CapturePoint);
				UpdateWarIcons(useBothSideColors: false);
			}
			else
			{
				Helpers.GameObjectSetActive(UnlocakContainer, value: true);
				Helpers.GameObjectSetActive(ShieldContainer, value: false);
				Helpers.GameObjectSetActive(UnShieldContainer, value: true);
				SetSpriteColor(UnlocakContainer.transform.Find("NameBG"), GrayColor);
				SetLabelText(FindChildLabel(UnlocakContainer.transform, "NameBG/WarLabel"), LocalizationManager.GetText(data.definition.BuildingName));
				SetLabelText(FindChildLabel(UnlocakContainer.transform, "WarBG/WarLabel"), AtWarText);
				UpdateHumanIconBar(worldBossModelManager, data.definition.CapturePoint);
				UpdateWarIcons(useBothSideColors: true);
			}
			UpdateDispatchedTeamContainers(worldBossModelManager, data.definition.CapturePoint);
		}
	}

	private void ApplyAtWarOccupiedUi(WorldBossModelManager worldBossModelManager, string capturePoint, string majorityGroupId)
	{
		Helpers.GameObjectSetActive(UnlocakContainer, value: true);
		Helpers.GameObjectSetActive(ShieldContainer, value: false);
		Helpers.GameObjectSetActive(UnShieldContainer, value: true);
		Color groupColor = GetGroupColor(majorityGroupId);
		SetSpriteColor(UnlocakContainer.transform.Find("NameBG"), groupColor);
		SetLabelText(FindChildLabel(UnlocakContainer.transform, "NameBG/WarLabel"), LocalizationManager.GetText(data.definition.BuildingName));
		SetLabelText(FindChildLabel(UnlocakContainer.transform, "WarBG/WarLabel"), AtWarText);
		UpdateHumanIconBar(worldBossModelManager, capturePoint);
		UpdateWarIcons(useBothSideColors: true);
	}

	private static string GetOwnershipJudgementGroupId(WorldBossModelManager worldBossModelManager, string capturePoint, WorldBossCapturePointView view)
	{
		string text = worldBossModelManager?.GetCapturePointOwnershipCountdownGroupId(capturePoint);
		if (!string.IsNullOrEmpty(text))
		{
			return text;
		}
		return view?.GroupId;
	}

	public void OpneRetreat1ConfirmPopup()
	{
		OpenRetreatConfirmPopup(0);
	}

	public void OpneRetreat2ConfirmPopup()
	{
		OpenRetreatConfirmPopup(1);
	}

	private void OpenRetreatConfirmPopup(int teamIndex)
	{
		if (worldBossModelManager == null)
		{
			worldBossModelManager = GameManager.Instance?.playerModel?.WorldBossModelManager;
		}
		string text = data?.definition?.CapturePoint;
		if (worldBossModelManager == null || string.IsNullOrEmpty(text))
		{
			return;
		}
		List<WorldBossDispatchedTeamView> list = new List<WorldBossDispatchedTeamView>();
		foreach (WorldBossDispatchedTeamView myDispatchedTeam in worldBossModelManager.GetMyDispatchedTeams())
		{
			if (myDispatchedTeam != null && myDispatchedTeam.CapturePoint == text)
			{
				list.Add(myDispatchedTeam);
			}
		}
		if (teamIndex >= 0 && teamIndex < list.Count)
		{
			WorldBossRetreatConfirmPopup worldBossRetreatConfirmPopup = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.WorldBossRetreatConfirmPopup) as WorldBossRetreatConfirmPopup;
			if (!(worldBossRetreatConfirmPopup == null))
			{
				worldBossRetreatConfirmPopup.SetTeam(list[teamIndex]);
				worldBossRetreatConfirmPopup.Open();
			}
		}
	}

	private void UpdateDispatchedTeamContainers(WorldBossModelManager worldBossModelManager, string capturePoint)
	{
		Transform transform = UnlocakContainer?.transform;
		if (transform == null || worldBossModelManager == null || string.IsNullOrEmpty(capturePoint))
		{
			return;
		}
		int num = 0;
		foreach (WorldBossDispatchedTeamView myDispatchedTeam in worldBossModelManager.GetMyDispatchedTeams())
		{
			if (myDispatchedTeam != null && myDispatchedTeam.CapturePoint == capturePoint)
			{
				num++;
			}
		}
		Transform transform2 = transform.Find("Get1Container");
		Transform obj = transform.Find("Get2Container");
		Helpers.GameObjectSetActive(transform2?.gameObject, num == 1);
		Helpers.GameObjectSetActive(obj?.gameObject, num >= 2);
	}

	private void ResolveContainers()
	{
		if (UnlocakContainer == null)
		{
			Transform transform = base.transform.Find("UnLockContainer") ?? base.transform.Find("UnlocakContainer") ?? base.transform.Find("UnlockContainer");
			if (transform != null)
			{
				UnlocakContainer = transform.gameObject;
			}
		}
		if (UnlocakContainer == null)
		{
			return;
		}
		if (LockTipContainer == null)
		{
			Transform transform2 = base.transform.Find("LockTipContainer") ?? UnlocakContainer.transform.Find("LockTipContainer");
			if (transform2 != null)
			{
				LockTipContainer = transform2.gameObject;
			}
		}
		if (ShieldContainer == null)
		{
			Transform transform3 = UnlocakContainer.transform.Find("Shielded");
			if (transform3 != null)
			{
				ShieldContainer = transform3.gameObject;
			}
		}
		if (UnShieldContainer == null)
		{
			Transform transform4 = UnlocakContainer.transform.Find("UnShielded");
			if (transform4 != null)
			{
				UnShieldContainer = transform4.gameObject;
			}
		}
	}

	private void UpdateShieldCountdownLabel()
	{
		UILabel uILabel = FindChildLabel(UnlocakContainer?.transform, "Shielded/TimeBG/ShieldedTimeLabel");
		if (!(uILabel == null))
		{
			long valueOrDefault = (GameManager.Instance?.playerModel?.UtcTimeStamp).GetValueOrDefault();
			long num = ((shieldEndUtcMs > valueOrDefault) ? (shieldEndUtcMs - valueOrDefault) : 0);
			HelpersUI.SetContentToLabel(uILabel, (num > 0) ? Helpers.FormatTimeAsHms(num) : "00:00:00");
			if (num <= 0)
			{
				isShieldCountdownActive = false;
			}
		}
	}

	private void UpdateHumanIconBar(WorldBossModelManager worldBossModelManager, string capturePoint)
	{
		Transform transform = UnlocakContainer?.transform.Find("HumanIcon");
		if (transform == null)
		{
			return;
		}
		WorldBossCellBarView capturePointCellBar = worldBossModelManager.GetCapturePointCellBar(capturePoint);
		int num = Mathf.Max(0, capturePointCellBar.MineOccupied);
		int num2 = Mathf.Max(0, capturePointCellBar.EnemyOccupied);
		bool num3 = IsMyGuildBlueSide();
		int num4 = (num3 ? num : num2);
		int num5 = (num3 ? num2 : num);
		for (int i = 0; i < 9; i++)
		{
			int num6 = i + 1;
			Transform transform2 = transform.Find("Icon" + num6);
			if (!(transform2 == null))
			{
				Color color = GrayColor;
				if (num6 <= num4)
				{
					color = BlueHumanIconColor;
				}
				else if (num6 > 9 - num5)
				{
					color = RedHumanIconColor;
				}
				Helpers.GameObjectSetActive(transform2.gameObject, value: true);
				UISprite component = transform2.GetComponent<UISprite>();
				if (component != null)
				{
					component.color = color;
				}
			}
		}
	}

	private void UpdateWarIcons(bool useBothSideColors)
	{
		Transform transform = ((UnShieldContainer != null) ? UnShieldContainer.transform : UnlocakContainer?.transform.Find("UnShielded"));
		if (!(transform == null))
		{
			Color color = (useBothSideColors ? BlueColor : GetMyGuildColor());
			Color color2 = (useBothSideColors ? RedColor : GetMyGuildColor());
			SetSpriteColor(transform.Find("WarIcon_Blue"), color);
			SetSpriteColor(transform.Find("WarIcon_Red"), color2);
		}
	}

	private static Color GetMyGuildColor()
	{
		if (!IsMyGuildBlueSide())
		{
			return RedColor;
		}
		return BlueColor;
	}

	private static Color GetGroupColor(string groupId)
	{
		WorldBossMatchSnapshot worldBossMatchSnapshot = GameManager.Instance?.playerModel?.WorldBossModelManager?.WorldBossGuildFullSnapshot?.Match;
		if (worldBossMatchSnapshot == null || string.IsNullOrEmpty(groupId))
		{
			return GrayColor;
		}
		if (groupId == worldBossMatchSnapshot.GroupIdA)
		{
			return BlueColor;
		}
		if (groupId == worldBossMatchSnapshot.GroupIdB)
		{
			return RedColor;
		}
		return GrayColor;
	}

	private static bool IsMyGuildBlueSide()
	{
		string text = GameManager.Instance?.playerModel?.GuildId;
		WorldBossMatchSnapshot worldBossMatchSnapshot = GameManager.Instance?.playerModel?.WorldBossModelManager?.WorldBossGuildFullSnapshot?.Match;
		if (!string.IsNullOrEmpty(text) && worldBossMatchSnapshot != null)
		{
			return text == worldBossMatchSnapshot.GroupIdA;
		}
		return false;
	}

	private static UILabel FindChildLabel(Transform root, string path)
	{
		return root?.Find(path)?.GetComponent<UILabel>();
	}

	private static void SetLabelText(UILabel label, string text)
	{
		if (!(label == null))
		{
			HelpersUI.SetContentToLabel(label, text);
		}
	}

	private static void SetSpriteColor(Transform target, Color color)
	{
		if (!(target == null))
		{
			UISprite component = target.GetComponent<UISprite>();
			if (component != null)
			{
				component.color = color;
			}
		}
	}
}
