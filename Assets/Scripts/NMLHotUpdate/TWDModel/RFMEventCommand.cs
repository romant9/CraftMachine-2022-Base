using BaseModel;

namespace TWDModel
{
	public class RFMEventCommand : ModelCommand
	{
		public RFMEvent RFMEvent { get; set; }

		public RFMEventCommand()
		{
		}

		public RFMEventCommand(RFMEvent rfmevent)
		{
			RFMEvent = rfmevent;
		}

		public override IModelCommandRespond Execute(ModelManager manager)
		{
			if (!(manager is TWDModelManager { Player: not null } tWDModelManager))
			{
				return new NGModelCommandRespond(this, TWDModelResult.Error);
			}
			tWDModelManager.Player.RFMGiftManager.TriggerRFMEvent(RFMEvent);
			return new NGModelCommandRespond(this, TWDModelResult.OK);
		}
	}
}
