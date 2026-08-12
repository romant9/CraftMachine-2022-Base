using UnityEngine;

public class TokenInfo: MonoBehaviour
{
    public LootEntry LootEntry {  get; set; }
    public UISprite sprite;
    public UILabel label;
    public int tokenAmount { get; set; }
    public int repeatCount { get; set; }

}

