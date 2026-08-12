using BaseModel;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CallGridItem : MonoBehaviour
{
	public bool IsAccepted { get; set; }
	public List<LootEntry> LootEntryList;

	public List<UISprite> tokenSprites;
	public List<UILabel> tokenValues;

	public List<Transform> AcceptIndexesGroups;
	public List<List<UIToggle>> AcceptIndexes;
	public Dictionary<string, ModelRandom> CallRandomCurrent { get; set; } //текущаяя - перед крафтом

	public int SummTokenValues;
	public UILabel SummTokenValuesLabel;

	public int CallIndex { get; set; }
	public UILabel callIndexLabel;

	public int CallType { get; set; }
	public UILabel CallTypeLabel;

	public int CallPrice { get; set; }
	public UILabel CallPriceLabel;

	public string CallInfoStr { get; set; }
	public UILabel CallInfoLabel;

	public int RerollCount {  get; set; }

	public ShowTooltip toolTip;

	public RadioCallButton CallButton { get; set; }
	public bool IsMaxSumm { get; private set; }


	void Start()
	{
	}

	public void Init(bool IsModified)
	{
		LootEntryList = new List<LootEntry>();

		for (int i = 0; i < tokenSprites.Count; i++)
		{
			tokenSprites[i].gameObject.transform.parent.gameObject.SetActive(false);
		}

		AcceptIndexes = new List<List<UIToggle>>();
		for (int i = 0; i < AcceptIndexesGroups.Count; i++)
		{
			AcceptIndexesGroups[i].gameObject.SetActive(true);

			var uIToggles = AcceptIndexesGroups[i].GetComponentsInChildren<UIToggle>(true).ToList();
			foreach (var tg in uIToggles)
			{
				tg.Set(false);
			}
			AcceptIndexes.Add(uIToggles);
			AcceptIndexesGroups[i].gameObject.SetActive(false);
		}

		if (!IsModified)
		{
			CallIndex = 0;
			callIndexLabel.text = CallIndex.ToString();
			CallType = 0;
			CallTypeLabel.text = CallType.ToString();
			CallPrice = 0;
			CallPriceLabel.text = CallPrice.ToString();
			CallInfoStr = "";
			CallInfoLabel.text = CallInfoStr;
			SummTokenValues = 0;
			SummTokenValuesLabel.text = SummTokenValues.ToString();
		}
		RerollCount = 0;

		CallCraft.Instance.CalculateHeroTokenQueue();
	}

	//OnClick
	public void OnCallClick()
	{
		if (CallCraft.Instance.IsVisualized)
		{
			DebugTWD.Log("Это лист джекпота. Return");
			return;
		}
		bool IsSelected = GetComponent<UIButtonToggle>().IsToggled;
		IsAccepted = true;
		CallCraft.Instance.OnClickCallItem(this, IsSelected);
	}

	public void SetData(CallGridItem currentCallGridItem)
	{
		LootEntryList = currentCallGridItem.LootEntryList;
		tokenSprites = currentCallGridItem.tokenSprites;
		tokenValues = currentCallGridItem.tokenValues;

		CallRandomCurrent = currentCallGridItem.CallRandomCurrent;
		AcceptIndexes = currentCallGridItem.AcceptIndexes;

		CallType = currentCallGridItem.CallType;
		CallPrice = currentCallGridItem.CallPrice;
	}

	public void SetData(int index, CallCraft.CallInfo info, Dictionary<string, ModelRandom> randomDic)
	{
		CallIndex = index;
		CallPrice = info.Price;
		CallType = info.CurrentTypeIndex;
		CallInfoStr = info.CurrentCallInfo;
		CallButton = info.CallButton;
		CallRandomCurrent = randomDic;
	}

	public void UpdateUI()
	{
		callIndexLabel.text = (CallIndex + 1).ToString();
		CallTypeLabel.text = CallType.ToString();
		CallPriceLabel.text = CallPrice.ToString();
		CallInfoLabel.text = CallInfoStr;

		SummTokenValues = 0;

		for (int i = 0; i < LootEntryList.Count; i++)
		{
			LootEntry loot = LootEntryList[i];

			if (loot.DropCurrencyType == TWDModel.DropCurrenciesProbabilitiesDefinition.DropCurrency.HeroToken)
			{
				int RewardedAmount = HelpersUI.GetActualRewardValue(CallButton, loot.RewardedAmount);
				if (RewardedAmount == GetMaxCallValue())
				{
					tokenValues[i].color = Color.green;
				}

				SummTokenValues += RewardedAmount;
			}
			else
			{
				SummTokenValues += loot.GeneratedSurvivor.DemoteTokens;
			}
		}

		SummTokenValuesLabel.text = SummTokenValues.ToString();

		DebugTWD.Log("Call Finish, CallPrice: " + CallPrice);
	}

	private int GetMaxCallValue()
	{
		return CallButton.parsedHeroTokensDropNumberValues.Last();
	}

	public void UpdateLabel(bool isMax)
	{
		SummTokenValuesLabel.color = isMax ? Color.green : Color.white;
		IsMaxSumm = isMax;
	}
}
