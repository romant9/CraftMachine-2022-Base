using TWDModel;
using UnityEngine;

[GraphItNode(NodeType.Action)]
public class TextPopupClientNode : ClientNodeBase
{
	[HideInInspector]
	public string TitleID;

	[HideInInspector]
	public string BodyID;

	[GraphItVariable("Use custom localization key")]
	public bool UseCustomKey;

	[GraphItVariable("Localization key")]
	public string CustomLocalizationKey;

	[Tooltip("First dialog line is the popup text; an optional second line is shown as body text. Ignored when Use custom localization key is enabled.")]
	[SerializeField]
	public CombatDialogPlayerView DialogReference;

	[GraphItInput("Activate", "")]
	public void Activate()
	{
		string heading;
		string body;
		if (UseCustomKey)
		{
			heading = LocalizationManager.GetText(CustomLocalizationKey);
			body = "";
		}
		else
		{
			heading = LocalizationManager.GetText(TitleID);
			body = ((!string.IsNullOrEmpty(BodyID) && SingularityMonoBehaviour<LocalizationManager>.Instance.HasLocalizedText(BodyID)) ? LocalizationManager.GetText(BodyID) : "");
		}
		VisualizationQueue.Instance.Add(new CombatTextPopupVisualizationTask(delegate
		{
			CombatHUD combatHUD = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.CombatHUD) as CombatHUD;
			if (combatHUD != null)
			{
				combatHUD.DisplayWaveNotification(heading, body);
			}
		}));
	}
}
