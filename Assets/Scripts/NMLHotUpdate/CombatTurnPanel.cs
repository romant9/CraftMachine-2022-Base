using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TWDModel;
using UnityEngine;

public class CombatTurnPanel : HUDElementFollowTarget
{
	public Color TurnCounterDefaultColor;

	public Color WaveIncomingWarningColor;

	public Color SurvivorTurnColor;

	public Color WalkerTurnColor;

	[Tooltip("Container for monster closet.")]
	public GameObject MonsterClosetContainer;

	[Tooltip("Background sprite for the turn counter.")]
	public UISprite TurnCountBgSprite;

	[Tooltip("Container for turn counter.")]
	public GameObject TurnCountContainer;

	[Tooltip("Label for the current redact number.")]
	public UILabel RedactCounter;

	[Tooltip("Label for the current turn number.")]
	public UILabel TurnCounterNewTurn;

	[Tooltip("Label for the previous turn number.")]
	public UILabel TurnCounterPreviousTurn;

	[Tooltip("Glow outline of the turn counter.")]
	public GameObject TurnCounterGlow;

	[Tooltip("Container for turn counter for max turn length of the mission.")]
	public GameObject MaxTurnCountContainer;

	[Tooltip("Label for the turns left in combat mission.")]
	public UILabel TurnsLeftLabel;

	[Tooltip("Sprite on the end turn button.")]
	public UISprite ButtonIconSprite;

	[Tooltip("Container for survivor turn button.")]
	public GameObject SurvivorTurnButton;

	[Tooltip("Container for redact button.")]
	public GameObject RedactButton;

	[Tooltip("Game Object for highlighted survivor turn button.")]
	public GameObject SurvivorTurnButtonActive;

	[Tooltip("Label for the button text.")]
	public UILabel SurvivorButtonLabel;

	[Tooltip("Container for faction turn indicator.")]
	public GameObject FactionTurnContainer;

	[Tooltip("Background sprite for faction turn indicator.")]
	public UISprite FactionTurnBgSprite;

	[Tooltip("Label for survivor turn in FactionTurnContainer.")]
	public UILabel SurvivorTurnLabel;

	[Tooltip("Label for walker turn in FactionTurnContainer.")]
	public UILabel IncomingLabel;

	[Tooltip("Container for walker turn object in FactionTurnContainer.")]
	public GameObject WalkerTurnContainer;

	[Tooltip("Label for the button text.")]
	public UILabel WalkerButtonLabel;

	[Tooltip("Container for wave content indicators.")]
	public GameObject WaveSpawnIndicatorContainer;

	[Tooltip("Indicator icon for normal walker.")]
	public GameObject WaveSpawnIndicatorNormal;

	[Tooltip("Background of the monster close.")]
	public GameObject MonsterClosetBg;

	[Tooltip("Glow outline of the monster closet.")]
	public GameObject MonsterClosetGlow;

	[Tooltip("Padding between monster icons.")]
	public float MonsterClosetPadding = 25f;

	[Tooltip("Delay between monster closet icon emptying effects in seconds.")]
	public float MosterClosetEmptyIconDelay = 1.5f;

	[Tooltip("How much monster closet lowered when it's hidden.")]
	public float HiddenMonsterClosetHeight = 80f;

	[Tooltip("Overall Wave counter for Endless Mode")]
	public UILabel WaveCountLabel;

	[Tooltip("Delay Between Endless Mode Spawn Animations")]
	public int EndlessModeWaveAnimationDelay;

	[Tooltip("Label for the current redact number.")]
	public UILabel ThreatOverCounter;

	private WaveNotification turnWarningNotification;

	private List<GameObject> waveSizeIndicators = new List<GameObject>();

	private int maxClosetSize = 12;

	private int prevTurn;

	private bool emptyCloset;

	private bool emptyingCloset;

	private int emptiedIconCount;

	private int emptyEffectsPlayed;

	private int pendingMonsterCloset;

	private int pendingRemoveMonsterCloset;

	private bool isRemovingMonsterCloset;

	private float dt;

	public int WaveSizeIndicatorSize => waveSizeIndicators.Count;

