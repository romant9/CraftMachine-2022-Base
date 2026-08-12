using System.Collections;
using System.Collections.Generic;
using BaseModel;
using Client.Framework.Audio;
using Fabric;
using UnityEngine;

public class AudioManager : SingularityMonoBehaviour<AudioManager>
{
	public enum CampMusicLogic
	{
		Randomized = 0,
		WhenInjured = 1
	}

	[SerializeField]
	[Tooltip("Seconds to fade out the current music.")]
	private float fadeTime = 1f;

	private AudioSource musicSource;

	private AudioSource[] effectSources;

	private bool musicIsFading;

	private bool partTriggered;

	private bool loopTriggered;

	private bool playCombatMusic;

	private bool combatMusicForcedMute;

	private int currentTempo = 80;

	private float samplePeriod;

	private float nextBarSample;

	private float currentSample;

	private float metronomeAdjust;

	private float componentTimeSamples;

	private float componentTimeSamplesAdjusted;

	private float componentLength;

	private float previousComponentTimeSamples;

	private float loopTime = 30f;

	private float campDefenseTimerStart = 9f;

	private float campDefenseTimer;

	private bool campDefenseTimerRunning;

	private bool campDefenseTriggered;

	private bool inCampDefenseTutorial;

	private string currentMusicPart = "";

	private string previousMusicPart = "";

	private string currentMusicEvent = "";

	private bool voiceOverEnabled = true;

	private VoiceOverResourcesMap voiceOverResourcesMap;

	private MedicTentModel medicTentModel;

	private float radioSfxInitialVolume = 0.7f;

	private float radioSfxDecrementAmount = 0.2f;

	private float radioSfxCurrentVolume;

	private Coroutine previousStateChange;

	private MusicState currentMusicState;

	private readonly Dictionary<string, int> temposByMusicType = new Dictionary<string, int>
	{
		{ "music/music_combat_forest", 80 },
		{ "music/music_combat_rural", 80 },
		{ "music/music_combat_prison_inside", 80 },
		{ "music/music_combat_highway", 100 },
		{ "music/music_combat_town", 120 },
		{ "music/music_combat_prison_yard", 120 },
		{ "music/music_combat_deadly", 120 },
		{ "music/music_combat_endless", 160 }
	};

	private const string NotificationSoundEventName = "combat_ui/";

	private readonly Dictionary<NotificationSound, string> eventNameByNotificationSoundsMap = new Dictionary<NotificationSound, string>
	{
		{
			NotificationSound.MinorInjury,
			"combat_ui/injury_minor"
		},
		{
			NotificationSound.MajorInjury,
			"combat_ui/injury_major"
		},
		{
			NotificationSound.CriticalInjury,
			"combat_ui/injury_critical"
		},
		{
			NotificationSound.CriticalHit,
			"combat_ui/injury_critical"
		},
		{
			NotificationSound.Stunned,
			"combat_ui/stunned"
		},
		{
			NotificationSound.CurrencyKey,
			"combat_ui/key_found"
		},
		{
			NotificationSound.CurrencySP,
			"combat_ui/collect_survival_points"
		},
		{
			NotificationSound.NothingUseful,
			"combat_ui/nothing_useful_found"
		}
	};

	public CampMusicLogic campMusicLogic = CampMusicLogic.WhenInjured;

	private List<string> loopingSounds = new List<string>();

	public bool combatSfxLoaded { get; private set; }

	public bool campSfxLoaded { get; private set; }

	public bool InCampDefenseTutorial
	{
		set
		{
			inCampDefenseTutorial = value;
			if (inCampDefenseTutorial)
			{
				OnCampDefenseKilled();
			}
			else
			{
				OnCampDefenseAdded(0);
			}
		}
	}

