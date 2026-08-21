using BaseModel;

namespace TWDModel
{
	public class CommandSkillModelManager : TWDModelObject, IDestructibleCombatModel
	{
		[IgnoreModelProperty]
		public ActorModel OwnActorModel { get; private set; }

		public ModelList<BaseCommandSkill> CommandSkills { get; private set; }

		public BaseCommandSkill ActorCommandSkill { get; private set; }

		public CommandSkillModelManager()
		{
		}

		public CommandSkillModelManager(CommandSkillModelManager backupCommandSkillModelManager)
		{
			OwnActorModel = backupCommandSkillModelManager.OwnActorModel;
		}

		public void BackupCommandSkills(ModelList<BaseCommandSkill> backupCommandSkills)
		{
			CommandSkills = backupCommandSkills;
		}

		public void BackActorCommandSkill(BaseCommandSkill backActorCommandSkill)
		{
			ActorCommandSkill = backActorCommandSkill;
		}

		public void CreateCommandSkill(CommandSkillDefinition skillDefinition)
		{
			if (CommandSkills == null)
			{
				CommandSkills = new ModelList<BaseCommandSkill>();
				CommandSkills.Initialize();
				CommandSkills.SetManager(base.manager);
			}
			BaseCommandSkill baseCommandSkill = null;
			switch (skillDefinition.Type)
			{
			case CommandSkillType.CommandSkillHealDamage:
				baseCommandSkill = new HealDamageSkill(skillDefinition.GetParameter<FixedPoint>(0) / 100.0, skillDefinition.GetParameter<int>(1), skillDefinition.GetParameter<int>(2), skillDefinition.GetParameter<FixedPoint>(3) / 100.0, skillDefinition.GetParameter<int>(4));
				break;
			case CommandSkillType.CommandSkillHealMaxHealth:
				baseCommandSkill = new HealMaxHealthSkill(skillDefinition.GetParameter<FixedPoint>(0) / 100.0, skillDefinition.GetParameter<int>(1), skillDefinition.GetParameter<int>(2), skillDefinition.GetParameter<FixedPoint>(3) / 100.0, skillDefinition.GetParameter<int>(4));
				break;
			case CommandSkillType.CommandSkillHealTargetHealth:
				baseCommandSkill = new HealTargetHealthSkill(skillDefinition.GetParameter<FixedPoint>(0) / 100.0, skillDefinition.GetParameter<int>(1), skillDefinition.GetParameter<int>(2), skillDefinition.GetParameter<FixedPoint>(3) / 100.0, skillDefinition.GetParameter<int>(4));
				break;
			case CommandSkillType.CommandSkillHealTargetMaxHealth:
				baseCommandSkill = new HealTargetMaxHealthSkill(skillDefinition.GetParameter<FixedPoint>(0) / 100.0, skillDefinition.GetParameter<int>(1), skillDefinition.GetParameter<int>(2), skillDefinition.GetParameter<FixedPoint>(3) / 100.0, skillDefinition.GetParameter<int>(4));
				break;
			case CommandSkillType.CommandSkillHealTargetLossHealth:
				baseCommandSkill = new HealTargetLossHealthSkill(skillDefinition.GetParameter<FixedPoint>(0) / 100.0, skillDefinition.GetParameter<int>(1), skillDefinition.GetParameter<int>(2), skillDefinition.GetParameter<FixedPoint>(3) / 100.0, skillDefinition.GetParameter<int>(4));
				break;
			case CommandSkillType.CommandSkillAdrenaline:
				baseCommandSkill = new AdrenalineSkill(skillDefinition.GetParameter<FixedPoint>(0) / 100.0);
				break;
			case CommandSkillType.CommandSkillShieldType1:
				baseCommandSkill = new ShieldType1Skill(skillDefinition.GetParameter<FixedPoint>(0) / 100.0, skillDefinition.GetParameter<int>(1), skillDefinition.GetParameter<int>(2), skillDefinition.GetParameter<int>(3));
				break;
			case CommandSkillType.CommandSkillIncreaseAttack:
				baseCommandSkill = new IncreaseAttackSkill(skillDefinition.GetParameter<FixedPoint>(0) / 100.0, skillDefinition.GetParameter<FixedPoint>(1) / 100.0, skillDefinition.GetParameter<int>(2));
				break;
			case CommandSkillType.CommandSkillGodWar:
				baseCommandSkill = new GodWarSkill(skillDefinition.GetParameter<string>(0), skillDefinition.GetParameter<FixedPoint>(1) / 100.0, skillDefinition.GetParameter<int>(2), skillDefinition.GetParameter<FixedPoint>(3) / 100.0, skillDefinition.GetParameter<int>(4));
				break;
			case CommandSkillType.CommandSkillEquipTaunt:
				baseCommandSkill = new EquipTauntSkill(skillDefinition.GetParameter<FixedPoint>(0) / 100.0, skillDefinition.GetParameter<int>(1), skillDefinition.GetParameter<int>(2));
				break;
			case CommandSkillType.CommandSkillBerserker:
				baseCommandSkill = new BerserkerSkill(skillDefinition.GetParameter<int>(0), skillDefinition.GetParameter<int>(1), skillDefinition.GetParameter<int>(2), skillDefinition.GetParameter<FixedPoint>(3) / 100.0);
				break;
			case CommandSkillType.CommandSkillGuardianVow:
				baseCommandSkill = new GuardianVowSkill(skillDefinition.GetParameter<FixedPoint>(0), skillDefinition.GetParameter<int>(1), skillDefinition.GetParameter<int>(2), skillDefinition.GetParameter<FixedPoint>(3) / 100.0, skillDefinition.GetParameter<int>(4), skillDefinition.GetParameter<int>(5), skillDefinition.GetParameter<FixedPoint>(6) / 100.0, skillDefinition.GetParameter<FixedPoint>(7) / 100.0);
				break;
			case CommandSkillType.CommandSkillDelayedActionGrenade:
				baseCommandSkill = new DelayedActionGrenadeSkill(skillDefinition.GetParameter<int>(0), skillDefinition.GetParameter<int>(1), skillDefinition.GetParameter<FixedPoint>(2) / 100.0, skillDefinition.GetParameter<FixedPoint>(3) / 100.0, skillDefinition.GetParameter<FixedPoint>(4) / 100.0, skillDefinition.GetParameter<FixedPoint>(5) / 100.0, skillDefinition.GetParameter<int>(6), skillDefinition.GetParameter<FixedPoint>(7) / 100.0, skillDefinition.SelfTraitsApply, skillDefinition.TargetTraitsApply);
				break;
			case CommandSkillType.CommandSkillAbilityRangeTrident:
				baseCommandSkill = new AbilityRangeTridentSkill(skillDefinition.GetParameter<int>(0), skillDefinition.GetParameter<int>(1), skillDefinition.GetParameter<int>(2), skillDefinition.GetParameter<int>(3), skillDefinition.GetParameter<int>(4), skillDefinition.GetParameter<int>(5), skillDefinition.GetParameter<int>(6));
				break;
			case CommandSkillType.CommandSkillFortifications:
				baseCommandSkill = new FortificationsSkill(skillDefinition.GetParameter<int>(0));
				break;
			case CommandSkillType.CommandSkillFortificationsRemove:
				baseCommandSkill = new FortificationsRemoveSkill(skillDefinition.GetParameter<int>(0), skillDefinition.GetParameter<FixedPoint>(1) / 100.0);
				break;
			}
			if (baseCommandSkill != null)
			{
				baseCommandSkill.SetSkillGEDParameter(skillDefinition.ID);
				baseCommandSkill.SetOwnActor(OwnActorModel);
				baseCommandSkill.Initialize();
				baseCommandSkill.SetManager(base.manager);
				baseCommandSkill.Start();
				CommandSkills.Add(baseCommandSkill);
			}
		}

