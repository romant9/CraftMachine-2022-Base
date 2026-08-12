using System.Collections.Generic;
using TWDModel;
using UnityEngine;
using TwdCustomMod;

public class SPRemoldSkillBagMidItem : MonoBehaviour
{
	[SerializeField]
	private UISprite traitIcon;

	[SerializeField]
	private UILabel level;

	[SerializeField]
	private UITableList starList;

	[SerializeField]
	private UISprite traitBg;

	[SerializeField]
	private GameObject usedGo;

	[SerializeField]
	private GameObject canClockGo;

	[SerializeField]
	private GameObject noticeGo;

	[SerializeField]
	private GameObject selectGo;

	[SerializeField]
	private ModSkillMode modSkillMode;

	[SerializeField]
	private GameObject Progress;

	[SerializeField]
	private UISprite ImageProgress;

	[SerializeField]
	private UILabel TextProgress;

	private void OnEnable()
	{
		UIEvent.OnUIEvent += OnUiEvent;
	}

	private void OnDisable()
	{
		UIEvent.OnUIEvent -= OnUiEvent;
	}

	private void OnUiEvent(string type, object parameter)
	{
		if (!(type == "SPRemoldBagItemClick"))
		{
			if (type == "SPRemoldUpgradeModSkillSuccess")
			{
				UpdateUI();
			}
		}
		else if (parameter != null && parameter is string)
		{
			if ((string)parameter == modSkillMode?.ID)
			{
				Helpers.GameObjectSetActive(selectGo, value: true);
			}
			else
			{
				Helpers.GameObjectSetActive(selectGo, value: false);
			}
		}
	}

	public void Setup(ModSkillMode modSkillMode)
	{
		this.modSkillMode = modSkillMode;
		UpdateUI();
	}

	public void UpdateUI()
	{
		if (modSkillMode == null)
		{
			return;
		}
		Helpers.GameObjectSetActive(selectGo, value: false);
		PlayerModel playerModel = GameManager.Instance.playerModel;
		ModSkillManager modSkillManager = playerModel.ModSkillManager;
		SPTraitsRemoldDefinitions spTraitsDefaultTrait = modSkillMode.GetSpTraitsDefaultTrait();
		level.text = "Lv." + spTraitsDefaultTrait.Level;
		HelpersUI.SetTraitsIconOnSprite(traitIcon, spTraitsDefaultTrait.SPTraitsIcon, spTraitsDefaultTrait.SPTraitsIconOnCloud);
		traitBg.color = Helpers.HexToColor(spTraitsDefaultTrait.Color);
		Helpers.GameObjectSetActive(usedGo, modSkillMode.ModSkillState == ModSkillState.Equipped);
		bool flag = modSkillManager.CanMakeModSkill(modSkillMode.ID);
		bool flag2 = modSkillManager.CanUpgradeModSkill(modSkillMode.ID);
		Helpers.GameObjectSetActive(canClockGo, flag && modSkillManager.GetModSkillMode(modSkillMode.ID) == null);
		Helpers.GameObjectSetActive(noticeGo, flag2 && !modSkillMode.IsMaxLevel());
		starList.Setup(spTraitsDefaultTrait.Star);
		int num = 0;
		int num2 = 0;
		Dictionary<CurrencyType, int> dictionary = modSkillManager.GetMakingCost(spTraitsDefaultTrait.ID);
		if (modSkillManager.GetModSkillMode(spTraitsDefaultTrait.ID) != null)
		{
			dictionary = modSkillManager.GetUpgradeModSkillCost(spTraitsDefaultTrait.ID);
		}
		if (dictionary != null)
		{
			using Dictionary<CurrencyType, int>.Enumerator enumerator = dictionary.GetEnumerator();
			if (enumerator.MoveNext())
			{
				KeyValuePair<CurrencyType, int> current = enumerator.Current;
				num = current.Value;
				num2 = playerModel.GetCurrency(current.Key)?.Value ?? 0;
			}
		}
		Helpers.GameObjectSetActive(Progress, num > 0);
		ImageProgress.fillAmount = ((num > 0) ? Mathf.Clamp01((float)num2 / (float)num) : 0f);
		if (num2 >= num && num > 0)
		{
			Helpers.GameObjectSetActive(TextProgress.gameObject, value: false);
			return;
		}
		Helpers.GameObjectSetActive(TextProgress.gameObject, value: true);
		TextProgress.text = $"{num2}/{num}";
	}

	public void Onclick()
	{
		if (modSkillMode != null)
		{
			UIEvent.Send("SPRemoldBagItemClick", modSkillMode.ID);
		}

		if (DataManager.Instance.IsCopyImageToBuffer)
		{
			var cdnTex = traitIcon.GetComponentInChildren<UITexture>();
			if (cdnTex != null && cdnTex.mainTexture != null)
			{
				UniversalClipboardManager.CopyToClipboard(cdnTex.mainTexture);
			}
			else
			{
				if (traitIcon.mainTexture != null)
				{
					UniversalClipboardManager.CopyToClipboard(traitIcon.mainTexture);
				}
			}		
		}
	}
}