	protected override void AwakeInternal()
	{
		if (IsLoadDataManager)
		{
			DebugTWD.LogMycode("if (IsLoadDataManager) return");
			IsInited = false;
			return;
		}
		IsInited = true;
		if (GameManager.Instance != null)
		{
			SetMute(mute: false, "all");
			SetMute(!GameManager.Instance.Settings.SoundFxOn, "ambience");
			SetMute(!GameManager.Instance.Settings.SoundFxOn, "sfx");
			if (GameManager.Instance.Settings.IPodPlaying)
			{
				SetMute(mute: true, "music");
			}
			else
			{
				SetMute(!GameManager.Instance.Settings.MusicOn, "music");
			}
			voiceOverEnabled = GameManager.Instance.gameEconomyData.ConfigData.VoiceOverEnabled;
		}
		StopAllLoopingSounds();
		musicSource = base.gameObject.AddComponent<AudioSource>();
		musicSource.loop = true;
		effectSources = new AudioSource[6];
		for (int i = 0; i < effectSources.Length; i++)
		{
			effectSources[i] = base.gameObject.AddComponent<AudioSource>();
		}
		if (voiceOverEnabled)
		{
			voiceOverResourcesMap = UnityUtils.LoadFromAssetBundle<VoiceOverResourcesMap>("Audio/VoiceOverResources", "scriptableobjects");
		}
	}

	public void SubsrcibeToPlayerModel()
	{
		if (GameManager.Instance.playerModel != null)
		{
			medicTentModel = GameManager.Instance.playerModel.Camp.GetBuilding("MedicTent") as MedicTentModel;
			medicTentModel.Changed += OnMedicTentChanged;
		}
		else
		{
			Debug.LogWarning("AudioManager: Failed to subscribe to MedicTentModel.Changed - PlayerModel is null!");
		}
	}

	public void StartCombatLoopingSounds(AmbienceType ambienceType, string musicOverride = "")
	{
		string text = "ambience/ambient_" + ambienceType.ToString().ToLower();
		string text2 = "music/music_combat_" + ambienceType.ToString().ToLower();
		if (!string.IsNullOrEmpty(musicOverride))
		{
			text2 = "music/music_combat_" + musicOverride;
		}
		List<string> sounds = new List<string>
		{
			text.ToLower(),
			text2
		};
		StartCombatMusicSync(text2);
		StartLoopingSounds(sounds);
		currentMusicState = MusicState.Combat;
	}

	private void OnEnable()
	{
		if (!IsInited) return;
		UIEvent.OnUIEvent += OnUIEvent;
	}

	private void OnUIEvent(string type, object parameter)
	{
		HUDElement hudElement = parameter as HUDElement;
		HUDElementConfig hudElementConfig = SingularityMonoBehaviour<HUDManager>.Instance.GetHudElementConfig(hudElement);
		if (hudElementConfig == null)
		{
			return;
		}
		if (type == "OnPopUpOpen")
		{
			RequestMusicStateChange(hudElementConfig.MusicType);
		}
		else if (type == "OnPopUpClose" && GameManager.Instance.State == GameState.Camp)
		{
			if (SingularityMonoBehaviour<HUDManager>.Instance.IsOpen(UIType.EndlessMissionHubPopup))
			{
				RequestMusicStateChange(MusicState.EndlessMenus);
			}
			else
			{
				RequestMusicStateChange(MusicState.Camp);
			}
		}
	}

	public void RequestMusicStateChange(MusicState state)
	{
		if (state == MusicState.None)
		{
			return;
		}
		if (currentMusicState == MusicState.None)
		{
			ChangeMusicState(state);
			return;
		}
		if (previousStateChange != null)
		{
			StopCoroutine(previousStateChange);
		}
		previousStateChange = StartCoroutine(DelayedMusicStateChange(state));
	}

	protected IEnumerator DelayedMusicStateChange(MusicState state)
	{
		yield return new WaitForSeconds(0.1f);
		ChangeMusicState(state);
	}

	private bool OpenPopupsDefineMusicState()
	{
		for (int i = 0; i < SingularityMonoBehaviour<HUDManager>.Instance.OpenPopups.Count; i++)
		{
			HUDElementConfig hudElementConfig = SingularityMonoBehaviour<HUDManager>.Instance.GetHudElementConfig(SingularityMonoBehaviour<HUDManager>.Instance.OpenPopups[i]);
			if (hudElementConfig != null && hudElementConfig.MusicType != MusicState.None)
			{
				return true;
			}
		}
		return false;
	}

