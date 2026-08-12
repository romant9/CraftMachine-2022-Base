using System.Collections.Generic;
using BaseModel;
using Newtonsoft.Json;

namespace TWDModel
{
	public class CombatSupportModel : TWDModelObject
	{
		public delegate void SupportActionExecuted(SupportModel supportModel);

		private ISupportExecution supportExecution;

		public string SupportId { get; set; }

		public int NextUsableTurn { get; set; }

		public int NextInnerUsableTurn { get; set; }

		public int usedCount { get; set; }

		public int SlotIndex { get; set; }

		public string SurvivorId { get; set; }

		public List<int> UsedTurns { get; set; }

		public List<int> AffectedTargets { get; set; }

		[JsonIgnore]
		public SupportModel SupportModel { get; private set; }

		[JsonIgnore]
		public SurvivorModel AttachedSurvivor { get; private set; }

		[JsonIgnore]
		public int RemainingCooldown
		{
			get
			{
				if (base.manager?.CombatModel?.TurnManager == null)
				{
					return 0;
				}
				int num = NextUsableTurn - base.manager.CombatModel.TurnManager.TurnCount;
				if (num > 0)
				{
					return num;
				}
				return 0;
			}
		}

		[JsonIgnore]
		public int RealCooldown
		{
			get
			{
				int result = 0;
				if (SupportModel == null)
				{
					return 0;
				}
				MapMissionModel mapMissionModel = base.manager?.Player?.MapContainerModel?.AttackTargetMissionModel;
				if (mapMissionModel != null)
				{
					if (mapMissionModel.IsInWeeklyChallenge || mapMissionModel.IsInApocalyptiWeeklyChallenge)
					{
						result = SupportModel.ChallengeCooldown;
					}
					if (mapMissionModel.IsInWeeklySurvival)
					{
						result = SupportModel.DistanceCooldown;
					}
				}
				if (base.manager?.CombatModel != null)
				{
					if (base.manager.CombatModel.IsEndlessBattleMission)
					{
						result = SupportModel.Cooldown;
					}
					if (base.manager.CombatModel.IsGuildBattleMission)
					{
						result = SupportModel.GVGCooldown;
					}
				}
				return result;
			}
		}

		[JsonIgnore]
		public int RemainingInnerCooldown
		{
			get
			{
				if (base.manager?.CombatModel?.TurnManager == null)
				{
					return 0;
				}
				int num = NextInnerUsableTurn - base.manager.CombatModel.TurnManager.TurnCount;
				if (num > 0)
				{
					return num;
				}
				return 0;
			}
		}

		public event SupportActionExecuted Executed;

		public CombatSupportModel()
		{
		}

		public CombatSupportModel(string supportId, int slotIndex, string survivorId)
		{
			SupportId = supportId;
			SlotIndex = slotIndex;
			SurvivorId = survivorId;
			UsedTurns = new List<int>();
			AffectedTargets = new List<int>();
		}

		public override void SetManager(ModelManager mgr)
		{
			base.SetManager(mgr);
			SupportModel = SupportHelpers.GetMissionSupport(base.manager.Player.MapContainerModel.AttackTargetMissionModel, base.manager.Player, SlotIndex);
			foreach (SurvivorModel survivor in base.manager.CombatModel.Survivors)
			{
				if (survivor.IdForAnalytics == SurvivorId)
				{
					AttachedSurvivor = survivor;
					break;
				}
			}
			supportExecution = CreateSupportExecution();
		}

		public override bool IsValid()
		{
			return SupportModel.IsValid();
		}

		private ISupportExecution CreateSupportExecution()
		{
			if (SupportModel == null)
			{
				return null;
			}
			return SupportModel.SupportId switch
			{
				"Shiva" => new ShivaSupportExecution(), 
				"Dog" => new DogSupportExecution(), 
				"WhisperersMask" => new WhisperersMaskSupportExecution(), 
				"CommonwealthArmor" => new CommonwealthArmorSupportExecution(), 
				"RainbowCat" => new RainbowCatSupportExecution(), 
				"Hwacha" => new HwachaSupportExecution(), 
				"CarolsCookies" => new CarolsCookiesSupportExecution(), 
				"WalkerMike" => new WalkerMikeSupportExecution(), 
				"Badge" => new BadgeSupportExecution(), 
				"Pasta" => new PastaSupportExecution(), 
				"Notebook" => new NotebookSupportExecution(), 
				"Cap" => new CapSupportExecution(), 
				_ => null, 
			};
		}

		public void Execute(GridCoordinate target, out ICollection<ActorModel> affectedTargets)
		{
			foreach (ModelAction item in supportExecution.Execute(SupportModel, AttachedSurvivor, target, out affectedTargets))
			{
				base.manager.ExecuteAction(item);
			}
			foreach (ActorModel affectedTarget in affectedTargets)
			{
				AffectedTargets.Add(affectedTarget.ModelId);
			}
			int turnCount = base.manager.CombatModel.TurnManager.TurnCount;
			NextUsableTurn = turnCount + RealCooldown;
			NextInnerUsableTurn = turnCount + SupportModel.InnerCooldown;
			UsedTurns.Add(turnCount);
			int num = usedCount + 1;
			usedCount = num;
			this.Executed?.Invoke(SupportModel);
		}

		public bool CanExecute(GridCoordinate target)
		{
			return supportExecution.CanExecute(SupportModel, AttachedSurvivor, target);
		}

		public ICollection<ActorModel> GetTargets(GridCoordinate target)
		{
			return supportExecution.GetTargets(SupportModel, AttachedSurvivor, target);
		}
	}
}
