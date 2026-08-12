using System.Collections.Generic;
using Client.Constants;
using TWDModel;
using UnityEngine;

public class MoveActionIndicator : HUDElementFollowTarget
{
	public Color attackBGColor = Color.red;

	public Color neutralBGColor = Color.white;

	public Color sprintMoveBGColor = Color.blue;

	public CoverIndicator CoverIndicator;

	public UISprite AttackCoverIcon;

	public UISprite BGIcon;

	public GameObject ActionIconObject;

	public GameObject turnCountRoot;

	public UILabel turnCountLabel;

	public UILabel turnTextLabel;

	public GameObject APCountRoot;

	public UILabel APCountLabel;

	public HealthIndicator healthIndicator;

	public List<MoveActionEntry> Actions = new List<MoveActionEntry>();

	private GameObject groundIndicator;

	public void ShowIndicator(MoveActionType actionType, GridCoordinate coordinate, GridCoordinate actionFromCoordinate)
	{
		CombatModel combat = GameManager.Instance.playerModel.Combat;
		SetIcon(actionType);
		CoverIndicator.SetCoverDirections(null);
		AttackCoverIcon.gameObject.SetActive(value: false);
		ActorView actorView = null;
		ActorModel occupier = combat.GetOccupier(coordinate);
		if (occupier != null)
		{
			actorView = GameManager.Instance.GetViewForModel(occupier) as ActorView;
			if (actionType == MoveActionType.Melee && occupier.IsEnemyNPC)
			{
				BGIcon.color = attackBGColor;
				if (healthIndicator != null)
				{
					healthIndicator.gameObject.SetActive(value: true);
					float value = (float)occupier.Hitpoints / (float)occupier.MaxHitPoints;
					healthIndicator.HealthBar.value = value;
					healthIndicator.ActorClass.spriteName = HelpersGfx.GetHealthbarClassIconName(occupier);
					if (actorView != null)
					{
						healthIndicator.ActorClass.color = actorView.HealthIndicator.ActorClass.color;
						healthIndicator.HealthBar.foregroundWidget.color = actorView.HealthIndicator.HealthBar.foregroundWidget.color;
					}
					healthIndicator.LevelLabel.text = (occupier.IsEnvironmental ? "" : occupier.Level.ToString());
					healthIndicator.ToughWalkerIcon.gameObject.SetActive(occupier.IsBoss);
					Helpers.GameObjectSetActive(healthIndicator.BossWalkerIcon, occupier.IsBossWalker);
				}
				if (!occupier.IsWalker && combat.HasCover(occupier.GridCoordinate))
				{
					AttackCoverIcon.gameObject.SetActive(value: true);
					AttackCoverIcon.spriteName = HelpersGfx.GetCoverIconName(CoverIconState.Flanked);
				}
			}
			else if (actionType == MoveActionType.Shoot && occupier.IsEnemyNPC)
			{
				BGIcon.color = attackBGColor;
				if (healthIndicator != null)
				{
					healthIndicator.gameObject.SetActive(value: true);
					float value2 = (float)occupier.Hitpoints / (float)occupier.MaxHitPoints;
					healthIndicator.HealthBar.value = value2;
					healthIndicator.ActorClass.spriteName = HelpersGfx.GetHealthbarClassIconName(occupier);
					if (actorView != null)
					{
						healthIndicator.ActorClass.color = actorView.HealthIndicator.ActorClass.color;
						healthIndicator.HealthBar.foregroundWidget.color = actorView.HealthIndicator.HealthBar.foregroundWidget.color;
					}
					healthIndicator.LevelLabel.text = (occupier.Definition.IsEnvironmental ? "" : occupier.Level.ToString());
					healthIndicator.ToughWalkerIcon.gameObject.SetActive(occupier.IsBoss);
					Helpers.GameObjectSetActive(healthIndicator.BossWalkerIcon, occupier.IsBossWalker);
				}
				if (!occupier.IsWalker && combat.HasCover(occupier.GridCoordinate))
				{
					AttackCoverIcon.gameObject.SetActive(value: true);
					if (combat.IsCoverFlankedAfterMove(occupier.GridCoordinate, occupier, actionFromCoordinate, combat.TurnManager.ActiveActor))
					{
						AttackCoverIcon.spriteName = HelpersGfx.GetCoverIconName(CoverIconState.Flanked);
					}
					else
					{
						AttackCoverIcon.spriteName = HelpersGfx.GetCoverIconName(CoverIconState.HalfCover);
					}
					if (SingularityMonoBehaviour<AudioManager>.Instance != null)
					{
						SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("combat_ui/cover_target");
					}
				}
			}
		}
		else
		{
			switch (actionType)
			{
			case MoveActionType.MoveSprint:
				BGIcon.color = sprintMoveBGColor;
				if (healthIndicator != null)
				{
					healthIndicator.gameObject.SetActive(value: false);
				}
				break;
			case MoveActionType.Cover:
			{
				List<CoverDirection> coverDirections = combat.GetCoverDirections(coordinate);
				CoverIconState coverState = ((!combat.IsCoverFlanked(coordinate, combat.TurnManager.ActiveActor)) ? CoverIconState.HalfCover : CoverIconState.Flanked);
				CoverIndicator.SetCoverDirections(coverDirections, coverState);
				BGIcon.color = neutralBGColor;
				if (healthIndicator != null)
				{
					healthIndicator.gameObject.SetActive(value: false);
				}
				break;
			}
			default:
				BGIcon.color = neutralBGColor;
				if (healthIndicator != null)
				{
					healthIndicator.gameObject.SetActive(value: false);
				}
				break;
			}
		}
		base.gameObject.SetActive(value: true);
	}

