using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TwdCustomMod;
using TWDModel;
using UnityEngine;

public class TraitSelectorButton : NUIListItem<TraitSelectorButton>
{
    public UISprite Background;

    public UISprite Icon;

    public string TraitId;

    [SerializeField]
    protected UIButtonExtended button;

    public int traitLevel = 0;

    public override void Clear()
    {
        base.Clear();
        Helpers.GameObjectSetActive(Background, value: false);
        Helpers.GameObjectSetActive(Icon, value: false);
        TraitId = "";
        if (button != null)
        {
            button.Clear();
        }
    }

    private int GetLevelFromTraitID(string traiID)
    {
        var traitLevel = traiID.Split('_').Last();
        switch (traitLevel)
        {
            case "Low": return 0;
            case "Middle": return 1;
            case "High": return 2;
            case "Highest": return 3;
            default: return 0;
        }
    }

    public void Initialize(string data, UIButtonExtended.OnClickCallback onClickCallback = null)
    {
        Background.gameObject.SetActive(false);

        if (string.IsNullOrEmpty(data))
        {
            TraitId = "Ui_Icon_Trait_Unknown";
        }
        else
        {
            TraitId = data;

            traitLevel = GetLevelFromTraitID(data); 
        }

        if (Helpers.GameObjectSetActive(Icon, value: true))
        {
            Icon.spriteName = TraitId;
        }

        if (button != null)
        {
            if (onClickCallback != null)
            {
                button.SetClickCallback(onClickCallback);
            }
            else
            {
                button.Clear();
            }
        }
    }
}
