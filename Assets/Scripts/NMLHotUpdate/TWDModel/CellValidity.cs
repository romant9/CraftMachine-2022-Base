namespace TWDModel
{
	public struct CellValidity
	{
		private CellStatus status;

		private InteractiveObjectModel interactiveObject;

		private ActorModel target;

		public bool Valid
		{
			get
			{
				if (Status != CellStatus.Invalid && Status != CellStatus.Friendly)
				{
					return Status != CellStatus.FriendlyExtended;
				}
				return false;
			}
		}

		public CellStatus Status => status;

		public InteractiveObjectModel InteractiveObject => interactiveObject;

		public ActorModel Target => target;

		public CellValidity(CellStatus status, InteractiveObjectModel interactiveObject, ActorModel target)
		{
			this.status = status;
			this.interactiveObject = interactiveObject;
			this.target = target;
		}

		public override string ToString()
		{
			return "Status = " + status.ToString() + " InteractiveObject = " + interactiveObject?.ToString() + " Actor = " + ((target != null) ? target.Name : "null");
		}
	}
}
