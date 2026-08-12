using UnityEngine;

public class ShowDebugPopup : MonoBehaviour
{
    private bool IsShow;
    public LogPopup logPopup { get; private set; }

    void Start()
    {       
    }

    public void ShowDebug()
    {
        IsShow = logPopup != null && logPopup.gameObject.activeSelf;

        if (!IsShow)
        {
            if (logPopup == null)
            {
                logPopup = Instantiate(Resources.Load<LogPopup>("prefabresources/LogPopup"), HUDManager.Instance.UIParent);
            }
            else
            {
                logPopup.gameObject.SetActive(true);
            }
        }
        else
        {
            logPopup.OnClickClose();
        }
    }
}
