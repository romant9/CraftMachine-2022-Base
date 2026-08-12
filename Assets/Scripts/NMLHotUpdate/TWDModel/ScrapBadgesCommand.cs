using System.Collections.Generic;
using BaseModel;

namespace TWDModel
{
	public class ScrapBadgesCommand : ModelCommand
	{
		public List<int> modelIds { get; set; }

		public override IModelCommandRespond Execute(ModelManager manager)
		{
			PlayerModel playerModel = manager.GetPlayer() as PlayerModel;
			List<BadgeModel> list = new List<BadgeModel>();
			for (int i = 0; i < modelIds.Count; i++)
			{
				BadgeModel model = manager.GetModel<BadgeModel>(modelIds[i]);
				list.Add(model);
			}
			bool flag = true;
			for (int j = 0; j < list.Count; j++)
			{
				BadgeModel badge = list[j];
				flag = flag && playerModel.Equipment.ScrapBadge(badge) == TWDModelResult.OK;
			}
			TWDModelResult result = ((!flag) ? TWDModelResult.Error : TWDModelResult.OK);
			return new NGModelCommandRespond(this, result);
		}
	}
}
