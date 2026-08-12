using TWDModel;

public class TeamSelectionEmptyCard : UIListCard<SurvivorModel>
{
	public UILabel NoSurvivorLabel;

	public UILabel ReservedLabel;

	private bool locked;

	public int SlotIndex { get; set; }

	public int MaxTeamSize { get; set; }

	public bool IsSurvivalMode { get; set; }

	public bool Locked
	{
		get
		{
			return locked;
		}
		set
		{
			locked = value;
			if (NoSurvivorLabel != null)
			{
				NoSurvivorLabel.gameObject.SetActive(!locked);
			}
			if (!(ReservedLabel != null))
			{
				return;
			}
			ReservedLabel.gameObject.SetActive(locked);
			if (IsSurvivalMode)
			{
				if (MaxTeamSize == 1)
				{
					ReservedLabel.text = LocalizationManager.GetText("Popup.TeamSelection.Card.SurvivalOnly1SurvivorRemains");
				}
				else if (MaxTeamSize == 2)
				{
					ReservedLabel.text = LocalizationManager.GetText("Popup.TeamSelection.Card.SurvivalOnly2SurvivorRemain");
				}
				else
				{
					ReservedLabel.text = LocalizationManager.GetText("Popup.TeamSelection.Card.SurvivalNoMoreSurvivors");
				}
			}
			else if (MaxTeamSize == 1)
			{
				ReservedLabel.text = LocalizationManager.GetText("Popup.TeamSelection.Card.Solo");
			}
			else
			{
				ReservedLabel.text = LocalizationManager.GetText("Popup.TeamSelection.Card.Reserved");
			}
		}
	}

	public void OnCardClicked()
	{
		if (!Locked)
		{
			UIEvent.Send("OnNewSurvivorSelected", SlotIndex);
			SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("global/survivor_card_click");
		}
	}

	public override int GetSortValue()
	{
		return int.MinValue;
	}
}
