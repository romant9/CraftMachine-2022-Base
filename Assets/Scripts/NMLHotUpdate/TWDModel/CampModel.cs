using System;
using System.Collections.Generic;
using BaseModel;
using Newtonsoft.Json;

namespace TWDModel
{
	public class CampModel : TWDModelObject
	{
		public const string EventLevelUpBuilding = "EventLevelUpBuilding";

		public const string EventAddBuilding = "EventAddBuilding";

		public const string EventPositionBuilding = "EventPositionBuilding";

		public const string EventUpgradeBuilding = "EventUpgradeBuilding";

		public const string EventBuildingCollected = "EventBuildingCollected";

		private List<string> availableBuildingsToBuild = new List<string>();

		[JsonIgnore]
		protected Dictionary<CurrencyType, bool> MaxCurrencyStorages;

		private ModelList<BuildingModel> _buildings;

		[JsonIgnore]
		public Dictionary<string, BuildingModel> BuildingsByTypeName = new Dictionary<string, BuildingModel>();

		private CampObjectModel[] grid;

		[JsonIgnore]
		private CampObjectModel fakeBuilding;

		[NonSerialized]
		[JsonIgnore]
		public bool InCamp;

		private const string ExpansionTypePrefix = "Camp_ExpansionZone_";

		[JsonIgnore]
		public int GridWidth
		{
			get
			{
				return Grid.Width;
			}
			set
			{
				Grid.SetWidth(value);
			}
		}

		[JsonIgnore]
		public int GridHeight
		{
			get
			{
				return Grid.Height;
			}
			set
			{
				Grid.SetHeight(value);
			}
		}

		public GridModel Grid { get; protected set; }

		[JsonIgnore]
		public PlayerModel Player { get; set; }

		public int Level { get; set; }

		public ModelList<BuildingModel> Buildings
		{
			get
			{
				return _buildings;
			}
			protected set
			{
				_buildings = value;
				if (_buildings == null)
				{
					BuildingsByTypeName = null;
					return;
				}
				if (BuildingsByTypeName == null)
				{
					BuildingsByTypeName = new Dictionary<string, BuildingModel>();
				}
				BuildingsByTypeName.Clear();
				foreach (BuildingModel building in _buildings)
				{
					if (!BuildingsByTypeName.ContainsKey(building.TypeName))
					{
						BuildingsByTypeName[building.TypeName] = building;
					}
				}
			}
		}

		public FixedVec2 GatePosition { get; private set; }

		public CampDefenseModel CampDefenseModel { get; private set; }

		[JsonIgnore]
		public List<NotificationQueueItem> NotificationQueue { get; protected set; }

		public List<string> UnlockedExpansionTypes { get; set; }

		public void SetGridModel(GridModel inGrid)
		{
			Grid = inGrid;
		}

		public override void Initialize()
		{
			base.Initialize();
			Level = -1;
			Grid = new GridModel();
			Grid.SetManager(base.manager);
			Grid.Initialize();
			Grid.SetCellSize(new FixedVec2(base.gameEconomyData.ConfigData.GridScale, base.gameEconomyData.ConfigData.GridScale));
			Buildings = new ModelList<BuildingModel>();
			BuildingsByTypeName = new Dictionary<string, BuildingModel>();
			Buildings.SetManager(base.manager);
			Buildings.Initialize();
			UnlockedExpansionTypes = new List<string>();
			CampDefenseModel = new CampDefenseModel();
			CampDefenseModel.SetManager(base.manager);
			CampDefenseModel.Initialize();
		}

		public override void Start()
		{
			if (OfflineManager.IsLoadDataManager)
			{
				DebugTWD.LogMycode("if (OfflineManager.IsLoadDataManager) return");
				return;
			}
			if (UnlockedExpansionTypes == null)
			{
				UnlockedExpansionTypes = new List<string>();
			}
			PurgeUnlockedExpansionBuildings();
			fakeBuilding = new CampObjectModel();
			foreach (BuildingModel building in Buildings)
			{
				building.Camp = this;
				building.Changed -= OnBuildingChange;
				building.Changed += OnBuildingChange;
			}
			base.Start();
			UpdateGrid();
			UpdateMaximumStorageFlags();
			NotificationQueue = new List<NotificationQueueItem>();
			EnsureExpansionVegetations();
		}

