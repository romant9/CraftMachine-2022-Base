namespace TWDModel
{
	public class TraitAbilityModel : AbilityModel
	{
		public string TraitDefinitionIdentifier { get; private set; }

		public TraitAbilityModel()
		{
		}

		public TraitAbilityModel(string traitDefinitionIdentifier)
		{
			TraitDefinitionIdentifier = traitDefinitionIdentifier;
		}
	}
}
