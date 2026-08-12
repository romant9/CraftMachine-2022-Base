using System.Collections.Generic;
using BaseModel;

namespace TWDModel
{
	public class FreeOWResetCommand : ModelCommand
	{
		public List<int> TargetActorIdList { get; private set; }

		public FreeOWResetCommand()
		{
		}

		public FreeOWResetCommand(ActorModel actor, List<int> targetActorIdList)
			: base(actor)
		{
			TargetActorIdList = targetActorIdList;
		}

		public override IModelCommandRespond Execute(ModelManager manager)
		{
			foreach (int targetActorId in TargetActorIdList)
			{
				manager.GetModel<ActorModel>(targetActorId).ResetFreeOW();
			}
			return new NGModelCommandRespond(this, TWDModelResult.OK);
		}
	}
}
