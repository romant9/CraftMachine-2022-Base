using System;
using System.Diagnostics;
using System.IO;
using UnityEngine;

public class SharePreview : HUDElement
{
	[SerializeField]
	private UITexture previewTexture;

	[SerializeField]
	private UISprite bg;

	[SerializeField]
	private int ratioPercent;

	[SerializeField]
	private UILabel buttonLabel;

	[SerializeField]
	private GameObject shareButtonGo;

	[SerializeField]
	private GameObject saveButtonGo;

	[SerializeField]
	private GameObject folderButtonGo;

	[SerializeField]
	private UILabel folderLabel;

	private Texture2D sharedTex;

	private string savedPath = "";

	public ScreenshotShare ScreenshotShare { get; set; }

	public override void Open()
	{
		base.Open();
		int width = Screen.width;
		int height = Screen.height;
		float num = (float)height / 640f;
		width = (int)((float)width / num + 0.5f);
		height = 640;
		shareButtonGo.SetActive(value: false);
		saveButtonGo.SetActive(value: true);
		folderButtonGo.SetActive(value: false);
		buttonLabel.text = LocalizationManager.GetText("Popup.Button.Share");
		bg.width = width * ratioPercent / 100;
		bg.height = height * ratioPercent / 100;
		previewTexture.height = height * ratioPercent / 100;
	}

	public void SetPreview(byte[] preview)
	{
		sharedTex = new Texture2D(2, 2);
		sharedTex.LoadImage(preview);
		float num = (float)previewTexture.height / (float)sharedTex.height;
		previewTexture.width = (int)((float)sharedTex.width * num);
		previewTexture.mainTexture = sharedTex;
	}

	public void OnShare()
	{
		ScreenshotShare.Share();
		Close();
	}

	public void OnSave()
	{
		string text = Application.persistentDataPath + "/gallery";
		if (!Directory.Exists(text))
		{
			Directory.CreateDirectory(text);
		}
		DateTime now = DateTime.Now;
		savedPath = string.Format("{0}{1:D4}{2:D2}{3:D2}_{4:D2}{5:D2}{6:D2}.png", text + "/TWD_NML_", now.Year, now.Month, now.Day, now.Hour, now.Minute, now.Second);
		byte[] bytes = sharedTex.EncodeToPNG();
		File.WriteAllBytes(savedPath, bytes);
		folderButtonGo.SetActive(value: true);
		folderLabel.text = "[u]" + folderLabel.text;
		ScreenshotShare.Share();
	}

	public void OnOpenSavedFolder()
	{
		Process.Start(Application.persistentDataPath + "/gallery");
	}

	public override void Close()
	{
		base.Close();
		ScreenshotShare.OnSharePreviewClosing();
		UIEvent.Send("OnSharePreviewClosing");
	}
}
