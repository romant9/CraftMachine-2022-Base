using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModelRandomInfoPopup : HUDElement
{
    [SerializeField]
    private UIButton exitButton;

    [SerializeField]
    private NUIScrollableList dropTableList;

    [SerializeField]
    private UITable contentsTable;

    public GameObject ModelRandomCardNormal;

    public void TryOpenWithNormalData(List<ModelRandomItem> items)
    {
        if (dropTableList != null && items != null && items.Count != 0)
        {
            if (ModelRandomCardNormal != null)
            {
                dropTableList.UpdateWithList(new List<ModelRandomItem>(items), ModelRandomCardNormal, ModelRandomCardNormal, callUpdateUI: true);
            }
            else
            {
                dropTableList.UpdateWithList(new List<ModelRandomItem>(items), "ModelRandom_List_Item", "ModelRandom_List_Item", callUpdateUI: true);
            }
            Open();
            if (dropTableList.currentItemsCount > 0)
            {
                contentsTable.Reposition();
                dropTableList.ResetScrollPosition();
            }
        }
    }  

    public override void Open()
    {
        base.Open();
    }

    public override void OnClickClose()
    {
        for (int i = 0; i < contentsTable.transform.childCount; i++)
        {
            contentsTable.transform.GetChild(i).gameObject.SetActive(false);
        }
        TweenManager.PlayTweenGroup(gameObject, 2, forward: true, OnCloseAnimOver);
    }

    public void CopyToClipboad()
    {
        PlayerRandomValues.Instance.CopyToClipboad();
    }
}
