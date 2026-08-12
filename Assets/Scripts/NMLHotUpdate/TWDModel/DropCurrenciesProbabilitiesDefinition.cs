using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace TWDModel
{
	[Serializable]
	public class DropCurrenciesProbabilitiesDefinition
	{
		public enum DropCurrency
		{
			Supplies = 0,
			SurvivalPoints = 1,
			Diamonds = 2,
			Weapon = 3,
			Armor = 4,
			Phone = 5,
			ReplayToken = 6,
			Survivor = 7,
			Inhabitant = 8,
			Outpost = 9,
			HeroToken = 10,
			ClassToken = 11,
			Component = 12,
			GvGGas = 13,
			BattlePass = 14,
			Consumable = 15,
			AnyCurrency = 16,
			Avatars = 17,
			ChallengeSkipToken = 18,
			EquipToken = 19,
			RemoldSkill = 20
		}

		public DropEventDefinition.DropEventType EventType;

		public DropType DropType;

		public DropEventDefinition.DropEventTag Tag;

		public int ControlLevelMin;

		public int ControlLevelMax;

		public float SuppliesProbability;

		public float SurvivalPointsProbability;

		public float DiamondsProbability;

		public float WeaponProbability;

		public float ArmorProbability;

		public float PhoneProbability;

		public float ReplayTokenProbability;

		public float SurvivorProbability;

		public float InhabitantProbability;

		public float HeroTokenProbability;

		public float ClassTokenProbability;

		public float ComponentProbability;

		public float GvGGasProbability;

		private List<KeyValuePair<float, DropCurrency>> currenciesProbabilities = new List<KeyValuePair<float, DropCurrency>>();

		[JsonIgnore]
		public float SumOfProbabilities => SuppliesProbability + SurvivalPointsProbability + DiamondsProbability + WeaponProbability + ArmorProbability + PhoneProbability + ReplayTokenProbability + SurvivorProbability + InhabitantProbability + HeroTokenProbability + ClassTokenProbability + ComponentProbability + GvGGasProbability;

		public void PopulateProbabilitiesList()
		{
			currenciesProbabilities.Clear();
			float num = 0f;
			if (SuppliesProbability > 0f)
			{
				KeyValuePair<float, DropCurrency> item = new KeyValuePair<float, DropCurrency>(num + SuppliesProbability, DropCurrency.Supplies);
				currenciesProbabilities.Add(item);
				num += SuppliesProbability;
			}
			if (SurvivalPointsProbability > 0f)
			{
				KeyValuePair<float, DropCurrency> item2 = new KeyValuePair<float, DropCurrency>(num + SurvivalPointsProbability, DropCurrency.SurvivalPoints);
				currenciesProbabilities.Add(item2);
				num += SurvivalPointsProbability;
			}
			if (DiamondsProbability > 0f)
			{
				KeyValuePair<float, DropCurrency> item3 = new KeyValuePair<float, DropCurrency>(num + DiamondsProbability, DropCurrency.Diamonds);
				currenciesProbabilities.Add(item3);
				num += DiamondsProbability;
			}
			if (WeaponProbability > 0f)
			{
				KeyValuePair<float, DropCurrency> item4 = new KeyValuePair<float, DropCurrency>(num + WeaponProbability, DropCurrency.Weapon);
				currenciesProbabilities.Add(item4);
				num += WeaponProbability;
			}
			if (ArmorProbability > 0f)
			{
				KeyValuePair<float, DropCurrency> item5 = new KeyValuePair<float, DropCurrency>(num + ArmorProbability, DropCurrency.Armor);
				currenciesProbabilities.Add(item5);
				num += ArmorProbability;
			}
			if (PhoneProbability > 0f)
			{
				KeyValuePair<float, DropCurrency> item6 = new KeyValuePair<float, DropCurrency>(num + PhoneProbability, DropCurrency.Phone);
				currenciesProbabilities.Add(item6);
				num += PhoneProbability;
			}
			if (ReplayTokenProbability > 0f)
			{
				KeyValuePair<float, DropCurrency> item7 = new KeyValuePair<float, DropCurrency>(num + ReplayTokenProbability, DropCurrency.ReplayToken);
				currenciesProbabilities.Add(item7);
				num += ReplayTokenProbability;
			}
			if (SurvivorProbability > 0f)
			{
				KeyValuePair<float, DropCurrency> item8 = new KeyValuePair<float, DropCurrency>(num + SurvivorProbability, DropCurrency.Survivor);
				currenciesProbabilities.Add(item8);
				num += SurvivorProbability;
			}
			if (InhabitantProbability > 0f)
			{
				KeyValuePair<float, DropCurrency> item9 = new KeyValuePair<float, DropCurrency>(num + InhabitantProbability, DropCurrency.Inhabitant);
				currenciesProbabilities.Add(item9);
				num += InhabitantProbability;
			}
			if (HeroTokenProbability > 0f)
			{
				KeyValuePair<float, DropCurrency> item10 = new KeyValuePair<float, DropCurrency>(num + HeroTokenProbability, DropCurrency.HeroToken);
				currenciesProbabilities.Add(item10);
				num += HeroTokenProbability;
			}
			if (ClassTokenProbability > 0f)
			{
				KeyValuePair<float, DropCurrency> item11 = new KeyValuePair<float, DropCurrency>(num + ClassTokenProbability, DropCurrency.ClassToken);
				currenciesProbabilities.Add(item11);
				num += ClassTokenProbability;
			}
			if (ComponentProbability > 0f)
			{
				KeyValuePair<float, DropCurrency> item12 = new KeyValuePair<float, DropCurrency>(num + ComponentProbability, DropCurrency.Component);
				currenciesProbabilities.Add(item12);
				num += ComponentProbability;
			}
			if (GvGGasProbability > 0f)
			{
				KeyValuePair<float, DropCurrency> item13 = new KeyValuePair<float, DropCurrency>(num + GvGGasProbability, DropCurrency.GvGGas);
				currenciesProbabilities.Add(item13);
				num += ComponentProbability;
			}
		}

		public DropCurrency GetDropCurrencyForRandomNumber(FixedPoint number)
		{
			if (currenciesProbabilities.Count == 0)
			{
				PopulateProbabilitiesList();
			}
			foreach (KeyValuePair<float, DropCurrency> currenciesProbability in currenciesProbabilities)
			{
				if (currenciesProbability.Key >= number)
				{
					return currenciesProbability.Value;
				}
			}
			return DropCurrency.Supplies;
		}
	}
}
