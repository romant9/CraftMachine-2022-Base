using UnityEngine;

public class CallCountMeter : MonoBehaviour
{
    [SerializeField]
    private UILabel label;

    private int targetValue;

    public int Value => targetValue;


    public void OnEnable()
    {
        if (OfflineManager.Instance)
        {
            OfflineManager.Instance.callCountMeter = this;
        }
    }

    public void ShowModdingPopup()
    {
        if (OfflineManager.IsLoadDataManager)
        {
            OfflineManager.Instance.ShowModdingPopup();
        }
    }

    public void SetValueImmediate(int value)
    {
        targetValue = value;
        label.text = ((int)value).ToString();
    }
}
