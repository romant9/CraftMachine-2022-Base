using System.Collections.Generic;
using System.Linq;
using BaseModel;
using TWDModel;
using UnityEngine;

public class BounsPopup : HUDElement
{
	[SerializeField]
	private BounsListPanel bounsListPanel;

	[SerializeField]
	private UILabel nameLabel;

	[SerializeField]
	private UILabel pageLabel;

	[SerializeField]
	[Tooltip("Label show when there is some info")]
	private UILabel infoLabel;

	[SerializeField]
	private UISprite infoSprite;

	[SerializeField]
	[Tooltip("Time to show in seconds")]
	private float timeToShow = 4f;

	[SerializeField]
	private Color errorColor;

	[SerializeField]
	private Color normalColor;

	private SurvivorModel survivorModel { get; set; }

	private List<BounsInfo> bounsInfos { get; set; }

	public override void OpenForModel(ModelObject model)
	{
		base.OpenForModel(model);
		survivorModel = model as SurvivorModel;
		bounsInfos = new List<BounsInfo>();
		CreateBounsInfoData();
		InternalHide();
		nameLabel.text = survivorModel?.FullName;
	}

	private void CreateBounsInfoData()
	{
		bounsInfos.Clear();
		List<BounsInfoDefinition> bounsInfoDefinitionsByOwner = GameManager.Instance.gameEconomyData.GetBounsInfoDefinitionsByOwner(survivorModel?.Definition?.ID);
		ModelList<BounsModel> bounsModes = GameManager.Instance.playerModel.Equipment.BounsModes;
		for (int i = 0; i < bounsInfoDefinitionsByOwner.Count; i++)
		{
			BounsInfoDefinition bounsInfoDefinition = bounsInfoDefinitionsByOwner[i];
			IsContains(bounsModes, bounsInfoDefinition.ItemID, out var level, out var bounsModel);
			bounsInfos.Add(new BounsInfo(level, survivorModel, bounsInfoDefinition, bounsModel));
		}
		bounsInfos = bounsInfos.OrderByDescending((BounsInfo info) => info.Level).ToList();
		if (bounsListPanel != null)
		{
			bounsListPanel.Init(bounsInfos);
		}
		if (pageLabel != null)
		{
			pageLabel.text = $"{1}/{bounsInfos.Count}";
		}
	}

	private bool IsContains(ModelList<BounsModel> modelList, int id, out int level, out BounsModel bounsModel)
	{
		level = 0;
		bounsModel = null;
		if (modelList == null)
		{
			return false;
		}
		for (int i = 0; i < modelList.Count; i++)
		{
			if (modelList[i].ItemID == id)
			{
				level = modelList[i].Level;
				bounsModel = modelList[i];
				return true;
			}
		}
		return false;
	}

	private void ShowInfo(string text, bool isError = false)
	{
		InternalHide();
		SetInfoText(infoLabel, text);
		infoSprite.color = (isError ? errorColor : normalColor);
	}

	private void SetInfoText(UILabel label, string text)
	{
		if (label != null && label.gameObject != null)
		{
			label.gameObject.SetActive(value: true);
			label.text = text;
			CancelInvoke("InternalHide");
			Invoke("InternalHide", timeToShow);
		}
		else
		{
			Debug.LogError("HUDNotification: Could not show notification because label is NULL!");
		}
	}

	private void InternalHide()
	{
		if (infoLabel != null && infoLabel.gameObject != null)
		{
			infoLabel.gameObject.SetActive(value: false);
		}
	}

	private void OnEnable()
	{
		UIEvent.OnUIEvent += OnUIEvent;
	}

	private void OnDisable()
	{
		UIEvent.OnUIEvent -= OnUIEvent;
	}

	private void OnUIEvent(string type, object parameter)
	{
		if (type == "BounsUpgrade")
		{
			CreateBounsInfoData();
			if (parameter is string text)
			{
				ShowInfo(text);
			}
		}
		else if (type == "BounsInfo" && parameter is string text2)
		{
			ShowInfo(text2, isError: true);
		}
	}
}
