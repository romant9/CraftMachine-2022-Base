using System.Collections.Generic;

namespace TWDModel
{
	public sealed class GodWarSkill : BaseCommandSkill
	{
		public string Parameter0 { get; private set; }

		public FixedPoint Parameter1 { get; private set; }

		public int Parameter2 { get; private set; }

		public FixedPoint Parameter3 { get; private set; }

		public int Parameter4 { get; private set; }

		public override CommandSkillType Type => CommandSkillType.CommandSkillGodWar;

		public GodWarSkill()
		{
		}

		public GodWarSkill(GodWarSkill skill)
			: base(skill)
		{
			Parameter0 = skill.Parameter0;
			Parameter1 = skill.Parameter1;
			Parameter2 = skill.Parameter2;
			Parameter3 = skill.Parameter3;
			Parameter4 = skill.Parameter4;
		}

		public GodWarSkill(string parameter0, FixedPoint parameter1, int parameter2, FixedPoint parameter3, int parameter4)
		{
			Parameter0 = parameter0;
			Parameter1 = parameter1;
			Parameter2 = parameter2;
			Parameter3 = parameter3;
			Parameter4 = parameter4;
		}

		public override void OnExecute(GridCoordinate targetCell)
		{
			ActorModel occupier = base.manager.CombatModel.GetOccupier(targetCell);
			if (occupier != null)
			{
				occupier.NotifyChange("AbilityVisited", new object[2] { "GodWarSkill", false });
				occupier.GodWarTraitTurns = Parameter4;
				List<TraitEntry> traitsThatContain = occupier.GetTraitsThatContain("GodWarBless");
				for (int i = 0; i < traitsThatContain.Count; i++)
				{
					occupier.RemoveTrait(traitsThatContain[i].TraitIdentifier);
				}
				occupier.AddTemporaryTrait(Parameter0, default(FixedPoint), null, 0L);
				occupier.NotifyChange("GodWarSkillChange");
			}
		}
	}
}
