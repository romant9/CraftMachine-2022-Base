namespace TWDModel
{
	public class GuildModelWrapper : TWDModelObject
	{
		public GuildModel GuildModel { get; set; }

		public GuildModelWrapper()
		{
		}

		public GuildModelWrapper(GuildModel gm)
		{
			GuildModel = gm;
		}

		public override bool IsValid()
		{
			return true;
		}
	}
}
