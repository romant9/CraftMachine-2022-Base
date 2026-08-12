using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TWDModel;
using BaseModel;
using TwdCustomMod;


public class ModelRandomCard : NUIListItem<ModelRandomItem>
{
    [SerializeField]
    private UILabel labelIndex;

    [SerializeField]
    private UILabel labelDescription;

    [SerializeField]
    private UILabel labelState;

    [SerializeField]
    private UILabel labelCallCount;

    [SerializeField]
    private UILabel labelType;

    [SerializeField]
    private UILabel labelSource;

    public ModelRandom modelRandom { get; private set; }

    [SerializeField]
    private BoxCollider tableCollider;

    public override void SetData(ModelRandomItem data)
    {
        base.SetData(data);
        
        if (labelDescription != null)
        {
            labelDescription.text = data.Description;
        }
        labelIndex.text = data.randomIndex.ToString();
        labelState.text = data.modelRandom.State.ToString();
        labelCallCount.text = data.modelRandom.CallCount.ToString();
        labelType.text = data.randomChangeType.ToString();
        labelSource.text = data.randomSource.ToString();
        modelRandom = data.modelRandom;
    }

    public override Vector3[] GetLocalCorners(bool useLocalScale = false)
    {
        float num = base.transform.localPosition.x + tableCollider.center.x - GetLocalSizeHalf(useLocalScale).x;
        float num2 = base.transform.localPosition.y + tableCollider.center.y - GetLocalSizeHalf(useLocalScale).y;
        float x = num + GetLocalSize(useLocalScale).x;
        float y = num2 + GetLocalSize(useLocalScale).y;
        NUIListItemBase.corners[0] = new Vector3(num, num2);
        NUIListItemBase.corners[1] = new Vector3(num, y);
        NUIListItemBase.corners[2] = new Vector3(x, y);
        NUIListItemBase.corners[3] = new Vector3(x, num2);
        return NUIListItemBase.corners;
    }

    public override void SetPosition(Vector3 newPosition)
    {
        if (newPosition != Vector3.zero)
        {
            newPosition.y -= boxCollider.center.y;
        }
        base.SetPosition(newPosition);
    }

    public void CopyData(UILabel label)
    {
        MyTools.CopyToClipboard(label.text);
    }
}
