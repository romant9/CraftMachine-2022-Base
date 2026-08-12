using UnityEngine;

public class PopupScanQuickTip : HUDElement
{
	private bool mousePressed;

	public override void Open()
	{
		base.Open();
		mousePressed = false;
	}

	public override void Close()
	{
		base.Close();
	}

	private void OnEnable()
	{
		mousePressed = false;
	}

	private void LateUpdate()
	{
		if (base.IsOpen)
		{
			if (Input.GetMouseButtonDown(0))
			{
				mousePressed = true;
			}
			if (Input.GetMouseButtonUp(0))
			{
				_ = mousePressed;
				mousePressed = false;
			}
		}
	}

	private void OnClick()
	{
	}
}
