using BaseModel;
using Newtonsoft.Json;
using TWDModel;

public class VegetationModel : BuildingModel
{
	private const string ExactSizeVegetationPrefix = "Camp_ExpansionZone_";

	public bool IsBeingCut
	{
		get
		{
			if (CutTimedActionModel != null)
			{
				return CutTimedActionModel.IsActionUnderway();
			}
			return false;
		}
	}

	public TimedActionModel CutTimedActionModel { get; protected set; }

	public int DestroyAtCoucilLevel { get; set; }

	public int CutDependencyLevelRequired { get; set; }

	[JsonIgnore]
	public Cashier GetCutCashier
	{
		get
		{
			Cashier cashier = new Cashier(base.manager);
			CashierItem cashierItem = new CashierItem(PurchaseType.CutVegetation);
			cashierItem.SetCost(CurrencyType.Supplies, GetCurrentUpgradeLevel().DestroyCost);
			cashier.AddItem(cashierItem);
			return cashier;
		}
	}

	public bool CanBeCutAt(int councilLevel)
	{
		if (CutDependencyLevelRequired > 0)
		{
			return councilLevel >= CutDependencyLevelRequired;
		}
		return true;
	}

	public override void Initialize()
	{
		base.Initialize();
		ApplyExactSizeIfNeeded();
	}

	public override void Start()
	{
		base.Start();
		ApplyExactSizeIfNeeded();
		if (CutTimedActionModel != null)
		{
			CutTimedActionModel.Changed += OnCutTimedActionModelChanged;
		}
	}

	private void ApplyExactSizeIfNeeded()
	{
		if (base.BuildingType != null && !string.IsNullOrEmpty(base.BuildingType.Name) && base.BuildingType.Name.StartsWith("Camp_ExpansionZone_"))
		{
			base.Size = new GridSize(base.gameEconomyData.ScaleToGrid(base.BuildingType.Size.X), base.gameEconomyData.ScaleToGrid(base.BuildingType.Size.Y));
		}
	}

	public TWDModelResult StartCut()
	{
		CutTimedActionModel = new TimedActionModel();
		CutTimedActionModel.SetManager(base.manager);
		CutTimedActionModel.Initialize();
		CutTimedActionModel.PurchaseType = PurchaseType.CutVegetation;
		CutTimedActionModel.Changed += OnCutTimedActionModelChanged;
		CutTimedActionModel.Start();
		UpdateModelObjects();
		int destroyTime = GetCurrentUpgradeLevel().DestroyTime;
		if (destroyTime > 0)
		{
			return CutTimedActionModel.StartAction(destroyTime, GetCutCashier, this);
		}
		return CutTimedActionModel.StartActionInstant(GetCutCashier, this);
	}

	public TWDModelResult CutInstant()
	{
		if (base.Camp != null && base.BuildingType != null)
		{
			base.Camp.MarkExpansionTypeUnlocked(base.BuildingType.Name);
		}
		DeleteMe();
		return TWDModelResult.OK;
	}

	private void OnCutTimedActionModelChanged(ModelObject model, string changed, object args)
	{
		if (model != CutTimedActionModel)
		{
			return;
		}
		if (changed == "ActionStartEvent")
		{
			NotifyChange("ActionStartEvent", model);
		}
		else if (changed == "ActionFinishedEvent")
		{
			if (base.Camp != null && base.BuildingType != null)
			{
				base.Camp.MarkExpansionTypeUnlocked(base.BuildingType.Name);
			}
			DeleteMe();
		}
	}
}
