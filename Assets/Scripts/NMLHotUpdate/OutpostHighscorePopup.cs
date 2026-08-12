using UnityEngine;

public class OutpostHighscorePopup : HUDElement
{
	public GameObject GuildTab;

	public void OnBackClicked()
	{
		Close();
		SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.OutpostPopupManagement).Open();
	}

	public void OnCloseClicked()
	{
		Close();
	}

	public override void Open()
	{
		base.Open();
		GuildTab.SetActive(GameManager.Instance.playerModel.HasGuild);
	}

	public override void Close()
	{
		base.Close();
		SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("camp/social_close");
	}
}
