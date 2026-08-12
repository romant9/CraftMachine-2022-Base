using System.Collections.Generic;
using Newtonsoft.Json;

namespace TWDModel
{
	public class LoginServerReportSurvivor
	{
		[JsonIgnore]
		public static readonly List<string> ReportSurvivorIds = new List<string>
		{
			"Hero_Aaron", "Hero_Abraham", "Hero_Alpha", "Hero_AssassinCarol", "Hero_Beta", "Hero_Beth", "Hero_BruiserGlenn", "Hero_BruiserRosita", "Hero_Carl", "Hero_Carol",
			"Hero_Connie", "Hero_CowboyNegan", "Hero_Daryl", "Hero_Dwight", "Hero_Eugene", "Hero_Ezekiel", "Hero_Gabriel", "Hero_GauntletAaron", "Hero_Glenn", "Hero_Governor",
			"Hero_HunterHershel", "Hero_HunterMorgan", "Hero_Jadis", "Hero_Jerry", "Hero_Jesus", "Hero_Maggie", "Hero_Magna", "Hero_Mercer", "Hero_Merle", "Hero_Michonne",
			"Hero_Morgan", "Hero_Negan", "Hero_Perlie", "Hero_Princess", "Hero_ProtectorDaryl", "Hero_Quinn", "Hero_Rick", "Hero_Rosita", "Hero_Sasha", "Hero_ScoutDaryl",
			"Hero_ScoutRick", "Hero_Shane", "Hero_ShooterMaggie", "Hero_Simon", "Hero_TalkingDead", "Hero_Tara", "Hero_TDog", "Hero_Tyreese", "Hero_Yumiko", "Hero_Croat",
			"Hero_QuickdrawCarol", "Hero_Lydia"
		};
	}
}