	private void ChangeMusicState(MusicState state)
	{
		if (state != currentMusicState)
		{
			List<string> list = AudioEvents.GetMusicList(state);
			if (state == MusicState.Camp && (OpenPopupsDefineMusicState() || GameManager.Instance.State != GameState.Camp))
			{
				list = new List<string>();
			}
			if (state != MusicState.Combat)
			{
				EndCombatMusicSync();
			}
			if (list.Count > 0)
			{
				StartLoopingSounds(list);
				currentMusicState = state;
			}
		}
	}

	public void PlayEvent(string eventName)
	{
		if (!IsInited) return;
		if (!GameManager.Instance.Settings.SoundFxOn && !eventName.Contains("music") && !eventName.Contains("ambience"))
		{
			return;
		}
		if (Fabric.EventManager.Instance != null)
		{
			if (!Fabric.EventManager.Instance.PostEvent(eventName))
			{
				Debug.LogWarning("Audio PlayEvent: '" + eventName + "' - Failed!");
			}
		}
		else
		{
			Debug.LogError("Audio PlayEvent: '" + eventName + "' - EventManager is NULL!");
		}
	}

	public void PlayEvent(string eventName, GameObject sourceObject)
	{
		if (!IsInited) return;
		if (!GameManager.Instance.Settings.SoundFxOn)
		{
			return;
		}
		if (Fabric.EventManager.Instance != null)
		{
			if (sourceObject == null)
			{
				Debug.LogError("Audio PlayEvent: '" + eventName + "' - source object is NULL!");
			}
			else if (!Fabric.EventManager.Instance.PostEvent(eventName, sourceObject))
			{
				Debug.LogWarning("Audio PlayEvent: '" + eventName + "' - Failed!");
			}
		}
		else
		{
			Debug.LogError("Audio PlayEvent: '" + eventName + "' - EventManager is NULL!");
		}
	}

	public void StopEvent(string eventName)
	{
		if (!IsInited) return;
		if (Fabric.EventManager.Instance != null)
		{
			if (!Fabric.EventManager.Instance.PostEvent(eventName, EventAction.StopSound))
			{
				Debug.LogWarning("Audio StopEvent: '" + eventName + "' - Failed!");
			}
		}
		else
		{
			Debug.LogError("Audio StopEvent: '" + eventName + "' - EventManager is NULL!");
		}
	}

	public void StopEvent(string eventName, GameObject sourceObject)
	{
		if (!IsInited) return;
		if (Fabric.EventManager.Instance != null)
		{
			if (sourceObject == null)
			{
				Debug.LogError("Audio StopEvent: '" + eventName + "' - source object is NULL!");
			}
			else if (!Fabric.EventManager.Instance.PostEvent(eventName, EventAction.StopSound, sourceObject))
			{
				Debug.LogWarning("Audio StopEvent: '" + eventName + "' - Failed!");
			}
		}
		else
		{
			Debug.LogError("Audio StopEvent: '" + eventName + "' - EventManager is NULL!");
		}
	}

	public void PlayEventDelayed(string eventName, float delay, GameObject sourceObject)
	{
		StartCoroutine(DelayEvent(eventName, delay, sourceObject));
	}

	private IEnumerator DelayEvent(string eventName, float delay, GameObject sourceObject)
	{
		yield return new WaitForSeconds(delay);
		PlayEvent(eventName, sourceObject);
	}

	public void PlayEventNotify(string eventName)
	{
		if (!IsInited) return;
		if (Fabric.EventManager.Instance != null)
		{
			if (!Fabric.EventManager.Instance.PostEventNotify(eventName, null, OnEventNotify))
			{
				Debug.LogWarning("Audio PlayEventNotify: '" + eventName + "' - Failed!");
			}
		}
		else
		{
			Debug.LogError("Audio PlayEventNotify: '" + eventName + "' - EventManager is NULL!");
		}
	}

