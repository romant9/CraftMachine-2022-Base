using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class DebugAnalytics : SingularityMonoBehaviour<DebugAnalytics>
{
	private StringBuilder analyticsSentBuilder;

	private StringBuilder analyticsIndividualMessageSentBuilder;

	private List<string> builtAnalyticsLines;

	private string analyticsSent;

	private Vector2 scrollPosition;

	private bool minimized;

	private const int maxAnalytics = 4;

	private int totalNumAnalytics;

	private float sizeScale;

	private GUIStyle guiStyle;

	private void OnEnable()
	{
		EventManager.OnEvent += OnEvent;
		analyticsSentBuilder = new StringBuilder();
		analyticsIndividualMessageSentBuilder = new StringBuilder();
		builtAnalyticsLines = new List<string>();
		analyticsSent = "";
		sizeScale = 1.5f;
		guiStyle = new GUIStyle();
		guiStyle.fontSize = (int)(20f * sizeScale);
		guiStyle.normal.textColor = Color.white;
		Texture2D texture2D = new Texture2D(128, 128);
		guiStyle.normal.background = null;
		for (int i = 0; i < texture2D.height; i++)
		{
			for (int j = 0; j < texture2D.width; j++)
			{
				texture2D.SetPixel(j, i, Color.black);
			}
		}
		texture2D.Apply();
	}

	private void OnDisable()
	{
		EventManager.OnEvent -= OnEvent;
	}

	private void OnEvent(EventManager.EventType eventtype, object parameter)
	{
		if (eventtype != EventManager.EventType.AnalyticsSent)
		{
			return;
		}
		if (builtAnalyticsLines.Count >= 4)
		{
			builtAnalyticsLines.RemoveAt(0);
			analyticsSent = "";
		}
		totalNumAnalytics++;
		analyticsIndividualMessageSentBuilder.Length = 0;
		object[] array = parameter as object[];
		analyticsIndividualMessageSentBuilder.Append(array[0] as string);
		analyticsIndividualMessageSentBuilder.Append("\n");
		string text = UnityUtils.DumpDictionary(array[1] as Dictionary<string, string>);
		int num = 130;
		int length = text.Length;
		for (int i = 0; i < length; i += num)
		{
			if (i + num > length)
			{
				num = length - i;
			}
			analyticsIndividualMessageSentBuilder.Append(text.Substring(i, num));
			analyticsIndividualMessageSentBuilder.Append("\n");
		}
		builtAnalyticsLines.Add(analyticsIndividualMessageSentBuilder.ToString());
		analyticsSentBuilder.Length = 0;
		foreach (string builtAnalyticsLine in builtAnalyticsLines)
		{
			analyticsSentBuilder.Append(builtAnalyticsLine);
		}
		analyticsSent = analyticsSentBuilder.ToString();
	}

	private void OnGUI()
	{
		if (!minimized)
		{
			scrollPosition = GUI.BeginScrollView(new Rect(100f * sizeScale, 50f * sizeScale, 800f * sizeScale, 200f * sizeScale), scrollPosition, new Rect(0f, 0f, 920f * sizeScale, 300f * sizeScale), alwaysShowHorizontal: false, alwaysShowVertical: false);
			GUILayout.TextField(analyticsSent, guiStyle);
			GUI.EndScrollView();
		}
		if (minimized)
		{
			if (GUI.Button(new Rect(920f * sizeScale, 100f * sizeScale, 50f * sizeScale, 50f * sizeScale), "+", guiStyle))
			{
				minimized = false;
			}
		}
		else if (GUI.Button(new Rect(920f * sizeScale, 100f * sizeScale, 50f * sizeScale, 50f * sizeScale), "-", guiStyle))
		{
			minimized = true;
		}
		GUI.TextField(new Rect(920f * sizeScale, 50f * sizeScale, 100f * sizeScale, 30f * sizeScale), "Total: " + totalNumAnalytics, guiStyle);
	}
}
