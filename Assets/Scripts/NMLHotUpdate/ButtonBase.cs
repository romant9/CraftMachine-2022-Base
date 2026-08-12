using System;
using UnityEngine;

[RequireComponent(typeof(UIButton))]
public class ButtonBase : MonoBehaviour
{
	public delegate void ButtonBaseCallback(ButtonBase origin);

	private ButtonBaseCallback CallbackInternal;

	private string IdInternal = "";

	private UIButton UIButtonInternal;

	private bool InputEnabledInternal = true;

	public string id
	{
		get
		{
			if (IdInternal != null)
			{
				return IdInternal;
			}
			Debug.LogError("ButtonBase: No Label Found!");
			return "";
		}
		set
		{
			if (IdInternal != null)
			{
				IdInternal = value;
			}
			else
			{
				Debug.LogError("ButtonBase: No Label Found!");
			}
		}
	}

	public bool InputEnabled
	{
		get
		{
			return InputEnabledInternal;
		}
		set
		{
			InputEnabledInternal = value;
		}
	}

	public UIButton Button
	{
		get
		{
			if (UIButtonInternal == null)
			{
				UIButtonInternal = GetComponent<UIButton>();
			}
			return UIButtonInternal;
		}
	}

	public virtual void OnClick()
	{
		if (CallbackInternal != null && InputEnabledInternal)
		{
			CallbackInternal(this);
		}
	}

	public void SetCallback(ButtonBaseCallback callback)
	{
		CallbackInternal = (ButtonBaseCallback)Delegate.Remove(CallbackInternal, callback);
		CallbackInternal = (ButtonBaseCallback)Delegate.Combine(CallbackInternal, callback);
	}

	public void RemoveCallback(ButtonBaseCallback callback)
	{
		CallbackInternal = (ButtonBaseCallback)Delegate.Remove(CallbackInternal, callback);
	}

	public virtual void Clear()
	{
		UIButtonInternal = null;
		CallbackInternal = null;
	}
}
