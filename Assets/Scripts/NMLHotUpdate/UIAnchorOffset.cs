using Client.Tweener;
using UnityEngine;

public class UIAnchorOffset : MonoBehaviour
{
	[Header("! Currently ONLY for IPhoneX !")]
	public int Left;

	public int Right;

	public int Top;

	public int Bottom;

	[ContextMenu("Awake")]
	public void Awake()
	{
		if (base.transform == null || !PlatformInfo.HasFlag(PlatformFlag.IPhoneX))
		{
			return;
		}
		TweenAnchors[] components = base.transform.GetComponents<TweenAnchors>();
		for (int i = 0; i < ((components != null) ? components.Length : 0); i++)
		{
			if (components[i] != null)
			{
				if (components[i].fromData != null)
				{
					TweenAnchorsData fromData = components[i].fromData;
					fromData.left += Left;
					fromData.right += Right;
					fromData.top += Top;
					fromData.bottom += Bottom;
					components[i].fromData = fromData;
				}
				if (components[i].toData != null)
				{
					TweenAnchorsData fromData = components[i].toData;
					fromData.left += Left;
					fromData.right += Right;
					fromData.top += Top;
					fromData.bottom += Bottom;
					components[i].toData = fromData;
				}
			}
		}
		UIRect component = base.transform.GetComponent<UIWidget>();
		if (component == null)
		{
			component = base.transform.GetComponent<UIPanel>();
		}
		if (component != null && component.isAnchored)
		{
			component.leftAnchor.absolute += Left;
			component.rightAnchor.absolute += Right;
			component.topAnchor.absolute += Top;
			component.bottomAnchor.absolute += Bottom;
		}
	}



	#region mycode
	private void OnEnable()
	{
		Awake();
	}
	#endregion
}