	public void Update()
	{
		if (emptyCloset && !isRemovingMonsterCloset)
		{
			dt += Time.deltaTime;
			if (emptiedIconCount < waveSizeIndicators.Count)
			{
				if (emptiedIconCount == 0 || dt > MosterClosetEmptyIconDelay)
				{
					TweenManager.PlayTweenGroup(waveSizeIndicators[emptiedIconCount], 1, forward: true, OnMonsterClosetEmptied);
					dt = 0f;
					emptiedIconCount++;
					emptyingCloset = true;
				}
			}
			else
			{
				emptyCloset = false;
			}
		}
		else if (pendingMonsterCloset > 0 && !emptyingCloset && !isRemovingMonsterCloset)
		{
			SetMonsterCloset(pendingMonsterCloset);
			pendingMonsterCloset = 0;
		}
		else if (!emptyingCloset && pendingRemoveMonsterCloset > 0 && !isRemovingMonsterCloset)
		{
			if (waveSizeIndicators.Count == 0)
			{
				pendingRemoveMonsterCloset = 0;
				return;
			}
			isRemovingMonsterCloset = true;
			pendingRemoveMonsterCloset--;
			TweenManager.PlayTweenGroup(waveSizeIndicators[waveSizeIndicators.Count - 1], 1, forward: true, OnRemoveFromClosetDone);
			TweenManager.PlayTweenGroup(MonsterClosetBg, 1);
		}
	}

	public void CreateTurnWarningNotification()
	{
		turnWarningNotification = CombatView.Instance.CombatHUD.CreateTurnNotificationIndicator();
	}

	public void SetSurvivorTurn(int turnsToWave, int threatLevel)
	{
		SetMessage("", threatLevel);
		SetSurvivorTurnContainer();
	}

	public void SetWalkerTurn(int turnsToWave, int threatLevel)
	{
		SetMessage("", threatLevel);
		SetWalkerTurnContainer();
	}

	public void SetRaiderTurn(int turnsToWave, int threatLevel)
	{
		SetMessage("", threatLevel);
		SetRaiderTurnContainer();
	}

	public bool UpdateThreatOverCount(int threatLevel)
	{
		Helpers.GameObjectSetActive(ThreatOverCounter, value: false);
		if (threatLevel >= maxClosetSize)
		{
			if (threatLevel - maxClosetSize > 0)
			{
				ThreatOverCounter.text = "+" + (threatLevel - maxClosetSize);
				Helpers.GameObjectSetActive(ThreatOverCounter, value: true);
			}
			return true;
		}
		return false;
	}

	public void AddToCloset(int count, int threatLevel)
	{
		if (!emptyingCloset)
		{
			if (maxClosetSize - waveSizeIndicators.Count > 0)
			{
				int count2 = waveSizeIndicators.Count;
				for (int i = 0; i < count; i++)
				{
					GameObject gameObject = UnityEngine.Object.Instantiate(WaveSpawnIndicatorNormal);
					gameObject.transform.parent = WaveSpawnIndicatorContainer.transform;
					gameObject.transform.localPosition = new Vector3(MonsterClosetPadding, 0f, 0f) * (i + count2);
					gameObject.gameObject.SetActive(value: true);
					waveSizeIndicators.Add(gameObject);
					TweenManager.PlayTweenGroup(gameObject, 0);
					TweenManager.PlayTweenGroup(MonsterClosetBg, 0);
					TweenManager.PlayTweenGroup(MonsterClosetGlow, 0);
				}
			}
		}
		else
		{
			pendingMonsterCloset += count;
		}
		UpdateThreatOverCount(threatLevel);
	}

	public void RemoveFromCloset(int threatLevel)
	{
		int num = WaveSizeIndicatorSize - threatLevel;
		if (num >= 0 && pendingRemoveMonsterCloset <= 0)
		{
			pendingRemoveMonsterCloset = num;
		}
		UpdateThreatOverCount(threatLevel);
	}

	private void OnRemoveFromClosetDone()
	{
		GameObject gameObject = waveSizeIndicators[waveSizeIndicators.Count - 1];
		waveSizeIndicators.Remove(gameObject);
		NGUITools.Destroy(gameObject);
		isRemovingMonsterCloset = false;
	}

	public void SetMonsterCloset(int count)
	{
		if (!emptyingCloset)
		{
			ClearWaveSpawnIndicators();
			count = Math.Min(count, maxClosetSize);
			for (int i = 0; i < count; i++)
			{
				GameObject gameObject = UnityEngine.Object.Instantiate(WaveSpawnIndicatorNormal);
				gameObject.transform.parent = WaveSpawnIndicatorContainer.transform;
				gameObject.transform.localPosition = new Vector3(MonsterClosetPadding, 0f, 0f) * i;
				gameObject.transform.localScale = new Vector3(1f, 1f, 1f);
				gameObject.gameObject.SetActive(value: true);
				waveSizeIndicators.Add(gameObject);
			}
		}
		else
		{
			pendingMonsterCloset = count;
		}
	}

