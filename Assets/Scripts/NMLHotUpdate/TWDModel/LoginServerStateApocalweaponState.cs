namespace TWDModel
{
	public class LoginServerStateApocalweaponState
	{
		public string WeaponName { get; set; }

		public int WeaponLevel { get; set; }

		public int WeaponRarityLevel { get; set; }

		public int WeaponBreakthroughLevel { get; set; }

		public LoginServerStateApocalweaponState(string weaponName, int weaponLevel, int breakthroughLevel, int weaponRarityLevel)
		{
			WeaponName = weaponName;
			WeaponLevel = weaponLevel;
			WeaponRarityLevel = weaponRarityLevel;
			WeaponBreakthroughLevel = breakthroughLevel;
		}
	}
}
