using System.Collections.Generic;
using BaseModel;

namespace TWDModel
{
	public class EquipmentActiveLightResetCommand : ModelCommand
	{
		public List<int> TargetActorIdList { get; private set; }

		public EquipmentActiveLightResetCommand()
		{
		}

		public EquipmentActiveLightResetCommand(ActorModel actor, List<int> targetActorIdList)
			: base(actor)
		{
			TargetActorIdList = targetActorIdList;
		}

		public override IModelCommandRespond Execute(ModelManager manager)
		{
			foreach (int targetActorId in TargetActorIdList)
			{
				manager.GetModel<ActorModel>(targetActorId).ResetActiveLight();
			}
			return new NGModelCommandRespond(this, TWDModelResult.OK);
		}
	}
}
