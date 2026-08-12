using TWDModel;
using UnityEngine;

[GraphItNode(NodeType.Action)]
public class TextPopupClientNode : ClientNodeBase
{
	[HideInInspector]
	public string TitleID;

	[HideInInspector]
	public string BodyID;

	[Tooltip("First dialog is the title, second is the body")]
	[SerializeField]
	public CombatDialogPlayerView DialogReference;

	[GraphItInput("Activate", "")]
	public void Activate()
	{
		VisualizationQueue.Instance.Add(new CombatTextPopupVisualizationTask(delegate
		{
			CombatHUD combatHUD = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.CombatHUD) as CombatHUD;
			if (combatHUD != null)
			{
				string body = ((!string.IsNullOrEmpty(BodyID) && SingularityMonoBehaviour<LocalizationManager>.Instance.HasLocalizedText(BodyID)) ? LocalizationManager.GetText(BodyID) : "");
				combatHUD.DisplayWaveNotification(LocalizationManager.GetText(TitleID), body);
			}
		}));
	}
}
