using System;
using System.Collections.Generic;

[Serializable]
public class HardCodedTutorialData
{
	public string PortraitId;

	public List<string> Localizations;

	public List<object> LocalizationArguments;

	public List<string> UIElementsToHighlight;

	public bool ShowDialogOnCenter;

	public float TutorialStartDelay;
}
