namespace TWDModel
{
	public class ModSkillSlot
	{
		public int Index { get; set; }

		public ModSkillMode ModSkillMode { get; set; }

		public ModSkillSlot()
		{
		}

		public ModSkillSlot(int index)
		{
			Index = index;
			ModSkillMode = null;
		}

		public ModSkillSlot(int index, ModSkillMode mode)
		{
			Index = index;
			ModSkillMode = mode;
		}

		public void Reset()
		{
			ModSkillMode = null;
		}
	}
}
