using System.Collections.Generic;
using BaseModel;
using Client.Framework.Audio;
using TWDModel;
using UnityEngine;

public class InteractiveObjectView : ModelView<InteractiveObjectModel>, InteractionReceiver, TriggerReceiver
{
	[SerializeField]
	[Tooltip("Sound type to be used when interacting with the object.")]
	public SoundType interactionSoundType = SoundType.InteractiveObject_Loot_Vehicle;

	public bool playSound = true;

	public IndicatorType IndicatorType;

	public bool ShowIndicator;

	public bool SkipUseAnimation;

	public Material glowIndicatorMaterial;

	public Vector2 glowScrollSpeed = new Vector2(0.5f, 0f);

	public GameObject[] glowSourceMeshes;

	private List<GameObject> glowGameObjects;

	private ActionIndicator actionIndicator;

	public override bool AutoGenerateViewID => true;

	public void Stop()
	{
		RemoveGlow();
	}

	private void CreateGlow()
	{
		if (!(glowIndicatorMaterial != null) || glowSourceMeshes == null || glowSourceMeshes.Length == 0 || (glowGameObjects != null && glowGameObjects.Count != 0))
		{
			return;
		}
		glowGameObjects = new List<GameObject>();
		for (int i = 0; i < glowSourceMeshes.Length; i++)
		{
			if (!(glowSourceMeshes[i] == null))
			{
				GameObject gameObject = new GameObject(glowSourceMeshes[i].name + "_InteractionIndicator");
				gameObject.transform.parent = glowSourceMeshes[i].transform;
				gameObject.transform.localPosition = new Vector3(0f, 0f, 0f);
				gameObject.transform.localEulerAngles = new Vector3(0f, 0f, 0f);
				gameObject.transform.localScale = new Vector3(1f, 1f, 1f);
				MeshFilter meshFilter = gameObject.AddMissingComponent<MeshFilter>();
				gameObject.AddMissingComponent<MeshRenderer>();
				meshFilter.mesh = glowSourceMeshes[i].GetComponent<MeshFilter>().mesh;
				gameObject.GetComponent<MeshRenderer>();
				gameObject.GetComponent<Renderer>().material = glowIndicatorMaterial;
				gameObject.AddMissingComponent<UvScroll>().uvScrollSpeed = glowScrollSpeed;
				glowGameObjects.Add(gameObject);
			}
		}
	}

	private void RemoveGlow()
	{
		if (glowGameObjects != null)
		{
			while (glowGameObjects.Count > 0)
			{
				Object.Destroy(glowGameObjects[0]);
				glowGameObjects.RemoveAt(0);
			}
		}
	}

	public override void Initialize(ModelObject model)
	{
		base.Initialize(model);
		(model as InteractiveObjectModel).AddNonModelReceiver(this);
		if (!base.Model.CanBeInteracted)
		{
			ClearActionIndicator();
		}
		if (base.Model.CanBeInteracted)
		{
			CreateGlow();
		}
		model.Changed += OnModelChanged;
	}

	private void OnModelChanged(ModelObject m, string changed, object args)
	{
		if (changed == "InteractionDisabledEvent")
		{
			if (base.Model.CanBeInteracted)
			{
				CreateGlow();
			}
			else
			{
				ClearActionIndicator();
			}
		}
	}

	public void SetVisible(bool visible)
	{
		Renderer[] componentsInChildren = GetComponentsInChildren<Renderer>();
		if (componentsInChildren != null)
		{
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				componentsInChildren[i].enabled = visible;
			}
		}
		if (ShowIndicator)
		{
			CombatHUD combatHUD = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.CombatHUD) as CombatHUD;
			if (visible)
			{
				combatHUD.ShowLocationIndicator(base.gameObject, IndicatorType);
			}
			else
			{
				combatHUD.HideLocationIndicator(base.gameObject);
			}
		}
	}

	public void OnInteractionCompleted(InteractiveObjectModel model, ActorModel interactingActor)
	{
		ClearActionIndicator();
	}

	public void OnInteractionCanceled(InteractiveObjectModel instigator, ActorModel interactingActor)
	{
		ClearActionIndicator();
	}

	public void OnInteractionStep(InteractiveObjectModel model, ActorModel interactingActor)
	{
	}

	public void OnAttacked(InteractiveObjectModel instigator, ActorModel attackingActor)
	{
	}

	public void OnDestroyed(InteractiveObjectModel instigator, ActorModel attackingActor)
	{
	}

	public void OnTriggered(ActorModel instigator)
	{
		ClearActionIndicator();
	}

	private void CreateActionIndicator()
	{
		actionIndicator = CombatView.Instance.CombatHUD.CreateActionIndicator();
		actionIndicator.FollowTarget(base.gameObject);
	}

	private void ClearActionIndicator()
	{
		if (!(this == null))
		{
			RemoveGlow();
			if (ShowIndicator)
			{
				(SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.CombatHUD) as CombatHUD).HideLocationIndicator(base.gameObject);
			}
		}
	}

	public void SetTargetHighlight()
	{
	}

	public void ClearTargetHighlight()
	{
	}

	public void PlayInteractionSoundEvent()
	{
		if (playSound)
		{
			string audioEvent = AudioEvents.GetAudioEvent(interactionSoundType);
			if (SingularityMonoBehaviour<AudioManager>.Instance != null && !string.IsNullOrEmpty(audioEvent))
			{
				SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent(audioEvent, base.gameObject);
			}
		}
	}
}
