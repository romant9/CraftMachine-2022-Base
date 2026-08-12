using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TWDModel;
using UnityEngine;
using UnityEngine.Rendering;

public class PortraitManager : MonoBehaviour
{
	public delegate void PortraitRenderedCallback(IPortraitRenderSource portraitSource);

	private class Job
	{
		public readonly IPortraitRenderSource info;

		public readonly ModularCharacter properties;

		public event PortraitRenderedCallback OnPortraitRendered;

		public Job(IPortraitRenderSource _info, ModularCharacter _props, PortraitRenderedCallback callback)
		{
			info = _info;
			properties = _props;
			OnPortraitRendered += callback;
		}

		public void Callback()
		{
			if (this.OnPortraitRendered != null)
			{
				this.OnPortraitRendered(info);
			}
		}
	}

	[SerializeField]
	private GameObject container;

	private int storeWidth = 512;

	private int storeHeight = 512;

	[SerializeField]
	public List<GameObject> setups = new List<GameObject>();

	private Queue<Job> queue = new Queue<Job>();

	private Dictionary<IPortraitRenderSource, RenderTexture> portraits = new Dictionary<IPortraitRenderSource, RenderTexture>();

	private static PortraitManager instance;

	private GameObject characterTemplate;

	private PortraitCache portraitCache;

	public PortraitCache PortraitCache
	{
		get
		{
			if (portraitCache == null)
			{
				portraitCache = new PortraitCache();
			}
			return portraitCache;
		}
	}

	public static PortraitManager Instance => instance;

	public RenderTextureFormat Format { get; private set; }

	public bool IsActive => base.gameObject.activeSelf;

	public void RemoveAllPortraits()
	{
		portraits.Clear();
		if (PortraitCache != null)
		{
			PortraitCache.RemoveAll();
		}
	}

	public void OnReload()
	{
		if (!(GameManager.Instance != null) || GameManager.Instance.gameEconomyData == null)
		{
			return;
		}
		switch (GameManager.Instance.gameEconomyData.ConfigData.PortraitManagerMode)
		{
		case PortraitManagerModeType.WipeOnReload:
			_ = portraits.Count;
			portraits.Clear();
			break;
		case PortraitManagerModeType.PruneOnReload:
		{
			List<IPortraitRenderSource> list = new List<IPortraitRenderSource>();
			foreach (KeyValuePair<IPortraitRenderSource, RenderTexture> portrait in portraits)
			{
				RenderTexture value = portrait.Value;
				if (value == null || value.width < 16 || value.height < 16)
				{
					list.Add(portrait.Key);
				}
			}
			for (int i = 0; i < list.Count; i++)
			{
				portraits.Remove(list[i]);
			}
			break;
		}
		}
	}

	public IEnumerator Refresh()
	{
		foreach (KeyValuePair<IPortraitRenderSource, RenderTexture> portrait in portraits)
		{
			IPortraitRenderSource key = portrait.Key;
			if (PortraitCache.Contains(key))
			{
				RenderTexture value = portrait.Value;
				if (!value.IsCreated())
				{
					PortraitCache.Load(key, storeWidth, storeHeight, value);
				}
			}
		}
		yield break;
	}

	public Texture GetPortrait(IPortraitRenderSource info)
	{
		if (!portraits.ContainsKey(info) || info.IsRebuild)
		{
			if (OfflineManager.ConfigBuildType == OfflineManager.ConfigDataType.Light)
			{
				if (!PortraitCache.Contains(info))
				{
					info.Prefab = null;
				}
				RenderTexture renderTexture = PortraitCache.Load(info, storeWidth, storeHeight);
				if (renderTexture != null)
				{
					portraits[info] = renderTexture;
					return renderTexture;
				}
			}
			else
			{
				bool isContain = PortraitCache.Contains(info);
				bool condition = OfflineManager.IsLoadDataManager && OfflineManager.IsUsePortraitManager || isContain;
				if (condition)
				{
					if (OfflineManager.IsLoadDataManager && !isContain)
					{
						info.Prefab = null;
					}
					RenderTexture renderTexture = PortraitCache.Load(info, storeWidth, storeHeight);
					if (renderTexture != null)
					{
						portraits[info] = renderTexture;
						return renderTexture;
					}
				}
			}
			return null;
		}
		return portraits[info];
	}

