using System.Collections.Generic;
using UnityEngine;
using TWDModel;
using BaseModel;
using System.Linq;
using Client.Connectivity;
using TwdCustomMod;


public class PlayerRandomValues : MonoBehaviour
{
	public static PlayerRandomValues Instance;

	private PlayerModel Player => OfflineManager.Instance.Player;

	public delegate void On_Change(int value);
	public event On_Change On_Call_Change;

	public delegate void On_Reset(bool IsZeroCounter);
	public event On_Reset On_Call_Reset;

	public delegate void On_BagCheck(bool IsTrue);
	public event On_BagCheck On_Call_BagCheck;

	public HUDMeter hudMeter;

	public static string ConditionOpenedPlayerPrefs = "ConditionOpenedPlayerPrefs";

	public static string ItemListOpenedPlayerPrefs = "ItemListOpenedPlayerPrefs";
	public ModelRandom PlayerRandomInit { get; set; }
	public ModelRandom PlayerRandomInitFix { get; set; }
	public ModelRandom PlayerRandomInitCopy { get; set; }

	public bool IsUseBorder { get; set; }
	public bool IsUseLastReload { get; set; }
	public bool IsUseFirstTenLucky { get; set; }
	public bool IsPlusOneFix { get; set; }


	public ModelRandom PlayerRandom
	{
		get
		{
			if (Player != null)
			{
				return Player.PlayerRandom;
			}
			return null;
		}
	}

	public int ReloadCounter { get; set; }

	public int KillZombieCounter { get; set; }

	public int GoHubCounter { get; set; }

	public int GoBagCounter { get; set; }

	//Перезапуск - index 0, Убийство ходячего - index 1, Лагерь - 2
	public List<int> callRandomTypeList { get; private set; }
	public RandomChangeType randomChangeType { get; private set; }
	public RandomSource randomSource { get; private set; }

	public bool TrigGoToCamp;

	//Random CallCount
	public int RandomCallCount
	{
		get
		{
			if (Player != null)
			{
				return Player.PlayerRandom.CallCount;
			}
			return 0;
		}
	}
	private int currentRandomState = 0;

	public List<ModelRandomItem> PlayerRandomList { get; private set; }


	private void Awake()
	{
		if (Instance != null)
		{
			DebugTWD.LogError("Multiple PlayerRandomValues!");
			Destroy(gameObject);
			return;
		}
		Instance = this;
		DontDestroyOnLoad(gameObject);

		randomChangeType = RandomChangeType.Internal;
		callRandomTypeList = new List<int> { 0,0,0 };
		PlayerRandomList = new List<ModelRandomItem>();
	}

	void Start()
	{

	}

	private void FixedUpdate()
	{
		if (TrigGoToCamp)
		{
			TrigGoToCamp = false;
			DebugTWD.Log("Вернулись в лагерь");
			ReturnCamp();
		}

		if (Player != null && currentRandomState != PlayerRandom.State)
		{
			currentRandomState = PlayerRandom.State;
			hudMeter.SetValueImmediate(PlayerRandom.CallCount);
			//On_Call_Change?.Invoke(RandomCallCount);
			var item = new ModelRandomItem(PlayerRandomList.Count + 1, new ModelRandom(Player.PlayerRandom), randomChangeType);
			if (randomChangeType != RandomChangeType.Internal)
				randomChangeType = RandomChangeType.Internal;
			if (!PlayerRandomList.Contains(item)) PlayerRandomList.Add(item);
		}
	}

