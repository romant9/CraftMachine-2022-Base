namespace TWDModel
{
	public class Migration7140 : TWDModelMigration
	{
		public Migration7140()
		{
			base.Version = "7.14.0";
		}

		public override bool Migrate(PlayerModel player, TWDModelManager manager)
		{
			bool flag = false;
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.ShooterStar) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.ShooterStar);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.HunterStar) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.HunterStar);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.BruiserStar) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.BruiserStar);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.WarriorStar) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.WarriorStar);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.ScoutStar) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.ScoutStar);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.AssaultStar) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.AssaultStar);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.CBPWarrior) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.CBPWarrior);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.CBPScout) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.CBPScout);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.CBPBruiser) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.CBPBruiser);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.CBPShooter) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.CBPShooter);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.CBPHunter) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.CBPHunter);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.CBPAssault) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.CBPAssault);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Shooter_1001) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Shooter_1001);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Shooter_1002) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Shooter_1002);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Shooter_1003) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Shooter_1003);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Shooter_1004) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Shooter_1004);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Shooter_1005) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Shooter_1005);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Shooter_1006) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Shooter_1006);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Shooter_1007) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Shooter_1007);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Shooter_1008) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Shooter_1008);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Shooter_1009) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Shooter_1009);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Shooter_1010) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Shooter_1010);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Shooter_1011) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Shooter_1011);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Shooter_1012) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Shooter_1012);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Shooter_1013) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Shooter_1013);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Shooter_1014) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Shooter_1014);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Shooter_1015) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Shooter_1015);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Shooter_1016) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Shooter_1016);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Shooter_1017) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Shooter_1017);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Shooter_1018) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Shooter_1018);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Shooter_1019) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Shooter_1019);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Shooter_1020) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Shooter_1020);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Shooter_1021) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Shooter_1021);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Shooter_2001) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Shooter_2001);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Shooter_2002) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Shooter_2002);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Shooter_2003) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Shooter_2003);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Shooter_2004) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Shooter_2004);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Shooter_2005) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Shooter_2005);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Shooter_2006) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Shooter_2006);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Shooter_2007) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Shooter_2007);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Shooter_2008) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Shooter_2008);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Shooter_2009) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Shooter_2009);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Shooter_2010) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Shooter_2010);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Shooter_2011) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Shooter_2011);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Shooter_2012) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Shooter_2012);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Shooter_2013) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Shooter_2013);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Shooter_2014) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Shooter_2014);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Shooter_2015) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Shooter_2015);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Shooter_2016) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Shooter_2016);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Shooter_2017) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Shooter_2017);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Shooter_2018) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Shooter_2018);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Shooter_2019) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Shooter_2019);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Shooter_2020) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Shooter_2020);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Shooter_2021) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Shooter_2021);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Shooter_2022) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Shooter_2022);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Shooter_2023) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Shooter_2023);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Shooter_2024) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Shooter_2024);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Shooter_2025) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Shooter_2025);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Shooter_2026) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Shooter_2026);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Shooter_2027) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Shooter_2027);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Shooter_2028) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Shooter_2028);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Shooter_2029) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Shooter_2029);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Shooter_2030) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Shooter_2030);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Shooter_2031) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Shooter_2031);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Shooter_2032) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Shooter_2032);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Shooter_2033) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Shooter_2033);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Shooter_2034) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Shooter_2034);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Shooter_2035) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Shooter_2035);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Shooter_2036) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Shooter_2036);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Shooter_2037) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Shooter_2037);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Shooter_2038) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Shooter_2038);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Shooter_2039) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Shooter_2039);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Shooter_2040) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Shooter_2040);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Shooter_2041) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Shooter_2041);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Shooter_3001) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Shooter_3001);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Shooter_3002) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Shooter_3002);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Shooter_3003) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Shooter_3003);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Shooter_3004) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Shooter_3004);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Shooter_3005) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Shooter_3005);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Shooter_3006) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Shooter_3006);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Shooter_3007) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Shooter_3007);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Shooter_3008) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Shooter_3008);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Shooter_3009) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Shooter_3009);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Shooter_3010) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Shooter_3010);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Shooter_3011) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Shooter_3011);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Shooter_3012) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Shooter_3012);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Shooter_3013) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Shooter_3013);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Shooter_3014) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Shooter_3014);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Shooter_3015) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Shooter_3015);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Shooter_3016) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Shooter_3016);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Shooter_3017) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Shooter_3017);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Shooter_3018) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Shooter_3018);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Shooter_3019) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Shooter_3019);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Shooter_3020) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Shooter_3020);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Shooter_3021) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Shooter_3021);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Shooter_3022) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Shooter_3022);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Shooter_3023) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Shooter_3023);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Shooter_3024) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Shooter_3024);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Shooter_3025) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Shooter_3025);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Shooter_3026) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Shooter_3026);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Shooter_3027) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Shooter_3027);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Shooter_3028) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Shooter_3028);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Shooter_3029) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Shooter_3029);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Shooter_3030) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Shooter_3030);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Shooter_3031) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Shooter_3031);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Shooter_3032) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Shooter_3032);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Shooter_3033) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Shooter_3033);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Shooter_3034) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Shooter_3034);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Shooter_3035) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Shooter_3035);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Shooter_3036) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Shooter_3036);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Shooter_3037) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Shooter_3037);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Shooter_3038) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Shooter_3038);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Shooter_3039) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Shooter_3039);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Shooter_3040) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Shooter_3040);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Shooter_3041) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Shooter_3041);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Shooter_3042) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Shooter_3042);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Shooter_3043) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Shooter_3043);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Shooter_3044) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Shooter_3044);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Shooter_3045) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Shooter_3045);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Shooter_3046) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Shooter_3046);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Shooter_3047) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Shooter_3047);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Shooter_4001) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Shooter_4001);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Shooter_4002) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Shooter_4002);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Shooter_4003) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Shooter_4003);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Shooter_4004) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Shooter_4004);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Shooter_4005) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Shooter_4005);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Shooter_4006) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Shooter_4006);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Shooter_4008) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Shooter_4008);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Shooter_4009) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Shooter_4009);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Shooter_4010) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Shooter_4010);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Shooter_4011) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Shooter_4011);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Shooter_4012) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Shooter_4012);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Shooter_4013) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Shooter_4013);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Shooter_4014) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Shooter_4014);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Shooter_4015) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Shooter_4015);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Shooter_4016) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Shooter_4016);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Scout_1001) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Scout_1001);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Scout_1002) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Scout_1002);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Scout_1003) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Scout_1003);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Scout_1004) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Scout_1004);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Scout_1005) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Scout_1005);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Scout_1006) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Scout_1006);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Scout_1007) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Scout_1007);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Scout_1008) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Scout_1008);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Scout_1009) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Scout_1009);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Scout_1010) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Scout_1010);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Scout_1011) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Scout_1011);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Scout_1012) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Scout_1012);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Scout_1013) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Scout_1013);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Scout_1014) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Scout_1014);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Scout_1015) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Scout_1015);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Scout_1016) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Scout_1016);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Scout_1017) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Scout_1017);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Scout_1018) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Scout_1018);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Scout_1019) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Scout_1019);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Scout_1020) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Scout_1020);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Scout_1021) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Scout_1021);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Scout_2001) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Scout_2001);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Scout_2002) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Scout_2002);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Scout_2003) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Scout_2003);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Scout_2004) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Scout_2004);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Scout_2005) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Scout_2005);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Scout_2006) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Scout_2006);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Scout_2007) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Scout_2007);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Scout_2008) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Scout_2008);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Scout_2009) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Scout_2009);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Scout_2010) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Scout_2010);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Scout_2011) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Scout_2011);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Scout_2012) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Scout_2012);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Scout_2013) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Scout_2013);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Scout_2014) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Scout_2014);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Scout_2015) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Scout_2015);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Scout_2016) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Scout_2016);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Scout_2017) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Scout_2017);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Scout_2018) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Scout_2018);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Scout_2019) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Scout_2019);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Scout_2020) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Scout_2020);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Scout_2021) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Scout_2021);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Scout_2022) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Scout_2022);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Scout_2023) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Scout_2023);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Scout_2024) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Scout_2024);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Scout_2025) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Scout_2025);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Scout_2026) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Scout_2026);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Scout_2027) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Scout_2027);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Scout_2028) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Scout_2028);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Scout_2029) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Scout_2029);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Scout_2030) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Scout_2030);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Scout_2031) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Scout_2031);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Scout_2032) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Scout_2032);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Scout_2033) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Scout_2033);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Scout_2034) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Scout_2034);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Scout_2035) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Scout_2035);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Scout_2036) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Scout_2036);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Scout_2037) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Scout_2037);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Scout_2038) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Scout_2038);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Scout_2039) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Scout_2039);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Scout_2040) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Scout_2040);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Scout_2041) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Scout_2041);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Scout_3001) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Scout_3001);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Scout_3002) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Scout_3002);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Scout_3003) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Scout_3003);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Scout_3004) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Scout_3004);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Scout_3005) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Scout_3005);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Scout_3006) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Scout_3006);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Scout_3007) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Scout_3007);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Scout_3008) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Scout_3008);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Scout_3009) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Scout_3009);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Scout_3010) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Scout_3010);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Scout_3011) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Scout_3011);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Scout_3012) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Scout_3012);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Scout_3013) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Scout_3013);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Scout_3014) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Scout_3014);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Scout_3015) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Scout_3015);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Scout_3016) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Scout_3016);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Scout_3017) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Scout_3017);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Scout_3018) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Scout_3018);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Scout_3019) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Scout_3019);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Scout_3020) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Scout_3020);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Scout_3021) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Scout_3021);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Scout_3022) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Scout_3022);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Scout_3023) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Scout_3023);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Scout_3024) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Scout_3024);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Scout_3025) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Scout_3025);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Scout_3026) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Scout_3026);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Scout_3027) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Scout_3027);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Scout_3028) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Scout_3028);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Scout_3029) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Scout_3029);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Scout_3030) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Scout_3030);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Scout_3031) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Scout_3031);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Scout_3032) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Scout_3032);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Scout_3033) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Scout_3033);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Scout_3034) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Scout_3034);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Scout_3035) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Scout_3035);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Scout_3036) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Scout_3036);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Scout_3037) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Scout_3037);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Scout_3038) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Scout_3038);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Scout_3039) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Scout_3039);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Scout_3040) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Scout_3040);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Scout_3041) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Scout_3041);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Scout_3042) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Scout_3042);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Scout_3043) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Scout_3043);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Scout_3044) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Scout_3044);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Scout_3045) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Scout_3045);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Scout_3046) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Scout_3046);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Scout_3047) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Scout_3047);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Scout_4001) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Scout_4001);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Scout_4002) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Scout_4002);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Scout_4003) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Scout_4003);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Scout_4004) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Scout_4004);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Scout_4005) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Scout_4005);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Scout_4006) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Scout_4006);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Scout_4008) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Scout_4008);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Scout_4009) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Scout_4009);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Scout_4010) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Scout_4010);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Scout_4011) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Scout_4011);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Scout_4012) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Scout_4012);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Scout_4013) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Scout_4013);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Scout_4014) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Scout_4014);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Scout_4015) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Scout_4015);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Scout_4016) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Scout_4016);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Hunter_1001) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Hunter_1001);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Hunter_1002) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Hunter_1002);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Hunter_1003) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Hunter_1003);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Hunter_1004) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Hunter_1004);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Hunter_1005) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Hunter_1005);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Hunter_1006) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Hunter_1006);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Hunter_1007) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Hunter_1007);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Hunter_1008) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Hunter_1008);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Hunter_1009) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Hunter_1009);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Hunter_1010) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Hunter_1010);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Hunter_1011) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Hunter_1011);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Hunter_1012) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Hunter_1012);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Hunter_1013) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Hunter_1013);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Hunter_1014) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Hunter_1014);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Hunter_1015) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Hunter_1015);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Hunter_1016) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Hunter_1016);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Hunter_1017) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Hunter_1017);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Hunter_1018) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Hunter_1018);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Hunter_1019) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Hunter_1019);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Hunter_1020) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Hunter_1020);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Hunter_1021) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Hunter_1021);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Hunter_2001) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Hunter_2001);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Hunter_2002) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Hunter_2002);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Hunter_2003) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Hunter_2003);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Hunter_2004) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Hunter_2004);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Hunter_2005) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Hunter_2005);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Hunter_2006) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Hunter_2006);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Hunter_2007) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Hunter_2007);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Hunter_2008) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Hunter_2008);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Hunter_2009) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Hunter_2009);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Hunter_2010) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Hunter_2010);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Hunter_2011) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Hunter_2011);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Hunter_2012) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Hunter_2012);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Hunter_2013) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Hunter_2013);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Hunter_2014) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Hunter_2014);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Hunter_2015) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Hunter_2015);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Hunter_2016) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Hunter_2016);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Hunter_2017) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Hunter_2017);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Hunter_2018) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Hunter_2018);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Hunter_2019) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Hunter_2019);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Hunter_2020) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Hunter_2020);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Hunter_2021) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Hunter_2021);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Hunter_2022) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Hunter_2022);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Hunter_2023) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Hunter_2023);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Hunter_2024) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Hunter_2024);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Hunter_2025) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Hunter_2025);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Hunter_2026) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Hunter_2026);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Hunter_2027) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Hunter_2027);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Hunter_2028) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Hunter_2028);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Hunter_2029) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Hunter_2029);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Hunter_2030) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Hunter_2030);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Hunter_2031) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Hunter_2031);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Hunter_2032) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Hunter_2032);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Hunter_2033) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Hunter_2033);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Hunter_2034) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Hunter_2034);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Hunter_2035) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Hunter_2035);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Hunter_2036) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Hunter_2036);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Hunter_2037) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Hunter_2037);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Hunter_2038) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Hunter_2038);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Hunter_2039) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Hunter_2039);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Hunter_2040) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Hunter_2040);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Hunter_2041) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Hunter_2041);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Hunter_3001) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Hunter_3001);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Hunter_3002) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Hunter_3002);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Hunter_3003) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Hunter_3003);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Hunter_3004) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Hunter_3004);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Hunter_3005) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Hunter_3005);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Hunter_3006) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Hunter_3006);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Hunter_3007) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Hunter_3007);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Hunter_3008) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Hunter_3008);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Hunter_3009) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Hunter_3009);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Hunter_3010) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Hunter_3010);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Hunter_3011) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Hunter_3011);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Hunter_3012) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Hunter_3012);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Hunter_3013) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Hunter_3013);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Hunter_3014) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Hunter_3014);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Hunter_3015) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Hunter_3015);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Hunter_3016) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Hunter_3016);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Hunter_3017) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Hunter_3017);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Hunter_3018) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Hunter_3018);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Hunter_3019) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Hunter_3019);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Hunter_3020) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Hunter_3020);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Hunter_3021) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Hunter_3021);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Hunter_3022) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Hunter_3022);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Hunter_3023) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Hunter_3023);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Hunter_3024) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Hunter_3024);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Hunter_3025) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Hunter_3025);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Hunter_3026) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Hunter_3026);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Hunter_3027) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Hunter_3027);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Hunter_3028) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Hunter_3028);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Hunter_3029) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Hunter_3029);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Hunter_3030) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Hunter_3030);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Hunter_3031) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Hunter_3031);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Hunter_3032) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Hunter_3032);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Hunter_3033) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Hunter_3033);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Hunter_3034) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Hunter_3034);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Hunter_3035) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Hunter_3035);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Hunter_3036) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Hunter_3036);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Hunter_3037) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Hunter_3037);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Hunter_3038) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Hunter_3038);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Hunter_3039) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Hunter_3039);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Hunter_3040) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Hunter_3040);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Hunter_3041) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Hunter_3041);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Hunter_3042) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Hunter_3042);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Hunter_3043) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Hunter_3043);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Hunter_3044) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Hunter_3044);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Hunter_3045) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Hunter_3045);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Hunter_3046) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Hunter_3046);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Hunter_3047) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Hunter_3047);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Hunter_4001) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Hunter_4001);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Hunter_4002) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Hunter_4002);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Hunter_4003) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Hunter_4003);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Hunter_4004) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Hunter_4004);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Hunter_4005) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Hunter_4005);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Hunter_4006) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Hunter_4006);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Hunter_4008) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Hunter_4008);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Hunter_4009) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Hunter_4009);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Hunter_4010) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Hunter_4010);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Hunter_4011) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Hunter_4011);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Hunter_4012) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Hunter_4012);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Hunter_4013) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Hunter_4013);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Hunter_4014) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Hunter_4014);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Hunter_4015) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Hunter_4015);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Hunter_4016) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Hunter_4016);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Warrior_1001) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Warrior_1001);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Warrior_1002) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Warrior_1002);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Warrior_1003) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Warrior_1003);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Warrior_1004) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Warrior_1004);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Warrior_1005) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Warrior_1005);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Warrior_1006) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Warrior_1006);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Warrior_1007) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Warrior_1007);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Warrior_1008) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Warrior_1008);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Warrior_1009) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Warrior_1009);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Warrior_1010) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Warrior_1010);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Warrior_1011) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Warrior_1011);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Warrior_1012) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Warrior_1012);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Warrior_1013) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Warrior_1013);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Warrior_1014) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Warrior_1014);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Warrior_1015) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Warrior_1015);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Warrior_1016) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Warrior_1016);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Warrior_1017) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Warrior_1017);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Warrior_1018) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Warrior_1018);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Warrior_1019) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Warrior_1019);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Warrior_1020) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Warrior_1020);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Warrior_1021) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Warrior_1021);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Warrior_2001) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Warrior_2001);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Warrior_2002) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Warrior_2002);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Warrior_2003) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Warrior_2003);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Warrior_2004) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Warrior_2004);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Warrior_2005) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Warrior_2005);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Warrior_2006) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Warrior_2006);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Warrior_2007) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Warrior_2007);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Warrior_2008) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Warrior_2008);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Warrior_2009) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Warrior_2009);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Warrior_2010) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Warrior_2010);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Warrior_2011) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Warrior_2011);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Warrior_2012) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Warrior_2012);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Warrior_2013) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Warrior_2013);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Warrior_2014) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Warrior_2014);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Warrior_2015) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Warrior_2015);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Warrior_2016) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Warrior_2016);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Warrior_2017) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Warrior_2017);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Warrior_2018) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Warrior_2018);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Warrior_2019) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Warrior_2019);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Warrior_2020) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Warrior_2020);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Warrior_2021) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Warrior_2021);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Warrior_2022) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Warrior_2022);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Warrior_2023) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Warrior_2023);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Warrior_2024) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Warrior_2024);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Warrior_2025) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Warrior_2025);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Warrior_2026) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Warrior_2026);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Warrior_2027) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Warrior_2027);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Warrior_2028) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Warrior_2028);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Warrior_2029) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Warrior_2029);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Warrior_2030) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Warrior_2030);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Warrior_2031) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Warrior_2031);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Warrior_2032) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Warrior_2032);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Warrior_2033) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Warrior_2033);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Warrior_2034) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Warrior_2034);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Warrior_2035) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Warrior_2035);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Warrior_2036) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Warrior_2036);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Warrior_2037) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Warrior_2037);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Warrior_2038) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Warrior_2038);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Warrior_2039) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Warrior_2039);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Warrior_2040) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Warrior_2040);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Warrior_2041) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Warrior_2041);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Warrior_3001) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Warrior_3001);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Warrior_3002) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Warrior_3002);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Warrior_3003) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Warrior_3003);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Warrior_3004) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Warrior_3004);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Warrior_3005) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Warrior_3005);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Warrior_3006) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Warrior_3006);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Warrior_3007) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Warrior_3007);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Warrior_3008) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Warrior_3008);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Warrior_3009) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Warrior_3009);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Warrior_3010) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Warrior_3010);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Warrior_3011) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Warrior_3011);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Warrior_3012) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Warrior_3012);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Warrior_3013) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Warrior_3013);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Warrior_3014) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Warrior_3014);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Warrior_3015) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Warrior_3015);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Warrior_3016) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Warrior_3016);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Warrior_3017) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Warrior_3017);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Warrior_3018) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Warrior_3018);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Warrior_3019) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Warrior_3019);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Warrior_3020) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Warrior_3020);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Warrior_3021) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Warrior_3021);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Warrior_3022) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Warrior_3022);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Warrior_3023) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Warrior_3023);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Warrior_3024) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Warrior_3024);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Warrior_3025) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Warrior_3025);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Warrior_3026) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Warrior_3026);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Warrior_3027) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Warrior_3027);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Warrior_3028) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Warrior_3028);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Warrior_3029) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Warrior_3029);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Warrior_3030) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Warrior_3030);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Warrior_3031) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Warrior_3031);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Warrior_3032) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Warrior_3032);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Warrior_3033) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Warrior_3033);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Warrior_3034) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Warrior_3034);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Warrior_3035) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Warrior_3035);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Warrior_3036) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Warrior_3036);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Warrior_3037) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Warrior_3037);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Warrior_3038) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Warrior_3038);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Warrior_3039) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Warrior_3039);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Warrior_3040) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Warrior_3040);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Warrior_3041) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Warrior_3041);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Warrior_3042) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Warrior_3042);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Warrior_3043) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Warrior_3043);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Warrior_3044) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Warrior_3044);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Warrior_3045) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Warrior_3045);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Warrior_3046) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Warrior_3046);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Warrior_3047) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Warrior_3047);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Warrior_4001) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Warrior_4001);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Warrior_4002) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Warrior_4002);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Warrior_4003) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Warrior_4003);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Warrior_4004) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Warrior_4004);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Warrior_4005) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Warrior_4005);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Warrior_4006) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Warrior_4006);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Warrior_4008) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Warrior_4008);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Warrior_4009) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Warrior_4009);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Warrior_4010) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Warrior_4010);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Warrior_4011) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Warrior_4011);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Warrior_4012) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Warrior_4012);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Warrior_4013) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Warrior_4013);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Warrior_4014) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Warrior_4014);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Warrior_4015) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Warrior_4015);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Warrior_4016) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Warrior_4016);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Assault_1001) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Assault_1001);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Assault_1002) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Assault_1002);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Assault_1003) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Assault_1003);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Assault_1004) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Assault_1004);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Assault_1005) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Assault_1005);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Assault_1006) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Assault_1006);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Assault_1007) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Assault_1007);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Assault_1008) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Assault_1008);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Assault_1009) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Assault_1009);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Assault_1010) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Assault_1010);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Assault_1011) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Assault_1011);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Assault_1012) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Assault_1012);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Assault_1013) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Assault_1013);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Assault_1014) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Assault_1014);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Assault_1015) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Assault_1015);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Assault_1016) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Assault_1016);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Assault_1017) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Assault_1017);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Assault_1018) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Assault_1018);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Assault_1019) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Assault_1019);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Assault_1020) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Assault_1020);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Assault_1021) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Assault_1021);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Assault_2001) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Assault_2001);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Assault_2002) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Assault_2002);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Assault_2003) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Assault_2003);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Assault_2004) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Assault_2004);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Assault_2005) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Assault_2005);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Assault_2006) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Assault_2006);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Assault_2007) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Assault_2007);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Assault_2008) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Assault_2008);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Assault_2009) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Assault_2009);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Assault_2010) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Assault_2010);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Assault_2011) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Assault_2011);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Assault_2012) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Assault_2012);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Assault_2013) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Assault_2013);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Assault_2014) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Assault_2014);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Assault_2015) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Assault_2015);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Assault_2016) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Assault_2016);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Assault_2017) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Assault_2017);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Assault_2018) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Assault_2018);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Assault_2019) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Assault_2019);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Assault_2020) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Assault_2020);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Assault_2021) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Assault_2021);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Assault_2022) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Assault_2022);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Assault_2023) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Assault_2023);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Assault_2024) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Assault_2024);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Assault_2025) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Assault_2025);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Assault_2026) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Assault_2026);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Assault_2027) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Assault_2027);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Assault_2028) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Assault_2028);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Assault_2029) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Assault_2029);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Assault_2030) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Assault_2030);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Assault_2031) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Assault_2031);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Assault_2032) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Assault_2032);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Assault_2033) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Assault_2033);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Assault_2034) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Assault_2034);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Assault_2035) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Assault_2035);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Assault_2036) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Assault_2036);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Assault_2037) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Assault_2037);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Assault_2038) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Assault_2038);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Assault_2039) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Assault_2039);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Assault_2040) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Assault_2040);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Assault_2041) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Assault_2041);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Assault_3001) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Assault_3001);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Assault_3002) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Assault_3002);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Assault_3003) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Assault_3003);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Assault_3004) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Assault_3004);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Assault_3005) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Assault_3005);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Assault_3006) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Assault_3006);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Assault_3007) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Assault_3007);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Assault_3008) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Assault_3008);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Assault_3009) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Assault_3009);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Assault_3010) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Assault_3010);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Assault_3011) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Assault_3011);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Assault_3012) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Assault_3012);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Assault_3013) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Assault_3013);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Assault_3014) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Assault_3014);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Assault_3015) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Assault_3015);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Assault_3016) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Assault_3016);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Assault_3017) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Assault_3017);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Assault_3018) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Assault_3018);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Assault_3019) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Assault_3019);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Assault_3020) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Assault_3020);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Assault_3021) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Assault_3021);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Assault_3022) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Assault_3022);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Assault_3023) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Assault_3023);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Assault_3024) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Assault_3024);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Assault_3025) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Assault_3025);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Assault_3026) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Assault_3026);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Assault_3027) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Assault_3027);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Assault_3028) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Assault_3028);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Assault_3029) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Assault_3029);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Assault_3030) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Assault_3030);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Assault_3031) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Assault_3031);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Assault_3032) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Assault_3032);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Assault_3033) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Assault_3033);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Assault_3034) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Assault_3034);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Assault_3035) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Assault_3035);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Assault_3036) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Assault_3036);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Assault_3037) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Assault_3037);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Assault_3038) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Assault_3038);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Assault_3039) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Assault_3039);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Assault_3040) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Assault_3040);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Assault_3041) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Assault_3041);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Assault_3042) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Assault_3042);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Assault_3043) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Assault_3043);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Assault_3044) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Assault_3044);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Assault_3045) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Assault_3045);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Assault_3046) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Assault_3046);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Assault_3047) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Assault_3047);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Assault_4001) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Assault_4001);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Assault_4002) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Assault_4002);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Assault_4003) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Assault_4003);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Assault_4004) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Assault_4004);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Assault_4005) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Assault_4005);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Assault_4006) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Assault_4006);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Assault_4008) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Assault_4008);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Assault_4009) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Assault_4009);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Assault_4010) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Assault_4010);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Assault_4011) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Assault_4011);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Assault_4012) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Assault_4012);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Assault_4013) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Assault_4013);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Assault_4014) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Assault_4014);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Assault_4015) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Assault_4015);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Assault_4016) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Assault_4016);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Bruiser_1001) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Bruiser_1001);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Bruiser_1002) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Bruiser_1002);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Bruiser_1003) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Bruiser_1003);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Bruiser_1004) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Bruiser_1004);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Bruiser_1005) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Bruiser_1005);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Bruiser_1006) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Bruiser_1006);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Bruiser_1007) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Bruiser_1007);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Bruiser_1008) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Bruiser_1008);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Bruiser_1009) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Bruiser_1009);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Bruiser_1010) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Bruiser_1010);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Bruiser_1011) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Bruiser_1011);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Bruiser_1012) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Bruiser_1012);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Bruiser_1013) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Bruiser_1013);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Bruiser_1014) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Bruiser_1014);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Bruiser_1015) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Bruiser_1015);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Bruiser_1016) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Bruiser_1016);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Bruiser_1017) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Bruiser_1017);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Bruiser_1018) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Bruiser_1018);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Bruiser_1019) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Bruiser_1019);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Bruiser_1020) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Bruiser_1020);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Bruiser_1021) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Bruiser_1021);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Bruiser_2001) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Bruiser_2001);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Bruiser_2002) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Bruiser_2002);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Bruiser_2003) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Bruiser_2003);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Bruiser_2004) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Bruiser_2004);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Bruiser_2005) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Bruiser_2005);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Bruiser_2006) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Bruiser_2006);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Bruiser_2007) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Bruiser_2007);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Bruiser_2008) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Bruiser_2008);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Bruiser_2009) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Bruiser_2009);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Bruiser_2010) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Bruiser_2010);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Bruiser_2011) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Bruiser_2011);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Bruiser_2012) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Bruiser_2012);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Bruiser_2013) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Bruiser_2013);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Bruiser_2014) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Bruiser_2014);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Bruiser_2015) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Bruiser_2015);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Bruiser_2016) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Bruiser_2016);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Bruiser_2017) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Bruiser_2017);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Bruiser_2018) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Bruiser_2018);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Bruiser_2019) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Bruiser_2019);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Bruiser_2020) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Bruiser_2020);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Bruiser_2021) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Bruiser_2021);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Bruiser_2022) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Bruiser_2022);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Bruiser_2023) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Bruiser_2023);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Bruiser_2024) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Bruiser_2024);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Bruiser_2025) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Bruiser_2025);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Bruiser_2026) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Bruiser_2026);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Bruiser_2027) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Bruiser_2027);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Bruiser_2028) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Bruiser_2028);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Bruiser_2029) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Bruiser_2029);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Bruiser_2030) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Bruiser_2030);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Bruiser_2031) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Bruiser_2031);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Bruiser_2032) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Bruiser_2032);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Bruiser_2033) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Bruiser_2033);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Bruiser_2034) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Bruiser_2034);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Bruiser_2035) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Bruiser_2035);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Bruiser_2036) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Bruiser_2036);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Bruiser_2037) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Bruiser_2037);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Bruiser_2038) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Bruiser_2038);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Bruiser_2039) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Bruiser_2039);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Bruiser_2040) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Bruiser_2040);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Bruiser_2041) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Bruiser_2041);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Bruiser_3001) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Bruiser_3001);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Bruiser_3002) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Bruiser_3002);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Bruiser_3003) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Bruiser_3003);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Bruiser_3004) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Bruiser_3004);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Bruiser_3005) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Bruiser_3005);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Bruiser_3006) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Bruiser_3006);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Bruiser_3007) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Bruiser_3007);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Bruiser_3008) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Bruiser_3008);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Bruiser_3009) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Bruiser_3009);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Bruiser_3010) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Bruiser_3010);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Bruiser_3011) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Bruiser_3011);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Bruiser_3012) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Bruiser_3012);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Bruiser_3013) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Bruiser_3013);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Bruiser_3014) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Bruiser_3014);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Bruiser_3015) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Bruiser_3015);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Bruiser_3016) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Bruiser_3016);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Bruiser_3017) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Bruiser_3017);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Bruiser_3018) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Bruiser_3018);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Bruiser_3019) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Bruiser_3019);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Bruiser_3020) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Bruiser_3020);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Bruiser_3021) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Bruiser_3021);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Bruiser_3022) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Bruiser_3022);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Bruiser_3023) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Bruiser_3023);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Bruiser_3024) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Bruiser_3024);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Bruiser_3025) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Bruiser_3025);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Bruiser_3026) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Bruiser_3026);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Bruiser_3027) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Bruiser_3027);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Bruiser_3028) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Bruiser_3028);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Bruiser_3029) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Bruiser_3029);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Bruiser_3030) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Bruiser_3030);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Bruiser_3031) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Bruiser_3031);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Bruiser_3032) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Bruiser_3032);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Bruiser_3033) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Bruiser_3033);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Bruiser_3034) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Bruiser_3034);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Bruiser_3035) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Bruiser_3035);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Bruiser_3036) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Bruiser_3036);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Bruiser_3037) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Bruiser_3037);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Bruiser_3038) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Bruiser_3038);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Bruiser_3039) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Bruiser_3039);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Bruiser_3040) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Bruiser_3040);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Bruiser_3041) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Bruiser_3041);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Bruiser_3042) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Bruiser_3042);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Bruiser_3043) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Bruiser_3043);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Bruiser_3044) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Bruiser_3044);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Bruiser_3045) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Bruiser_3045);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Bruiser_3046) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Bruiser_3046);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Bruiser_3047) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Bruiser_3047);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Bruiser_4001) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Bruiser_4001);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Bruiser_4002) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Bruiser_4002);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Bruiser_4003) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Bruiser_4003);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Bruiser_4004) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Bruiser_4004);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Bruiser_4005) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Bruiser_4005);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Bruiser_4006) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Bruiser_4006);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Bruiser_4008) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Bruiser_4008);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Bruiser_4009) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Bruiser_4009);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Bruiser_4010) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Bruiser_4010);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Bruiser_4011) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Bruiser_4011);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Bruiser_4012) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Bruiser_4012);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Bruiser_4013) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Bruiser_4013);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Bruiser_4014) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Bruiser_4014);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Bruiser_4015) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Bruiser_4015);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Bruiser_4016) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Bruiser_4016);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.StrandToken) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.StrandToken);
				flag = true;
			}
			if (flag)
			{
				MigrationUtils.DeleteCombatModel(player);
			}
			return flag;
		}
	}
}