		private void PurgeUnlockedExpansionBuildings()
		{
			if (UnlockedExpansionTypes == null || UnlockedExpansionTypes.Count == 0 || Buildings == null || Buildings.Count == 0)
			{
				return;
			}
			List<BuildingModel> list = null;
			for (int i = 0; i < Buildings.Count; i++)
			{
				BuildingModel buildingModel = Buildings[i];
				if (buildingModel != null && !string.IsNullOrEmpty(buildingModel.TypeName) && buildingModel.TypeName.StartsWith("Camp_ExpansionZone_") && UnlockedExpansionTypes.Contains(buildingModel.TypeName))
				{
					if (list == null)
					{
						list = new List<BuildingModel>();
					}
					list.Add(buildingModel);
				}
			}
			if (list != null)
			{
				for (int j = 0; j < list.Count; j++)
				{
					RemoveBuilding(list[j]);
				}
			}
		}

		public void MarkExpansionTypeUnlocked(string typeName)
		{
			if (!string.IsNullOrEmpty(typeName) && typeName.StartsWith("Camp_ExpansionZone_"))
			{
				if (UnlockedExpansionTypes == null)
				{
					UnlockedExpansionTypes = new List<string>();
				}
				if (!UnlockedExpansionTypes.Contains(typeName))
				{
					UnlockedExpansionTypes.Add(typeName);
				}
			}
		}

		private void EnsureExpansionVegetations()
		{
			if (base.manager == null || base.manager.Player == null || base.manager.Player.CampMover == null)
			{
				return;
			}
			CampType campType = base.manager.Player.CampMover.GetCampType(Level);
			if (campType == null || campType.Buildings == null)
			{
				return;
			}
			for (int i = 0; i < campType.Buildings.Count; i++)
			{
				InitialCampBuildingData initialCampBuildingData = campType.Buildings[i];
				if (initialCampBuildingData != null && !string.IsNullOrEmpty(initialCampBuildingData.TypeName) && initialCampBuildingData.TypeName.StartsWith("Camp_ExpansionZone_") && (UnlockedExpansionTypes == null || !UnlockedExpansionTypes.Contains(initialCampBuildingData.TypeName)) && !HasBuildingOfType(initialCampBuildingData.TypeName))
				{
					FixedVec2 position = TransformGroundToGridPosition(initialCampBuildingData.Position);
					BuildingModel buildingModel = AddBuilding(initialCampBuildingData, position, createView: false);
					if (buildingModel != null && buildingModel.ModelId == 0)
					{
						buildingModel.Start();
					}
				}
			}
		}

		private bool HasBuildingOfType(string typeName)
		{
			if (Buildings == null)
			{
				return false;
			}
			for (int i = 0; i < Buildings.Count; i++)
			{
				BuildingModel buildingModel = Buildings[i];
				if (buildingModel != null && buildingModel.TypeName == typeName)
				{
					return true;
				}
			}
			return false;
		}

		public void AddNotificationQueueItem(NotificationQueueItem.Type notificationType, int modelId, string name, int level)
		{
			NotificationQueue.Add(new NotificationQueueItem
			{
				NotificationType = notificationType,
				ModelId = modelId,
				Name = name,
				Level = level
			});
		}

		public List<string> GetAvailableBuildingsToBuild()
		{
			availableBuildingsToBuild.Clear();
			int level = GetBuilding("Council").Level;
			BuildingsAmountsDefinition buildingsAmountsAtCouncilLevel = base.gameEconomyData.GetBuildingsAmountsAtCouncilLevel(level);
			BuildingType[] buildingTypes = base.gameEconomyData.BuildingTypes;
			foreach (BuildingType buildingType in buildingTypes)
			{
				int amountsForBuilding = buildingsAmountsAtCouncilLevel.GetAmountsForBuilding(buildingType.Name);
				int buildingCount = GetBuildingCount(buildingType.Name);
				int num = Math.Max(amountsForBuilding - buildingCount, 0);
				for (int j = 0; j < num; j++)
				{
					availableBuildingsToBuild.Add(buildingType.Name);
				}
			}
			return availableBuildingsToBuild;
		}

		public override bool IsValid()
		{
			foreach (BuildingModel building in Buildings)
			{
				if (!building.IsValid())
				{
					return false;
				}
			}
			return true;
		}

		public void SetGridSize(int gridWidth, int gridHeight)
		{
			if (gridWidth != GridWidth || gridHeight != GridHeight)
			{
				grid = null;
			}
			GridWidth = gridWidth;
			GridHeight = gridHeight;
		}