	public void ShowGroundIndicator(MoveActionType actionType)
	{
		if (!(groundIndicator != null))
		{
			return;
		}
		groundIndicator.gameObject.SetActive(value: true);
		Renderer componentInChildren = groundIndicator.GetComponentInChildren<Renderer>();
		if (componentInChildren != null)
		{
			switch (actionType)
			{
			case MoveActionType.Melee:
				componentInChildren.material.SetColor(MaterialParameters.TintColor, attackBGColor);
				break;
			case MoveActionType.Shoot:
				componentInChildren.material.SetColor(MaterialParameters.TintColor, attackBGColor);
				break;
			case MoveActionType.MoveSprint:
				componentInChildren.material.SetColor(MaterialParameters.TintColor, sprintMoveBGColor);
				break;
			default:
				componentInChildren.material.SetColor(MaterialParameters.TintColor, neutralBGColor);
				break;
			}
		}
	}

	public void SetPosition(Vector3 pos)
	{
		if (groundIndicator == null)
		{
			InstantiateGroundIndicator(pos);
		}
		groundIndicator.transform.position = pos;
		FollowTarget(groundIndicator);
		Vector3 vector = Camera.main.WorldToScreenPoint(pos);
		float num = (float)Screen.height - vector.y;
		if (num < (float)BGIcon.height)
		{
			bool flag = vector.x > (float)Screen.width / 2f;
			float num2 = Mathf.Atan2(BGIcon.height, num) * 57.29578f;
			base.gameObject.transform.rotation = Quaternion.Euler(0f, 0f, flag ? num2 : (0f - num2));
			turnCountLabel.transform.rotation = Quaternion.identity;
			UISprite[] componentsInChildren = ActionIconObject.GetComponentsInChildren<UISprite>(includeInactive: true);
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				componentsInChildren[i].gameObject.transform.rotation = Quaternion.identity;
			}
		}
		else
		{
			base.gameObject.transform.rotation = Quaternion.identity;
			turnCountLabel.transform.rotation = Quaternion.identity;
			UISprite[] componentsInChildren = ActionIconObject.GetComponentsInChildren<UISprite>(includeInactive: true);
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				componentsInChildren[i].gameObject.transform.rotation = Quaternion.identity;
			}
		}
	}

	public void HideIndicator()
	{
		base.gameObject.SetActive(value: false);
	}

	public void HideGroundIndicator()
	{
		if (groundIndicator != null)
		{
			groundIndicator.gameObject.SetActive(value: false);
			Renderer componentInChildren = groundIndicator.GetComponentInChildren<Renderer>();
			if (componentInChildren != null)
			{
				componentInChildren.material.SetColor(MaterialParameters.TintColor, neutralBGColor);
			}
		}
	}

	public void SetTurnCount(int count)
	{
		if (count > 0)
		{
			turnCountRoot.SetActive(value: true);
			turnCountLabel.text = count.ToString();
			if (count == 1)
			{
				turnTextLabel.text = LocalizationManager.GetText("Combat.Indicator.TurnCountSingular");
			}
			else
			{
				turnTextLabel.text = LocalizationManager.GetText("Combat.Indicator.TurnCount");
			}
		}
		else
		{
			turnCountRoot.SetActive(value: false);
		}
	}

	public void SetAPCount(int count)
	{
		if (count > 0)
		{
			string textId = ((count == 1) ? "Combat.Indicator.APCountHalf" : "Combat.Indicator.APCountFull");
			APCountRoot.SetActive(value: true);
			APCountLabel.text = LocalizationManager.GetText(textId);
		}
		else
		{
			APCountRoot.SetActive(value: false);
		}
	}

	private void SetIcon(MoveActionType type)
	{
		foreach (MoveActionEntry action in Actions)
		{
			if (action.MoveActionType == type)
			{
				action.Marker.SetActive(value: true);
			}
			else
			{
				action.Marker.SetActive(value: false);
			}
		}
	}

	private void InstantiateGroundIndicator(Vector3 pos)
	{
		groundIndicator = CombatView.Instance.CombatHUD.CreateMoveGroundIndicator();
		groundIndicator.transform.parent = Object.FindObjectOfType<Scenario>().transform;
	}
}
