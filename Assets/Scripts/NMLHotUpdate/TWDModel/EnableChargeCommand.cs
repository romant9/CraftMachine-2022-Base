using BaseModel;

namespace TWDModel
{
	public class EnableChargeCommand : ModelCommand
	{
		public bool ChargeEnabled;

		public EnableChargeCommand()
		{
		}

		public EnableChargeCommand(ActorModel actor, bool enabled)
			: base(actor)
		{
			ChargeEnabled = enabled;
		}

		public override IModelCommandRespond Execute(ModelManager manager)
		{
			ActorModel model = manager.GetModel<ActorModel>(base.ModelId);
			if (model == null)
			{
				return new NGModelCommandRespond(this, TWDModelResult.ModelObjectNotFound);
			}
			model.ChargeMeter.ChargeEnabled = ChargeEnabled;
			return new NGModelCommandRespond(this, TWDModelResult.OK);
		}
	}
}
