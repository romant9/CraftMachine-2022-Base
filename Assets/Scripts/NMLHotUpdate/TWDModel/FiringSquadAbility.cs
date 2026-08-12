using Newtonsoft.Json;

namespace TWDModel
{
	public class FiringSquadAbility : AbilityModel
	{
		[JsonIgnore]
		protected override bool BypassTacticalCheck => true;

		public FiringSquadAbility(string definitionId)
		{
			base.DefinitionID = definitionId;
		}
	}
}