		public void UpdateGridPosition()
		{
			int num = int.MaxValue;
			int num2 = int.MaxValue;
			if (base.manager?.Player?.CampMover?.ValidBuildingPositions != null)
			{
				RectData[] validBuildingPositions = base.manager.Player.CampMover.ValidBuildingPositions;
				foreach (RectData obj in validBuildingPositions)
				{
					num = Math.Min(obj.X, num);
					num2 = Math.Min(obj.Y, num2);
				}
			}
			Grid.SetPosition(new FixedVec3(num, 0.0, num2));
		}

		public int GetBuildingCount(string typeName)
		{
			int num = 0;
			if (Buildings != null)
			{
				foreach (BuildingModel building in Buildings)
				{
					if (building.TypeName == typeName && !building.MarkedToBeDeleted)
					{
						num++;
					}
				}
			}
			return num;
		}

		public bool CanAddBuilding(string buildingType)
		{
			BuildingType buildingType2 = base.manager.GameEconomyData.GetBuildingType(buildingType);
			if (buildingType2 == null)
			{
				return false;
			}
			if (!string.IsNullOrEmpty(buildingType2.RequiredBuilding) && GetBuildingLevel(buildingType2.RequiredBuilding) <= 0)
			{
				return false;
			}
			return GetAvailableBuildingsToBuild().Contains(buildingType);
		}

		public void AddBuildings(List<InitialCampBuildingData> buildings, bool createView = true)
		{
			int num = GetBuilding("Council")?.Level ?? 1;
			if (buildings == null)
			{
				return;
			}
			foreach (InitialCampBuildingData building in buildings)
			{
				if (building.SpawnAtCouncilLevel == num)
				{
					FixedVec2 position = TransformGroundToGridPosition(building.Position);
					BuildingModel buildingModel = AddBuilding(building, position, createView);
					if (buildingModel != null)
					{
						buildingModel.RepairDependencyLevelUpgrade = building.RepairDependencyLevelRequired;
					}
				}
			}
		}

		public FixedVec2 TransformGroundToGridPosition(FixedVec2 groundPosition)
		{
			FixedVec2 fixedVec = new FixedVec2(Grid.Position.X, Grid.Position.Z);
			FixedVec2 result = (groundPosition - fixedVec) / (float)base.gameEconomyData.ConfigData.GridScale;
			result.X = FixedPoint.Floor(result.X);
			result.Y = FixedPoint.Floor(result.Y);
			return result;
		}

		public TWDModelResult CreateNewBuilding(string buildingType, GridPosition position, ref BuildingModel outNewBuilding, int useDiamondsAmount = -1)
		{
			if (CanAddBuilding(buildingType))
			{
				InitialCampBuildingData initialCampBuildingData = new InitialCampBuildingData();
				initialCampBuildingData.TypeName = buildingType;
				initialCampBuildingData.Level = 0;
				initialCampBuildingData.Position = new FixedVec2(position.X, position.Y);
				initialCampBuildingData.RotationAngle = 0f;
				BuildingModel buildingModel = AddBuilding(initialCampBuildingData, new FixedVec2(position.X, position.Y));
				if (buildingModel != null)
				{
					TWDModelResult num = buildingModel.StartUpgrade(useDiamondsAmount);
					if (num == TWDModelResult.OK)
					{
						UpdateGrid();
					}
					else
					{
						buildingModel.DeleteMe();
						buildingModel = null;
					}
					outNewBuilding = buildingModel;
					return num;
				}
				return TWDModelResult.InvalidBuildCreationRequested;
			}
			return TWDModelResult.InvalidBuildCreationRequested;
		}

		public void DestroyBuildingWhenCouncilUpgrade()
		{
			int level = GetBuilding("Council").Level;
			for (int i = 0; i < Buildings.Count; i++)
			{
				if (Buildings[i] is VegetationModel)
				{
					VegetationModel vegetationModel = Buildings[i] as VegetationModel;
					if (vegetationModel.DestroyAtCoucilLevel == level)
					{
						vegetationModel.DeleteMe();
					}
				}
			}
		}

