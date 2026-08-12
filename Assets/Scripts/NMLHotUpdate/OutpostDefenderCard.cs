using TWDModel;
using UnityEngine;

public class OutpostDefenderCard : MonoBehaviour
{
	[SerializeField]
	private UILabel nameLabel;

	[SerializeField]
	private UISprite classIcon;

	[SerializeField]
	private UILabel levelLabel;

	[SerializeField]
	private UITexture portrait;

	[SerializeField]
	private GameObject deadContainer;

	private SurvivorModel Item;

	public void Setup(int defenderIndex)
	{
		CombatModel combat = GameManager.Instance.playerModel.Combat;
		if (combat == null || combat.OutpostCombat == null)
		{
			return;
		}
		Item = combat.OutpostCombat.DefendingSurvivors[defenderIndex];
		nameLabel.text = Item.Name;
		classIcon.spriteName = HelpersGfx.GetSurvivorClassIconName(Item);
		levelLabel.text = Item.Level.ToString();
		if (PortraitManager.Instance != null)
		{
			PortraitRenderSource info = PortraitRenderSource.fromActorModel(Item);
			if (PortraitManager.Instance.GetPortrait(info) == null)
			{
				ModularCharacter prefabForActor = ActorView.GetPrefabForActor(Item);
				PortraitManager.Instance.CreatePortrait(info, prefabForActor, OnMissingPortraitRendered);
				portrait.gameObject.SetActive(value: false);
			}
			else
			{
				portrait.mainTexture = PortraitManager.Instance.GetPortrait(info);
				portrait.gameObject.SetActive(value: true);
			}
		}
		bool pvPDefenderKilled = GameManager.Instance.playerModel.Combat.GetPvPDefenderKilled(defenderIndex);
		base.gameObject.SetActive(pvPDefenderKilled);
	}

	public void RenderPortrait(SurvivorModel defender)
	{
		if (PortraitManager.Instance != null && PortraitManager.Instance.GetPortrait(PortraitRenderSource.fromActorModel(defender)) == null)
		{
			ModularCharacter prefabForActor = ActorView.GetPrefabForActor(defender);
			PortraitManager.Instance.CreatePortrait(PortraitRenderSource.fromActorModel(defender), prefabForActor, OnMissingPortraitRendered);
		}
	}

	private void OnMissingPortraitRendered(IPortraitRenderSource info)
	{
		if (portrait != null && info != null && Item.ActorDefinitionID == info.ActorDefinitionId)
		{
			portrait.mainTexture = PortraitManager.Instance.GetPortrait(info);
			portrait.gameObject.SetActive(value: true);
		}
	}
}
