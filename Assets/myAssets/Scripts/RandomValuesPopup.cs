using TwdCustomMod;
using UnityEngine;

public class RandomValuesPopup : HUDElement
{
	[SerializeField]
	private UILabel ReloadCounterLabel;
	[SerializeField]
	private UIInput ReloadCounterInput;

	[SerializeField]
	private UILabel KillZombieCounterLabel;
	[SerializeField]
	private UIInput KillZombieCounterInput;

	//Он же сумка
	[SerializeField]
	private UILabel GoHubCounterLabel;
	[SerializeField]
	private UIInput GoHubCounterInput;

	public UIButtonExtended btBagAdd;
	public UIButtonExtended btBagRemove;

	public UIToggle PlusOneFixToggle;

	public UILabel CheckBagLabel;

	private bool IsBagChecked;

	private void OnEnable()
	{
		PlayerRandomValues.Instance.On_Call_Reset += OnClickReset;
		PlayerRandomValues.Instance.On_Call_BagCheck += OnBagCheck;

		btBagAdd.SetState(UIButtonColor.State.Disabled, true);
		btBagRemove.SetState(UIButtonColor.State.Disabled, true);

		if (PlayerPrefs.HasKey(UserPrefsKeys.Key_PlusOneFix))
		{
			PlusOneFixToggle.value = bool.Parse(PlayerPrefs.GetString(UserPrefsKeys.Key_PlusOneFix));
		}
	}

	public override void Open()
	{
		base.Open();

		UpdateUI();
	}

	public override void OnClickClose()
	{
		TweenManager.PlayTweenGroup(gameObject, 2, forward: true, OnCloseAnimOver);
		gameObject.SetActive(false);
	}

	private void OnDisable()
	{
		PlayerRandomValues.Instance.On_Call_Reset -= OnClickReset;
		PlayerRandomValues.Instance.On_Call_BagCheck -= OnBagCheck;
	}

	public void ReturnCampFake()
	{
		PlayerRandomValues.ReturnCamp();
	}

	public void OnBagCheck(bool IsTrue)
	{
		IsBagChecked = true;

		if (CheckBagLabel.TryGetComponent<LocalizationUIUpdater>(out var localizationUIUpdater))
		{
			if (IsTrue)
			{
				localizationUIUpdater.RuCustomText = "Сумка работает. Fix+ отключить";
				localizationUIUpdater.EnCustomText = "Bag is work. Fix+ set disable";
			}
			else
			{
				localizationUIUpdater.RuCustomText = "Сумка НЕ работает. Fix+ включить";
				localizationUIUpdater.EnCustomText = "Bag is NOT work. Fix+ set enable";
			}
			localizationUIUpdater.UpdateContent();
			//localizationUIUpdater.UpdateCustomContent(IsTrue ? "BagWork" : "BagNotWork");
		}
	}

	public void Change_Reload(UIInput input)
	{
		if (int.TryParse(input.value, out int result))
		{
			DebugTWD.Log("Change_Reload " + result);
			PlayerRandomValues.Instance.AddReloadCount(result, false);
		}
	}
	public void Change_ReloadUp()
	{
		var count = PlayerRandomValues.Instance.ReloadCounter + 1;
		ReloadCounterInput.value = count.ToString();
	}
	public void Change_ReloadDown()
	{
		var count = PlayerRandomValues.Instance.ReloadCounter - 1;
		if (count == -1) return;
		ReloadCounterInput.value = count.ToString();
	}
	public void Change_KillZombie(UIInput input)
	{
		if (int.TryParse(input.value, out int result))
		{
			PlayerRandomValues.Instance.AddKillZombieCount(result, false);
		}
	}
	public void Change_KillZombieUp()
	{
		var count = PlayerRandomValues.Instance.KillZombieCounter + 1;
		KillZombieCounterInput.value = count.ToString();
	}
	public void Change_KillZombieDown()
	{
		var count = PlayerRandomValues.Instance.KillZombieCounter - 1;
		if (count == -1) return;
		KillZombieCounterInput.value = count.ToString();
	}
	public void Change_Hub(UIInput input)
	{
		if (int.TryParse(input.value, out int result))
		{
			PlayerRandomValues.Instance.AddHubCount(result, false);
		}
	}
	public void Change_HubUp()
	{
		var count = PlayerRandomValues.Instance.GoHubCounter + 1;
		GoHubCounterInput.value = count.ToString();
	}

	public void Change_HubDown()
	{
		var count = PlayerRandomValues.Instance.GoHubCounter - 1;
		if (count == -1) return;
		GoHubCounterInput.value = count.ToString();
	}

	public void OnClickReset(bool isZeroCounter)
	{
		ReloadCounterInput.value = PlayerRandomValues.Instance.ReloadCounter.ToString();
		KillZombieCounterInput.value = PlayerRandomValues.Instance.KillZombieCounter.ToString();
		GoHubCounterInput.value = PlayerRandomValues.Instance.GoHubCounter.ToString();
	}

	public void Reset()
	{
		PlayerRandomValues.Instance.Reset();
		//OnClickReset(true);
	}

	public void ShowHelp()
	{
		MyTools.OpenAlert("Сдвигать очередь можно:\n- перезапуском игры\n- убийством ходячих в лагере\n- При выходе в лагерь после входа в некоторые режимы, открывающиеся в новом окне, например:\n" +
			"Профиль, Мастерская, Выжившие, Вызовы, Сумка, Магазин, Гильдия, Хаб Миссий, Пропуск.\nИспользуйте счетчики + -. " +
			"Порядок изменения рандома не важен. Конечный результат будет одинаковый. Рекоммендуется имитировать не более одного перезапуска." +
			"Это окно можно использовать в разделах: Вызовы снаряжения, Прорыв, Ремодел! Кнопка Сброса сбрасывает все счетчики и изменения в разделах." );
	}

	public void OpenDebugPopup()
	{
		PlayerRandomValues.Instance.ShowStates();
	}

	public void ChageInitPlayerRandom(UIToggle tg)
	{
		PlayerRandomValues.Instance.ChageInitPlayerRandom(tg.value);
		PlayerRandomValues.Instance.ReseedRandom();
		PlayerPrefs.SetString(UserPrefsKeys.Key_PlusOneFix, tg.value.ToString());
	}

	public void ChageLastReloadRandom(UIToggle tg)
	{
		PlayerRandomValues.Instance.IsUseLastReload = tg.value;
		PlayerRandomValues.Instance.ReseedRandom();
	}

	public void ShowHelpFix(UIButton bt)
	{
		string text = "После определенного времени возникает сдвиг +1 влево. Для корректировки рандома включите!!!";
		TooltipManager.OpenTextBoxWithText(bt.gameObject, text, CraftSettings.Instance.tooltipPrefab);
	}

	public void ShowHelpLastRestart(UIButton bt)
	{
		string text = "Для подстраховки результата переката после длииинной очереди тапов по сумке. " +
			"Можно рассчитать результат с учетом перезапуска игры после всех действий в игре. В этом случае после выхода из игры можно зайти в мод и проверить результат.";
		TooltipManager.OpenTextBoxWithText(bt.gameObject, text, CraftSettings.Instance.tooltipPrefab);
	}
}