		public void CreateActorCommandSkill(CommandSkillDefinition skillDefinition)
		{
			BaseCommandSkill baseCommandSkill = null;
			switch (skillDefinition.Type)
			{
			case CommandSkillType.CommandSkillSurvivalGame:
				baseCommandSkill = new SurvivalGameSkill();
				break;
			case CommandSkillType.CommandSkillShadowedGuard:
				baseCommandSkill = new ShadowedGuardSkill();
				break;
			}
			if (baseCommandSkill != null)
			{
				baseCommandSkill.SetSkillGEDParameter(skillDefinition.ID);
				baseCommandSkill.SetOwnActor(OwnActorModel);
				baseCommandSkill.Initialize();
				baseCommandSkill.SetManager(base.manager);
				baseCommandSkill.Start();
				ActorCommandSkill = baseCommandSkill;
			}
		}

		public void ClearCommandSkills()
		{
			bool flag = false;
			if (CommandSkills != null)
			{
				CommandSkills.Clear();
				flag = true;
			}
			if (ActorCommandSkill != null)
			{
				ActorCommandSkill = null;
				flag = true;
			}
			if (flag)
			{
				UpdateModelObjects();
			}
		}

		public T GetCommandSkill<T>(CommandSkillType commandSkillType) where T : BaseCommandSkill
		{
			if (CommandSkills == null || CommandSkills.Count == 0)
			{
				return null;
			}
			return CommandSkills.Find((BaseCommandSkill x) => x.Type == commandSkillType) as T;
		}

		public T GetActorCommandSkill<T>(CommandSkillType commandSkillType) where T : BaseCommandSkill
		{
			if (ActorCommandSkill == null)
			{
				return null;
			}
			if (ActorCommandSkill.Type == commandSkillType)
			{
				return ActorCommandSkill as T;
			}
			return null;
		}

		public void SetOwnActorModel(ActorModel ownActorModel)
		{
			OwnActorModel = ownActorModel;
			if (CommandSkills != null)
			{
				for (int i = 0; i < CommandSkills.Count; i++)
				{
					if (CommandSkills[i] != null)
					{
						CommandSkills[i].SetOwnActor(ownActorModel);
					}
				}
			}
			if (ActorCommandSkill != null)
			{
				ActorCommandSkill.SetOwnActor(ownActorModel);
			}
		}

