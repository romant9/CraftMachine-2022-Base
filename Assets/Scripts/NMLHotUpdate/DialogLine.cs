using System;
using UnityEngine;

[Serializable]
public class DialogLine
{
	public string LocalizationKey;

	[TextArea(7, 3)]
	public string LocalizedText;

	[TextArea(7, 3)]
	public string ContextText;

	public DialogSource DialogSource;

	public int SourceActorTag;

	public int VoiceOverIndex;

	public string LineId;

	[HideInInspector]
	public int guid;

	public DialogLine()
	{
	}

	public DialogLine(string key)
	{
		LocalizationKey = key;
	}
}
