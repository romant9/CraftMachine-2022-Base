using UnityEngine;

public class QuitPopup : HUDElement
{
	public void OnQuitGame()
	{
		Application.Quit();
	}
}
