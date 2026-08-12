using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace TWDModel
{
	public class ReturnEndlessDealModel : TWDModelObject
	{
		public const string ReturnEndlessDealChanged = "ReturnEndlessDealChanged";

		[JsonIgnore]
		public long RefreshIntervalMs
		{
			get
			{
				int num = base.manager?.GameEconomyData?.ReturnConfig?.EndlessDealRefreshDays ?? 6;
				if (num <= 0)
				{
					num = 6;
				}
				return (long)num * 24L * 60 * 60 * 1000;
			}
		}

		public int CurrentClass { get; set; }

		public int CurrentPackIndex { get; set; }

		public long NextRefreshTime { get; set; }

		[JsonIgnore]
		public List<ReturnEndlessDealDefinition> CurrentDefinitions
		{
			get
			{
				if (base.manager?.GameEconomyData?.ReturnEndlessDealDefinitions == null)
				{
					return new List<ReturnEndlessDealDefinition>();
				}
				return (from d in base.manager.GameEconomyData.ReturnEndlessDealDefinitions
					where d.Class == CurrentClass
					orderby d.Id
					select d).ToList();
			}
		}

		[JsonIgnore]
		public ReturnEndlessDealDefinition CurrentPack
		{
			get
			{
				List<ReturnEndlessDealDefinition> currentDefinitions = CurrentDefinitions;
				if (currentDefinitions != null && CurrentPackIndex >= 0 && CurrentPackIndex < currentDefinitions.Count)
				{
					return currentDefinitions[CurrentPackIndex];
				}
				return null;
			}
		}

		[JsonIgnore]
		public bool IsActivityAvailable
		{
			get
			{
				if (base.manager?.Player?.ReturnActivityManager != null)
				{
					return base.manager.Player.ReturnActivityManager.IsReturnActivityAvailable();
				}
				return false;
			}
		}

		[JsonIgnore]
		public bool HasRedDot
		{
			get
			{
				if (!IsActivityAvailable)
				{
					return false;
				}
				ReturnEndlessDealDefinition currentPack = CurrentPack;
				if (currentPack != null)
				{
					return currentPack.Type == ReturnEndlessDealPackType.Free;
				}
				return false;
			}
		}

		public override bool IsValid()
		{
			return true;
		}

		public override void Initialize()
		{
			base.Initialize();
			CurrentClass = -1;
			CurrentPackIndex = 0;
			NextRefreshTime = 0L;
		}

		public void OnActivityEnded()
		{
			if (CurrentClass != -1)
			{
				base.manager?.Debug?.LogInfo($"[ReturnEndlessDealModel] Activity ended. Clearing state. Class={CurrentClass}");
				ClearState();
				NotifyChange("ReturnEndlessDealChanged");
			}
		}

		public override void Tick(long deltaTime)
		{
			base.Tick(deltaTime);
			if (CurrentClass != -1)
			{
				long valueOrDefault = (base.manager?.Player?.UtcTimeStamp).GetValueOrDefault();
				if (NextRefreshTime > 0 && valueOrDefault >= NextRefreshTime)
				{
					RefreshPacks(valueOrDefault, isReset: false);
				}
			}
		}

		public void ResetForNewActivity(long currentTimestamp)
		{
			RefreshPacks(currentTimestamp, isReset: true);
		}

		private void RefreshPacks(long now, bool isReset)
		{
			int councilLevel = (base.manager?.Player?.CouncilLevel).GetValueOrDefault();
			List<ReturnEndlessDealDefinition> returnEndlessDealDefinitions = base.manager.GameEconomyData.ReturnEndlessDealDefinitions;
			if (returnEndlessDealDefinitions != null && returnEndlessDealDefinitions.Count > 0)
			{
				ReturnEndlessDealDefinition returnEndlessDealDefinition = returnEndlessDealDefinitions.FirstOrDefault((ReturnEndlessDealDefinition d) => councilLevel >= d.CouncilLevelMin && councilLevel <= d.CouncilLevelMax);
				if (returnEndlessDealDefinition != null)
				{
					CurrentClass = returnEndlessDealDefinition.Class;
				}
				else
				{
					CurrentClass = returnEndlessDealDefinitions.First().Class;
					base.manager.Debug.LogWarning($"[ReturnEndlessDealModel] No matching ReturnEndlessDealDefinition for CouncilLevel={councilLevel}. Fallback to Class={CurrentClass}");
				}
			}
			else
			{
				CurrentClass = -1;
			}
			CurrentPackIndex = 0;
			if (isReset || NextRefreshTime == 0L)
			{
				NextRefreshTime = now + RefreshIntervalMs;
			}
			else
			{
				while (NextRefreshTime <= now)
				{
					NextRefreshTime += RefreshIntervalMs;
				}
			}
			base.manager.Debug.LogInfo($"[ReturnEndlessDealModel] RefreshPacks: Class={CurrentClass}, NextRefreshTime={NextRefreshTime}, IsReset={isReset}");
			NotifyChange("ReturnEndlessDealChanged");
		}

		private void ClearState()
		{
			CurrentClass = -1;
			CurrentPackIndex = 0;
			NextRefreshTime = 0L;
		}

		public bool ClaimFreePack()
		{
			if (!IsActivityAvailable)
			{
				base.manager.Debug.LogWarning("[ReturnEndlessDealModel] ClaimFreePack: Return activity is not available.");
				return false;
			}
			ReturnEndlessDealDefinition currentPack = CurrentPack;
			if (currentPack == null)
			{
				base.manager.Debug.LogWarning("[ReturnEndlessDealModel] ClaimFreePack: No current pack available.");
				return false;
			}
			if (currentPack.Type != ReturnEndlessDealPackType.Free)
			{
				base.manager.Debug.LogWarning($"[ReturnEndlessDealModel] ClaimFreePack: Current pack is not free. Type={currentPack.Type}");
				return false;
			}
			if (!string.IsNullOrEmpty(currentPack.Reward))
			{
				new Rewards(currentPack.Reward)?.Give(base.manager);
			}
			CurrentPackIndex++;
			base.manager.Debug.LogInfo($"[ReturnEndlessDealModel] ClaimFreePack: Claimed free pack ID={currentPack.Id}. New index={CurrentPackIndex}");
			NotifyChange("ReturnEndlessDealChanged");
			return true;
		}

		public bool OnBuyBundle(string bundleId)
		{
			ReturnEndlessDealDefinition currentPack = CurrentPack;
			if (currentPack == null)
			{
				base.manager.Debug.LogWarning("[ReturnEndlessDealModel] OnBuyBundle: No current pack available. BundleId=" + bundleId);
				return false;
			}
			if (currentPack.Type != ReturnEndlessDealPackType.Paid)
			{
				base.manager.Debug.LogWarning($"[ReturnEndlessDealModel] OnBuyBundle: Current pack is not paid. Type={currentPack.Type}, BundleId={bundleId}");
				return false;
			}
			if (currentPack.BundleIdentifier != bundleId)
			{
				base.manager.Debug.LogWarning("[ReturnEndlessDealModel] OnBuyBundle: Bundle identifier mismatch. Expected=" + currentPack.BundleIdentifier + ", Actual=" + bundleId);
				return false;
			}
			CurrentPackIndex++;
			base.manager.Debug.LogInfo($"[ReturnEndlessDealModel] OnBuyBundle: Paid pack claimed via IAP. BundleId={bundleId}. New index={CurrentPackIndex}");
			NotifyChange("ReturnEndlessDealChanged");
			return true;
		}
	}
}