	public void PlayLevelEvent(SoundType levelSound)
	{
		string text = "combat_level/";
		if (levelSound == SoundType.Alarm)
		{
			text += "alarm";
		}
		PlayEvent(text);
	}

	public void PlayNotificationSound(NotificationSound sound)
	{
		if (eventNameByNotificationSoundsMap.TryGetValue(sound, out var value))
		{
			PlayEvent(value);
		}
	}

	public void StartLoopingSounds(List<string> sounds)
	{
		if (loopingSounds.Count == sounds.Count)
		{
			for (int i = 0; i < sounds.Count && !(sounds[i] != loopingSounds[i]); i++)
			{
				if (i == sounds.Count - 1)
				{
					return;
				}
			}
		}
		StopAllLoopingSounds();
		loopingSounds = sounds;
		for (int j = 0; j < loopingSounds.Count; j++)
		{
			if (loopingSounds[j].Contains("combat"))
			{
				continue;
			}
			if (loopingSounds[j] == "music/music_camp")
			{
				if (inCampDefenseTutorial)
				{
					loopingSounds[j] = "music/music_camp_defense";
				}
				else
				{
					switch (campMusicLogic)
					{
					case CampMusicLogic.Randomized:
					{
						Random.InitState((int)Time.time);
						int num = Random.Range(1, 3);
						List<string> list = loopingSounds;
						int index = j;
						list[index] = list[index] + "_" + num;
						break;
					}
					case CampMusicLogic.WhenInjured:
						if (medicTentModel != null && medicTentModel.HasPatients)
						{
							loopingSounds[j] += "_2";
						}
						else
						{
							loopingSounds[j] += "_1";
						}
						break;
					}
				}
			}
			PlayEvent(loopingSounds[j]);
		}
	}

	public void StopAllLoopingSounds()
	{
		for (int i = 0; i < loopingSounds.Count; i++)
		{
			if (loopingSounds[i].Contains("combat"))
			{
				StopEvent("music/music_combat_all");
				EndCombatMusicSync();
			}
			else
			{
				StopEvent(loopingSounds[i]);
			}
		}
		loopingSounds.Clear();
	}

	public void StopAllSounds(float fadeOutTime)
	{
		StopAllLoopingSounds();
		SetRTP("music/music_camp_all", "volumedamper", 0f);
		SetRTP("music/music_map_all", "volumedamper", 0f);
		if (FabricManager.Instance != null)
		{
			FabricManager.Instance.Stop(fadeOutTime);
		}
	}

	private void SwitchMusicPart(string musicevent, string part)
	{
		if (!(part == previousMusicPart) || loopTriggered)
		{
			currentMusicEvent = musicevent;
			currentMusicPart = part;
			StopEvent(currentMusicEvent + "_" + previousMusicPart);
			if (currentMusicPart == "spawn")
			{
				PlayEventNotify(currentMusicEvent + "_" + currentMusicPart);
			}
			else if (currentMusicPart == "")
			{
				PlayEventNotify(currentMusicEvent);
				currentMusicPart = "threat_1";
			}
			else
			{
				PlayEventNotify(currentMusicEvent + "_" + part);
			}
			previousMusicPart = currentMusicPart;
			loopTriggered = false;
		}
	}

	public void SetMusicThreat(int threatLevel)
	{
		if (threatLevel >= 6)
		{
			SwitchMusicPart(currentMusicEvent, "threat_3", syncOnBar: true);
		}
		else if (threatLevel >= 3)
		{
			SwitchMusicPart(currentMusicEvent, "threat_2", syncOnBar: true);
		}
		else if (threatLevel >= 0)
		{
			SwitchMusicPart(currentMusicEvent, "threat_1", syncOnBar: true);
		}
		else
		{
			SwitchMusicPart(currentMusicEvent, "spawn", syncOnBar: true);
		}
	}

	private void SwitchMusicPart(string musicevent, string part, bool syncOnBar)
	{
		partTriggered = true;
		currentMusicEvent = musicevent;
		currentMusicPart = part;
	}

	public void LoopMusicPart(string musicevent, string part)
	{
		loopTriggered = true;
		currentMusicEvent = musicevent;
		currentMusicPart = part;
	}

