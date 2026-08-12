using BaseModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TWDModel;
using UnityEngine;

public class EqupmentWithTraitsClass
{
    public EqupmentWithTraitsClass() { }

    public SurvivorModel Owner { get; set; }

    public string Id { get; set; }

    public string OriginModelSerialised { get; set; }
    public string OriginDataSerialised { get; set; }
    public ModelRandom PlayerRandom { get; set; }

    public EqupmentWithTraitsClass(EquipmentItemModel model, UpgradeTraitsData data)
    {
        Id = model.IdForAnalytics;
        Owner = model.Owner != null ? (SurvivorModel)model.Owner : null;
        OriginModelSerialised = OfflineManager.JsonSerializer.Serialize(model);
        DebugTWD.Log("OriginModelSerialised size is: " + (Encoding.Default.GetBytes(OriginModelSerialised).Length * 1024) + "kb");
        OriginDataSerialised = OfflineManager.JsonSerializer.Serialize(data);
        DebugTWD.Log("OriginDataSerialised size is: " + (Encoding.Default.GetBytes(OriginModelSerialised).Length * 1024) + "kb");
        PlayerRandom = new ModelRandom(GameManager.Instance.playerModel.PlayerRandom);
    }
}

