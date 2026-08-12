using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIButtonToggleHelper : MonoBehaviour
{
    [SerializeField]
    private GameObject SpriteChecked;
    [SerializeField]
    private GameObject SpriteUnchecked;
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
                SpriteChecked.SetActive(true);
            }
            else
            {
                toggle.tweenTarget = SpriteUnchecked;
                SpriteChecked.SetActive(false);
                SpriteUnchecked.SetActive(true);
            }
        }
    }
}
