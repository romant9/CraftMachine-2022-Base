using System.Collections;
using UnityEngine;

public class Loader : MonoBehaviour
{
	private IEnumerator Start()
	{
		if (GameConfiguration.Instance.Config.UseBundles)
		{
			yield return null;
			while (ContentPackManager.Instance.ContentPackManifest == null)
			{
				yield return null;
			}
			AsyncLoadingHandle cp0DownloadHandle = ContentPackManager.Instance.DownloadContentPack("CP0");
			while (!cp0DownloadHandle.IsFinished)
			{
				yield return null;
			}
			_ = ContentPackManager.Instance.GetContentPack("CP0") == null;
		}
		if (GameManager.Instance != null)
		{
			GameManager.Instance.LoadPersistentElementsScene();
		}
	}
}
