using System;

namespace TWDModel
{
	public class SurvivorStatistics : TWDModelObject
	{
		public DateTime BirthDate { get; set; }

		public DateTime DeathDate { get; set; }

		public int NumberMissionPlayed { get; set; }

		public int NumberWalkersKilled { get; set; }

		public int NumberRaidersKilled { get; set; }

		public int HitsInflictedInMission { get; set; }

		public int NumberOfChargeAbilitiesUsedInMission { get; set; }

		public int HitsTakenInMission { get; set; }

		public int TotalDamageTakenInMission { get; set; }

		public int TotalDamageInflictedInCombat { get; set; }

		public int TotalHealingReceivedInCombat { get; set; }

		public override void Initialize()
		{
			BirthDate = DateTime.Today;
		}

		public override bool IsValid()
		{
			return true;
		}

		public void AddWalkersKilled()
		{
			NumberWalkersKilled++;
		}

		public void AddRaidersKilled()
		{
			NumberRaidersKilled++;
		}

		public void AddMissionPlayed()
		{
			NumberMissionPlayed++;
		}

		public int GetNumberDaysAlive()
		{
			return (DeathDate - BirthDate).Days + 1;
		}

		public void IncreaseHitsTakenInCombat()
		{
			HitsTakenInMission++;
		}

		public void IncreaseTotalDamageTakenInCombat(int damage)
		{
			TotalDamageTakenInMission += damage;
		}

		public void IncreaseTotalDamageInflictedInCombat(int damage)
		{
			TotalDamageInflictedInCombat += damage;
		}

		public void IncreaseTotalHealingReceivedInCombat(int amount)
		{
			TotalHealingReceivedInCombat += amount;
		}

		public void IncreaseChargeAbilitiesUse()
		{
			NumberOfChargeAbilitiesUsedInMission++;
		}

		public void IncreaseHitsInflictedInMission()
		{
			HitsInflictedInMission++;
		}

		public void SurvivorDied()
		{
			DeathDate = DateTime.Today;
		}
	}
}