	public void SetEndlessModeMonsterCloset(int count)
	{
		List<WalkerType> currentWaveWalkerTypes = EndlessModeHelpers.GetCurrentWaveWalkerTypes();
		UITable component = WaveSpawnIndicatorContainer.GetComponent<UITable>();
		ClearWaveSpawnIndicators();
		for (int i = 0; i < count; i++)
		{
			GameObject gameObject = WaveSpawnIndicatorContainer.AddChild(WaveSpawnIndicatorNormal);
			NGUITools.SetActive(gameObject, state: true);
			string text = currentWaveWalkerTypes[i].ToString();
			if (text.Contains("_Boss"))
			{
				text = text.Replace("_Boss", "");
				Helpers.GameObjectSetActive(gameObject.transform.GetChild(1).gameObject, value: true);
			}
			gameObject.GetComponentInChildren<UISprite>().spriteName = "Ui_Icon_Class_" + text;
			waveSizeIndicators.Add(gameObject);
		}
		component.Reposition();
	}

	public void SetMaxClosetSize(int size)
	{
		maxClosetSize = size;
	}

	public void EmptyMosterCloset()
	{
		emptyCloset = true;
		emptiedIconCount = 0;
		emptyEffectsPlayed = 0;
		SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("combat_ui/wave_incoming");
		SingularityMonoBehaviour<AudioManager>.Instance.SetMusicThreat(-1);
	}

	public void SetTurnCount(int turn)
	{
		TurnCounterNewTurn.text = FormatTurnCount(turn);
		prevTurn = turn;
	}

	public void SetMaxTurnCount(int turn)
	{
		TurnsLeftLabel.text = turn.ToString();
	}

	public void SetMaxTurnCountEnabled(bool enabled)
	{
		MaxTurnCountContainer.SetActive(enabled);
	}

	private string FormatTurnCount(int turnCount)
	{
		if (turnCount <= 99)
		{
			return turnCount.ToString();
		}
		return "-";
	}

	public void SetRedactCount(int count)
	{
		Helpers.GameObjectSetActive(RedactButton, value: false);
		if (count > 0)
		{
			Helpers.GameObjectSetActive(RedactButton, value: true);
			RedactCounter.text = count.ToString() ?? "";
		}
	}

	public void ChangeTurnCount(int currentTurn)
	{
		TurnCounterNewTurn.text = FormatTurnCount(currentTurn);
		TweenManager.PlayTweenGroup(TurnCounterNewTurn.transform.parent.gameObject, 0);
		TurnCounterPreviousTurn.text = FormatTurnCount(prevTurn);
		TweenManager.PlayTweenGroup(TurnCounterPreviousTurn.transform.parent.gameObject, 0);
		switch (currentTurn)
		{
		case 1:
			TweenManager.PlayTweenGroup(TurnCounterGlow, 0);
			break;
		case 0:
			TweenManager.PlayTweenGroup(TurnCounterGlow, 0, forward: false);
			break;
		}
		prevTurn = currentTurn;
	}

	public void ChangeTurnsLeft(int currentTurn)
	{
		TurnsLeftLabel.text = currentTurn.ToString();
		if (turnWarningNotification != null && MaxTurnCountContainer.activeSelf && currentTurn <= 3 && currentTurn > 0)
		{
			turnWarningNotification.SetMessage(LocalizationManager.GetText("Popup.TurnWarning.Title"), LocalizationManager.GetText("Popup.TurnWarning.Body{parameter}", currentTurn));
			turnWarningNotification.Reset();
			SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("combat_ui/turn_warning");
		}
	}

	public void PlayTurnWarning(string title, string text)
	{
		turnWarningNotification.SetMessage(title, text);
		turnWarningNotification.Reset();
		SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("combat_ui/turn_warning");
	}

	public void SetIncomingNotification()
	{
		IncomingLabel.gameObject.SetActive(value: false);
		WalkerTurnContainer.gameObject.SetActive(value: false);
		IncomingLabel.gameObject.SetActive(value: true);
		FactionTurnBgSprite.color = WaveIncomingWarningColor;
		TweenManager.PlayTweenGroup(FactionTurnContainer, 1);
		EmptyMosterCloset();
	}

