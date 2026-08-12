using System.Collections.Generic;
using TWDModel;
using UnityEngine;

public class OutpostDetailsPanelStored : MonoBehaviour
{
	[SerializeField]
	private UILabel OutpostStatusLabel;

	[SerializeField]
	protected UILabel ProductionAmountLabel;

	[SerializeField]
	protected UILabel ProductionTimerLabel;

	[SerializeField]
	protected UIProgressBar ProductionAmountBar;

	[SerializeField]
	private ButtonWithLabel OutpostCollectButton;

	[SerializeField]
	private UISprite[] DefendersIconsArray;

	[SerializeField]
	private UILabel[] DefendersLevelArray;

	[SerializeField]
	private UILabel OutpostLevelLabel;

	private BuildingModel outpostBuilding;

	public virtual void UpdateUI()
	{
		bool flag = GameManager.Instance.playerModel.OutpostModel.StoredLevelModel != null;
		if (flag)
		{
			if (ProductionAmountLabel != null && ProductionAmountBar != null)
			{
				BuildingModel building = GameManager.Instance.playerModel.Camp.GetBuilding("Outpost");
				float b = (float)building.Producer.Amount / (float)building.Producer.Capacity;
				ProductionAmountLabel.text = building.Producer.Amount.ToString();
				ProductionAmountBar.value = Mathf.Max(0f, b);
			}
			UpdateLevel(GameManager.Instance.playerModel.Camp.GetBuilding("Outpost"));
			UpdateDefendersIconsAndLevel(GameManager.Instance.playerModel.SurvivorContainer.OutpostDefendingSurvivors);
		}
		else
		{
			base.gameObject.SetActive(flag);
		}
	}

	public virtual void OnDisable()
	{
		if (OutpostCollectButton != null)
		{
			OutpostCollectButton.Clear();
		}
		outpostBuilding = null;
	}

	public virtual void OnEnable()
	{
		if (OutpostCollectButton != null)
		{
			OutpostCollectButton.SetCallback(OnCollectClicked);
		}
		outpostBuilding = GameManager.Instance.playerModel.Camp.GetBuilding("Outpost");
	}

	public void UpdateLevel(BuildingModel building)
	{
		if (OutpostLevelLabel != null && building != null)
		{
			OutpostLevelLabel.text = building.Level.ToString();
		}
	}

	public void UpdateLevel(int level)
	{
		if (OutpostLevelLabel != null)
		{
			OutpostLevelLabel.text = level.ToString();
		}
	}

	public void UpdateDefendersIconsAndLevel(List<SurvivorModel> defendersList)
	{
		if (defendersList == null)
		{
			return;
		}
		for (int i = 0; i < defendersList.Count; i++)
		{
			if (defendersList[i] != null)
			{
				if (DefendersIconsArray != null && i < DefendersIconsArray.Length && DefendersIconsArray[i] != null)
				{
					DefendersIconsArray[i].spriteName = HelpersGfx.GetSurvivorClassIconName(defendersList[i].SurvivorClass.ToString(), defendersList[i].SurvivorRarityLevel);
				}
				if (DefendersLevelArray != null && i < DefendersLevelArray.Length && DefendersLevelArray[i] != null)
				{
					DefendersLevelArray[i].text = defendersList[i].Level.ToString();
				}
			}
		}
	}

	public virtual void Update()
	{
		if (OutpostStatusLabel != null && GameManager.Instance.playerModel.OutpostModel.StoredLevelModel != null)
		{
			long shieldTimeMillisLeft = GameManager.Instance.playerModel.GetShieldTimeMillisLeft(GameManager.Instance.playerModel.UtcTimeStamp);
			if (shieldTimeMillisLeft > 0)
			{
				OutpostStatusLabel.gameObject.SetActive(value: true);
				long milliSeconds = shieldTimeMillisLeft;
				OutpostStatusLabel.text = "Shield for " + Helpers.FormatTime(milliSeconds);
			}
			else
			{
				OutpostStatusLabel.gameObject.SetActive(value: false);
			}
			if (OutpostCollectButton != null && outpostBuilding != null)
			{
				OutpostCollectButton.gameObject.SetActive(outpostBuilding != null && outpostBuilding.CanCollect);
			}
		}
	}

	public void OnCollectClicked(ButtonBase origin)
	{
		if (outpostBuilding != null)
		{
			outpostBuilding.Collect();
			OutpostCollectButton.Button.isEnabled = false;
			UpdateUI();
		}
	}
}
