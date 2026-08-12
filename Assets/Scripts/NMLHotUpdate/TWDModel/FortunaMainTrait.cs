using System.Collections.Generic;

namespace TWDModel
{
	public class FortunaMainTrait : ActionModifier
	{
		private int RandomNum;

		private List<int> RandomTalentIds;

		private List<RandomTalentChart> RandomTalentCharts;

		public FortunaMainTrait(int randomNum, List<int> randomTalentIds)
		{
			RandomNum = randomNum;
			RandomTalentIds = randomTalentIds;
		}

		public override ActionListClearFlag VisitActions(ModelAction action, ActorModel actor, List<ModelAction> addedActions)
		{
			CombatModel combatModel = base.manager.CombatModel;
			if (RandomTalentCharts == null)
			{
				RandomTalentCharts = new List<RandomTalentChart>();
				foreach (int randomTalentId in RandomTalentIds)
				{
					RandomTalentChart randomTalentChartById = base.manager.GameEconomyData.GetRandomTalentChartById(randomTalentId);
					RandomTalentCharts.Add(randomTalentChartById);
				}
			}
			if (action is PostChangeTurnAction && combatModel.TurnManager.ActiveFaction == actor.Faction && !actor.IsDead)
			{
				if (actor.FortunaRandomTraitIds.Count > 0)
				{
					foreach (string fortunaRandomTraitId in actor.FortunaRandomTraitIds)
					{
						actor.RemoveTrait(fortunaRandomTraitId);
					}
					actor.FortunaRandomTraitIds.Clear();
				}
				List<RandomTalentChart> list = base.manager.Player.PlayerRandom.WeightedRandomList(RandomTalentCharts, RandomNum, (RandomTalentChart chart) => chart.GetWeight(), isRepeat: false);
				if (list.Count > 0)
				{
					foreach (RandomTalentChart item in list)
					{
						actor.AddTrait(item.TraitsId);
						actor.FortunaRandomTraitIds.Add(item.TraitsId);
						string text = item.TraitsId.Replace("Equipment_Passive_", "");
						actor.NotifyChange("AbilityVisited", new object[2] { text, false });
						actor.NotifyChange("FortunaMainTraitChange");
					}
				}
			}
			return ActionListClearFlag.Keep;
		}
	}
}
