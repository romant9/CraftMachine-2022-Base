using System;
using UnityEngine;

[Serializable]
public class BMSymbol
{
	public string sequence;

	public string spriteName;

	public bool colored;

	public bool pixelPerfect;

	[NonSerialized]
	private UISpriteData mSprite;

	[NonSerialized]
	private bool mIsValid;

	[NonSerialized]
	private int mLength;

	[NonSerialized]
	private int mOffsetX;

	[NonSerialized]
	private int mOffsetY;

	[NonSerialized]
	private int mWidth;

	[NonSerialized]
	private int mHeight;

	[NonSerialized]
	private int mAdvance;

	[NonSerialized]
	private Rect mUV;

	public int length
	{
		get
		{
			if (mLength == 0)
			{
				mLength = sequence.Length;
			}
			return mLength;
		}
	}

	public int offsetX => mOffsetX;

	public int offsetY => mOffsetY;

	public int width => mWidth;

	public int height => mHeight;

	public int paddedHeight
	{
		get
		{
			if (mSprite == null)
			{
				return mHeight;
			}
			return mSprite.paddingTop + mSprite.paddingBottom + mSprite.height;
		}
	}

	public int advance => mAdvance;

	public Rect uvRect => mUV;

	public void MarkAsChanged()
	{
		mIsValid = false;
	}

	public bool Validate(INGUIAtlas atlas)
	{
		if (atlas == null)
		{
			return false;
		}
		if (!mIsValid)
		{
			if (string.IsNullOrEmpty(spriteName))
			{
				return false;
			}
			Texture texture = null;
			mSprite = atlas.GetSprite(spriteName);
			if (mSprite != null)
			{
				texture = atlas.texture;
				if (texture == null)
				{
					mSprite = null;
				}
				else
				{
					mUV = new Rect(mSprite.x, mSprite.y, mSprite.width, mSprite.height);
					mUV = NGUIMath.ConvertToTexCoords(mUV, texture.width, texture.height);
					mOffsetX = mSprite.paddingLeft;
					mOffsetY = mSprite.paddingTop;
					mWidth = mSprite.width;
					mHeight = mSprite.height;
					mAdvance = mSprite.width + (mSprite.paddingLeft + mSprite.paddingRight);
					mIsValid = true;
				}
			}
		}
		return mSprite != null;
	}
}
