using System.Collections.Generic;
using UnityEngine;

public class DropRatesInfoPopup : HUDElement
{
	[SerializeField]
	private UIButton exitButton;

	[SerializeField]
	private DropRateTableList dropTableList;

	[SerializeField]
	private UITable contentsTable;

	public const string DropTableNormalPrefabPath = "DropRateTableNormal";

	public const string DropTableRadioCallPrefabPath = "DropRateTableRadioCall2";

	public void TryOpenWithNormalData(params DropTableItem[] items)
	{
		if (dropTableList != null && items != null && items.Length != 0)
		{
			if (DropRateTableNormal != null)
			{
				dropTableList.UpdateWithList(new List<DropTableItem>(items), DropRateTableNormal, DropRateTableNormal, callUpdateUI: true);
			}
			else
			{
				dropTableList.UpdateWithList(new List<DropTableItem>(items), "DropRateTableNormal", "DropRateTableNormal", callUpdateUI: true);
			}
			Open();
			if (dropTableList.currentItemsCount > 0)
			{
				dropTableList.Sort();
				contentsTable.Reposition();
				dropTableList.ResetScrollPosition();
			}
		}
	}

	public void TryOpenWithHeroData(params RadioCallTableItem[] items)
	{
		if (dropTableList != null && items != null && items.Length != 0)
		{
			if (DropRateTableRadioCall != null)
			{
				dropTableList.UpdateWithList(new List<RadioCallTableItem>(items), DropRateTableRadioCall, DropRateTableRadioCall, callUpdateUI: true);
			}
			else
			{
				dropTableList.UpdateWithList(new List<RadioCallTableItem>(items), "DropRateTableRadioCall2", "DropRateTableRadioCall2", callUpdateUI: true);
			}
			Open();
			if (dropTableList.currentItemsCount > 0)
			{
				dropTableList.Sort();
				contentsTable.Reposition();
				dropTableList.ResetScrollPosition();
			}
		}
	}

	public override void Open()
	{
		base.Open();
		SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("camp/ui_open_info");
	}


	#region myparams
	public GameObject DropRateTableNormal;
	public GameObject DropRateTableRadioCall;
	#endregion

	#region mycode
	public override void OnClickClose()
	{
		for (int i = 0; i < contentsTable.transform.childCount; i++)
		{
			contentsTable.transform.GetChild(i).gameObject.SetActive(false);
		}
		if (!OfflineManager.IsNoEffects) TweenManager.PlayTweenGroup(gameObject, 2, forward: true, OnCloseAnimOver);
		gameObject.SetActive(false);
	}
	#endregion
}