		public Cashier GetBuildingUpgradeCashier(string buildingType, int nextLevel, bool instantUpgrade, bool addSpeedUpCashier = true)
		{
			if (!Player.Tutorial.HasCompletedPart("Tutorial") && buildingType == "BuildingProduceSupplies")
			{
				return Cashier.CreateOneItemCashier(base.manager, PurchaseType.BuildingPurchase, CurrencyType.Supplies, 0);
			}
			Cashier cashier = new Cashier(base.manager);
			BuildingUpgradeLevel buildingUpgradeLevel = base.gameEconomyData.GetBuildingUpgradeLevel(buildingType, nextLevel);
			PurchaseType purchaseType = ((nextLevel == 1) ? PurchaseType.BuildingPurchase : PurchaseType.BuildingUpgrade);
			if (instantUpgrade)
			{
				purchaseType = PurchaseType.InstantBuildingUpgrade;
				CashierItem cashierItem = new CashierItem(purchaseType);
				int num = base.gameEconomyData.TimeToDiamonds(base.manager.Player.ActivityManager.GetBuildingUpgradeTime(buildingUpgradeLevel) * 1000);
				int cost = base.gameEconomyData.CurrencyToDiamonds(CurrencyType.Supplies, buildingUpgradeLevel.CostSupplies) + base.gameEconomyData.CurrencyToDiamonds(CurrencyType.Inhabitants, buildingUpgradeLevel.CostInhabitants) + num;
				cashierItem.SetCost(CurrencyType.Diamonds, cost);
				cashier.AddItem(cashierItem);
			}
			else
			{
				if (addSpeedUpCashier)
				{
					BuildingModel buildingModel = base.manager.Player.Camp.IsBuildingUpgradeInProgress();
					if (buildingModel != null)
					{
						purchaseType = PurchaseType.SpeedupAndBuildingUpgrade;
						CashierItem cashierItem2 = new CashierItem(purchaseType);
						cashierItem2.SetCost(CurrencyType.Diamonds, buildingModel.GetSpeedUpUpgradeCost());
						cashier.AddItem(cashierItem2);
					}
				}
				CashierItem cashierItem3 = new CashierItem(purchaseType);
				if (buildingUpgradeLevel.CostDiamonds != 0)
				{
					cashierItem3.SetCost(CurrencyType.Diamonds, buildingUpgradeLevel.CostDiamonds);
				}
				else
				{
					cashierItem3.SetCost(CurrencyType.Supplies, buildingUpgradeLevel.CostSupplies);
					cashierItem3.SetCost(CurrencyType.Inhabitants, buildingUpgradeLevel.CostInhabitants);
				}
				cashier.AddItem(cashierItem3);
			}
			return cashier;
		}

		public Cashier GetInstantBuildingUpgradeCashierWithTokens()
		{
			Cashier cashier = new Cashier(base.manager);
			CashierItem cashierItem = new CashierItem(PurchaseType.InstantBuildingUpgrade);
			cashierItem.SetCost(CurrencyType.SuperBuildingTokenBP, 1);
			cashier.AddItem(cashierItem);
			cashier.useTokensForPayment = true;
			return cashier;
		}

		public BuildingModel AddBuilding(InitialCampBuildingData buildingData, FixedVec2 position, bool createView = true)
		{
			string typeName = buildingData.TypeName;
			BuildingModel buildingModel;
			switch (typeName)
			{
			case "Workshop":
				buildingModel = new WorkshopBuildingModel();
				break;
			case "TrainingGround":
				buildingModel = new TrainingGroundBuildingModel();
				break;
			case "Cage":
				buildingModel = new CageBuildingModel();
				break;
			case "MedicTent":
				buildingModel = new MedicTentModel();
				break;
			case "Residence":
				buildingModel = new ResidenceBuildingModel();
				break;
			default:
				if (base.gameEconomyData.GetBuildingType(typeName).Category == BuildingCategory.Vegetation)
				{
					buildingModel = new VegetationModel();
					((VegetationModel)buildingModel).DestroyAtCoucilLevel = buildingData.DestroyAtCouncilLevel;
					((VegetationModel)buildingModel).CutDependencyLevelRequired = buildingData.CutDependencyLevelRequired;
				}
				else
				{
					BuildingUpgradeLevel buildingUpgradeLevel = base.gameEconomyData.GetBuildingUpgradeLevel(typeName, 0);
					buildingModel = ((buildingUpgradeLevel == null || buildingUpgradeLevel.BuffEffectType == BuffEffectType.None) ? new BuildingModel() : new BuffBuildingModel());
				}
				break;
			}
			buildingModel.SetManager(base.manager);
			buildingModel.SetTypeName(typeName);
			buildingModel.Initialize();
			buildingModel.SetGridPosition(position);
			buildingModel.SetRotationAngle(buildingData.RotationAngle);
			buildingModel.SetLevel(buildingData.Level);
			buildingModel.Camp = this;
			buildingModel.AddedAtCampLevel = Level;
			if (base.manager.IsStarted)
			{
				buildingModel.Start();
			}
			if (createView)
			{
				NotifyChange("EventAddBuilding", buildingModel);
			}
			Buildings.Add(buildingModel);
			if (!BuildingsByTypeName.ContainsKey(typeName))
			{
				BuildingsByTypeName.Add(typeName, buildingModel);
			}
			buildingModel.Changed -= OnBuildingChange;
			buildingModel.Changed += OnBuildingChange;
			return buildingModel;
		}

