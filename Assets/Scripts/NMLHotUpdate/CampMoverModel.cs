using System.Collections.Generic;
using Newtonsoft.Json;
using TWDModel;

public class CampMoverModel : TWDModelObject
{
	public int CurrentSubtype { get; protected set; }

	public string BackgroundName { get; set; }

	public RectData[] ValidBuildingPositions { get; set; }

	public FixedVec2 GatePosition { get; private set; }

	[JsonIgnore]
	public CampModel PlayerCamp { get; set; }

	public override void Start()
	{
		base.Start();
		PlayerCamp = base.manager.Player.Camp;
	}

	public override bool IsValid()
	{
		return true;
	}

	public CampType GetCampType(int level)
	{
		List<CampType> campTypes = base.manager.GameEconomyData.CampTypes;
		if (campTypes == null)
		{
			return null;
		}
		foreach (CampType item in campTypes)
		{
			if (item.Level == level)
			{
				return item;
			}
		}
		return null;
	}

	public CampType GetNextLevelCampType()
	{
		return GetCampType(PlayerCamp.Level + 1);
	}

	public CampType GetCurrentCampType()
	{
		return GetCampType(PlayerCamp.Level);
	}

	public CampSubtype GetCampSubtype(CampType campType, int subtypeIndex = 1)
	{
		return campType.CampSubtypes[subtypeIndex];
	}

	public TWDModelResult MoveCamp()
	{
		return Move(PlayerCamp.Level + 1, 0);
	}

	public TWDModelResult Move(int level, int subtype, PlayerModel player = null)
	{
		if (PlayerCamp.Level + 1 != level)
		{
			base.Debug.LogError("Cannot move camp to that level");
			return TWDModelResult.Error;
		}
		CampType campType = GetCampType(level);
		if (campType == null)
		{
			base.Debug.LogError("This camp type doesn't exist " + level);
			return TWDModelResult.CampTypeNotFound;
		}
		if (subtype >= campType.CampSubtypes.Count)
		{
			base.Debug.LogError("This camp subtype doesn't exist " + subtype);
			return TWDModelResult.CampTypeNotFound;
		}
		TWDModelResult tWDModelResult = GetCashier().Pay();
		if (tWDModelResult != TWDModelResult.OK)
		{
			return tWDModelResult;
		}
		if (level > 0)
		{
			base.manager.Player.Blackboard.SetToggle("Toggle.CampMoved");
		}
		CampSubtype campSubtype = campType.CampSubtypes[subtype];
		PlayerCamp.Level = level;
		CurrentSubtype = subtype;
		BackgroundName = campSubtype.Background;
		ValidBuildingPositions = campSubtype.ValidBuildingPositions;
		GatePosition = campSubtype.GatePosition;
		if (player != null)
		{
			player.SetMapName("Map" + (level + 1));
		}
		else if (base.manager.Player != null)
		{
			base.manager.Player.SetMapName("Map" + (level + 1));
		}
		FixedVec3 position = PlayerCamp.Grid.Position;
		PlayerCamp.UpdateGridPosition();
		PlayerCamp.SetGridSize(base.gameEconomyData.ScaleToGrid(campSubtype.Size.X), base.gameEconomyData.ScaleToGrid(campSubtype.Size.Y));
		PlayerCamp.RepositionBuildings(position);
		PlayerCamp.AddBuildings(campType.Buildings, createView: false);
		PlayerCamp.AddBuildings(campSubtype.Buildings, createView: false);
		PlayerCamp.UpdateGrid();
		NotifyChange("campMoved");
		return TWDModelResult.OK;
	}

	public Cashier GetCashier()
	{
		int level = PlayerCamp.Level + 1;
		CampType campType = GetCampType(level);
		if (campType == null)
		{
			base.Debug.LogError("This camp type doesn't exist " + level);
		}
		Cashier cashier = new Cashier(base.manager);
		CashierItem cashierItem = new CashierItem(PurchaseType.MoveCamp);
		cashierItem.SetCost(CurrencyType.Gas, campType.MoveCostGas);
		cashier.AddItem(cashierItem);
		return cashier;
	}
}
