using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TWDModel;

public class ModdingPopup : HUDElement
{
    public UITextList LogList;
    public UILabel OfflineLabel;
    public UIToggle OfflineToggle;
    public UILabel AnalyticsLabel;
    public UIToggle AnalyticsToggle;
    public UILabel DodgeLabel;


    private void OnEnable()
    {
        AnalyticsToggle.value = !HelpersModel.IsOffThinkingAnalytics;
    }

    public override void Open()
    {
        base.Open();

        //TweenManager.PlayTweenGroup(gameObject, 2, forward: false, OnOpenAnimationOver);
        //UpdateUI();
    }

    public void ShowRandomState()
    {
        var randoms = OfflineManager.Instance.queueRandom.ToArray().Select(x => x.ToString());
        string text = "Current 3 PlayerRandom values : " + string.Join("\n", randoms) + "\n---------------";
        DebugTWD.LogWarning(text);
        LogList.Add(text);
    }

    public void ResetRandomCounter()
    {
        if (OfflineManager.Instance.callCountMeter != null)
        {
            OfflineManager.Instance.ResetRandomCounter();
        }
    }

    public void ReloadGame()
    {
        OfflineManager.Instance.ReloadGame();
    }

    public void SwitchOnlineMode(UIToggle tg)
    {
        if (tg.value)
        {
            OfflineManager.Instance.SetOnlineMode();
        }
        else
        {
            OfflineManager.Instance.SetOfflineMode();
        }

        OfflineLabel.text = tg.value ? "ONLINE" : "OFFLINE";
        string text = tg.value ? "Включен ONLINE" : "Включен OFFLINE";
        LogList.Add(text);
    }

    public void SwitchAnalytics(UIToggle tg)
    {
        AnalyticsLabel.text = tg.value ? "Аналитика ON" : "Аналитика OFF";
        string text = tg.value ? "Аналитика Включена" : "Аналитика Выключена";
        LogList.Add(text);
        OfflineManager.Instance.SwitchAnalytics(tg.value);
    }

    public void SwitchDodge(UIToggle tg)
    {
        DodgeLabel.text = tg.value ? "Уклонение ON" : "Уклонение OFF";
        string text = tg.value ? "Уклонение Включено" : "Уклонение Выключено";
        LogList.Add(text);
        OfflineManager.Instance.SwitchDodge(tg.value);
    }

    public override void OnClickClose()
    {
        base.OnClickClose();
        //TweenManager.PlayTweenGroup(gameObject, 2, forward: true, OnCloseAnimOver);
        //gameObject.SetActive(false);
    }

    private void OnDisable()
    {
    }
}