	public void SetMonsterClosetVisible(bool visible)
	{
		if (MonsterClosetContainer != null)
		{
			float y = (visible ? 0f : (0f - HiddenMonsterClosetHeight));
			MonsterClosetContainer.transform.localPosition = new Vector3(0f, y, 0f);
		}
		else
		{
			Debug.LogWarning("Couldn't set Monster Closet visibility: Container is NULL!");
		}
	}

	public async void ShowNextWaveComposition(int count)
	{
		if (waveSizeIndicators.Count > 0)
		{
			EmptyMosterCloset();
			await Task.Delay(EndlessModeWaveAnimationDelay);
		}
		SetEndlessModeMonsterCloset(count);
	}

	public void SetEndTurnButtonHighlight(bool enabled)
	{
		SurvivorTurnButtonActive.SetActive(enabled);
	}

	public async void SetWaveCount(int waveCount)
	{
		TweenManager.PlayTweenGroup(WaveCountLabel.parent.gameObject, 5);
		await Task.Delay(500);
		if (WaveCountLabel != null)
		{
			WaveCountLabel.text = waveCount.ToString();
		}
	}

	private void SetMessage(string turnText, int spawnCount)
	{
		TurnCountBgSprite.color = TurnCounterDefaultColor;
	}

	private void SetIncomingMessage(string turnText, int spawnCount)
	{
		TurnCountBgSprite.color = WaveIncomingWarningColor;
	}

	private void SetSurvivorTurnContainer()
	{
		SurvivorTurnLabel.gameObject.SetActive(value: true);
		WalkerTurnContainer.gameObject.SetActive(value: false);
		IncomingLabel.gameObject.SetActive(value: false);
		FactionTurnBgSprite.color = SurvivorTurnColor;
		TweenManager.PlayTweenGroup(FactionTurnContainer, 1);
		CombatHUD combatHUD = SingularityMonoBehaviour<HUDManager>.Instance.GetNoCreation(UIType.CombatHUD) as CombatHUD;
		if (combatHUD != null)
		{
			combatHUD.SetSkipTurnEnabled(enabled: true);
		}
	}

	private void SetWalkerTurnContainer()
	{
		SurvivorTurnLabel.gameObject.SetActive(value: false);
		WalkerTurnContainer.gameObject.SetActive(value: true);
		IncomingLabel.gameObject.SetActive(value: false);
		FactionColorEntry factionColorData = GameManager.Instance.GetFactionColorData(Faction.Walker);
		if (factionColorData != null)
		{
			FactionTurnBgSprite.color = factionColorData.UIColor;
		}
		else
		{
			FactionTurnBgSprite.color = WalkerTurnColor;
		}
		TweenManager.PlayTweenGroup(FactionTurnContainer, 1, forward: true, OnWalkerTurnNotificationPlayed);
	}

	private void SetRaiderTurnContainer()
	{
		SurvivorTurnLabel.gameObject.SetActive(value: false);
		WalkerTurnContainer.gameObject.SetActive(value: true);
		IncomingLabel.gameObject.SetActive(value: false);
		FactionColorEntry factionColorData = GameManager.Instance.GetFactionColorData(Faction.Raider);
		if (factionColorData != null)
		{
			FactionTurnBgSprite.color = factionColorData.UIColor;
		}
		else
		{
			FactionTurnBgSprite.color = WalkerTurnColor;
		}
		TweenManager.PlayTweenGroup(FactionTurnContainer, 1, forward: true, OnRaiderTurnNotificationPlayed);
	}

	private void ClearWaveSpawnIndicators()
	{
		for (int i = 0; i < waveSizeIndicators.Count; i++)
		{
			NGUITools.Destroy(waveSizeIndicators[i]);
		}
		waveSizeIndicators.Clear();
		emptyingCloset = false;
	}

	private void OnWalkerTurnNotificationPlayed()
	{
		TweenManager.RemoveCallback(FactionTurnContainer, 1, OnWalkerTurnNotificationPlayed);
		if (prevTurn == 0)
		{
			SetIncomingNotification();
		}
	}

	private void OnRaiderTurnNotificationPlayed()
	{
		TweenManager.RemoveCallback(FactionTurnContainer, 1, OnRaiderTurnNotificationPlayed);
	}

	private void OnMonsterClosetEmptied()
	{
		emptyEffectsPlayed++;
		if (emptyEffectsPlayed >= waveSizeIndicators.Count)
		{
			emptyingCloset = false;
		}
	}
}
