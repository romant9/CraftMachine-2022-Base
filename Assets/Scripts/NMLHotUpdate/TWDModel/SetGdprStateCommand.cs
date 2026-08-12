using BaseModel;

namespace TWDModel
{
	public class SetGdprStateCommand : ModelCommand
	{
		public string Key { get; set; }

		public bool Accepted { get; set; }

		public long Timestamp { get; set; }

		public SetGdprStateCommand()
		{
		}

		public SetGdprStateCommand(string key, bool accepted, long timeStamp)
		{
			Key = key;
			Accepted = accepted;
			Timestamp = timeStamp;
		}

		public override IModelCommandRespond Execute(ModelManager manager)
		{
			(manager as TWDModelManager).Player.SetGdprAction(Key, new TimestampedActionResult
			{
				Timestamp = Timestamp,
				ActionTaken = true,
				Accepted = Accepted
			});
			return new NGModelCommandRespond(this, TWDModelResult.OK);
		}
	}
}
