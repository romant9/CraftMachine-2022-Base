using System.Collections.Generic;

namespace TWDModel
{
	public class Migration171 : TWDModelMigration
	{
		public Migration171()
		{
			base.Version = "1.7.1";
		}

		public override bool Migrate(PlayerModel player, TWDModelManager manager)
		{
			if (player.DefenseOutpostVisitLog == null)
			{
				player.DefenseOutpostVisitLog = new List<OutpostVisitEntry>();
			}
			if (player.AttackOutpostVisitLog == null)
			{
				player.AttackOutpostVisitLog = new List<OutpostVisitEntry>();
			}
			if (player.SurvivorContainer != null)
			{
				foreach (SurvivorModel survivor in player.SurvivorContainer.Survivors)
				{
					if (survivor.MissionFailCondition != MissionFailCondition.None)
					{
						survivor.MissionFailCondition = MissionFailCondition.None;
					}
				}
			}
			return true;
		}
	}
}
