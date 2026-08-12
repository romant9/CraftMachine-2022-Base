using UnityEngine;

public class ToolTipTextBoxAuto : HUDElement
{
	[SerializeField]
	protected UILabel label;

	[SerializeField]
	private UIScrollView desScrollView;

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
	}

	public void OpenForAuto(string str)
	{
		base.Open();
		SetText(str);
		desScrollView.ResetPosition();
		desScrollView.UpdatePosition();
	}

	public void OnclickClose()
	{
		base.Close();
	}

	public void SetText(string text)
	{
		if (label != null)
		{
			label.text = text;
		}
	}

	public void OnDestroy()
	{
	}

	public override void UpdateUI()
	{
		base.UpdateUI();
	}
}
