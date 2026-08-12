using BaseModel;

namespace TWDModel
{
	public abstract class ModelUpgraderBuildingModel : BuildingModel
	{
		public const string UpgradeSeenEvent = "UpgradeSeen";

		public const string UpgradingItemReady = "UpgradingItemReady";

		public const string NewItemStartedUpgrading = "NewItemStartedUpgrading";

		public const string UpgradingItemCancelled = "UpgradingItemCancelled";

		[IgnoreModelProperty]
		public TWDModelObject UpgradingModel { get; private set; }

		[IgnoreModelProperty]
		public TWDModelObject UpgradedUnseenModel { get; private set; }

		public ModelUpgraderBuildingModel()
		{
		}

		public override void Start()
		{
			base.Start();
			if (UpgradingModel != null)
			{
				UpgradingModel.Changed += OnUpgradingModelChanged;
			}
		}

		public void OnUpgradingModelChanged(ModelObject model, string changed, object args)
		{
			if (UpgradingModel == args && changed == "ActionFinishedEvent")
			{
				UpgradedUnseenModel = UpgradingModel;
				ResetUpgradingModel();
				NotifyChange("UpgradingItemReady");
			}
		}

		public void SetUpgradingModel(TWDModelObject model)
		{
			UpgradingModel = model;
			UpgradingModel.Changed += OnUpgradingModelChanged;
			NotifyChange("NewItemStartedUpgrading");
		}

		public virtual void MarkModelUpgradeAsSeen()
		{
			UpgradedUnseenModel = null;
			NotifyChange("UpgradeSeen");
		}

		public void MarkModelUpgradeAsSeenHack()
		{
			UpgradedUnseenModel = null;
			NotifyChange("UpgradeSeen");
		}

		public void ResetUpgradingModel()
		{
			if (UpgradingModel != null)
			{
				UpgradingModel.Changed -= OnUpgradingModelChanged;
			}
			UpgradingModel = null;
		}
	}
}