	public void StartCombatMusicSync(string musicEventname)
	{
		if (!playCombatMusic)
		{
			playCombatMusic = true;
			if (temposByMusicType.ContainsKey(musicEventname))
			{
				currentTempo = temposByMusicType[musicEventname];
			}
			previousMusicPart = "threat_1";
			currentMusicPart = "threat_1";
			currentMusicEvent = musicEventname;
			samplePeriod = 60f / (float)currentTempo * 44100f * 4f;
			StartMetronome(AudioSettings.dspTime);
			if (currentMusicEvent.Contains("prison_inside"))
			{
				LoopMusicPart(currentMusicEvent, "threat_1");
			}
			else if (currentMusicEvent.Contains("endless"))
			{
				currentMusicEvent = "music/music_combat_endless_1";
				SwitchMusicPart(currentMusicEvent, string.Empty, syncOnBar: true);
			}
			else
			{
				SwitchMusicPart(currentMusicEvent, string.Empty, syncOnBar: true);
			}
		}
	}

	public void EndCombatMusicSync()
	{
		playCombatMusic = false;
		combatMusicForcedMute = false;
		SetMute(!GameManager.Instance.Settings.MusicOn, "music");
		StopEvent(currentMusicEvent);
	}

	private void StartMetronome(double syncTime)
	{
		nextBarSample = (float)syncTime * 44100f;
		StartCoroutine(Metronome());
	}

	protected IEnumerator Metronome()
	{
		while (playCombatMusic)
		{
			currentSample = (float)AudioSettings.dspTime * 44100f;
			if (currentSample >= nextBarSample)
			{
				if (partTriggered)
				{
					partTriggered = false;
					SwitchMusicPart(currentMusicEvent, currentMusicPart);
				}
				else if (loopTriggered)
				{
					SwitchMusicPart(currentMusicEvent, currentMusicPart);
					loopTriggered = false;
				}
				nextBarSample += samplePeriod;
			}
			yield return new WaitForSeconds(loopTime / 1000f);
		}
	}

	public bool IsEventPlaying(string eventName)
	{
		if (Fabric.EventManager.Instance != null)
		{
			return Fabric.EventManager.Instance.IsEventActive(eventName, null);
		}
		Debug.LogWarning("IsEventPlaying: '" + eventName + "'- EventManager is NULL!");
		return false;
	}

	public void SetMute(bool mute, string target)
	{
		if (Fabric.EventManager.Instance == null)
		{
			Debug.LogWarning("SetMute - EventManager is NULL!");
		}
		else if (mute)
		{
			if (!Fabric.EventManager.Instance.PostEvent("DynamicMixer", EventAction.AddPreset, "mute_" + target, null))
			{
				Debug.LogWarning("SetMusicMute - '" + mute + ", " + target + "'Failed!");
			}
		}
		else if ((!(target == "music") || !combatMusicForcedMute) && !Fabric.EventManager.Instance.PostEvent("DynamicMixer", EventAction.RemovePreset, "mute_" + target, null))
		{
			Debug.LogWarning("SetMusicMute - '" + mute + ", " + target + "'Failed!");
		}
	}

	public void SetForcedMusicMuteState(bool state)
	{
		combatMusicForcedMute = state;
		if (GameManager.Instance.Settings.MusicOn)
		{
			SetMute(state, "music");
		}
	}

	public void SetRTP(string eventName, string parameterName, float value, GameObject parent = null)
	{
		if (!IsInited) return;
		if (Fabric.EventManager.Instance != null)
		{
			if (parent != null)
			{
				Fabric.EventManager.Instance.SetParameter(eventName, parameterName, value, parent);
			}
			else
			{
				Fabric.EventManager.Instance.SetParameter(eventName, parameterName, value);
			}
			return;
		}
		Debug.LogWarning("SetRTP: '" + eventName + " , " + parameterName + "'- EventManager is NULL!");
	}

