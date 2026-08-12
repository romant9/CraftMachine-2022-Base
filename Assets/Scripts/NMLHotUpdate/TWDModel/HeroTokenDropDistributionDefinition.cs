using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace TWDModel
{
	[Serializable]
	public class HeroTokenDropDistributionDefinition
	{
		public const string SkipHeroTokenDistributionBucketId = "HeroGrouping";

		public string BucketId;

		[Probability]
		public FixedPoint CarolToken;

		[Probability]
		public FixedPoint RickToken;

		[Probability]
		public FixedPoint AbrahamToken;

		[Probability]
		public FixedPoint CarlToken;

		[Probability]
		public FixedPoint NeganToken;

		[Probability]
		public FixedPoint MichonneToken;

		[Probability]
		public FixedPoint MorganToken;

		[Probability]
		public FixedPoint MaggieToken;

		[Probability]
		public FixedPoint JesusToken;

		[Probability]
		public FixedPoint GlennToken;

		[Probability]
		public FixedPoint DarylToken;

		[Probability]
		public FixedPoint TaraToken;

		[Probability]
		public FixedPoint RositaToken;

		[Probability]
		public FixedPoint TalkingDeadToken;

		[Probability]
		public FixedPoint EugeneToken;

		[Probability]
		public FixedPoint AaronToken;

		[Probability]
		public FixedPoint GabrielToken;

		[Probability]
		public FixedPoint EzekielToken;

		[Probability]
		public FixedPoint DwightToken;

		[Probability]
		public FixedPoint SashaToken;

		[Probability]
		public FixedPoint MerleToken;

		[Probability]
		public FixedPoint GovernorToken;

		[Probability]
		public FixedPoint JerryToken;

		[Probability]
		public FixedPoint BruiserGlennToken;

		[Probability]
		public FixedPoint HunterMorganToken;

		[Probability]
		public FixedPoint ScoutRickToken;

		[Probability]
		public FixedPoint ScoutDarylToken;

		[Probability]
		public FixedPoint AltAbrahamToken;

		[Probability]
		public FixedPoint AlphaToken;

		[Probability]
		public FixedPoint BetaToken;

		[Probability]
		public FixedPoint TDogToken;

		[Probability]
		public FixedPoint ShaneToken;

		[Probability]
		public FixedPoint AssassinCarolToken;

		[Probability]
		public FixedPoint BethToken;

		[Probability]
		public FixedPoint BruiserRositaToken;

		[Probability]
		public FixedPoint ConnieToken;

		[Probability]
		public FixedPoint CowboyNeganToken;

		[Probability]
		public FixedPoint CroatToken;

		[Probability]
		public FixedPoint QuickdrawCarolToken;

		[Probability]
		public FixedPoint LydiaToken;

		[Probability]
		public FixedPoint StrandToken;

		[Probability]
		public FixedPoint ScoutMaggieToken;

		[Probability]
		public FixedPoint HunterHershelToken;

		[Probability]
		public FixedPoint JadisToken;

		[Probability]
		public FixedPoint MagnaToken;

		[Probability]
		public FixedPoint MercerToken;

		[Probability]
		public FixedPoint PrincessToken;

		[Probability]
		public FixedPoint QuinnToken;

		[Probability]
		public FixedPoint ShooterMaggieToken;

		[Probability]
		public FixedPoint TyreeseToken;

		[Probability]
		public FixedPoint YumikoToken;

		[Probability]
		public FixedPoint ProtectorDarylToken;

		public FixedPoint GauntletAaronToken;

		private List<KeyValuePair<FixedPoint, CurrencyType>> heroTokenProbabilitiesList = new List<KeyValuePair<FixedPoint, CurrencyType>>();

		[JsonIgnore]
		public FixedPoint SumOfProbabilities => CarolToken + RickToken + AbrahamToken + CarlToken + NeganToken + MichonneToken + MorganToken + MaggieToken + JesusToken + GlennToken + DarylToken + TaraToken + RositaToken + TalkingDeadToken + EugeneToken + AaronToken + GabrielToken + EzekielToken + DwightToken + SashaToken + MerleToken + GovernorToken + JerryToken + BruiserGlennToken + HunterMorganToken + ScoutRickToken + ScoutDarylToken + AltAbrahamToken + AlphaToken + BetaToken + TDogToken + ShaneToken + AssassinCarolToken + BethToken + BruiserRositaToken + ConnieToken + CowboyNeganToken + HunterHershelToken + JadisToken + MagnaToken + MercerToken + PrincessToken + QuinnToken + ScoutDarylToken + ScoutRickToken + ShooterMaggieToken + TyreeseToken + YumikoToken;

		public void PopulateProbabilitiesList()
		{
			GameEconomyData.GetProbabilitiesAsList(ref heroTokenProbabilitiesList, this);
		}

		public CurrencyType GetTokenTypeForRandomNumber(FixedPoint number)
		{
			if (heroTokenProbabilitiesList.Count == 0)
			{
				PopulateProbabilitiesList();
			}
			foreach (KeyValuePair<FixedPoint, CurrencyType> heroTokenProbabilities in heroTokenProbabilitiesList)
			{
				if (heroTokenProbabilities.Key >= number)
				{
					return heroTokenProbabilities.Value;
				}
			}
			return CurrencyType.None;
		}
	}
}
