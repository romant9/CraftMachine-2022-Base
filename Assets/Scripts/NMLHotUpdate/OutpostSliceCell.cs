using UnityEngine;

public class OutpostSliceCell : MonoBehaviour
{
	public string SpriteAttackerStartLocation = "Ui_Tile_Zone_Object";

	public string SpriteThreatSpawn = "Ui_Tile_Zone_Object";

	public string SpriteFlag = "Ui_Icon_Flag";

	public string SpriteResourceContainer = "Ui_Icon_Crate";

	public const string SpriteTileFullFilled = "Ui_Tile_Fullcover_Filled1";

	public string SpriteTileHalfWallPrefix = "Ui_Tile_Halfcover_";

	public string SpriteTileFullWallPrefix = "Ui_Tile_Fullcover_";

	public UISprite Background;

	public UISprite HalfWall;

	public UISprite FullWall;

	public int Depth;

	public string GetBackgroundSpriteName(bool isStartLocation, bool isThreatSpawn, bool isFlagPosition, bool isResourceContainerPosition)
	{
		if (isStartLocation)
		{
			return SpriteAttackerStartLocation;
		}
		if (isThreatSpawn)
		{
			return SpriteThreatSpawn;
		}
		if (isFlagPosition)
		{
			return SpriteFlag;
		}
		if (isResourceContainerPosition)
		{
			return SpriteResourceContainer;
		}
		return "";
	}

	public string GetWallSpriteName(string prefix, bool topBlocked, bool rightBlocked, bool bottomBlocked, bool leftBlocked)
	{
		if (!topBlocked && !rightBlocked && !bottomBlocked && !leftBlocked)
		{
			return null;
		}
		string text = prefix;
		if (topBlocked)
		{
			text += "0";
		}
		if (rightBlocked)
		{
			text += "1";
		}
		if (bottomBlocked)
		{
			text += "2";
		}
		if (leftBlocked)
		{
			text += "3";
		}
		return text;
	}

	public string GetHalfWallSpriteName(bool topBlocked, bool rightBlocked, bool bottomBlocked, bool leftBlocked)
	{
		return GetWallSpriteName(SpriteTileHalfWallPrefix, topBlocked, rightBlocked, bottomBlocked, leftBlocked);
	}

	public string GetFullWallSpriteName(bool topBlocked, bool rightBlocked, bool bottomBlocked, bool leftBlocked)
	{
		return GetWallSpriteName(SpriteTileFullWallPrefix, topBlocked, rightBlocked, bottomBlocked, leftBlocked);
	}

	private static bool HasBit(int bits, int bitIndex)
	{
		return (bits & (1 << bitIndex)) != 0;
	}

	public void Set(int moveBlockedBits, int visibilityBlockedBits, int dynamicMoveBlockedBits, int dynamicVisibilityBlockedBits, bool isStartLocation, bool isThreatSpawn, bool isFlagPosition, bool isResourceContainerPosition, int threatQuadrant = 0)
	{
		bool[] array = new bool[5];
		bool[] array2 = new bool[5];
		bool[] array3 = new bool[5];
		for (int i = 0; i < 5; i++)
		{
			array[i] = HasBit(moveBlockedBits, i) || HasBit(dynamicMoveBlockedBits, i);
			array3[i] = isStartLocation || isThreatSpawn || isFlagPosition || isResourceContainerPosition || HasBit(moveBlockedBits, i);
		}
		for (int j = 0; j < 5; j++)
		{
			array2[j] = HasBit(visibilityBlockedBits, j) || HasBit(dynamicVisibilityBlockedBits, j);
			array3[j] |= HasBit(visibilityBlockedBits, j);
		}
		if (Background != null)
		{
			Background.depth = 5 + Depth;
			string backgroundSpriteName = GetBackgroundSpriteName(isStartLocation, isThreatSpawn, isFlagPosition, isResourceContainerPosition);
			if (backgroundSpriteName != "")
			{
				Background.spriteName = backgroundSpriteName;
				Background.gameObject.SetActive(value: true);
				if (isThreatSpawn)
				{
					Background.transform.localEulerAngles = new Vector3(0f, 0f, 90f * (float)threatQuadrant);
				}
			}
			else
			{
				Background.gameObject.SetActive(value: false);
			}
		}
		if (FullWall != null)
		{
			string text = null;
			text = (((!array2[4] || !array[4]) && (array2[4] || !array[4])) ? GetFullWallSpriteName(array2[0], array2[1], array2[2], array2[3]) : "Ui_Tile_Fullcover_Filled1");
			if (text != null)
			{
				FullWall.spriteName = text;
			}
			FullWall.depth = 20 + Depth;
			FullWall.gameObject.SetActive(text != null);
		}
		if (HalfWall != null)
		{
			string halfWallSpriteName = GetHalfWallSpriteName(!array2[0] && array[0], !array2[1] && array[1], !array2[2] && array[2], !array2[3] && array[3]);
			if (halfWallSpriteName != null)
			{
				HalfWall.spriteName = halfWallSpriteName;
			}
			HalfWall.depth = 15 + Depth;
			HalfWall.gameObject.SetActive(halfWallSpriteName != null);
		}
	}
}
