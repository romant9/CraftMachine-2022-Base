using BaseModel;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CallCountBase : MonoBehaviour
{
    public static CallCountBase Instance;

    public UISprite InternetStatusLabel;
    public UISprite SignalRStatusLabel;

    public UISprite CommandErrorLabel;
    public UISprite ReconnectLabel;
    public UILabel CommandErrorCountLabel;
    public UILabel ReconnectCountLabel;

    public int command_errors_count = 0;
    public int ReconnectCount = 0;

    public bool SignalRStatusLabelOn { get; set; }

    private void Awake()
    {
        if (Instance != null)
        {
            UnityEngine.Object.Destroy(base.gameObject);
            return;
        }
        Instance = this;
    }

    void Start()
    {
        SetSignalRUI(false);
        Show_Command_Error(null);
        Show_Reconnect_Error(0);
    }

    void Update()
    {
    }

    public void Show_Command_Error(IModelCommandRespond respond)
    {
        GameManager.Instance.Show_Command_Error(respond == null ? -1 : respond.Code);
        if (respond == null || respond.Code == 0 || respond.Code == 42)
        {
            command_errors_count = 0;
            CommandErrorLabel.GetComponent<TweenColor>().PlayForward();
            CommandErrorCountLabel.transform.parent.gameObject.SetActive(false);
        }
        else
        {
            command_errors_count++;
            CommandErrorLabel.GetComponent<TweenColor>().PlayReverse();
            CommandErrorCountLabel.transform.parent.gameObject.SetActive(true);
            CommandErrorCountLabel.text = command_errors_count.ToString();
        }
    }

    public void Show_Reconnect_Error(int count)
    {
        ReconnectCount = count;

        if (ReconnectCount < 1)
        {
            ReconnectLabel.GetComponent<TweenColor>().PlayForward();
            ReconnectCountLabel.transform.parent.gameObject.SetActive(false);
        }
        else
        {
            ReconnectLabel.GetComponent<TweenColor>().PlayReverse();
            ReconnectCountLabel.transform.parent.gameObject.SetActive(false);
            ReconnectCountLabel.text = ReconnectCount.ToString();
        }
    }

    public void AddReconnectError()
    {
        ReconnectCount++;
        Show_Reconnect_Error(ReconnectCount);
    }

    public void SetInternetUI(bool state)
    {
        if (state)
        {
            InternetStatusLabel.GetComponent<TweenColor>().PlayReverse();
        }
        else
        {
            InternetStatusLabel.GetComponent<TweenColor>().PlayForward();
        }
    }

    public void SetSignalRUI(bool state)
    {
        if (state)
        {
            SignalRStatusLabel.GetComponent<TweenColor>().PlayReverse();
            SignalRStatusLabelOn = true;
        }
        else
        {
            SignalRStatusLabel.GetComponent<TweenColor>().PlayForward();
            SignalRStatusLabelOn = false;
        }
    }
}
