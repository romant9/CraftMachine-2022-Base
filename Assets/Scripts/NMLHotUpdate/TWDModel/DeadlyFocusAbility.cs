using Newtonsoft.Json;

namespace TWDModel
{
	public class DeadlyFocusAbility : AbilityModel
	{
		[JsonIgnore]
		protected override bool BypassTacticalCheck => true;

		public DeadlyFocusAbility(string definitionId)
		{
			base.DefinitionID = definitionId;
		}
	}
}
