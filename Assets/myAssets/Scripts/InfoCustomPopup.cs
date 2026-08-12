using UnityEngine;
using static NGUIText;

public class InfoCustomPopup : HUDElement
{
    [SerializeField]
    protected UILabel titleLabel;
    [SerializeField]
    protected UILabel infoLabel;
    [SerializeField]
    protected UITexture infoTexture;

    [SerializeField]
    protected UILabel okButtonLabel;

    protected Callback okCallback;

    public void SetContent(string title, string info, Texture2D tex = null)
    {
        infoTexture.gameObject.SetActive(false);
        infoLabel.gameObject.SetActive(false);
        titleLabel.gameObject.SetActive(false);

        if (titleLabel != null)
        {
            titleLabel.text = title;
            titleLabel.gameObject.SetActive(true);
        }
        if (infoLabel != null)
        {
            infoLabel.text = title;
            infoLabel.gameObject.SetActive(true);          
        }
        if (tex != null && infoTexture != null)
        {
            infoTexture.gameObject.SetActive(true);
            infoTexture.mainTexture = tex;
        }
    }

    public void SetOkButtonLabel(string text)
    {
        if (okButtonLabel != null)
        {
            okButtonLabel.text = text;
        }
    }

    public virtual void OkPressed()
    {
        EventManager.NotifyClick("OkPressed");
        base.Close();
        okCallback?.Invoke();
    }

    public override void OnBackButtonClicked()
    {
        OkPressed();
    }

    public void SetCallbacks(Callback okCallback = null)
    {
        this.okCallback = okCallback;
    }

    public bool HasCallback()
    {
        return okCallback != null;
    }

    public static void ShowPopup(string title, string message, string okButtonLabel, Callback okCallback = null, Texture2D tex = null)
    {
        InfoCustomPopup unfoPopup = null;
        if (SingularityMonoBehaviour<HUDManager>.Instance != null)
        {
            unfoPopup = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.InfoCustomPopup) as InfoCustomPopup;
        }
        if (unfoPopup == null)
        {
            DebugTWD.LogWarning("Alert popup not found!");
        }      
        else
        {
            unfoPopup.SetContent(title, message, tex);
            var tables = unfoPopup.GetComponentsInChildren<UITable>();
            if (tables != null) foreach (var table in tables) { table.repositionNow = true; }
            unfoPopup.SetOkButtonLabel(okButtonLabel);
            unfoPopup.SetCallbacks(okCallback);
            unfoPopup.Open();
        }
    }

    public static void ShowPopupGetText(string title, string message, string okButtonLabel, Callback okCallback, Texture2D tex = null)
    {
        ShowPopup(LocalizationManager.GetText(title), LocalizationManager.GetText(message), LocalizationManager.GetText(okButtonLabel), okCallback, tex);
    }

    public void SetTransform(Vector2 size, float offsetY, Alignment alignment)
    {
        infoLabel.transform.localPosition = new Vector3(infoLabel.transform.localPosition.x, offsetY, infoLabel.transform.localPosition.z);
        infoLabel.alignment = alignment;
        infoLabel.width = (int)size.x;
        infoLabel.height = (int)size.y;
    }
}
