using TWDModel;
using UnityEngine;

public class BounsPortrait : MonoBehaviour
{
	[SerializeField]
	private UITexture portrait;

	private ActorDefinition actorDefinition;

	public void Init(string heroId)
	{
		actorDefinition = GameManager.Instance.gameEconomyData.GetActorDefinition(heroId);
		bool flag = GameManager.Instance.playerModel.SurvivorContainer.HasHero(heroId);
		portrait.color = (flag ? Color.white : Color.grey);
		portrait.gameObject.SetActive(value: false);
		UpdatePortrait();
	}

	private void UpdatePortrait()
	{
		if (actorDefinition != null && GameManager.Instance != null)
		{
			if (!(portrait != null) || !(portrait.gameObject != null) || !(PortraitManager.Instance != null))
			{
				return;
			}
			PortraitRenderSource info = PortraitRenderSource.fromActorDefinition(actorDefinition);
			Texture texture = PortraitManager.Instance.GetPortrait(info);
			if (texture == null)
			{
				ModularCharacter modularCharacter = ActorView.GetPrefabForActor(actorDefinition.ID, actorDefinition.VisualAsset);
				if (modularCharacter == null)
				{
					modularCharacter = ActorView.SelectRandomPrefabForActorDefinition(actorDefinition.ID, actorDefinition.Gender);
				}
				PortraitManager.Instance.CreatePortrait(PortraitRenderSource.fromActorDefinition(actorDefinition), modularCharacter, OnMissingPortraitRendered);
				portrait.gameObject.SetActive(value: false);
			}
			else
			{
				portrait.mainTexture = texture;
				portrait.gameObject.SetActive(value: true);
			}
		}
		else if (portrait != null && portrait.gameObject != null)
		{
			portrait.gameObject.SetActive(value: false);
		}
	}

	private void OnMissingPortraitRendered(IPortraitRenderSource info)
	{
		if (portrait != null && info != null && actorDefinition.ID == info.ActorDefinitionId)
		{
			portrait.mainTexture = PortraitManager.Instance.GetPortrait(info);
			portrait.gameObject.SetActive(value: true);
		}
	}
}
