using TWDModel;
using UnityEngine;

public class SurvivorClassIcon : MonoBehaviour
{
	private ActorDefinition actorDefinition;

	public UISprite Icon;

	public ActorDefinition ActorDefinition
	{
		get
		{
			return actorDefinition;
		}
		set
		{
			actorDefinition = value;
			Vector3 localScale = Icon.gameObject.transform.localScale;
			string text = "Ui_Icon_Class_" + actorDefinition.Class;
			base.gameObject.SetActive(!string.IsNullOrEmpty(text));
			Icon.spriteName = text;
			Icon.MakePixelPerfect();
			Icon.gameObject.transform.localScale = localScale;
		}
	}
}
