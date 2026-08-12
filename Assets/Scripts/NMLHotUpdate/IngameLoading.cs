using UnityEngine;

public class IngameLoading : HUDElement
{
	[SerializeField]
	private UILabel text;

	[SerializeField]
	private GameObject background;

	[SerializeField]
	private UITweener backgroundTweener;

	public bool isShowLootCard;

	public override void Close()
	{
		base.Close();
		isShowLootCard = false;
	}

	private void OnEnable()
	{
		CampView instance = CampView.Instance;
		if (instance != null)
		{
			instance.EnableCampControls(enable: false);
		}
		text.gameObject.SetActive(value: false);
		background.SetActive(value: false);
	}

	public void OnDisable()
	{
		CampView instance = CampView.Instance;
		if (instance != null)
		{
			instance.EnableCampControls(enable: true);
		}
	}

	public void SetText(string s, bool blockInputWithBackground = true)
	{
		CampView instance = CampView.Instance;
		if (instance != null)
		{
			instance.EnableCampControls(!blockInputWithBackground);
		}
		background.SetActive(blockInputWithBackground);
		PlayTween(blockInputWithBackground);
		text.gameObject.SetActive(value: true);
		text.text = s;
	}

	private void PlayTween(bool play)
	{
		if (backgroundTweener != null && play)
		{
			backgroundTweener.ResetToBeginning();
			backgroundTweener.PlayForward();
		}
	}
}
