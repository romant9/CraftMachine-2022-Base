using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TWDModel;
using UnityEngine;

public class GvgDefendersTeamSelection : MonoBehaviour
{
	[SerializeField]
	private UILabel title;

	[SerializeField]
	private TeamSelectionSelectedSurvivorPanel teamSelectionSelectedSurvivorPanel;

	[SerializeField]
	private GameObject changedDefendersAmountContainer;

	[SerializeField]
	private UILabel changedDefendersAmountLabel;

	[SerializeField]
	private UIButton submitButton;

	[SerializeField]
	private GameObject survivorAChangedContainer;

	[SerializeField]
	private UILabel survivorALabel;

	[SerializeField]
	private GameObject survivorBChangedContainer;

	[SerializeField]
	private UILabel survivorBLabel;

	[SerializeField]
	private GameObject survivorCChangedContainer;

	[SerializeField]
	private UILabel survivorCLabel;

	[SerializeField]
	private GameObject teamAChanged;

	[SerializeField]
	private GameObject teamBChanged;

	[SerializeField]
	private GameObject teamCChanged;

	private static int teamSelected;

	private List<SurvivorModel> newSurvivors = new List<SurvivorModel>(9);

	private const string localizationKeyTeamA = "Popup.TeamSelect.GvgDefenders.DefendingTeamA";

	private const string localizationKeyTeamB = "Popup.TeamSelect.GvgDefenders.DefendingTeamB";

	private const string localizationKeyTeamC = "Popup.TeamSelect.GvgDefenders.DefendingTeamC";

	private const string localizationKeySurvivorsChanged = "Popup.TeamSelect.GvgDefenders.DefendersChanged{Amount}";

	private const string localizationKeyPreviousSurvivor = "Popup.TeamSelect.GvgDefenders.PreviousSurvivor{Name}";

	private const string localizationKeyUnsavedChanges = "Popup.TeamSelect.GvgDefenders.UnsavedChanges";

	private const string localizationKeyTimeLeft = "Popup.TeamSelect.GvgDefenders.LockedTimer{TimeLeft}";

	private Coroutine timeLeftCoroutine;

	private void OnEnable()
	{
		if (GameManager.Instance.playerModel.IsGuildMember && GetComponent<TeamSelectionPopup>().SurvivorType == SurvivorContainerModel.SurvivorType.GvGDefenders)
		{
			newSurvivors = GetGvgDefenders();
			UIEvent.OnUIEvent += OnUIEventHandler;
			UpdateUI();
		}
	}

	private void OnDisable()
	{
		UIEvent.OnUIEvent -= OnUIEventHandler;
		if (timeLeftCoroutine != null)
		{
			StopCoroutine(timeLeftCoroutine);
			timeLeftCoroutine = null;
		}
	}

	private void OnUIEventHandler(string type, object parameter)
	{
		if (type == "EventSurvivorReplaced")
		{
			object[] array = (object[])parameter;
			if (!(array[0] is SurvivorModel survivorModel) || !(array[1] is int num))
			{
				return;
			}
			int num2 = -1;
			for (int i = 0; i < newSurvivors.Count; i++)
			{
				if (newSurvivors[i].IdForAnalytics == survivorModel.IdForAnalytics)
				{
					num2 = i;
					break;
				}
			}
			if (num2 == -1)
			{
				newSurvivors[teamSelected * 3 + num] = survivorModel;
				UpdateUI();
				return;
			}
			SurvivorModel value = newSurvivors[teamSelected * 3 + num];
			newSurvivors[teamSelected * 3 + num] = survivorModel;
			newSurvivors[num2] = value;
			UpdateUI();
		}
		else if (type == "OnSurvivorInfoClosed")
		{
			UpdateUI();
		}
	}

	private IEnumerator UpdateTimeLeft()
	{
		for (long defendersCooldown = GetDefendersCooldown(); defendersCooldown > 0; defendersCooldown = GetDefendersCooldown())
		{
			changedDefendersAmountContainer.SetActive(value: true);
			changedDefendersAmountLabel.text = LocalizationManager.GetText("Popup.TeamSelect.GvgDefenders.LockedTimer{TimeLeft}", Helpers.FormatTime(defendersCooldown));
			yield return new WaitForSeconds(1f);
		}
		timeLeftCoroutine = null;
		UpdateUI();
	}

