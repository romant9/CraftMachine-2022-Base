using UnityEngine;

public class UIButtonToggleHelper : MonoBehaviour
{
    [SerializeField]
    private GameObject SpriteChecked;
    [SerializeField]
    private GameObject[] SpriteCheckedGroup;
    [SerializeField]
    private GameObject SpriteUnchecked;
    [SerializeField]
    private GameObject[] SpriteUnCheckedGroup;
    private UIButtonToggle toggle => this.GetComponent<UIButtonToggle>() ?? null;

    private void OnEnable()
    {
        if (toggle)
        {
            toggle.OnClickToggleEvent += SetSprites;
        }
        SetSprites(toggle.IsToggled);
	}

    private void OnDisable()
    {
        if (toggle)
        {
            toggle.OnClickToggleEvent -= SetSprites;
        }
    }

    public void SetSprites(bool IsToggle)
    {
        if (OfflineManager.IsLoadDataManager)
        {
            if (IsToggle)
            {
                toggle.tweenTarget = SpriteChecked;
                SpriteUnchecked.SetActive(false);
                if (SpriteUnCheckedGroup != null)
                {
                    foreach (var go in SpriteUnCheckedGroup) { go.SetActive(false); }
                }
                SpriteChecked.SetActive(true);
                if (SpriteCheckedGroup != null)
                {
                    foreach (var go in SpriteCheckedGroup) { go.SetActive(true); }
                }
            }
            else
            {
                toggle.tweenTarget = SpriteUnchecked;
                SpriteChecked.SetActive(false);
                if (SpriteCheckedGroup != null)
                {
                    foreach (var go in SpriteCheckedGroup) { go.SetActive(false); }
                }
                SpriteUnchecked.SetActive(true);
                if (SpriteUnCheckedGroup != null)
                {
                    foreach (var go in SpriteUnCheckedGroup) { go.SetActive(true); }
                }
            }
        }
    }
}
