using System.Collections.Generic;
using BaseModel;
using TWDModel;

public class SelectedTeamPanel : ListPanel
{
	private void Start()
	{
		CreateSurvivorsPanel();
	}

	private void OnEnable()
	{
		UpdateSlots();
		GameManager.Instance.playerModel.SurvivorContainer.Changed += PlayerModelChanged;
	}

	private void OnDisable()
	{
		GameManager.Instance.playerModel.SurvivorContainer.Changed -= PlayerModelChanged;
	}

	private void UpdateSlots()
	{
		List<SurvivorModel> combatSurvivors = GameManager.Instance.playerModel.SurvivorContainer.CombatSurvivors;
		for (int i = 0; i < base.NumberSlots; i++)
		{
			SurvivorCard component = GetSlotAt(i).GetComponent<SurvivorCard>();
			component.Item = ((i < combatSurvivors.Count) ? combatSurvivors[i] : null);
			component.UpdateUI();
		}
	}

	private void CreateSurvivorsPanel()
	{
		CreateSlots(3);
		UpdateSlots();
	}

	private void PlayerModelChanged(ModelObject m, string changed, object args)
	{
		if (changed == "combatSurvivorsChanged")
		{
			UpdateSlots();
		}
	}

	public void OnOpenTeamSelection()
	{
		TeamSelectionPopup obj = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.MapTeamSelection) as TeamSelectionPopup;
		obj.SurvivorType = SurvivorContainerModel.SurvivorType.Combat;
		obj.Open();
	}
}
