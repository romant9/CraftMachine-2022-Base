using System;
using System.Collections.Generic;
using BaseModel;
using Newtonsoft.Json;

namespace TWDModel
{
	public class RotatingBundleManager : TWDModelObject
	{
		private static long CheckForNewSpenderTierDefaultTime = 60000L;

		public static string RotationDedicatedRandomID = "BundleRotationRandom";

		public List<string> PurchasedRotations { get; set; }

		public List<string> PurchasedBundlesAtRotation { get; set; }

		public List<int> PurchaseStep { get; set; }

		public int CurrentRotationStep { get; set; }

		public int TotalRotationLoopsPerfomed { get; set; }

		public string CurrentRotationIdentifier { get; set; }

		public string CurrentRotatingBundleIdentifier { get; set; }

		public long CheckForNewSpenderTierTimer { get; set; }

		[JsonIgnore]
		public BundleRotationDefinition CurrentRotationDefinition
		{
			get
			{
				if (!string.IsNullOrEmpty(CurrentRotationIdentifier))
				{
					return base.manager.GameEconomyData.GetBundleRotationDefinition(CurrentRotationIdentifier);
				}
				return null;
			}
		}

		private bool NeedsToUpdateRotation
		{
			get
			{
				BundleRotationDefinition rotationToOffer = GetRotationToOffer();
				if (!string.IsNullOrEmpty(CurrentRotationIdentifier))
				{
					if (rotationToOffer != null)
					{
						return CurrentRotationIdentifier != rotationToOffer.RotationIdentifier;
					}
					return true;
				}
				return rotationToOffer != null;
			}
		}

		public override void Initialize()
		{
			base.Initialize();
			PurchasedRotations = new List<string>();
			PurchasedBundlesAtRotation = new List<string>();
			PurchaseStep = new List<int>();
			CurrentRotationStep = 0;
			TotalRotationLoopsPerfomed = 0;
			CurrentRotationIdentifier = "";
			CheckForNewSpenderTierTimer = 0L;
		}

		public override void Start()
		{
			base.Start();
			if (NeedsToUpdateRotation)
			{
				StartNewRotation(GetRotationToOffer());
			}
			if (base.manager.Player.BundleManager != null)
			{
				base.manager.Player.BundleManager.Changed -= OnBundleManagerChanged;
				base.manager.Player.BundleManager.Changed += OnBundleManagerChanged;
			}
		}

		public override bool IsValid()
		{
			return true;
		}

		public override void Tick(long deltaTime)
		{
			base.Tick(deltaTime);
			CheckForNewSpenderTierTimer -= deltaTime;
			if (CheckForNewSpenderTierTimer <= 0)
			{
				if (NeedsToUpdateRotation)
				{
					StartNewRotation(GetRotationToOffer());
				}
				else
				{
					CheckForNewSpenderTierTimer = CheckForNewSpenderTierDefaultTime;
				}
			}
		}

		private BundleRotationDefinition GetRotationToOffer()
		{
			long secondsSinceLastPurchase = (base.manager.Player.UtcTimeStamp - base.manager.Player.BundleManager.LastPurchaseUTCTime) / 1000;
			List<BundleRotationDefinition> list = new List<BundleRotationDefinition>();
			List<BundleRotationDefinition> tierAvailableBundleRotationDefinitions = base.manager.GameEconomyData.GetTierAvailableBundleRotationDefinitions(base.manager.Player, secondsSinceLastPurchase);
			for (int i = 0; i < tierAvailableBundleRotationDefinitions.Count; i++)
			{
				BundleRotationDefinition bundleRotationDefinition = tierAvailableBundleRotationDefinitions[i];
				if (bundleRotationDefinition == null || (!string.IsNullOrEmpty(bundleRotationDefinition.RequiredRotation) && PurchasedRotations != null && !PurchasedRotations.Contains(bundleRotationDefinition.RequiredRotation)) || (PurchasedRotations != null && PurchasedRotations.Contains(bundleRotationDefinition.RotationIdentifier)))
				{
					list.Add(bundleRotationDefinition);
				}
			}
			for (int j = 0; j < list.Count; j++)
			{
				BundleRotationDefinition item = list[j];
				if (tierAvailableBundleRotationDefinitions.Contains(item))
				{
					tierAvailableBundleRotationDefinitions.Remove(item);
				}
			}
			if (tierAvailableBundleRotationDefinitions.Count > 0)
			{
				return tierAvailableBundleRotationDefinitions[tierAvailableBundleRotationDefinitions.Count - 1];
			}
			return null;
		}

		private void StartNewRotation(BundleRotationDefinition rotationDefinition)
		{
			if (rotationDefinition != null)
			{
				CheckForNewSpenderTierTimer = CheckForNewSpenderTierDefaultTime;
				CurrentRotationStep = -1;
				TotalRotationLoopsPerfomed = 0;
				CurrentRotationIdentifier = rotationDefinition.RotationIdentifier;
				MoveToNextStep();
			}
		}

