using System.Collections.Generic;
using System.Linq;
using BaseModel;
using Newtonsoft.Json;

namespace TWDModel
{
	public class RecycleWeaponManager : TWDModelObject
	{
		[JsonIgnore]
		private long _lastCheckedTimeKey = -1L;

		public ModelList<RecycleWeaponActivityModel> ActivityDataList { get; set; }

		public override void Initialize()
		{
			base.Initialize();
			if (ActivityDataList == null)
			{
				ActivityDataList = new ModelList<RecycleWeaponActivityModel>();
			}
		}

		public override void Start()
		{
			if (base.manager?.Player == null)
			{
				base.manager?.Debug?.LogError("RecycleWeaponManager Start: manager or Player is null");
				return;
			}
			if (ActivityDataList == null)
			{
				ActivityDataList = new ModelList<RecycleWeaponActivityModel>();
			}
			ActivityDataList.SetManager(base.manager);
			_lastCheckedTimeKey = -1L;
			RefreshData();
			base.Start();
		}

		public bool RefreshData()
		{
			if (base.manager?.GameEconomyData == null)
			{
				return false;
			}
			if (ActivityDataList == null)
			{
				ActivityDataList = new ModelList<RecycleWeaponActivityModel>();
			}
			List<RecycleWeaponDefinition> activeRecycleWeaponDefinitions = base.manager.GameEconomyData.GetActiveRecycleWeaponDefinitions(base.manager.Player.UtcTimeStamp);
			if (activeRecycleWeaponDefinitions == null || activeRecycleWeaponDefinitions.Count == 0)
			{
				ActivityDataList.Clear();
				return true;
			}
			HashSet<int> validIds = new HashSet<int>(activeRecycleWeaponDefinitions.Select((RecycleWeaponDefinition d) => d.Identifier));
			foreach (RecycleWeaponActivityModel item in (from a in ActivityDataList.ToList()
				where a == null || !validIds.Contains(a.Identifier)
				select a).ToList())
			{
				ActivityDataList.Remove(item);
			}
			foreach (RecycleWeaponActivityModel item2 in (from a in ActivityDataList.ToList()
				where a != null && !a.IsRecycleWeaponActive()
				select a).ToList())
			{
				ActivityDataList.Remove(item2);
			}
			foreach (RecycleWeaponDefinition def in activeRecycleWeaponDefinitions)
			{
				if (ActivityDataList.Find((RecycleWeaponActivityModel a) => a != null && a.Identifier == def.Identifier) == null)
				{
					RecycleWeaponActivityModel recycleWeaponActivityModel = new RecycleWeaponActivityModel(def.Identifier, def.Type);
					recycleWeaponActivityModel.SetManager(base.manager);
					recycleWeaponActivityModel.Initialize();
					recycleWeaponActivityModel.Start();
					recycleWeaponActivityModel.IsCanPopOpenStatus = true;
					ActivityDataList.Add(recycleWeaponActivityModel);
				}
			}
			return true;
		}

		public override void Tick(long deltaTime)
		{
			base.Tick(deltaTime);
			if (base.manager?.Player != null)
			{
				long num = base.manager.Player.UtcTimeStamp / 60000;
				if (num != _lastCheckedTimeKey)
				{
					_lastCheckedTimeKey = num;
					CheckActivityChange();
					base.manager.Player.ActivityIntegrationManager?.RegisterRecycleWeaponActivities();
				}
			}
		}

		private void CheckActivityChange()
		{
			if (base.manager?.Player != null && base.manager?.GameEconomyData != null)
			{
				HashSet<int> hashSet = new HashSet<int>(base.manager.GameEconomyData.GetActiveRecycleWeaponDefinitions(base.manager.Player.UtcTimeStamp)?.Select((RecycleWeaponDefinition d) => d.Identifier) ?? Enumerable.Empty<int>());
				HashSet<int> hashSet2 = new HashSet<int>((from a in ActivityDataList?.Where((RecycleWeaponActivityModel a) => a != null)
					select a.Identifier) ?? Enumerable.Empty<int>());
				if (!hashSet.SetEquals(hashSet2))
				{
					RefreshData();
				}
			}
		}

		public override bool IsValid()
		{
			return true;
		}
	}
}
