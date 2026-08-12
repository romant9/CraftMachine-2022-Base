using Client.Support.Interaction.Implementations;
using TWDModel;

public class SupportInteractionManager
{
	public delegate void SupportInteractionEvent(int equipIndex);

	public delegate void SupportInteractionFailEvent(int equipIndex, SupportTargetsMessage targetsMessage);

	private readonly CombatSupportManager combatSupportManager;

	public ISupportInteraction ActiveSupportInteraction { get; private set; }

	public event SupportInteractionEvent SupportActivated;

	public event SupportInteractionEvent SupportDeactivated;

	public event SupportInteractionEvent SupportExecuted;

	public event SupportInteractionFailEvent SupportExecutionFailed;

	public bool IsUsingSurpportInteraction()
	{
		return ActiveSupportInteraction != null;
	}

	public SupportInteractionManager(CombatModel combatModel)
	{
		combatSupportManager = combatModel.SupportManager;
		combatSupportManager.manager.CombatModel.TurnManager.FactionChanging += TurnManagerOnFactionChanging;
	}

	private void TurnManagerOnFactionChanging(Faction currentFaction, Faction newFaction)
	{
		Deactivate();
	}

	public int GetSupportIndexBy_actorSlotIndex(int actorSlotIndex)
	{
		ActorModel actor = combatSupportManager.manager.CombatModel.GetFactionActors(Faction.Survivor)[actorSlotIndex];
		if (!combatSupportManager.TryGetSupport(actor, out var combatSupportModel))
		{
			return -1;
		}
		return combatSupportModel.SlotIndex;
	}

	public bool Activate(int index)
	{
		if (ActiveSupportInteraction != null)
		{
			Deactivate();
		}
		if (combatSupportManager.TryGetSupport(index, out var combatSupportModel) && combatSupportManager.GetAvailability(combatSupportModel) == CombatSupportAvailability.Executable)
		{
			switch (combatSupportModel.SupportId)
			{
			case "WhisperersMask":
				ActiveSupportInteraction = new WhisperersMaskSupportInteraction(index, combatSupportModel.AttachedSurvivor, combatSupportModel.SupportModel);
				break;
			case "RainbowCat":
				ActiveSupportInteraction = new RainbowCatSupportInteraction(index, combatSupportModel.AttachedSurvivor);
				break;
			case "Hwacha":
				ActiveSupportInteraction = new HwachaSupportInteraction(index, combatSupportModel.AttachedSurvivor, combatSupportModel.SupportModel);
				break;
			default:
				ActiveSupportInteraction = new SimpleSupportInteraction(index, combatSupportModel.AttachedSurvivor);
				break;
			}
			this.SupportActivated?.Invoke(index);
			return true;
		}
		return false;
	}

	public void Deactivate()
	{
		ISupportInteraction activeSupportInteraction = ActiveSupportInteraction;
		ActiveSupportInteraction = null;
		if (activeSupportInteraction != null)
		{
			this.SupportDeactivated?.Invoke(activeSupportInteraction.EquipIndex);
		}
	}

	public void Execute(GridCoordinate? target = null, GridPath runPath = null)
	{
		if (ActiveSupportInteraction == null || !combatSupportManager.TryGetSupport(ActiveSupportInteraction.EquipIndex, out var combatSupportModel))
		{
			return;
		}
		GridCoordinate target2 = target ?? combatSupportModel.AttachedSurvivor.GridCoordinate;
		int equipIndex = ActiveSupportInteraction.EquipIndex;
		if (combatSupportModel.CanExecute(target2))
		{
			if ((runPath?.Count ?? 0) > 0)
			{
				Helpers.ExecuteCommand(new MoveCommand(combatSupportModel.AttachedSurvivor, runPath));
			}
			Helpers.ExecuteCommand(new ExecuteSupportActionCommand(equipIndex, target2));
			ActiveSupportInteraction = null;
			this.SupportExecuted?.Invoke(equipIndex);
		}
		else
		{
			this.SupportExecutionFailed?.Invoke(equipIndex, ActiveSupportInteraction.NotExecutableMessage);
			Deactivate();
		}
	}
}
