using UnityEngine;

[RequireComponent(typeof(UIButton))]
public class ButtonOpenURL : MonoBehaviour
{
	[Tooltip("URL that'll be opened upon clicking the button.")]
	public string url = "";

	private void OnClick()
	{
		if (!string.IsNullOrEmpty(url))
		{
			Application.OpenURL(url);
		}
	}
}
