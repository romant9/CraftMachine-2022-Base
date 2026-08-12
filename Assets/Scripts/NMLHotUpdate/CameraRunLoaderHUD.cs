using UnityEngine.SceneManagement;

public class CameraRunLoaderHUD : HUDElement
{
	public void OnRun1Pressed()
	{
		Close();
		SceneManager.LoadScene("town_house");
	}
}