		public void RepositionBuildings(FixedVec3 oldCampGridPosition)
		{
			if (Buildings == null)
			{
				return;
			}
			FixedVec2 gatePosition = Player.CampMover.GatePosition;
			FixedPoint fixedPoint = (oldCampGridPosition.X - Grid.Position.X) / Grid.CellSize.X;
			FixedPoint fixedPoint2 = (oldCampGridPosition.Z - Grid.Position.Z) / Grid.CellSize.Y;
			foreach (BuildingModel building in Buildings)
			{
				GridPosition gridPosition = new GridPosition();
				gridPosition.X = gatePosition.X + (building.GridPosition.X - GatePosition.X) + fixedPoint;
				gridPosition.Y = gatePosition.Y + (building.GridPosition.Y - GatePosition.Y) + fixedPoint2;
				building.GridPosition = gridPosition;
				building.CampMoved = true;
			}
			GatePosition = gatePosition;
		}

		private void RemoveBuilding(BuildingModel buildingModel)
		{
			if (Buildings.Contains(buildingModel))
			{
				Buildings.Remove(buildingModel);
			}
			if (BuildingsByTypeName.ContainsKey(buildingModel.TypeName))
			{
				BuildingsByTypeName.Remove(buildingModel.TypeName);
			}
		}

		public CampObjectModel GetGridObject(int x, int y)
		{
			if (x >= 0 && x < GridWidth && y >= 0 && y < GridHeight)
			{
				return grid[y * GridWidth + x];
			}
			return fakeBuilding;
		}

		public bool IsValidPosition(int x, int y)
		{
			CampObjectModel gridObject = GetGridObject(x, y);
			bool flag = gridObject != fakeBuilding;
			if (gridObject is BuildingModel)
			{
				flag = flag && (gridObject as BuildingModel).IsMoveable;
			}
			return flag;
		}

		public bool CanPlaceBuilding(BuildingModel building)
		{
			return CanPlaceBuildingAtLocation(building, building.GridPosition);
		}

		public GridPosition GetFreePositionToPlaceBuilding(GridPosition initialPosition, GridSize size)
		{
			int x = (int)(initialPosition.X - size.X / 2);
			int y = (int)(initialPosition.Y - size.Y / 2);
			SquareSpiralIterator squareSpiralIterator = new SquareSpiralIterator(x, y);
			int num = (GridWidth + size.X) * (GridHeight + size.Y);
			for (int i = 0; i < num; i++)
			{
				int x2 = squareSpiralIterator.GetX();
				int y2 = squareSpiralIterator.GetY();
				int num2 = x2 + size.X;
				int num3 = y2 + size.Y;
				bool flag = false;
				for (int j = y2; j < num3; j++)
				{
					for (int k = x2; k < num2; k++)
					{
						if (GetGridObject(k, j) != null)
						{
							flag = true;
							k = num2;
							j = num3;
						}
					}
				}
				if (!flag)
				{
					return new GridPosition(squareSpiralIterator.GetX(), squareSpiralIterator.GetY());
				}
				squareSpiralIterator.MoveNext();
			}
			return null;
		}

		public bool CanPlaceBuildingAtLocation(BuildingModel building, GridPosition targetPosition)
		{
			GridSize size = building.Size;
			int num = (int)targetPosition.X;
			int num2 = (int)targetPosition.Y;
			int num3 = num + size.X;
			int num4 = num2 + size.Y;
			for (int i = num2; i < num4; i++)
			{
				for (int j = num; j < num3; j++)
				{
					CampObjectModel gridObject = GetGridObject(j, i);
					if (gridObject != null && gridObject != building)
					{
						return false;
					}
				}
			}
			return true;
		}

