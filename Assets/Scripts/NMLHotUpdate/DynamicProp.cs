using System.Collections.Generic;
using ThinkingAnalytics;

public class DynamicProp : IDynamicSuperProperties
{
	public Dictionary<string, object> GetDynamicSuperProperties()
	{
		int? num = GameManager.Instance.playerModel?.Level;
		string text = GameManager.Instance.playerModel?.Name;
		string text2 = GameManager.Instance.playerModel?.GuildModel?.Name;
		int? num2 = GameManager.Instance.playerModel?.CouncilLevel;
		string value = "epic";
		return new Dictionary<string, object>
		{
			{
				"level_current",
				num ?? 1
			},
			{
				"player_name",
				text ?? ""
			},
			{
				"guild_name",
				text2 ?? ""
			},
			{
				"council_level",
				num2 ?? 1
			},
			{ "channel", value }
		};
	}
}
