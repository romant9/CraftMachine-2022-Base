using System.Collections.Generic;
using BaseModel;

namespace TWDModel
{
	public class GvgMarkUnseenBattleSignUpRemove : ModelCommand
	{
		public override IModelCommandRespond Execute(ModelManager manager)
		{
			TWDModelManager tWDModelManager = (TWDModelManager)manager;
			if (tWDModelManager == null)
			{
				return new NGModelCommandRespond(this, TWDModelResult.Error);
			}
			GvGSeasonModelPlayer gvGSeasonModelPlayer = tWDModelManager.Player.GvGSeasonModelPlayer;
			if (gvGSeasonModelPlayer == null)
			{
				return new NGModelCommandRespond(this, TWDModelResult.Error);
			}
			List<long> list = FindRemovedTimeSlots(tWDModelManager);
			if (list.Count > 0)
			{
				foreach (long item in list)
				{
					Metrics metrics = tWDModelManager.Metrics;
					metrics.AddStart();
					metrics.AddGvG();
					metrics.AddBattleSignupKick(item);
					metrics.AddKick();
					metrics.Send();
					if (gvGSeasonModelPlayer.GuildWarModelPlayer.RegisteredBattleSlots.Contains(item))
					{
						gvGSeasonModelPlayer.GuildWarModelPlayer.RegisteredBattleSlots.Remove(item);
					}
				}
			}
			return new NGModelCommandRespond(this, TWDModelResult.OK);
		}

		private List<long> FindRemovedTimeSlots(TWDModelManager modelManager)
		{
			GvGSeasonModelPlayer gvGSeasonModelPlayer = modelManager.Player.GvGSeasonModelPlayer;
			List<long> list = new List<long>();
			GuildWarModel guildWarModel = modelManager.Player.GuildModel?.GuildWarModel;
			if (gvGSeasonModelPlayer?.GuildWarModelPlayer?.RegisteredBattleSlots == null || guildWarModel?.RegisteredPlayersForBattleSlot == null)
			{
				return list;
			}
			for (int i = 0; i < gvGSeasonModelPlayer.GuildWarModelPlayer.RegisteredBattleSlots.Count; i++)
			{
				long num = gvGSeasonModelPlayer.GuildWarModelPlayer.RegisteredBattleSlots[i];
				if (!guildWarModel.RegisteredPlayersForBattleSlot.TryGetValue(num, out var value))
				{
					list.Add(num);
				}
				else if (value == null || !value.Contains(modelManager.Player.HashedId))
				{
					list.Add(num);
				}
			}
			return list;
		}
	}
}