		public override void Start()
		{
			base.Start();
			TurnManager turnManager = base.manager.CombatModel?.TurnManager;
			if (turnManager != null)
			{
				turnManager.FactionChanged -= OnFactionChanged;
				turnManager.FactionChanged += OnFactionChanged;
			}
		}

		public override void Initialize()
		{
			base.Initialize();
			CommandSkills = new ModelList<BaseCommandSkill>();
			CommandSkills.Initialize();
			CommandSkills.SetManager(base.manager);
		}

		public void SetupForCombat()
		{
			TurnManager turnManager = base.manager.CombatModel?.TurnManager;
			if (turnManager != null)
			{
				turnManager.FactionChanged -= OnFactionChanged;
				turnManager.FactionChanged += OnFactionChanged;
				InitialActorCommandSkills();
				InitialEquipmentCommandSkills();
			}
		}

		private void InitialActorCommandSkills()
		{
			if (OwnActorModel.Definition.CommandSkill != 0)
			{
				CommandSkillDefinition commandSkillDefinition = base.manager.GameEconomyData.GetCommandSkillDefinition(OwnActorModel.Definition.CommandSkill);
				if (commandSkillDefinition != null)
				{
					CreateActorCommandSkill(commandSkillDefinition);
					UpdateModelObjects();
				}
			}
		}

		private void InitialEquipmentCommandSkills()
		{
			EquipmentItemModel equipmentItemModel = null;
			if (OwnActorModel.EquipmentItems.Count == 0)
			{
				return;
			}
			for (int i = 0; i < OwnActorModel.EquipmentItems.Count; i++)
			{
				EquipmentItemModel equipmentItemModel2 = OwnActorModel.EquipmentItems[i];
				if (equipmentItemModel2.IsWeaponEquipment && !equipmentItemModel2.IsChargeEquipment)
				{
					equipmentItemModel = equipmentItemModel2;
					break;
				}
			}
			if (equipmentItemModel != null && equipmentItemModel.Definition != null && equipmentItemModel.Definition.CommandSkills != null && equipmentItemModel.Definition.CommandSkills.Count > 0)
			{
				foreach (int commandSkill in equipmentItemModel.Definition.CommandSkills)
				{
					CommandSkillDefinition commandSkillDefinition = base.manager.GameEconomyData.GetCommandSkillDefinition(commandSkill);
					if (commandSkillDefinition == null)
					{
						return;
					}
					CreateCommandSkill(commandSkillDefinition);
				}
			}
			InitialBreakthroughCommandSkills(equipmentItemModel);
			UpdateModelObjects();
		}

		private void InitialBreakthroughCommandSkills(EquipmentItemModel equipmentItemModel)
		{
			if (equipmentItemModel.Definition.CommandSkillsBreakthroughLv == null || equipmentItemModel.Definition.CommandSkillsBreakthroughLv.Count == 0 || equipmentItemModel.Definition.CommandSkillsBreakthroughLv.Count % 2 != 0)
			{
				return;
			}
			int num;
			for (num = 0; num < equipmentItemModel.Definition.CommandSkillsBreakthroughLv.Count; num++)
			{
				int id = equipmentItemModel.Definition.CommandSkillsBreakthroughLv[num];
				num++;
				int num2 = equipmentItemModel.Definition.CommandSkillsBreakthroughLv[num];
				if (equipmentItemModel.BreakthroughLevel >= num2)
				{
					CommandSkillDefinition commandSkillDefinition = base.manager.GameEconomyData.GetCommandSkillDefinition(id);
					if (commandSkillDefinition != null)
					{
						int count = CommandSkills.Count;
						CreateCommandSkill(commandSkillDefinition);
						if (CommandSkills.Count > count)
						{
							BaseCommandSkill newSkill = CommandSkills[CommandSkills.Count - 1];
							CommandSkills.RemoveAll((BaseCommandSkill s) => s.Type == newSkill.Type && s.SkillID != newSkill.SkillID);
						}
					}
				}
			}
		}

		public void OnFactionChanged(Faction currentFaction, Faction newFaction)
		{
			if (CommandSkills != null && CommandSkills.Count > 0)
			{
				for (int num = CommandSkills.Count - 1; num >= 0; num--)
				{
					if (CommandSkills[num].CooldownLeftTurnCheckFaction == newFaction)
					{
						CommandSkills[num].OnFactionChangeReduceCooldownLeftTurns();
					}
					if (CommandSkills[num] is AbilityRangeTridentSkill { IsActive: not false } abilityRangeTridentSkill && newFaction == Faction.Survivor)
					{
						abilityRangeTridentSkill.TickTurnEndCharge();
					}
				}
			}
			if (ActorCommandSkill != null && ActorCommandSkill.CooldownLeftTurnCheckFaction == newFaction)
			{
				ActorCommandSkill.OnFactionChangeReduceCooldownLeftTurns();
			}
		}

		public void Destroy()
		{
			TurnManager turnManager = base.manager.CombatModel?.TurnManager;
			if (turnManager != null)
			{
				turnManager.FactionChanged -= OnFactionChanged;
			}
		}

		public override bool IsValid()
		{
			return true;
		}
	}
}
