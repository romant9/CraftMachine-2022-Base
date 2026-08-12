namespace TWDModel
{
	public class Migration7190 : TWDModelMigration
	{
		public Migration7190()
		{
			base.Version = "7.19.0";
		}

		public override bool Migrate(PlayerModel player, TWDModelManager manager)
		{
			bool flag = false;
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.ScoutMaggieToken) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.ScoutMaggieToken);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Warrior_4017) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Warrior_4017);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Warrior_4018) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Warrior_4018);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Warrior_4019) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Warrior_4019);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Warrior_4020) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Warrior_4020);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Warrior_4021) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Warrior_4021);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Warrior_4022) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Warrior_4022);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Warrior_4023) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Warrior_4023);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Warrior_4024) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Warrior_4024);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Warrior_4025) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Warrior_4025);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Warrior_4026) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Warrior_4026);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Warrior_4027) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Warrior_4027);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Warrior_4028) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Warrior_4028);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Warrior_4029) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Warrior_4029);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Warrior_4030) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Warrior_4030);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Warrior_3048) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Warrior_3048);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Warrior_3049) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Warrior_3049);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Warrior_3050) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Warrior_3050);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Warrior_3051) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Warrior_3051);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Warrior_3052) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Warrior_3052);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Warrior_3053) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Warrior_3053);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Warrior_3054) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Warrior_3054);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Warrior_3055) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Warrior_3055);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Warrior_3056) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Warrior_3056);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Warrior_3057) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Warrior_3057);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Warrior_3058) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Warrior_3058);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Warrior_3059) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Warrior_3059);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Warrior_3060) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Warrior_3060);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Warrior_3061) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Warrior_3061);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Warrior_3062) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Warrior_3062);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Warrior_3063) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Warrior_3063);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Warrior_3064) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Warrior_3064);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Warrior_3065) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Warrior_3065);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Warrior_3066) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Warrior_3066);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Warrior_3067) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Warrior_3067);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Warrior_3068) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Warrior_3068);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Warrior_3069) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Warrior_3069);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Warrior_3070) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Warrior_3070);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Scout_4017) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Scout_4017);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Scout_4018) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Scout_4018);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Scout_4019) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Scout_4019);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Scout_4020) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Scout_4020);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Scout_4021) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Scout_4021);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Scout_4022) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Scout_4022);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Scout_4023) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Scout_4023);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Scout_4024) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Scout_4024);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Scout_4025) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Scout_4025);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Scout_4026) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Scout_4026);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Scout_4027) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Scout_4027);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Scout_4028) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Scout_4028);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Scout_4029) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Scout_4029);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Scout_4030) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Scout_4030);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Scout_3048) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Scout_3048);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Scout_3049) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Scout_3049);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Scout_3050) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Scout_3050);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Scout_3051) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Scout_3051);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Scout_3052) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Scout_3052);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Scout_3053) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Scout_3053);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Scout_3054) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Scout_3054);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Scout_3055) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Scout_3055);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Scout_3056) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Scout_3056);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Scout_3057) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Scout_3057);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Scout_3058) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Scout_3058);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Scout_3059) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Scout_3059);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Scout_3060) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Scout_3060);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Scout_3061) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Scout_3061);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Scout_3062) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Scout_3062);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Scout_3063) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Scout_3063);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Scout_3064) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Scout_3064);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Scout_3065) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Scout_3065);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Scout_3066) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Scout_3066);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Scout_3067) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Scout_3067);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Scout_3068) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Scout_3068);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Scout_3069) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Scout_3069);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Scout_3070) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Scout_3070);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Bruiser_4017) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Bruiser_4017);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Bruiser_4018) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Bruiser_4018);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Bruiser_4019) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Bruiser_4019);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Bruiser_4020) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Bruiser_4020);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Bruiser_4021) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Bruiser_4021);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Bruiser_4022) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Bruiser_4022);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Bruiser_4023) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Bruiser_4023);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Bruiser_4024) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Bruiser_4024);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Bruiser_4025) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Bruiser_4025);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Bruiser_4026) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Bruiser_4026);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Bruiser_4027) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Bruiser_4027);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Bruiser_4028) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Bruiser_4028);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Bruiser_4029) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Bruiser_4029);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Bruiser_4030) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Bruiser_4030);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Bruiser_3048) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Bruiser_3048);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Bruiser_3049) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Bruiser_3049);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Bruiser_3050) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Bruiser_3050);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Bruiser_3051) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Bruiser_3051);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Bruiser_3052) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Bruiser_3052);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Bruiser_3053) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Bruiser_3053);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Bruiser_3054) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Bruiser_3054);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Bruiser_3055) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Bruiser_3055);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Bruiser_3056) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Bruiser_3056);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Bruiser_3057) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Bruiser_3057);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Bruiser_3058) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Bruiser_3058);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Bruiser_3059) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Bruiser_3059);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Bruiser_3060) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Bruiser_3060);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Bruiser_3061) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Bruiser_3061);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Bruiser_3062) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Bruiser_3062);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Bruiser_3063) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Bruiser_3063);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Bruiser_3064) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Bruiser_3064);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Bruiser_3065) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Bruiser_3065);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Bruiser_3066) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Bruiser_3066);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Bruiser_3067) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Bruiser_3067);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Bruiser_3068) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Bruiser_3068);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Bruiser_3069) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Bruiser_3069);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Bruiser_3070) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Bruiser_3070);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Assault_4017) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Assault_4017);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Assault_4018) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Assault_4018);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Assault_4019) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Assault_4019);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Assault_4020) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Assault_4020);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Assault_4021) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Assault_4021);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Assault_4022) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Assault_4022);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Assault_4023) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Assault_4023);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Assault_4024) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Assault_4024);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Assault_4025) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Assault_4025);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Assault_4026) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Assault_4026);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Assault_4027) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Assault_4027);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Assault_4028) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Assault_4028);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Assault_4029) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Assault_4029);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Assault_4030) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Assault_4030);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Assault_3048) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Assault_3048);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Assault_3049) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Assault_3049);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Assault_3050) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Assault_3050);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Assault_3051) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Assault_3051);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Assault_3052) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Assault_3052);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Assault_3053) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Assault_3053);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Assault_3054) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Assault_3054);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Assault_3055) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Assault_3055);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Assault_3056) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Assault_3056);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Assault_3057) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Assault_3057);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Assault_3058) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Assault_3058);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Assault_3059) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Assault_3059);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Assault_3060) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Assault_3060);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Assault_3061) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Assault_3061);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Assault_3062) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Assault_3062);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Assault_3063) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Assault_3063);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Assault_3064) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Assault_3064);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Assault_3065) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Assault_3065);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Assault_3066) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Assault_3066);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Assault_3067) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Assault_3067);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Assault_3068) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Assault_3068);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Assault_3069) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Assault_3069);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Assault_3070) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Assault_3070);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Shooter_4017) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Shooter_4017);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Shooter_4018) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Shooter_4018);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Shooter_4019) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Shooter_4019);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Shooter_4020) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Shooter_4020);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Shooter_4021) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Shooter_4021);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Shooter_4022) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Shooter_4022);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Shooter_4023) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Shooter_4023);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Shooter_4024) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Shooter_4024);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Shooter_4025) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Shooter_4025);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Shooter_4026) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Shooter_4026);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Shooter_4027) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Shooter_4027);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Shooter_4028) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Shooter_4028);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Shooter_4029) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Shooter_4029);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Shooter_4030) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Shooter_4030);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Shooter_3048) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Shooter_3048);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Shooter_3049) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Shooter_3049);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Shooter_3050) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Shooter_3050);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Shooter_3051) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Shooter_3051);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Shooter_3052) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Shooter_3052);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Shooter_3053) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Shooter_3053);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Shooter_3054) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Shooter_3054);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Shooter_3055) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Shooter_3055);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Shooter_3056) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Shooter_3056);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Shooter_3057) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Shooter_3057);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Shooter_3058) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Shooter_3058);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Shooter_3059) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Shooter_3059);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Shooter_3060) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Shooter_3060);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Shooter_3061) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Shooter_3061);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Shooter_3062) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Shooter_3062);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Shooter_3063) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Shooter_3063);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Shooter_3064) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Shooter_3064);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Shooter_3065) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Shooter_3065);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Shooter_3066) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Shooter_3066);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Shooter_3067) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Shooter_3067);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Shooter_3068) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Shooter_3068);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Shooter_3069) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Shooter_3069);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Shooter_3070) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Shooter_3070);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Hunter_4017) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Hunter_4017);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Hunter_4018) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Hunter_4018);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Hunter_4019) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Hunter_4019);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Hunter_4020) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Hunter_4020);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Hunter_4021) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Hunter_4021);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Hunter_4022) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Hunter_4022);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Hunter_4023) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Hunter_4023);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Hunter_4024) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Hunter_4024);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Hunter_4025) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Hunter_4025);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Hunter_4026) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Hunter_4026);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Hunter_4027) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Hunter_4027);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Hunter_4028) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Hunter_4028);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Hunter_4029) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Hunter_4029);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Hunter_4030) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Hunter_4030);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Hunter_3048) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Hunter_3048);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Hunter_3049) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Hunter_3049);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Hunter_3050) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Hunter_3050);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Hunter_3051) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Hunter_3051);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Hunter_3052) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Hunter_3052);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Hunter_3053) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Hunter_3053);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Hunter_3054) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Hunter_3054);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Hunter_3055) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Hunter_3055);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Hunter_3056) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Hunter_3056);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Hunter_3057) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Hunter_3057);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Hunter_3058) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Hunter_3058);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Hunter_3059) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Hunter_3059);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Hunter_3060) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Hunter_3060);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Hunter_3061) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Hunter_3061);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Hunter_3062) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Hunter_3062);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Hunter_3063) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Hunter_3063);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Hunter_3064) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Hunter_3064);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Hunter_3065) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Hunter_3065);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Hunter_3066) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Hunter_3066);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Hunter_3067) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Hunter_3067);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Hunter_3068) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Hunter_3068);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Hunter_3069) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Hunter_3069);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SkillToken_Hunter_3070) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SkillToken_Hunter_3070);
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