	public void LoadAudio(string prefabName)
	{
		if (FabricManager.Instance == null)
		{
			Debug.LogWarning("Failed to load " + prefabName + " sounds - FabricManager is NULL!");
		}
		else if (prefabName == "CombatSfx")
		{
			FabricManager.Instance.LoadAsset("Audio/CombatSfx", "FabricAudioManager_Combat Audio_combat_sfx");
			if (!Fabric.EventManager.Instance.PostEvent("DynamicMixer", EventAction.RemovePreset, "mute_combat_dynamic", null))
			{
				Debug.LogWarning("Unmuting dynamic combat sounds failed!");
			}
			combatSfxLoaded = true;
		}
		else if (prefabName == "CampSfx")
		{
			FabricManager.Instance.LoadAsset("Audio/CampSfx", "FabricAudioManager_Camp Audio_camp_sfx");
			if (!Fabric.EventManager.Instance.PostEvent("DynamicMixer", EventAction.AddPreset, "mute_combat_dynamic", null))
			{
				Debug.LogWarning("Muting dynamic combat sounds failed!");
			}
			campSfxLoaded = true;
		}
	}

	public void UnloadAudio(string prefabName)
	{
		if (FabricManager.Instance == null)
		{
			Debug.LogWarning("Failed to unload " + prefabName + " sounds - FabricManager is NULL!");
		}
		else if (prefabName == "CampSfx" && campSfxLoaded)
		{
			FabricManager.Instance.UnloadAsset("FabricAudioManager_Camp Audio_camp_sfx_CampSfx");
			campSfxLoaded = false;
		}
		else if (prefabName == "CombatSfx" && combatSfxLoaded)
		{
			FabricManager.Instance.UnloadAsset("FabricAudioManager_Combat Audio_combat_sfx_CombatSfx");
			combatSfxLoaded = false;
		}
	}

	protected void OnEventNotify(EventNotificationType type, string s, object info, GameObject gameObject)
	{
		if (type == EventNotificationType.OnMarker)
		{
			if (currentMusicPart == "spawn")
			{
				SwitchMusicPart(currentMusicEvent, "threat_1", syncOnBar: true);
			}
			else
			{
				LoopMusicPart(currentMusicEvent, currentMusicPart);
			}
		}
	}

	public void OnRadioCallStarted()
	{
		PlayEvent("camp/phonecall");
		PlayEvent("music/music_phonecall");
		SetRTP("music/music_camp_all", "volumedamper", 1f);
		SetRTP("music/music_map_all", "volumedamper", 1f);
		radioSfxCurrentVolume = radioSfxInitialVolume;
		SetRTP("camp/phonecall", "volume", radioSfxCurrentVolume);
	}

	public void OnRadioCallCardClicked()
	{
		radioSfxCurrentVolume -= radioSfxDecrementAmount;
		SetRTP("camp/phonecall", "volume", radioSfxCurrentVolume);
	}

	public void OnRadioCallDone()
	{
		StopEvent("music/music_phonecall");
		StopEvent("camp/phonecall");
		SetRTP("music/music_camp_all", "volumedamper", 0f);
		SetRTP("music/music_map_all", "volumedamper", 0f);
	}

	public void PlayVoiceOver(int voiceOverIndex)
	{
		if (!voiceOverEnabled || voiceOverResourcesMap == null)
		{
			return;
		}
		VoiceOverResourceEntry voiceOverResourceEntry = voiceOverResourcesMap.resources[voiceOverIndex];
		if (voiceOverResourceEntry == null)
		{
			Debug.LogWarning("PlayVoiceOver: Could not find voice over resource, index " + voiceOverIndex);
		}
		else if (voiceOverResourceEntry.AudioClipNames != null)
		{
			int num = voiceOverResourceEntry.AudioClipNames.Length;
			string text = "";
			text = ((num <= 1) ? voiceOverResourceEntry.AudioClipNames[0] : voiceOverResourceEntry.AudioClipNames[Random.Range(0, voiceOverResourceEntry.AudioClipNames.Length)]);
			if (Fabric.EventManager.Instance != null)
			{
				Fabric.EventManager.Instance.PostEvent("dialog_player", EventAction.SetAudioClipReference, text);
			}
			else
			{
				Debug.LogWarning("PlayVoiceOver: EventManager is null!");
			}
			PlayEvent("dialog_player");
		}
		else
		{
			Debug.LogWarning("PlayVoiceOver: Could not find voice over audio");
		}
	}