		public bool CanPlaceAtLocation(string buildingType, GridSize size, GridPosition targetPosition, BuildingModel buildingToIgnore)
		{
			if (buildingType == "Cage")
			{
				return true;
			}
			int num = (int)targetPosition.X;
			int num2 = (int)targetPosition.Y;
			int num3 = num + size.X;
			int num4 = num2 + size.Y;
			for (int i = num2; i < num4; i++)
			{
				for (int j = num; j < num3; j++)
				{
					CampObjectModel gridObject = GetGridObject(j, i);
					if (gridObject != null && gridObject != buildingToIgnore)
					{
						return false;
					}
				}
			}
			return true;
		}

		public void UpdateGrid()
		{
			if (grid == null)
			{
				grid = new CampObjectModel[GridWidth * GridHeight];
			}
			UtilsArray.Fill(grid, fakeBuilding);
			if (Player.CampMover.ValidBuildingPositions != null)
			{
				RectData[] validBuildingPositions = Player.CampMover.ValidBuildingPositions;
				foreach (RectData rectData in validBuildingPositions)
				{
					int num = (rectData.X - (int)Grid.Position.X) / base.gameEconomyData.ConfigData.GridScale;
					int num2 = (rectData.Y - (int)Grid.Position.Z) / base.gameEconomyData.ConfigData.GridScale;
					int num3 = base.gameEconomyData.ScaleToGrid(rectData.X - (int)Grid.Position.X + rectData.Width);
					int num4 = base.gameEconomyData.ScaleToGrid(rectData.Y - (int)Grid.Position.Z + rectData.Height);
					int length = num3 - num;
					int num5 = num2 * GridWidth;
					for (int j = num; j < num3; j++)
					{
						grid[num5 + j] = null;
					}
					for (int k = num2 + 1; k < num4; k++)
					{
						Array.Copy(grid, num5 + num, grid, k * GridWidth + num, length);
					}
				}
			}
			for (int l = 0; l < Buildings.Count; l++)
			{
				BuildingModel buildingModel = Buildings[l];
				int num6 = (int)buildingModel.GridPosition.X;
				int num7 = (int)buildingModel.GridPosition.Y;
				int x = buildingModel.Size.X;
				int y = buildingModel.Size.Y;
				for (int m = num7; m < num7 + y; m++)
				{
					if (m < 0 || m >= GridHeight)
					{
						continue;
					}
					int num8 = m * GridWidth;
					for (int n = num6; n < num6 + x; n++)
					{
						if (n >= 0 && n < GridWidth)
						{
							grid[num8 + n] = buildingModel;
						}
					}
				}
			}
		}

		public bool HasMaximumStorages(CurrencyType type)
		{
			if (MaxCurrencyStorages != null)
			{
				if (MaxCurrencyStorages.ContainsKey(type))
				{
					return MaxCurrencyStorages[type];
				}
				base.Debug.Log("Unknown currency in HasMaxStorages: " + type);
			}
			return false;
		}

