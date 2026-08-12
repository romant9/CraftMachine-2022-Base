public class SPRemoldSkillNotEnoughPopup : HUDElement
{
	public override void Open()
	{
		base.Open();
		UpdateUI();
	}

	public override void Close()
	{
		base.Close();
	}

	public override void UpdateUI()
	{
		base.UpdateUI();
	}

	public void OnClickGoShop()
	{
		if (OfflineManager.IsLoadDataManager) return;
		SingularityMonoBehaviour<HUDManager>.Instance.CloseAllOpenPopupsAndDialogs();
		ShopPopupHelper.OpenWithIndex(2);
	}

	public void OnClickGoPhone()
	{
		if (OfflineManager.IsLoadDataManager)
		{
			return;
			//NewPhonePopup.OpenRadiophoneFeaturePopup();
		}
		else
		{
			SingularityMonoBehaviour<HUDManager>.Instance.CloseAllOpenPopupsAndDialogs();
			NewPhonePopup.OpenRadiophoneFeaturePopup();
			(SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.NewRadioPopup) as NewPhonePopup).OnClickGoldRadio();
		}
	}
}
