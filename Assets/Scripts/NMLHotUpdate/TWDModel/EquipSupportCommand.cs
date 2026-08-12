using BaseModel;

namespace TWDModel
{
	public class EquipSupportCommand : ModelCommand
	{
		public int index;

		public string supportId;

		public EquipSupportCommand(int index, string supportId)
		{
			this.index = index;
			this.supportId = supportId;
		}

		public EquipSupportCommand()
		{
		}

		public override IModelCommandRespond Execute(ModelManager manager)
		{
			TWDModelResult result = TWDModelResult.Error;
			if (manager is TWDModelManager { Player: var player })
			{
				SupportModel supportModel = player.GetSupportModel(supportId);
				if (supportModel != null)
				{
					if (supportModel.Unlocked)
					{
						int equippedSupportIndex = player.GetEquippedSupportIndex(supportId);
						if (equippedSupportIndex >= 0)
						{
							player.EquippedSupportIds[equippedSupportIndex] = player.EquippedSupportIds[index];
						}
						player.EquippedSupportIds[index] = supportId;
						result = TWDModelResult.OK;
					}
				}
				else
				{
					player.EquippedSupportIds[index] = null;
					result = TWDModelResult.OK;
				}
			}
			return new NGModelCommandRespond(this, result);
		}
	}
}