	private void UpdateUI()
	{
		List<bool> unsavedDefendersChanged = GetUnsavedDefendersChanged();
		int num = unsavedDefendersChanged.Count((bool x) => x);
		long defendersCooldown = GetDefendersCooldown();
		if (defendersCooldown > 0 && timeLeftCoroutine == null)
		{
			timeLeftCoroutine = StartCoroutine(UpdateTimeLeft());
		}
		HelpersUI.SetButtonState(submitButton, (num == 0 || defendersCooldown > 0) ? UIButtonColor.State.Disabled : UIButtonColor.State.Normal);
		if (num == 0)
		{
			changedDefendersAmountContainer.SetActive(defendersCooldown > 0);
			teamAChanged.SetActive(value: false);
			teamBChanged.SetActive(value: false);
			teamCChanged.SetActive(value: false);
			survivorAChangedContainer.SetActive(value: false);
			survivorBChangedContainer.SetActive(value: false);
			survivorCChangedContainer.SetActive(value: false);
		}
		else
		{
			changedDefendersAmountContainer.SetActive(value: true);
			if (defendersCooldown <= 0)
			{
				changedDefendersAmountLabel.text = SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText("Popup.TeamSelect.GvgDefenders.DefendersChanged{Amount}", num);
			}
			teamAChanged.SetActive(unsavedDefendersChanged.GetRange(0, 3).Any((bool x) => x));
			teamBChanged.SetActive(unsavedDefendersChanged.GetRange(3, 3).Any((bool x) => x));
			teamCChanged.SetActive(unsavedDefendersChanged.GetRange(6, 3).Any((bool x) => x));
			survivorAChangedContainer.SetActive(unsavedDefendersChanged[teamSelected * 3]);
			survivorBChangedContainer.SetActive(unsavedDefendersChanged[teamSelected * 3 + 1]);
			survivorCChangedContainer.SetActive(unsavedDefendersChanged[teamSelected * 3 + 2]);
			List<bool> defendersChanged = GetDefendersChanged();
			UpdateSurvivorChangedIndicator(defendersChanged, survivorAChangedContainer, survivorALabel, teamSelected * 3);
			UpdateSurvivorChangedIndicator(defendersChanged, survivorBChangedContainer, survivorBLabel, teamSelected * 3 + 1);
			UpdateSurvivorChangedIndicator(defendersChanged, survivorCChangedContainer, survivorCLabel, teamSelected * 3 + 2);
		}
		SurvivorCard[] array = Object.FindObjectsOfType<SurvivorCard>();
		for (int num2 = 0; num2 < array.Length; num2++)
		{
			array[num2].OnlyShowSurvivorTop();
		}
	}

	private void UpdateSurvivorChangedIndicator(List<bool> defendersChanged, GameObject container, UILabel survivorLabel, int survivorIndex)
	{
		if (container.activeSelf)
		{
			if (defendersChanged[survivorIndex])
			{
				survivorLabel.text = SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText("Popup.TeamSelect.GvgDefenders.PreviousSurvivor{Name}", GameManager.Instance.playerModel.GvGDefenders[survivorIndex].Name);
			}
			else
			{
				survivorLabel.text = SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText("Popup.TeamSelect.GvgDefenders.UnsavedChanges");
			}
		}
	}

	private List<bool> GetDefendersChanged()
	{
		return GameManager.Instance.playerModel.GvGDefenders.Select((SurvivorMockData t, int i) => newSurvivors[i].IdForAnalytics != t.AnalyticsId).ToList();
	}

	public List<bool> GetUnsavedDefendersChanged()
	{
		List<bool> list = new List<bool>();
		for (int i = 0; i < GameManager.Instance.playerModel.GvGDefenders.Count; i++)
		{
			SurvivorModel survivorModel = newSurvivors[i];
			SurvivorMockData survivorMockData = GameManager.Instance.playerModel.GvGDefenders[i];
			if (survivorModel.IdForAnalytics != survivorMockData.AnalyticsId)
			{
				list.Add(item: true);
			}
			else if (survivorModel.GetWeaponEquipment().IdForAnalytics != survivorMockData.MockWeapon.AnalyticsId || survivorModel.GetEquipmentOfCategory(EquipmentCategory.Armor).IdForAnalytics != survivorMockData.MockArmor.AnalyticsId)
			{
				list.Add(item: true);
			}
			else if (survivorModel.SurvivorRarityLevel != survivorMockData.RarityLevel)
			{
				list.Add(item: true);
			}
			else if (survivorModel.GetUpgradeTraitsList() != survivorMockData.UpgradeTraitsList)
			{
				list.Add(item: true);
			}
			else
			{
				list.Add(item: false);
			}
		}
		return list;
	}

	public bool HasSurvivorAsDefender(SurvivorModel survivorModel)
	{
		return newSurvivors.Contains(survivorModel);
	}

