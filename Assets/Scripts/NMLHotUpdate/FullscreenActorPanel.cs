using TWDModel;
using UnityEngine;
using UnityEngine.Rendering;

public class FullscreenActorPanel : MonoBehaviour
{
	[SerializeField]
	private GameObject levelIndicatorPrefab;

	[SerializeField]
	private Camera currentCamera;

	private SurvivorAnimationController survivorAnimationController;

	private ModularCharacterCombiner actorModularCharacterCombiner;

	private GameObject actorGameObject;

	private ActorModel actorModel;

	private ActorView actorView;

	private string currentActorId;

	public bool allowRotate = true;

	public float rotationSpeed = 360f;

	private bool drag;

	private Vector3 previousMousePosition;

	private SurvivorInfoLevelIndicator levelIndicator;

	public void InitActor(ActorModel model, bool forceUpdate = false, ModularCharacter characterOverridePreview = null)
	{
		if ((forceUpdate || actorModel != model) && actorGameObject != null)
		{
			bool unloadTexturesAll = actorModel != model;
			DestoryAndClearActorObject(unloadTexturesAll);
		}
		if (actorGameObject == null || actorModel == null || actorModel != model)
		{
			actorModel = model;
			if (actorModel is SurvivorModel)
			{
				InitSurvivor(characterOverridePreview);
			}
			else
			{
				InitGenericActor();
			}
			InitCharacter();
		}
	}

	public void InitActor(string actorId, bool forceUpdate = false)
	{
		if ((forceUpdate || currentActorId != actorId) && actorGameObject != null)
		{
			bool unloadTexturesAll = currentActorId != actorId;
			DestoryAndClearActorObject(unloadTexturesAll);
		}
		if (actorGameObject == null || currentActorId == null || currentActorId != actorId)
		{
			currentActorId = actorId;
			InitGenericActor();
			InitCharacter();
		}
	}

