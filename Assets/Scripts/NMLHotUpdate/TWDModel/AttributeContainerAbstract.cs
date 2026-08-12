namespace TWDModel
{
	public abstract class AttributeContainerAbstract : TWDModelObject
	{
		public override void Start()
		{
			base.Start();
			RegisterAttributeTypes();
		}

		public abstract void RegisterAttributeTypes();

		public override bool IsValid()
		{
			return true;
		}
	}
}
