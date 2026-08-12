using TWDModel;
using UnityEngine;

public class SPRemoldSkillBagClassFilterItem : MonoBehaviour
{
	[SerializeField]
	private SurvivorClass filterSurvivorClass = SurvivorClass.None;

	[SerializeField]
	private GameObject selectGO;

	[SerializeField]
	private GameObject unselectGO;

	[SerializeField]
	private GameObject noticeGO;

	private void OnEnable()
	{
		UIEvent.OnUIEvent += OnUiEvent;
		UpdateNotice();
	}

	private void OnDisable()
	{
		UIEvent.OnUIEvent -= OnUiEvent;
	}

	private void OnUiEvent(string type, object parameter)
	{
		switch (type)
		{
		case "SPRemoldChangeSurvivorClassFilter":
			if (parameter != null && parameter is SurvivorClass)
			{
				if ((SurvivorClass)parameter == filterSurvivorClass)
				{
					SetSelectStatus(isSelect: true);
				}
				else
				{
					SetSelectStatus(isSelect: false);
				}
			}
			break;
		case "SPRemoldMakeModSkillSuccess":
		{
			string text = (string)parameter;
			ModSkillMode modSkillMode = GameManager.Instance.playerModel.ModSkillManager.GetModSkillMode(text);
			if (modSkillMode != null && modSkillMode.SurvivorClass == filterSurvivorClass)
			{
				UpdateNotice();
				SPRemoldSkillBagMid.SetPendingSelectAfterListRefresh(text);
				UIEvent.Send("SPRemoldChangeSurvivorClassFilter", modSkillMode.SurvivorClass);
				UIEvent.Send("SPRemoldBagItemClick", text);
			}
			break;
		}
		case "SPRemoldUpgradeModSkillSuccess":
			UpdateNotice();
			break;
		}
	}

	private void UpdateNotice()
	{
		bool value = Helpers.IsSkillKitNotice(filterSurvivorClass);
		Helpers.GameObjectSetActive(noticeGO, value);
	}

	public void SetSelectStatus(bool isSelect)
	{
		Helpers.GameObjectSetActive(selectGO, isSelect);
		Helpers.GameObjectSetActive(unselectGO, !isSelect);
	}

	public void OnFilterClicked()
	{
		UIEvent.Send("SPRemoldChangeSurvivorClassFilter", filterSurvivorClass);
	}
}
