using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using TwdCustomMod;
using TWDModel;
using UnityEngine;

public class TokenSelectorButton : NUIListItem<TokenSelectorButton>
{
    public UISprite Icon;
    public UISprite Any_Icon;

    public string TokenID;
    public CurrencyType currencyType { get; private set; }
    public UILabel TokenName;

    [SerializeField]
    protected UIButtonExtended button;

    public int traitLevel = 0;

    public override void Clear()
    {
        base.Clear();
        Helpers.GameObjectSetActive(Icon, value: false);
        Helpers.GameObjectSetActive(Any_Icon, value: false);

        TokenID = "";
        if (button != null)
        {
            button.Clear();
        }
    }

    public void Initialize(string data, UIButtonExtended.OnClickCallback onClickCallback = null)
    {
        bool IsAny = false;
        currencyType = CurrencyType.None;

        if (string.IsNullOrEmpty(data) || data == "None")
        {
            data = "None";
            IsAny = true;
            TokenID = "Ui_Icon_GreyOut";
        }
        else
        {
            if (Enum.IsDefined(typeof(CurrencyType), data))
            {
                currencyType = (CurrencyType)Enum.Parse(typeof(CurrencyType), data);
            }
            if (currencyType != CurrencyType.None)
            {
                TokenID = HelpersGfx.GetTokenCurrencyIconName(currencyType);
            }
            else
            {
                TokenID = "Ui_Icon_GreyOut";
            }
        }

        Helpers.GameObjectSetActive(Any_Icon, value: IsAny);

        if (Helpers.GameObjectSetActive(Icon, value: true))
        {
            Icon.spriteName = TokenID;
        }

        if (TokenName != null)
        {
            var name = SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText("Currency." + data);
            HelpersUI.SetContentToLabel(TokenName, name);
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
