using System;
using System.Collections.Generic;
using BaseModel;

namespace TWDModel
{
	public class DropMagazineAction : ModelAction
	{
		public ActorModel Actor { get; private set; }

		public int DropRadius { get; private set; }

		public int DropCount { get; private set; }

		public int Duration { get; private set; }

		public int MaxMagazinesPerActor { get; private set; }

		public string RequiredTraitIdentifier { get; private set; }

		public DropMagazineAction(ActorModel actor, int dropRadius, int dropCount, int duration, int maxMagazinesPerActor, string requiredTraitIdentifier = null)
			: base(actor)
		{
			Actor = actor;
			DropRadius = dropRadius;
			DropCount = dropCount;
			Duration = duration;
			MaxMagazinesPerActor = maxMagazinesPerActor;
			RequiredTraitIdentifier = requiredTraitIdentifier;
		}

		public override bool Execute(ModelManager manager)
		{
			if (Actor == null || Actor.IsDead)
			{
				return false;
			}
			if (!(manager is TWDModelManager { CombatModel: var combatModel } tWDModelManager))
			{
				return false;
			}
			if (combatModel == null)
			{
				return false;
			}
			MagazineAreasManager magazineAreasManager = combatModel.GetModel<MagazineAreasManager>();
			if (magazineAreasManager == null)
			{
				magazineAreasManager = new MagazineAreasManager();
				magazineAreasManager.SetManager(tWDModelManager);
				combatModel.AddModel(magazineAreasManager);
			}
			int magazineCountByFaction = MagazineAreasManager.GetMagazineCountByFaction(combatModel, Actor.Faction);
			int num = MaxMagazinesPerActor - magazineCountByFaction;
			if (num <= 0)
			{
				return true;
			}
			List<GridCoordinate> emptyGridsAround = MagazineAreasManager.GetEmptyGridsAround(combatModel, Actor.GridCoordinate, DropRadius, Actor, requirePathReachableFromIgnoreActor: true);
			if (emptyGridsAround == null || emptyGridsAround.Count == 0)
			{
				return true;
			}
			int num2 = Math.Min(DropCount, Math.Min(num, emptyGridsAround.Count));
			if (num2 <= 0)
			{
				return true;
			}
			GridCoordinate[] array = emptyGridsAround.ToArray();
			Actor.manager.Player.PlayerRandom.ShuffleArray(array);
			int expiryTurn = combatModel.TurnManager.TurnCount + Duration;
			for (int i = 0; i < num2; i++)
			{
				MagazineArea magazineArea = new MagazineArea(array[i], Actor.Faction, expiryTurn, RequiredTraitIdentifier);
				magazineArea.SetManager(tWDModelManager);
				magazineAreasManager.AddArea(magazineArea);
			}
			combatModel.NotifyChange("MagazineAreasUpdate");
			return num2 > 0;
		}
	}
}