	private void OnMedicTentChanged(ModelObject model, string changed, object args)
	{
		if (changed == "EventStatusUpdated" && medicTentModel != null && !medicTentModel.HasPatients && campMusicLogic == CampMusicLogic.WhenInjured && loopingSounds.Count > 1 && loopingSounds[1] == "music/music_camp_2")
		{
			StopEvent("music/music_camp_2");
			PlayEvent("music/music_camp_1");
			loopingSounds[1] = "music/music_camp_1";
		}
	}

	public void OnCampDefenseAdded(int walkerCount)
	{
		if (loopingSounds.Count < 2 || !campDefenseTriggered)
		{
			return;
		}
		if (inCampDefenseTutorial)
		{
			campDefenseTimer = 100f;
		}
		else
		{
			campDefenseTimer = campDefenseTimerStart;
		}
		if (walkerCount > 0)
		{
			if (!campDefenseTimerRunning)
			{
				campDefenseTimerRunning = true;
				StartCoroutine(CampDefenseTimer());
			}
			if (loopingSounds[1] != "music/music_camp_defense")
			{
				StopEvent(loopingSounds[1]);
				PlayEvent("music/music_camp_defense");
				loopingSounds[1] = "music/music_camp_defense";
			}
		}
		else if (loopingSounds[1] == "music/music_camp_defense")
		{
			StartLoopingSounds(new List<string> { "ambience/ambient_camp", "music/music_camp" });
		}
	}

	protected IEnumerator CampDefenseTimer()
	{
		while (campDefenseTimer > 0f)
		{
			campDefenseTimer -= Time.deltaTime;
			yield return null;
		}
		OnCampDefenseAdded(0);
		campDefenseTimerRunning = false;
		campDefenseTriggered = false;
	}

	public void OnCampDefenseKilled(int walkerCount = 0)
	{
		if (walkerCount == 0)
		{
			campDefenseTimer -= campDefenseTimerStart * 0.8f;
			return;
		}
		campDefenseTriggered = true;
		if (!inCampDefenseTutorial)
		{
			OnCampDefenseAdded(walkerCount);
		}
	}

	protected void Play(AudioSource source, AudioClip clip, float volume, bool loop = false)
	{
		source.clip = clip;
		source.volume = volume;
		source.loop = loop;
		source.Play();
	}

	protected IEnumerator FadeOutAndPlay(AudioSource source, AudioClip nextClip, float volume)
	{
		musicIsFading = true;
		float speed = source.volume / fadeTime;
		while (source.volume > 0f)
		{
			float num = Mathf.Min(Time.deltaTime, 0.1f);
			source.volume = Mathf.Max(source.volume - speed * num, 0f);
			yield return null;
		}
		source.Stop();
		musicIsFading = false;
		if (nextClip != null)
		{
			Play(source, nextClip, volume);
		}
	}

	public void UpdateEndlessModeAudioTrack(float survivorsHpPercentage)
	{
		if (survivorsHpPercentage == 1f)
		{
			if (currentMusicEvent != "music/music_combat_endless_1")
			{
				StopEvent(currentMusicEvent);
				SwitchMusicPart("music/music_combat_endless_1", "");
			}
		}
		else if ((double)survivorsHpPercentage > 0.5)
		{
			if (currentMusicEvent != "music/music_combat_endless_2")
			{
				StopEvent(currentMusicEvent);
				SwitchMusicPart("music/music_combat_endless_2", "");
			}
		}
		else if (currentMusicEvent != "music/music_combat_endless_3")
		{
			StopEvent(currentMusicEvent);
			SwitchMusicPart("music/music_combat_endless_3", "");
		}
	}


	#region myparams
	private bool IsLoadDataManager => OfflineManager.IsLoadDataManager;
	private bool IsInited;
	#endregion
}