		public void LimitedBundleRemovedFromBundleManager(string bundleStoreDefinition)
		{
			if (bundleStoreDefinition == CurrentRotatingBundleIdentifier)
			{
				CurrentRotatingBundleIdentifier = "";
				StartNewRotationOrStep();
			}
		}

		private void MoveToNextStep()
		{
			BundleRotationDefinition currentRotationDefinition = CurrentRotationDefinition;
			if (currentRotationDefinition == null)
			{
				return;
			}
			CurrentRotationStep++;
			if (CurrentRotationStep >= currentRotationDefinition.TotalSteps)
			{
				TotalRotationLoopsPerfomed++;
				CurrentRotationStep = currentRotationDefinition.RestartingPoint;
			}
			string text = "";
			bool flag = currentRotationDefinition.BundlesToRandomizeIgnoresHighesUnlockClass[CurrentRotationStep];
			List<string> onlyUnlockedBundlesFromList = GetOnlyUnlockedBundlesFromList(currentRotationDefinition.BundlesToRandomizeOnSteps[CurrentRotationStep]);
			List<string> list = ((onlyUnlockedBundlesFromList.Count > 0) ? onlyUnlockedBundlesFromList : currentRotationDefinition.BundlesToRandomizeOnSteps[CurrentRotationStep]);
			if (TotalRotationLoopsPerfomed == 0 && !flag)
			{
				text = GetBestBundle(list);
			}
			else
			{
				ModelRandom dedicatedRandom = base.manager.Player.LootManager.GetDedicatedRandom(RotationDedicatedRandomID);
				if (dedicatedRandom != null)
				{
					text = dedicatedRandom.GetRandomElement(list, remove: false);
				}
				else if (list.Count > 0)
				{
					text = list[TotalRotationLoopsPerfomed % list.Count];
				}
			}
			BundleStoreDefinition bundleStoreDefinition = base.manager.GameEconomyData.GetBundleStoreDefinition(text);
			if (!string.IsNullOrEmpty(text) && base.manager.Player.BundleManager.CanTriggerNewRotatingBundle(bundleStoreDefinition))
			{
				CurrentRotatingBundleIdentifier = text;
				base.manager.Player.BundleManager.InitiateRotatingBundle(bundleStoreDefinition);
			}
		}

		private List<string> GetOnlyUnlockedBundlesFromList(List<string> bundleIDs)
		{
			List<string> list = new List<string>();
			for (int i = 0; i < (bundleIDs?.Count ?? 0); i++)
			{
				string text = bundleIDs[i];
				if (!string.IsNullOrEmpty(text))
				{
					BundleStoreDefinition bundleStoreDefinition = base.manager.Player.gameEconomyData.GetBundleStoreDefinition(text);
					if (bundleStoreDefinition != null && (bundleStoreDefinition.SurvivorClassRequired == SurvivorClass.None || base.manager.Player.SurvivorContainer.IsSurvivorClassUnlocked(bundleStoreDefinition.SurvivorClassRequired)))
					{
						list.Add(text);
					}
				}
			}
			return list;
		}

		private string GetBestBundle(List<string> bundles)
		{
			if (bundles != null && bundles.Count > 0)
			{
				bundles.StableSort(delegate(string a, string b)
				{
					if (string.IsNullOrEmpty(a) || string.IsNullOrEmpty(b))
					{
						return 0;
					}
					BundleStoreDefinition bundleStoreDefinition = base.manager.Player.gameEconomyData.GetBundleStoreDefinition(a);
					BundleStoreDefinition bundleStoreDefinition2 = base.manager.Player.gameEconomyData.GetBundleStoreDefinition(b);
					SurvivorClass survivorClass = ((bundleStoreDefinition != null && bundleStoreDefinition.SurvivorClassRequired != SurvivorClass.None && base.manager.Player.SurvivorContainer.IsSurvivorClassUnlocked(bundleStoreDefinition.SurvivorClassRequired)) ? bundleStoreDefinition.SurvivorClassRequired : SurvivorClass.None);
					SurvivorClass survivorClass2 = ((bundleStoreDefinition2 != null && bundleStoreDefinition2.SurvivorClassRequired != SurvivorClass.None && base.manager.Player.SurvivorContainer.IsSurvivorClassUnlocked(bundleStoreDefinition2.SurvivorClassRequired)) ? bundleStoreDefinition2.SurvivorClassRequired : SurvivorClass.None);
					return (survivorClass == SurvivorClass.None || survivorClass2 == SurvivorClass.None) ? Math.Sign(survivorClass - survivorClass2) : Math.Sign(survivorClass2 - survivorClass);
				});
				return bundles[0];
			}
			return "";
		}

