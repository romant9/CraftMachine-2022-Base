using System;

namespace TWDModel
{
	public class GameVersion : IComparable<GameVersion>
	{
		public int Major;

		public int Minor;

		public int Patch;

		public string Label;

		public static char VersionSeparator = '.';

		public static char LabelSeparator = '-';

		public GameVersion(string version)
		{
			if (!string.IsNullOrEmpty(version))
			{
				int num = version.IndexOf(LabelSeparator);
				if (num != -1)
				{
					Label = version.Substring(num + 1);
					version = version.Remove(num);
				}
				string[] array = version.Split(VersionSeparator);
				if (array.Length >= 1)
				{
					int.TryParse(array[0], out Major);
				}
				if (array.Length >= 2)
				{
					int.TryParse(array[1], out Minor);
				}
				if (array.Length >= 3)
				{
					int.TryParse(array[2], out Patch);
				}
			}
		}

		public override string ToString()
		{
			return Major + "." + Minor + "." + Patch;
		}

		public int CompareTo(GameVersion other)
		{
			if (Major > other.Major)
			{
				return 1;
			}
			if (Major < other.Major)
			{
				return -1;
			}
			if (Minor > other.Minor)
			{
				return 1;
			}
			if (Minor < other.Minor)
			{
				return -1;
			}
			if (Patch > other.Patch)
			{
				return 1;
			}
			if (Patch < other.Patch)
			{
				return -1;
			}
			return 0;
		}
	}
}