		protected void UpdateMaximumStorageFlags()
		{
			bool value = true;
			bool value2 = true;
			bool value3 = true;
			bool value4 = true;
			bool value5 = true;
			foreach (BuildingModel building in Buildings)
			{
				BuildingUpgradeLevel currentUpgradeLevel = building.GetCurrentUpgradeLevel();
				BuildingUpgradeLevel nextUpgradeLevel = building.GetNextUpgradeLevel();
				if (currentUpgradeLevel != null && !building.HasReachedMaxUpgradeLevel && building.DependencyLevelRequiredToUpgrade <= base.gameEconomyData.ConfigData.ForceCouncilMaxLevel && nextUpgradeLevel != null)
				{
					if (nextUpgradeLevel.SuppliesCapacity > currentUpgradeLevel.SuppliesCapacity)
					{
						value = false;
					}
					if (nextUpgradeLevel.SPTraitCapacity > currentUpgradeLevel.SPTraitCapacity)
					{
						value2 = false;
					}
					if (nextUpgradeLevel.SPCapacity > currentUpgradeLevel.SPCapacity)
					{
						value3 = false;
					}
					if (nextUpgradeLevel.OutpostCapacity > currentUpgradeLevel.OutpostCapacity)
					{
						value5 = false;
					}
					if (nextUpgradeLevel.ReplayTokenCapacity > currentUpgradeLevel.ReplayTokenCapacity)
					{
						value4 = false;
					}
				}
			}
			List<string> list = GetAvailableBuildingsToBuild();
			for (int i = 0; i < list.Count; i++)
			{
				for (int j = 1; j < 100; j++)
				{
					BuildingUpgradeLevel buildingUpgradeLevel = base.gameEconomyData.GetBuildingUpgradeLevel(list[i], j);
					if (buildingUpgradeLevel == null)
					{
						break;
					}
					if (buildingUpgradeLevel.SuppliesCapacity > 0)
					{
						value = false;
					}
					if (buildingUpgradeLevel.SPTraitCapacity > 0)
					{
						value2 = false;
					}
					if (buildingUpgradeLevel.SPCapacity > 0)
					{
						value3 = false;
					}
					if (buildingUpgradeLevel.ReplayTokenCapacity > 0)
					{
						value4 = false;
					}
					if (buildingUpgradeLevel.OutpostCapacity > 0)
					{
						value5 = false;
					}
				}
			}
			MaxCurrencyStorages = new Dictionary<CurrencyType, bool>();
			MaxCurrencyStorages.Add(CurrencyType.Supplies, value);
			MaxCurrencyStorages.Add(CurrencyType.SPTraitsUpgradeToken, value2);
			MaxCurrencyStorages.Add(CurrencyType.SurvivalPoints, value3);
			MaxCurrencyStorages.Add(CurrencyType.ReplayToken, value4);
			MaxCurrencyStorages.Add(CurrencyType.Outpost, value5);
		}

		public int GetBuildingDependencyLevel()
		{
			return GetBuilding(base.gameEconomyData.ConfigData.DependencyLevelBuilding)?.Level ?? int.MaxValue;
		}

		public int GetWeaponDependencyLevel()
		{
			return GetBuilding("Workshop")?.Level ?? int.MaxValue;
		}

		public BuildingModel GetBuilding(string typeName)
		{
			if (BuildingsByTypeName.TryGetValue(typeName, out var value))
			{
				return value;
			}
			return null;
		}

		public int GetCarExplorationLevel()
		{
			return GetBuilding("MissionCar")?.Level ?? 0;
		}

		public int GetTrainingGroundLevel()
		{
			return GetBuilding("TrainingGround")?.Level ?? 0;
		}

		public int GetBuildingLevel(string buildingType)
		{
			return GetBuilding(buildingType)?.Level ?? 0;
		}

		public int GetCouncilLevel()
		{
			return GetBuilding("Council")?.Level ?? 0;
		}

		public BuildingModel IsBuildingUpgradeInProgress()
		{
			int count = Buildings.Count;
			for (int i = 0; i < count; i++)
			{
				BuildingModel buildingModel = Buildings[i];
				if (buildingModel.IsUpgrading)
				{
					return buildingModel;
				}
			}
			return null;
		}

		public override void Tick(long deltaTime)
		{
			base.Tick(deltaTime);
			RemoveBuildingsFromRemoveList();
		}

		private void OnBuildingChange(ModelObject m, string changed, object args)
		{
			if (changed == "level")
			{
				if ((m as BuildingModel).TypeName == "Council")
				{
					DestroyBuildingWhenCouncilUpgrade();
					AddBuildings(base.manager.Player.CampMover.GetCurrentCampType().Buildings);
				}
				NotifyChange("EventLevelUpBuilding", m);
			}
			if (changed == "position")
			{
				UpdateGrid();
				NotifyChange("EventPositionBuilding", m);
			}
			if (changed == "build")
			{
				NotifyChange("EventUpgradeBuilding", m);
			}
			if (changed == "collected")
			{
				NotifyChange("EventBuildingCollected", m);
			}
		}

		private void RemoveBuildingsFromRemoveList()
		{
			List<BuildingModel> list = null;
			for (int i = 0; i < Buildings.Count; i++)
			{
				if (Buildings[i].MarkedToBeDeleted)
				{
					if (list == null)
					{
						list = new List<BuildingModel>();
					}
					list.Add(Buildings[i]);
				}
			}
			if (list == null)
			{
				return;
			}
			foreach (BuildingModel item in list)
			{
				NotifyChange("RemoveBuilding", item);
				RemoveBuilding(item);
				UpdateGrid();
			}
		}
	}
}