	private static List<SurvivorModel> GetGvgDefenders()
	{
		List<SurvivorModel> list = new List<SurvivorModel>();
		foreach (SurvivorMockData survivorMockData in GameManager.Instance.playerModel.GvGDefenders)
		{
			list.Add(GameManager.Instance.playerModel.SurvivorContainer.Survivors.First((SurvivorModel x) => x.IdForAnalytics == survivorMockData.AnalyticsId));
		}
		return list;
	}

	public void ButtonAPressed()
	{
		if (teamSelected != 0)
		{
			teamSelected = 0;
			UpdateSurvivors();
			UpdateUI();
		}
	}

	public void ButtonBPressed()
	{
		if (teamSelected != 1)
		{
			teamSelected = 1;
			UpdateSurvivors();
			UpdateUI();
		}
	}

	public void ButtonCPressed()
	{
		if (teamSelected != 2)
		{
			teamSelected = 2;
			UpdateSurvivors();
			UpdateUI();
		}
	}

	public void UpdateTitle()
	{
		title.text = LocalizationManager.GetText(GetTitleLocalizationKey());
	}

	private void UpdateSurvivors()
	{
		teamSelectionSelectedSurvivorPanel.UpdateSlots();
		GetComponent<TeamSelectionPopup>().UpdateUI();
	}

	private string GetTitleLocalizationKey()
	{
		return teamSelected switch
		{
			0 => "Popup.TeamSelect.GvgDefenders.DefendingTeamA", 
			1 => "Popup.TeamSelect.GvgDefenders.DefendingTeamB", 
			2 => "Popup.TeamSelect.GvgDefenders.DefendingTeamC", 
			_ => string.Empty, 
		};
	}

	public List<SurvivorModel> GetCurrentSelectedTeam()
	{
		return GetTeam(newSurvivors);
	}

	private static List<SurvivorModel> GetTeam(List<SurvivorModel> survivors)
	{
		return teamSelected switch
		{
			0 => survivors.GetRange(0, 3), 
			1 => survivors.GetRange(3, 3), 
			2 => survivors.GetRange(6, 3), 
			_ => new List<SurvivorModel>(), 
		};
	}

	public static List<SurvivorModel> GetDefaultGvgDefenders()
	{
		List<SurvivorModel> list = new List<SurvivorModel>();
		foreach (SurvivorMockData survivorMockData in GameManager.Instance.playerModel.GvGDefenders)
		{
			list.Add(GameManager.Instance.playerModel.SurvivorContainer.Survivors.First((SurvivorModel x) => x.IdForAnalytics == survivorMockData.AnalyticsId));
		}
		return GetTeam(list);
	}

	public void SubmitAndExit()
	{
		ConfirmationPopup.ShowPopup(LocalizationManager.GetText("Popup.TeamSelect.GvgDefenders.SaveConfirmation.AlertTitle"), LocalizationManager.GetText("Popup.TeamSelect.GvgDefenders.SaveConfirmation.AlertText{Time}", Helpers.FormatTime(GameManager.Instance.playerModel.gameEconomyData.ConfigData.GvGDefendersCooldown)), LocalizationManager.GetText("Button.Save"), delegate
		{
			Helpers.ExecuteCommand(new UpdateGvgDefendersCommand
			{
				GvgDefendersIds = newSurvivors.Select((SurvivorModel x) => x.IdForAnalytics).ToList()
			});
			SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.MapTeamSelection).Close();
		}, LocalizationManager.GetText("Button.Cancel"), delegate
		{
		});
	}

	private long GetDefendersCooldown()
	{
		return GameManager.Instance.playerModel.UtcTimestampLastGvgDefendersUpdate + GameManager.Instance.playerModel.gameEconomyData.ConfigData.GvGDefendersCooldown - GameManager.Instance.playerModel.UtcTimeStamp;
	}

	public void InfoButton()
	{
		PopupQuickTip popupQuickTip = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.GuildBattleInfoPopup) as PopupQuickTip;
		if (popupQuickTip != null && !popupQuickTip.IsOpen)
		{
			popupQuickTip.TipId = "Info_GuildBattle";
			popupQuickTip.Open();
			popupQuickTip.CustomBulletsPosition();
			popupQuickTip.ShowStep(2);
		}
	}

	public string GetTeamForSurvivorAsLetter(SurvivorModel survivorModel)
	{
		int num = 0;
		for (int i = 0; i < newSurvivors.Count; i++)
		{
			if (newSurvivors[i].IdForAnalytics == survivorModel.IdForAnalytics)
			{
				num = i / 3;
			}
		}
		return num switch
		{
			0 => "A", 
			1 => "B", 
			2 => "C", 
			_ => string.Empty, 
		};
	}
}