	public void CreatePortrait(IPortraitRenderSource info, ModularCharacter characterInfo, PortraitRenderedCallback callback)
	{
		foreach (Job item in queue)
		{
			if (item.info == info)
			{
				item.OnPortraitRendered -= callback;
				item.OnPortraitRendered += callback;
				return;
			}
		}
		queue.Enqueue(new Job(info, characterInfo, callback));
		SetRenderingActive(shouldBeActive: true);
	}

	public void RemovePortrait(IPortraitRenderSource info)
	{
		if (portraits.ContainsKey(info))
		{
			portraits.Remove(info);
		}
		PortraitCache.Remove(info);
	}

	private GameObject GetCharacterTemplate()
	{
		if (characterTemplate == null)
		{
			if (Application.isPlaying)
			{
				characterTemplate = GameManager.Instance.CharacterTemplateForPortrait;
			}
			else
			{
				PrefabResource prefabResource = UnityUtils.LoadFromAssetBundle<PrefabResource>("CharacterTemplatePortrait", "scriptableobjects");
				if (prefabResource == null)
				{
					Debug.LogError("Could not load the modular character template!");
				}
				else
				{
					characterTemplate = prefabResource.GetPrefab();
				}
			}
		}
		return characterTemplate;
	}

	private void Awake()
	{
		if (instance != null)
		{
			Debug.LogError("Multiple portrait managers!");
			return;
		}
		instance = this;
		DebugTWD.Log("Awake PortraitManager" + name, DebugType.ActivateObject);
		Object.DontDestroyOnLoad(base.gameObject);
		if (!OfflineManager.IsUsePortraitManager) return;

		Format = SelectTextureFormat();
		Camera[] componentsInChildren = GetComponentsInChildren<Camera>(includeInactive: true);
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].enabled = false;
		}
		EnsureSetupsHaveDirectionalLight();
		SetRenderingActive(shouldBeActive: false);
		if (setups.Count == 0)
		{
			Debug.LogError("No portrait setups configured");
		}
		if (SystemInfo.npotSupport != NPOTSupport.None)
		{
			storeHeight = (storeWidth = Screen.width * 32 / 2732 * 16);
		}
		else
		{
			storeHeight = (storeWidth = 512);
		}
		if (Screen.width > 2560)
		{
			storeWidth = Mathf.Min(storeWidth, 512);
			storeHeight = Mathf.Min(storeHeight, 512);
		}
		else
		{
			storeWidth = Mathf.Min(storeWidth, 256);
			storeHeight = Mathf.Min(storeHeight, 256);
		}
		base.transform.localPosition = new Vector3(0f, -100f, 0f);
	}

	private static RenderTextureFormat SelectTextureFormat()
	{
		if (SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.RGB565))
		{
			return RenderTextureFormat.RGB565;
		}
		if (SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.ARGB1555))
		{
			return RenderTextureFormat.ARGB1555;
		}
		if (SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.ARGB4444))
		{
			return RenderTextureFormat.ARGB4444;
		}
		return RenderTextureFormat.ARGB32;
	}

	private void SetRenderingActive(bool shouldBeActive)
	{
		if (shouldBeActive && !base.gameObject.activeSelf)
		{
			base.gameObject.SetActive(value: true);
			StartCoroutine(RenderPortraits());
		}
		else if (base.gameObject.activeSelf && !shouldBeActive)
		{
			base.gameObject.SetActive(value: false);
		}
	}

	private IEnumerator RenderPortraits()
	{
		while (queue.Count > 0)
		{
			Job entry = queue.Dequeue();
			IPortraitRenderSource info = entry.info;
			ModularCharacter properties = entry.properties;
			if (PortraitCache.Contains(info) && !info.IsRebuild)
			{
				entry.Callback();
			}
			else
			{
				if (properties == null)
				{
					continue;
				}
				ModularCharacter characterOverride = null;
				if (Application.isPlaying)
				{
					characterOverride = ActorView.GetPrefabOverrideForActorDefinition(info.OutfitDefinitionId, info.Gender);
				}
				GameObject prefabInstance = Helpers.InstantiateToParent(GetCharacterTemplate(), container);
				prefabInstance.GetComponent<ModularCharacterCombiner>().GenerateCharacter(properties, characterOverride, CharacterBuildType.Portrait);
				if (prefabInstance.GetComponentInChildren<Animator>() != null)
				{
					prefabInstance.GetComponentInChildren<Animator>().enabled = false;
				}
				foreach (SkinnedMeshRenderer item in new List<SkinnedMeshRenderer>(prefabInstance.GetComponentsInChildren<SkinnedMeshRenderer>()))
				{
					item.lightProbeUsage = LightProbeUsage.Off;
				}
				prefabInstance.transform.Reset();
				prefabInstance.layer = LayerMask.NameToLayer("TextureRendering");
				SetLayerInChildren(prefabInstance.transform, LayerMask.NameToLayer("TextureRendering"));
				Light[] componentsInChildren = prefabInstance.GetComponentsInChildren<Light>(includeInactive: true);
				Light[] array = componentsInChildren;
				for (int i = 0; i < array.Length; i++)
				{
					array[i].cullingMask = 1 << prefabInstance.layer;
				}
				bool flag = componentsInChildren.Length == 0;
				int setupIndex;
				if (properties.PortraitSetup == ActorProperties.PortraitSetupType.Random)
				{
					setupIndex = Random.Range(0, setups.Count);
					if (Application.isPlaying)
					{
					}
				}
				else
				{
					setupIndex = (int)(properties.PortraitSetup - 1);
					if (setupIndex >= setups.Count)
					{
						Debug.LogWarning("Invalid setup index " + setupIndex + " in portrait " + properties.name);
						setupIndex = 0;
					}
				}
				Camera currentCamera = null;
				for (int j = 0; j < setups.Count; j++)
				{
					if (j == setupIndex)
					{
						setups[j].gameObject.SetActive(value: true);
						array = setups[j].gameObject.GetComponentsInChildren<Light>(includeInactive: true);
						for (int i = 0; i < array.Length; i++)
						{
							array[i].enabled = flag;
						}
						currentCamera = setups[j].gameObject.GetComponentsInChildren<Camera>()[0];
					}
					else
					{
						setups[j].gameObject.SetActive(value: false);
					}
				}
				if (Application.isPlaying)
				{
					yield return new WaitForEndOfFrame();
				}
				if (currentCamera == null)
				{
					Debug.LogError("Camera not found for setup index " + setupIndex + " (setups:" + setups.Count + ")");
					continue;
				}
				RenderTexture portraitRenderTexture = new RenderTexture(storeWidth * PortraitSize, storeHeight * PortraitSize, 24, RenderTextureFormat.ARGB32);
				bool enqueue = true;
				portraitRenderTexture.hideFlags = HideFlags.HideAndDontSave;
				portraitRenderTexture.filterMode = FilterMode.Bilinear;
				portraitRenderTexture.autoGenerateMips = false;
				portraitRenderTexture.wrapMode = TextureWrapMode.Clamp;
				int renderWidth = storeWidth * 2 * PortraitSize;
				int renderHeight = storeHeight * 2 * PortraitSize;
				for (int attempt = 0; attempt < 2; attempt++)
				{
					yield return new WaitForEndOfFrame();
					RenderTexture doubleSizedRenderTexture = RenderTexture.GetTemporary(renderWidth, renderHeight, 24, RenderTextureFormat.ARGB32);
					doubleSizedRenderTexture.filterMode = FilterMode.Bilinear;
					doubleSizedRenderTexture.wrapMode = TextureWrapMode.Clamp;
					currentCamera.targetTexture = doubleSizedRenderTexture;
					currentCamera.enabled = false;
					yield return new WaitForEndOfFrame();
					currentCamera.Render();
					yield return new WaitForEndOfFrame();
					Graphics.Blit(doubleSizedRenderTexture, portraitRenderTexture);
					yield return new WaitForEndOfFrame();
					currentCamera.targetTexture = null;
					RenderTexture.ReleaseTemporary(doubleSizedRenderTexture);
					if (Application.isPlaying)
					{
						yield return new WaitForEndOfFrame();
					}
					if (portraitRenderTexture.IsCreated())
					{
						enqueue = false;
						break;
					}
				}
				if (enqueue)
				{
					Debug.LogWarning("Creating render texture failed");
					if (Application.isPlaying)
					{
						Object.Destroy(portraitRenderTexture);
						while (portraitRenderTexture != null)
						{
							yield return new WaitForEndOfFrame();
						}
					}
					else
					{
						Object.DestroyImmediate(portraitRenderTexture);
					}
					if (Application.isPlaying)
					{
						Object.Destroy(prefabInstance);
						while (portraitRenderTexture != null)
						{
							yield return new WaitForEndOfFrame();
						}
					}
					else
					{
						Object.DestroyImmediate(prefabInstance);
					}
					queue.Enqueue(entry);
					continue;
				}
				Texture2D createdTexture = new Texture2D(portraitRenderTexture.width, portraitRenderTexture.height, TextureFormat.RGB24, mipChain: false);
				RenderTexture.active = portraitRenderTexture;
				createdTexture.ReadPixels(new Rect(0f, 0f, portraitRenderTexture.width, portraitRenderTexture.height), 0, 0);
				createdTexture.Apply(updateMipmaps: false, makeNoLongerReadable: false);
				PortraitCache.Store(info, createdTexture);
				if (Application.isPlaying)
				{
					Object.Destroy(createdTexture);
					while (createdTexture != null)
					{
						yield return new WaitForEndOfFrame();
					}
				}
				else
				{
					Object.DestroyImmediate(createdTexture);
				}
				entry.Callback();
				if (Application.isPlaying)
				{
					yield return new WaitForEndOfFrame();
				}
				if (Application.isPlaying)
				{
					Object.Destroy(prefabInstance);
					while (prefabInstance != null)
					{
						yield return new WaitForEndOfFrame();
					}
				}
				else
				{
					Object.DestroyImmediate(prefabInstance);
				}
			}
		}
		SetRenderingActive(shouldBeActive: false);
	}

	private void SetLayerInChildren(Transform transform, int layer)
	{
		for (int i = 0; i < transform.childCount; i++)
		{
			GameObject gameObject = transform.GetChild(i).gameObject;
			gameObject.layer = layer;
			SetLayerInChildren(gameObject.transform, layer);
		}
	}

	public void RemoveUnusedPortraits()
	{
		List<IPortraitRenderSource> list = null;
		foreach (KeyValuePair<IPortraitRenderSource, RenderTexture> portrait in portraits)
		{
			IPortraitRenderSource key = portrait.Key;
			bool flag = false;
			for (int i = 0; i < GameManager.Instance.playerModel.SurvivorContainer.Survivors.Count; i++)
			{
				if (flag)
				{
					break;
				}
				SurvivorModel survivorModel = GameManager.Instance.playerModel.SurvivorContainer.Survivors[i];
				if (key != null && survivorModel.ModelId.ToString() == key.UniqueId)
				{
					flag = true;
				}
			}
			if (!flag)
			{
				if (list == null)
				{
					list = new List<IPortraitRenderSource>();
				}
				list.Add(key);
			}
		}
		if (list != null)
		{
			int count = 0;
			for (int j = 0; j < list.Count; j++)
			{
				RemovePortrait(list[j]);
				count++;
			}
			if (OfflineManager.IsLoadDataManager && count > 0)
			{
				DebugTWD.LogWarning("Remove Unused Portraits: " + count);
			}
		}
	}

	public List<ActorModel> RenderAllPortraits()
	{
		List<ActorModel> list = new List<ActorModel>();
		string[] array = new string[17]
		{
			"DefaultScout", "DefaultBruiser", "DefaultShooter", "DefaultHunter", "DefaultWarrior", "DefaultAssault", "InitialSurvivor01", "InitialSurvivor02", "InitialSurvivor03", "Unique_DarylDixon",
			"Unique_Angie", "Unique_Wanda", "Unique_Bud", "Hero_Rick", "Hero_Carol", "Hero_Michonne", "Hero_Negan"
		};
		foreach (string identifier in array)
		{
			foreach (string character in GameManager.Instance.GetResources<CharacterResourceEntry>(identifier).Characters)
			{
				if (character[1].Equals('_'))
				{
					ActorModel actorModel = GameManager.Instance.playerModel.SurvivorContainer.CreateRandomSurvivor(0, 0, 0, 0, SurvivorClass.Shooter, character);
					ModularCharacter characterInfo = ActorView.LoadAsset(character);
					CreatePortrait(PortraitRenderSource.fromActorModel(actorModel), characterInfo, delegate
					{
					});
					list.Add(actorModel);
				}
			}
		}
		return list;
	}

	[ContextMenu("Render All Portraits")]
	public void RenderAllPortraitsMain()
	{
		RenderAllPortraitsInternal(false);
	}
	[ContextMenu("Recreate All Portraits")]
	public void ReCreateAllPortraits()
	{
		RenderAllPortraitsInternal(true);
	}

	[ContextMenu("Remove All Portraits")]
	public void ClearPortraitCache()
	{
		PortraitCache.RemoveAll();
	}

	private void RenderAllPortraitsInternal(bool recreate)
	{
		int count = 0;

		if (recreate)
		{
			ClearPortraitCache();
		}

		List<ActorModel> survivors = GameManager.Instance.playerModel.SurvivorContainer.Survivors.Select(x => x as ActorModel)?.ToList();
		foreach (var actor in survivors)
		{
			PortraitRenderSource portraitRenderSource = PortraitRenderSource.fromActorModel(actor);

			if (!PortraitCache.Contains(portraitRenderSource))
			{
				ModularCharacter modularCharacter = ActorView.GetPrefabForActor(actor);
				if (modularCharacter == null)
				{
					modularCharacter = ActorView.SelectRandomPrefabForActorDefinition(actor.Definition.ID, actor.Definition.Gender);
				}

				CreatePortrait(portraitRenderSource, modularCharacter, delegate
				{
				});
				count++;
			}
		}

		RemoveUnusedPortraits();
		PlayerRandomValues.Instance.ResetRandomToInit();
		DebugTWD.LogWarning("All Portraits renderered: " + count + "/" + GameManager.Instance.playerModel.SurvivorContainer.Survivors.Count());
	}

	public void PreRenderInitialSurvivors()
	{
		string[] array = GameManager.Instance.gameEconomyData.ConfigData.InitialSurvivors.ToArray();
		foreach (string text in array)
		{
			CharacterResourceEntry resources = GameManager.Instance.GetResources<CharacterResourceEntry>(text);
			if (resources == null)
			{
				continue;
			}
			foreach (string character in resources.Characters)
			{
				if (character[1].Equals('_'))
				{
					PortraitRenderSource portraitRenderSource = PortraitRenderSource.fromActorDefinition(GameManager.Instance.gameEconomyData.GetActorDefinition(text));
					if (!PortraitCache.Contains(portraitRenderSource))
					{
						ModularCharacter characterInfo = ActorView.LoadAsset(character);
						CreatePortrait(portraitRenderSource, characterInfo, delegate
						{
						});
					}
					break;
				}
			}
		}
	}

	public void DumpAllPortraits(List<ActorModel> actors)
	{
		foreach (ActorModel actor in actors)
		{
			PortraitRenderSource info = PortraitRenderSource.fromActorModel(actor);
			Texture portrait = GetPortrait(info);
			if (portrait is RenderTexture)
			{
				Texture2D texture2D = new Texture2D(portrait.width, portrait.height, TextureFormat.ARGB32, mipChain: false);
				RenderTexture.active = (RenderTexture)portrait;
				texture2D.ReadPixels(new Rect(0f, 0f, portrait.width, portrait.height), 0, 0);
				texture2D.Apply(updateMipmaps: false, makeNoLongerReadable: false);
				byte[] bytes = texture2D.EncodeToJPG();
				File.WriteAllBytes(Application.dataPath + "/screenshots/portrait_" + actor.CharacterPrefab + ".jpg", bytes);
			}
			RemovePortrait(info);
		}
	}

	private void EnsureSetupsHaveDirectionalLight()
	{
	}


	#region myparams
	private int PortraitSize => OfflineManager.IsLoadDataManager && OfflineManager.PortraitSize > 0 ? OfflineManager.PortraitSize : 2;
	#endregion

	#region mycode
	private Dictionary<string, Texture> PortraitsRT { get; set; }
	public IEnumerator SetPortaits()
	{
		List<ActorModel> survivors = GameManager.Instance.playerModel.SurvivorContainer.Survivors.Select(x => x as ActorModel)?.ToList();

		ClearPortraitCache();
		PortraitsRT = new Dictionary<string, Texture>();

		for (int i=0; i < survivors.Count; i++)
		{
			var actor = survivors[i];
			PortraitRenderSource portraitRenderSource = PortraitRenderSource.fromActorModel(actor);
			Texture portrait = GetPortrait(portraitRenderSource);
			if (portrait == null)
			{
				ModularCharacter prefabForActor = ActorView.GetPrefabForActor(actor);
				if (portraitRenderSource != null)
				{
					CreatePortrait(portraitRenderSource, prefabForActor, OnPortraitRendered);
					yield return new WaitUntil(() => PortraitsRT.ContainsKey(portraitRenderSource.ActorDefinitionId));
				}
			}
			else
			{
				PortraitsRT.Add(portraitRenderSource.ActorDefinitionId, portrait);
			}
			yield return null;
		}

		if (PortraitsRT.Count > 0)
		{

		}
	}

	private void OnPortraitRendered(IPortraitRenderSource info)
	{
		PortraitsRT.Add(info.ActorDefinitionId, GetPortrait(info));
	}
	#endregion
}
