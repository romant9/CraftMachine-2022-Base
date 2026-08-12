using System.Text.RegularExpressions;
using TWDModel;
using UnityEngine;

public static class ClientUtils
{
	public static readonly GameVersion ClientVersion;

	static ClientUtils()
	{
		Regex regex = new Regex("[0-9]+\\.[0-9]+\\.[0-9]+");
		string text = Application.version;
		Match match = regex.Match(text);
		if (match.Success)
		{
			text = match.Value;
		}
		ClientVersion = new GameVersion(text);
	}
}
