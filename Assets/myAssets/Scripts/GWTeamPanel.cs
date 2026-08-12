using System;
using System.Collections.Generic;
using TwdCustomMod;
using TWDModel;
using UnityEngine;

public class GWTeamPanel : MonoBehaviour
{
	[Serializable]
	private class SurvivorSlot
	{
		public SurvivorCard SurvivorCard;
		public TeamSelectionEmptyCard EmptyCard;
		public SurvivorStatisticsPanel survivorPanel;
	}

	[SerializeField]
	private SurvivorSlot[] survivorSlots;

	[SerializeField]
	private SmallSupportCard[] supportCards;

	public UILabel teamOwnPlayer;
	public UILabel teamDamage;
	public UILabel teamHealth;

	public UILabel teamLevel;
	public UILabel teamIndex;
	public UILabel Index;
	public UILabel IndexAdv;

	private List<SurvivorMockData> PvpTeam;

	public ShowTooltip TooltipLevel;

	//скрыть UI для скриншота
	private bool IsHideUI;
	public List<GameObject> gosUIHide;

	private PlayerModel Player => DataManager.Instance.Player;

	void Start()
	{

	}

	void Update()
	{

	}

	public void ShowHideUI()
	{
		IsHideUI = !IsHideUI;
		teamIndex.transform.parent.GetComponent<UIWidget>().alpha = IsHideUI ? 0f : 1f;
		foreach (var obj in gosUIHide)
		{
			obj.SetActive(!IsHideUI);
		}
	}

	public void RefreshSurvivorSlots()
	{
		if (PvpTeam != null)
		{
			int index = -1;
			if (Index != null && !string.IsNullOrEmpty(Index.text)) index = int.Parse(Index.text);

			RefreshSurvivorSlots(PvpTeam, "", teamOwnPlayer.text, index);
		}
	}

	public void RefreshSurvivorSlots(List<SurvivorMockData> pvpTeam, string tmIndex, string name, int index)
	{
		PvpTeam = pvpTeam;

		int AdjustedLevelTotal = 0;

		string AdjustedLevelTotalString = "";
		string RealLevelTotalString = "";
		string AdjustedLevelCalculate = "";

		int TotalDamage = 0;
		int TotalHealth = 0;

		teamOwnPlayer.text = name;
		if (teamIndex != IndexAdv) teamIndex.text = tmIndex;
		//else teamIndex.text = index.ToString();
		//Debug.Log("IndexAdv is " + (index + 1).ToString());
		if (IndexAdv != null && index > -1) IndexAdv.text = (index + 1).ToString();
		//уровень героев конкретного юзера
		int currentUserLevel = GvGModelHelper.GetPlayerSpecificDifficulty(Player);

		for (int i = 0; i < pvpTeam.Count; i++)
		{
			SurvivorSlot survivorSlot = survivorSlots[i];
			SurvivorModel survivorModel = Player.SurvivorContainer.CreateSurvivorFromSurvivorMockData(pvpTeam[i], currentUserLevel, preview: true);
			survivorSlot.SurvivorCard.IsProtector = true;
			Helpers.GameObjectSetActive(survivorSlot.EmptyCard, survivorModel == null);
			Helpers.GameObjectSetActive(survivorSlot.SurvivorCard, survivorModel != null);
			if (survivorModel != null)
			{
				var itemLevel = GWTeamsManager.Instance.btLevelChangeList[i];
				if (pvpTeam[i].AdjustedLevelAdd == 0)
				{
					itemLevel.signPlus.transform.parent.gameObject.SetActive(false);
				}
				else
				{
					itemLevel.signPlus.transform.parent.gameObject.SetActive(true);
					itemLevel.signPlus.text = pvpTeam[i].AdjustedLevelAdd > 0 ? "+" : "-";
					itemLevel.signCount.text = Math.Abs(pvpTeam[i].AdjustedLevelAdd).ToString();
				}

				survivorSlot.SurvivorCard.Item = survivorModel;
				survivorSlot.SurvivorCard.UpdateUI();
				survivorSlot.SurvivorCard.SetInfoButtonActive(active: true);
				survivorSlot.SurvivorCard.Type = SurvivorCard.CardType.TeamSelect;
				AdjustedLevelTotal += pvpTeam[i].AdjustedLevel;
				AdjustedLevelTotalString += pvpTeam[i].AdjustedLevel.ToString() + (i < pvpTeam.Count - 1 ? " + " : "");
				RealLevelTotalString += pvpTeam[i].Level.ToString() + (i < pvpTeam.Count - 1 ? " + " : "");
				var survivor = pvpTeam[i];
				var AdjustedLevelFloat = survivor.Level + (survivor.IsHero ? 1 : 0) * DataManager.Instance.GameData.GuildWarConfig.HeroLevelEq +
				UtilsMath.Max(0, survivor.RarityLevel - 4) * DataManager.Instance.GameData.GuildWarConfig.PinkLevelEq;
				AdjustedLevelCalculate += $"{survivor.AdjustedLevel} : {survivor.Level} + {(survivor.IsHero ? 1 : 0)} * " +
					$"{DataManager.Instance.GameData.GuildWarConfig.HeroLevelEq} + {UtilsMath.Max(0, survivor.RarityLevel - 4)} * " +
					$"{DataManager.Instance.GameData.GuildWarConfig.PinkLevelEq} = {AdjustedLevelFloat}" + (i < pvpTeam.Count - 1 ? "\n\n" : "");

				var damage = survivorSlot.SurvivorCard.GetStrengthValue();
				TotalDamage += damage;
				var health = pvpTeam[i].TotalDamage;
				TotalHealth += health;

				if (survivorSlot.survivorPanel != null)
				{
					survivorSlot.survivorPanel.SetInfo(survivorModel);
				}
				//survivorSlot.SurvivorCard.SetDamageValue(damage);
				survivorSlot.SurvivorCard.SetHealthValue(health);
			}
		}
		teamDamage.text = "Damage : " + TotalDamage.ToString();
		if (teamHealth != null) teamHealth.text = "Health : " + TotalHealth.ToString();
		teamLevel.text = "GW Level : " + GWTeamUtils.GetAverageAdjustedLevel(pvpTeam).ToString() + " (" + AdjustedLevelTotalString + ")";
		if (TooltipLevel != null)
		{
			TooltipLevel.EnCustomText = AdjustedLevelCalculate + "\nResults is round to integer.";
			TooltipLevel.RuCustomText = AdjustedLevelCalculate + "\n\nОтбрасываем дробную часть.";
		}
	}

	public void RefreshSurvivorSlots(GuildBattlePvpTeam pvpTeam, string tmIndex, string name, int index)
	{
		var Survivors = pvpTeam.Survivors;
		RefreshSurvivorSlots(Survivors, tmIndex, name, index);
	}

}