	public void ShowStates()
	{
		if (Player == null || PlayerRandomList.Count == 0) return;
		ModelRandomInfoPopup obj = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.ModelRandomInfoPopup) as ModelRandomInfoPopup;
		if (obj != null)
		{
			obj.TryOpenWithNormalData(PlayerRandomList);
		}
	}

	public void CopyToClipboad()
	{
		if (PlayerRandomList.Count > 0)
		{
			string text = string.Join("\n", PlayerRandomList.Select(x => x.modelRandom.State));
			MyTools.CopyToClipboard(text);
		}
	}

	public void AddReloadCount(int value, bool IsIncremental = true)
	{
		if (IsIncremental)
		{
			if (ReloadCounter == 0 && value == -1) return;
			ReloadCounter += value;
		}
		else
		{
			if (value < 0 || ReloadCounter == value) return;
			ReloadCounter = value;
		}
		randomChangeType = RandomChangeType.Reload_Game;
		callRandomTypeList[0] = ReloadCounter;

		ReseedRandom();
	}
	public void AddKillZombieCount(int value, bool IsIncremental = true)
	{
		if (IsIncremental)
		{
			if (KillZombieCounter == 0 && value == -1) return;
			KillZombieCounter += value;
		}
		else
		{
			if (value < 0 || KillZombieCounter == value) return;
			KillZombieCounter = value;
		}
		randomChangeType = RandomChangeType.Kill;
		callRandomTypeList[1] = KillZombieCounter;

		ReseedRandom();
	}
	public void AddHubCount(int value, bool IsIncremental = true)
	{
		if (IsIncremental)
		{
			if (GoHubCounter == 0 && value == -1) return;
			GoHubCounter += value;
		}
		else
		{
			if (value < 0 || GoHubCounter == value) return;
			GoHubCounter = value;
		}

		randomChangeType = RandomChangeType.Bag_Camp;
		callRandomTypeList[2] = GoHubCounter;

		ReseedRandom();
	}

	public void Reset()
	{
		ResetAll(true);
	}

	public void ResetAll(bool isZeroCounter)
	{
		GameManager.Instance.playerModel.PlayerRandom = new ModelRandom(PlayerRandomInit);

		if (isZeroCounter)
		{
			ReloadCounter = 0;
			KillZombieCounter = 0;
			GoHubCounter = 0;
			GoBagCounter = 0;

			PlayerRandomList.Clear();
			callRandomTypeList = new List<int>() { 0, 0, 0 };
		}

		On_Call_Reset?.Invoke(isZeroCounter);
	}

	public void ReseedRandom()
	{
		ResetAll(false);

		//Reload
		for (int i = 0; i < callRandomTypeList[0]; i++)
		{
			int state = PlayerRandom.State;
			Player.PlayerRandom = new ModelRandom(state);
			if (!IsPlusOneFix)
			{
				Player.PlayerRandom.Next();
			}
		}

		//Зомби
		for (int j = 0; j < callRandomTypeList[1]; j++)
		{
			Player.PlayerRandom = new ModelRandom(PlayerRandom.State);
			Player.PlayerRandom.Next();
			Player.PlayerRandom.Next();
		}

		//Hub и др
		for (int k = 0; k < callRandomTypeList[2]; k++)
		{
			Player.PlayerRandom.Next();
		}

		if (IsUseLastReload)
		{
			int state = PlayerRandom.State;
			Player.PlayerRandom = new ModelRandom(state);
			if (!IsPlusOneFix)
			{
				Player.PlayerRandom.Next();
			}
		}

		On_Call_Change?.Invoke(Player.PlayerRandom.CallCount);
	}

	public static void ReturnCamp()
	{
		Instance.InvokeBagCheck(false);

		if (GameManager.Instance.playerModel != null)
		{
			GameManager.Instance.playerModel.RFMGiftManager.TriggerRFMEvent(RFMEvent.ReturnCamp);
			if (!IsConditionOpened() && GameManager.Instance.IsConnectedToServer)
			{
				TryOpenConditionBundle();
			}
		}
	}

	public void InvokeBagCheck(bool IsTrue)
	{
		On_Call_BagCheck.Invoke(IsTrue);
	}

	public void ReseedRandomStart()
	{
		//callcount 2 - NoFix (3 раза state)
		//callcount 1 - OneFix (2 раза state)

		//загрузка игры
		//int state = Player.PlayerRandom.State;
		//Player.PlayerRandom = new ModelRandom(state);
		//Player.PlayerRandom.Next();
		//playerRandomInit = new ModelRandom(Player.PlayerRandom);

		int state = Player.PlayerRandom.State;
		DebugTWD.LogWarning("Previous State is : " + state);
		Player.PlayerRandom = new ModelRandom(state);

		PlayerRandomInitFix = new ModelRandom(Player.PlayerRandom);
		DebugTWD.LogWarning("playerPlusOneFix state is : " + PlayerRandomInitFix.State);

		Player.PlayerRandom.Next();
		PlayerRandomInitCopy = new ModelRandom(Player.PlayerRandom);
		DebugTWD.LogWarning("playerNoFix state is : " + PlayerRandomInitCopy.State);
		ChageInitPlayerRandom(IsPlusOneFix);

		Player.PlayerRandom = new ModelRandom(PlayerRandomInit);

		//рамка
		//Player.PlayerRandom = new ModelRandom(playerRandomInit.State);
		//playerRandomInitBorder = new ModelRandom(Player.PlayerRandom);
		//Debug.LogWarning("playerRandomInitBorder is : " + playerRandomInitBorder.State);

		//Player.PlayerRandom = new ModelRandom(playerRandomInit);
		DebugTWD.LogWarning("Final State is : " + Player.PlayerRandom.State);
	}

	public void ChageInitPlayerRandom(bool isPlusOneFix)
	{
		IsPlusOneFix = isPlusOneFix;
		PlayerRandomInit = new ModelRandom(isPlusOneFix ? PlayerRandomInitFix : PlayerRandomInitCopy);
		Debug.LogWarning("PlayerRandomInit is : " + PlayerRandomInit.State);
	}

	public void ResetRandomToInit()
	{
		var player = OfflineManager.IsLoadDataManager ? DataManager.Instance.Player : GameManager.Instance.playerModel;
		player.PlayerRandom = new ModelRandom(PlayerRandomInit);
	}

	//static
	private static bool IsConditionOpened()
	{
		if (TWDPlayerPrefs.GetInt(ConditionOpenedPlayerPrefs) == 1)
		{
			return true;
		}
		return false;
	}

	public static void TryOpenConditionBundle()
	{
		ConditionBundleDefinition firstConditionBundle = GetFirstConditionBundle();
		if (firstConditionBundle != null)
		{
			GameManager.Instance.BundleSource = Metrics.BundleSource.ConditionBundle;
			BundleCardPopup.OpenBundle(firstConditionBundle.BundleIdentifier);
			if (GameManager.Instance.IsConnectedToServer)
			{
                SetConditionOpened(on: true);
            }
        }
	}

	public static ConditionBundleDefinition GetFirstConditionBundle()
	{
		List<string> currentGift = GameManager.Instance.playerModel.RFMGiftManager.CurrentGift;
		if (currentGift == null || currentGift.Count <= 0)
		{
			return null;
		}
		return GameManager.Instance.gameEconomyData.GetConditionBundleDefinition(currentGift[0]);
	}

	public static void SetConditionOpened(bool on)
	{
		if (on)
		{
			TWDPlayerPrefs.SetInt(ConditionOpenedPlayerPrefs, 1);
		}
		else
		{
			TWDPlayerPrefs.SetInt(ConditionOpenedPlayerPrefs, 0);
		}
		TWDPlayerPrefs.Save();
	}
}

public enum RandomSource
{
	Player_Random,
	Badge_Random,
	Hero_Call
}

public enum RandomChangeType
{
	Reload_Game,
	Kill,
	Hub_Camp,
	Bag_Camp,
	Internal,
	None
}