		private void StartNewRotationOrStep()
		{
			if (!string.IsNullOrEmpty(CurrentRotatingBundleIdentifier))
			{
				base.manager.Player.BundleManager.RemoveRotatingBundle(base.manager.GameEconomyData.GetBundleStoreDefinition(CurrentRotatingBundleIdentifier));
			}
			if (NeedsToUpdateRotation)
			{
				StartNewRotation(GetRotationToOffer());
			}
			else
			{
				MoveToNextStep();
			}
		}

		public void LimitedBundlePurchased(string bundleIdentifier)
		{
			BundleRotationDefinition currentRotationDefinition = CurrentRotationDefinition;
			if (currentRotationDefinition != null)
			{
				for (int i = 0; i < currentRotationDefinition.BundlesToRandomizeOnSteps.Count; i++)
				{
					if (currentRotationDefinition.BundlesToRandomizeOnSteps[i].Contains(bundleIdentifier))
					{
						PurchasedRotations.Add(currentRotationDefinition.RotationIdentifier);
						PurchasedBundlesAtRotation.Add(bundleIdentifier);
						PurchaseStep.Add(CurrentRotationStep);
						StartNewRotationOrStep();
						return;
					}
				}
			}
			foreach (KeyValuePair<string, BundleRotationDefinition> allBundleRotationDefinition in base.manager.GameEconomyData.GetAllBundleRotationDefinitions())
			{
				BundleRotationDefinition value = allBundleRotationDefinition.Value;
				if (value == null || PurchasedRotations.Contains(value.RotationIdentifier))
				{
					continue;
				}
				bool flag = false;
				for (int j = 0; j < value.BundlesToRandomizeOnSteps.Count; j++)
				{
					if (value.BundlesToRandomizeOnSteps[j].Contains(bundleIdentifier))
					{
						PurchasedRotations.Add(value.RotationIdentifier);
						PurchasedBundlesAtRotation.Add(bundleIdentifier);
						PurchaseStep.Add(CurrentRotationStep);
						flag = true;
						break;
					}
				}
				if (flag)
				{
					StartNewRotationOrStep();
					break;
				}
			}
		}

		public string GetRotationPurchasedFromBundle(string bundleIdentifier)
		{
			if (!string.IsNullOrEmpty(bundleIdentifier) && PurchasedBundlesAtRotation != null && PurchasedBundlesAtRotation.Count > 0 && PurchasedRotations != null && PurchasedRotations.Count > 0)
			{
				int num = PurchasedBundlesAtRotation.FindIndex((string x) => x == bundleIdentifier);
				if (num >= 0 && num < PurchasedRotations.Count)
				{
					return PurchasedRotations[num];
				}
			}
			return null;
		}

		public int GetBestRotationStepThatContainsBundle(string bundleIdentifier)
		{
			if (!string.IsNullOrEmpty(bundleIdentifier) && CurrentRotationDefinition != null)
			{
				if (CurrentRotationStep > -1 && CurrentRotationDefinition.BundlesToRandomizeOnSteps != null && CurrentRotationDefinition.BundlesToRandomizeOnSteps.Count > CurrentRotationStep && CurrentRotationDefinition.BundlesToRandomizeOnSteps[CurrentRotationStep].FindIndex((string x) => x == bundleIdentifier) > -1)
				{
					return CurrentRotationStep;
				}
				for (int num = 0; num < ((CurrentRotationDefinition.BundlesToRandomizeOnSteps != null) ? CurrentRotationDefinition.BundlesToRandomizeOnSteps.Count : 0); num++)
				{
					if (CurrentRotationDefinition.BundlesToRandomizeOnSteps[num].FindIndex((string x) => x == bundleIdentifier) > -1)
					{
						return num;
					}
				}
			}
			return -1;
		}

		public int GetRotationStepPurchasedFromBundle(string bundleIdentifier)
		{
			if (!string.IsNullOrEmpty(bundleIdentifier) && PurchasedBundlesAtRotation != null && PurchasedBundlesAtRotation.Count > 0 && PurchaseStep != null && PurchaseStep.Count > 0)
			{
				int num = PurchasedBundlesAtRotation.FindIndex((string x) => x == bundleIdentifier);
				if (num >= 0 && num < PurchaseStep.Count)
				{
					return PurchaseStep[num];
				}
			}
			return -1;
		}

		public void OnBundleManagerChanged(ModelObject m, string changed, object args)
		{
			if (changed == "LimitedBundleAvailableEvent")
			{
				string text = (string)args;
				if (string.IsNullOrEmpty(CurrentRotatingBundleIdentifier) || CurrentRotatingBundleIdentifier == text)
				{
					StartNewRotationOrStep();
				}
			}
		}
	}
}