	private void InitCharacter()
	{
		actorGameObject.SetLayerRecursively(18);
		Collider[] componentsInChildren = actorGameObject.GetComponentsInChildren<Collider>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].enabled = true;
		}
		actorGameObject.GetComponent<ShadowBlobOrient>().enabled = false;
		SkinnedMeshRenderer[] componentsInChildren2 = actorGameObject.GetComponentsInChildren<SkinnedMeshRenderer>(includeInactive: true);
		for (int j = 0; j < componentsInChildren2.Length; j++)
		{
			componentsInChildren2[j].lightProbeUsage = LightProbeUsage.Off;
		}
		MeshRenderer[] componentsInChildren3 = actorGameObject.GetComponentsInChildren<MeshRenderer>(includeInactive: true);
		for (int j = 0; j < componentsInChildren3.Length; j++)
		{
			componentsInChildren3[j].lightProbeUsage = LightProbeUsage.Off;
		}
	}

	private void InitGenericActor(bool useRandomPrefab = false)
	{
		string text = "";
		if (actorModel != null)
		{
			text = actorModel.Definition.ID;
		}
		else if (currentActorId != null)
		{
			text = currentActorId;
		}
		ActorResourceEntry resources = GameManager.Instance.GetResources<ActorResourceEntry>(text);
		if (resources == null)
		{
			Debug.LogError("Could not find resources for actor prefab list " + text + "!");
			return;
		}
		GameObject gameObject = null;
		if (resources.CharacterScreenPrefab != null)
		{
			gameObject = resources.GetCharacterScreenPrefab();
		}
		else
		{
			if (resources.PrefabResourceList == null)
			{
				Debug.LogError("Could not load prefab list for actor" + actorModel.Definition.ID + "!");
				return;
			}
			if (resources.PrefabResourceList.Contains(actorModel.CharacterPrefab))
			{
				gameObject = resources.GetPrefab(resources.PrefabResourceList.IndexOf(actorModel.CharacterPrefab));
			}
			if (gameObject == null)
			{
				gameObject = ((!useRandomPrefab) ? resources.GetPrefab(0) : resources.GetRandomPrefab());
			}
		}
		actorGameObject = Helpers.InstantiateToParent(gameObject, base.gameObject);
		actorGameObject.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
		actorGameObject.transform.localPosition = Vector3.zero;
		if (actorModel != null)
		{
			actorView = actorGameObject.GetComponent<ActorView>();
			actorView.setRegisterViewToModel(value: false);
			actorView.Initialize(actorModel);
			actorView.SetVisible(visible: true);
		}
	}

	private void InitSurvivor(ModularCharacter characterOverridePreview)
	{
		if (levelIndicator == null)
		{
			levelIndicator = Helpers.InstantiateToParent(levelIndicatorPrefab, base.gameObject).GetComponent<SurvivorInfoLevelIndicator>();
		}
		UpdateIndicator();
		actorGameObject = Helpers.InstantiateToParent(GameManager.Instance.CharacterTemplate, base.gameObject);
		actorGameObject.SetLayerRecursively(18);
		actorModularCharacterCombiner = actorGameObject.GetComponent<ModularCharacterCombiner>();
		ActorView.PrepareActor(actorModel, isTransient: false, isInPreview: true);
		ModularCharacter prefabForActor = ActorView.GetPrefabForActor(actorModel);
		ModularCharacter characterOverride = ((!(characterOverridePreview == null)) ? characterOverridePreview : ActorView.GetPrefabOverrideForActor(actorModel));
		actorModularCharacterCombiner.GenerateCharacter(prefabForActor, characterOverride);
		actorView = actorGameObject.GetComponent<ActorView>();
		actorView.setRegisterViewToModel(value: false);
		actorView.Initialize(actorModel);
		actorView.SetVisible(visible: true);
		actorView.SetMirrored(prefabForActor.Mirrored);
		EquipmentItemModel weaponEquipment = ((SurvivorModel)actorModel).GetWeaponEquipment();
		if (weaponEquipment != null)
		{
			actorView.RequestSwitchEquipment(weaponEquipment);
		}
		survivorAnimationController = actorGameObject.GetComponent<SurvivorAnimationController>();
		survivorAnimationController.NotifyWeaponSwitch();
		survivorAnimationController.ForceIdle();
		survivorAnimationController.CharacterManagement();
		actorGameObject.transform.localPosition = Vector3.zero;
		actorGameObject.transform.localScale = Vector3.one;
		actorGameObject.transform.localRotation = Quaternion.Euler(Vector3.zero);
	}

	private void Update()
	{
		if (!allowRotate || !(actorGameObject != null))
		{
			return;
		}
		Quaternion quaternion = Quaternion.identity;
		if (Input.GetMouseButton(0))
		{
			if (!drag)
			{
				if (Physics.Raycast(currentCamera.ScreenPointToRay(Input.mousePosition), out var hitInfo))
				{
					GameObject gameObject = ((hitInfo.collider != null) ? hitInfo.collider.gameObject : null);
					if (gameObject != null)
					{
						ActorView component = gameObject.GetComponent<ActorView>();
						if (component != null && component == actorView)
						{
							drag = true;
							previousMousePosition = Input.mousePosition;
						}
					}
				}
			}
			else
			{
				Vector3 vector = Input.mousePosition - previousMousePosition;
				previousMousePosition = Input.mousePosition;
				quaternion = Quaternion.AngleAxis((0f - vector.x) * 0.5f, new Vector3(0f, 1f, 0f));
			}
		}
		else
		{
			drag = false;
		}
		if (drag)
		{
			actorGameObject.transform.localRotation = actorGameObject.transform.localRotation * quaternion;
		}
		else
		{
			actorGameObject.transform.localRotation = Quaternion.RotateTowards(actorGameObject.transform.localRotation, Quaternion.AngleAxis(0f, new Vector3(0f, 1f, 0f)), rotationSpeed * Time.deltaTime);
		}
	}

	public void open()
	{
		if (actorGameObject != null)
		{
			actorGameObject.SetActive(value: true);
		}
		base.gameObject.SetActive(value: true);
	}

	public void close()
	{
		if (actorGameObject != null)
		{
			DestoryAndClearActorObject();
			Helpers.ClearUnusedMemory();
		}
		base.gameObject.SetActive(value: false);
	}

	public void RequestShowUpgradeAnim()
	{
		if (actorView != null)
		{
			SurvivorAnimationController survivorAnimationController = actorView.CharacterAnimationController as SurvivorAnimationController;
			if (survivorAnimationController != null)
			{
				survivorAnimationController.CharacterManagementUpgrade();
			}
		}
		if (actorGameObject != null)
		{
			WalkerAnimationController component = actorGameObject.GetComponent<WalkerAnimationController>();
			if (component != null)
			{
				component.UseWeapon(criticalDamage: false, useFenceAttack: false, useChargeAttack: false);
			}
		}
		UpdateIndicator();
	}

	public void RequestShowUnlockAnim()
	{
		if (actorView != null)
		{
			SurvivorAnimationController survivorAnimationController = actorView.CharacterAnimationController as SurvivorAnimationController;
			if (survivorAnimationController != null)
			{
				survivorAnimationController.CharacterManagementUnlock();
			}
		}
	}

	public void RequestSwitchEquipment(EquipmentItemModel equipment)
	{
		if (equipment != null && actorView != null)
		{
			actorView.ForceWeaponSwitch(equipment);
			MeshRenderer[] componentsInChildren = actorGameObject.GetComponentsInChildren<MeshRenderer>(includeInactive: true);
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				componentsInChildren[i].lightProbeUsage = LightProbeUsage.Off;
			}
		}
	}

	public void RequestSwitchOutfit(OutfitDefinition outfit)
	{
		if (actorModel != null && outfit != null)
		{
			ModularCharacter prefabOverrideWithDefinition = ActorView.GetPrefabOverrideWithDefinition(actorModel, outfit);
			if (prefabOverrideWithDefinition != null)
			{
				InitActor(actorModel, forceUpdate: true, prefabOverrideWithDefinition);
			}
			else
			{
				Debug.LogWarning("FullscreenActorPanel: Could not update to ModularCharacter: " + outfit.ID);
			}
		}
	}

	public void RequestSwitchSkin()
	{
		if (actorModel != null)
		{
			ModularCharacter prefabForActor = ActorView.GetPrefabForActor(actorModel);
			if (prefabForActor != null)
			{
				InitActor(actorModel, forceUpdate: true, prefabForActor);
			}
			else
			{
				Debug.LogWarning("FullscreenActorPanel: Could not update to ModularCharacter: " + actorModel.ActorDefinitionID);
			}
		}
	}

	public void PermanentlySwitchToOutfit(OutfitDefinition outfit, PortraitManager.PortraitRenderedCallback portraitRenderedCallback)
	{
		if (actorModel != null && actorView != null)
		{
			ModularCharacter prefabForActor = ActorView.GetPrefabForActor(actorModel);
			if (prefabForActor != null)
			{
				RemovePortrait(actorModel);
				Helpers.ExecuteCommand(new AssignCharacterPrefabCommand(actorModel, prefabForActor.name, outfit.ID));
				UpdatePortrait(actorModel, prefabForActor, portraitRenderedCallback);
			}
		}
	}

	public void PermanentlySwitchToSkin(HeroSkinInfo heroSkin, PortraitManager.PortraitRenderedCallback portraitRenderedCallback)
	{
		if (actorModel == null || !(actorView != null))
		{
			return;
		}
		ModularCharacter prefabForActor = ActorView.GetPrefabForActor(actorModel.ActorDefinitionID, heroSkin.PrefabId);
		if (prefabForActor != null)
		{
			RemovePortrait(actorModel);
			if (GameManager.Instance.modelManager.GetModel<ActorModel>(actorModel.ModelId) == null)
			{
				ModularCharacter prefabForActor2 = ActorView.GetPrefabForActor(actorModel.Definition.ID, heroSkin.PrefabId);
				InitActor(actorModel, forceUpdate: true, prefabForActor2);
			}
			else
			{
				Helpers.ExecuteCommand(new AssignCharacterPrefabCommand(actorModel, heroSkin.PrefabId, null));
			}
			UpdatePortrait(actorModel, prefabForActor, portraitRenderedCallback);
		}
	}

	public void PermanentlySwitchBackToDefault(PortraitManager.PortraitRenderedCallback portraitRenderedCallback)
	{
		if (actorModel != null && actorView != null)
		{
			ModularCharacter prefabForActor = ActorView.GetPrefabForActor(actorModel);
			if (prefabForActor != null)
			{
				RemovePortrait(actorModel);
				Helpers.ExecuteCommand(new AssignCharacterPrefabCommand(actorModel, prefabForActor.name, null));
				UpdatePortrait(actorModel, prefabForActor, portraitRenderedCallback);
				InitActor(actorModel, forceUpdate: true);
			}
		}
	}

	public void UpdateIndicator()
	{
		if (levelIndicator != null)
		{
			levelIndicator.SetSurvivor(actorModel as SurvivorModel);
		}
	}

	public void SetActorVisibility(bool visible)
	{
		actorView.gameObject.SetActive(visible);
	}

	private void RemovePortrait(ActorModel actorModel)
	{
		if (actorModel is SurvivorModel survivorModel)
		{
			PortraitRenderSource info = PortraitRenderSource.fromActorModel(survivorModel);
			PortraitManager.Instance.RemovePortrait(info);
		}
	}

	private void UpdatePortrait(ActorModel actorModel, ModularCharacter modularCharacter, PortraitManager.PortraitRenderedCallback portraitRenderedCallback)
	{
		if (actorModel is SurvivorModel survivorModel)
		{
			PortraitRenderSource info = PortraitRenderSource.fromActorModel(survivorModel);
			PortraitManager.Instance.CreatePortrait(info, modularCharacter, portraitRenderedCallback);
			UIEvent.Send("SurvivorPortraitUpdated");
		}
	}

	private void DestoryAndClearActorObject(bool unloadTexturesAll = true)
	{
		Object.Destroy(actorGameObject);
		if (actorModularCharacterCombiner != null)
		{
			if (unloadTexturesAll)
			{
				UnityUtils.UnloadUsedTextures(actorModularCharacterCombiner.UsedTextures);
			}
			UnityUtils.UnloadUsedTextures(actorModularCharacterCombiner.UsedTexturesOutfit);
		}
		actorModel = null;
		actorModularCharacterCombiner = null;
		actorGameObject = null;
		actorView = null;
	}
}
