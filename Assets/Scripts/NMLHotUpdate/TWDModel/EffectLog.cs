using System.Collections.Generic;

namespace TWDModel
{
	public class EffectLog
	{
		public string EffectType;

		public bool Success;

		public List<ModifierLog> ModifierLogs;

		public void CreateModifiers()
		{
			if (ModifierLogs == null)
			{
				ModifierLogs = new List<ModifierLog>();
			}
		}

		public override string ToString()
		{
			string text = "\t" + EffectType + "(" + Success + ")";
			if (ModifierLogs != null && ModifierLogs.Count > 0)
			{
				text += "\n";
				for (int i = 0; i < ModifierLogs.Count; i++)
				{
					text = text + "\t\t" + ModifierLogs[i].ToString();
					if (i < ModifierLogs.Count - 1)
					{
						text += "\n";
					}
				}
			}
			return text;
		}
	}
}
