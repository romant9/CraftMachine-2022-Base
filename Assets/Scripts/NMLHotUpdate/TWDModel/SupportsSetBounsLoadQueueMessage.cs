using System.Collections.Generic;

namespace TWDModel
{
	public class SupportsSetBounsLoadQueueMessage : SupportLoadQueueMessage
	{
		public List<SupportSetBounsEntry> Bouns { get; set; }

		public override bool Execute(TWDModelManager manager)
		{
			foreach (SupportSetBounsEntry boun in Bouns)
			{
				int level = boun.Level;
				BounsModel bounsModel = manager.Player.Equipment.GetBounsModelWithItemId(boun.ItemId);
				if (level > 0)
				{
					if (bounsModel == null)
					{
						bounsModel = new BounsModel(boun.ItemId);
						bounsModel.SetManager(manager);
						bounsModel.Initialize();
						bounsModel.Start();
						manager.Player.Equipment.AddBounsModel(bounsModel);
					}
					bounsModel.SetLevel(level);
				}
				else if (bounsModel != null)
				{
					if (bounsModel.UsingSurvivor != null)
					{
						bounsModel.UsingSurvivor.UnequipBouns(bounsModel);
					}
					manager.Player.Equipment.BounsModes.Remove(bounsModel);
				}
			}
			return true;
		}
	}
}
