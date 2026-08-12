using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using UnityEngine;

public static class NGUIText
{
	[DoNotObfuscateNGUI]
	public enum Alignment
	{
		Automatic = 0,
		Left = 1,
		Center = 2,
		Right = 3,
		Justified = 4
	}

	[DoNotObfuscateNGUI]
	public enum SymbolStyle
	{
		None = 0,
		Normal = 1,
		Colored = 2,
		NoOutline = 3
	}

	public class GlyphInfo
	{
		public Vector2 v0;

		public Vector2 v1;

		public Vector2 u0;

		public Vector2 u1;

		public Vector2 u2;

		public Vector2 u3;

		public float advance;

		public int channel;
	}

	public static INGUIFont nguiFont;

	public static Font dynamicFont;

	public static GlyphInfo glyph = new GlyphInfo();

	public static int spaceWidth = 0;

	public static int fontSize = 16;

	public static float fontScale = 1f;

	public static float pixelDensity = 1f;

	public static FontStyle fontStyle = FontStyle.Normal;

	public static Alignment alignment = Alignment.Left;

	public static Color tint = Color.white;

	public static int rectWidth = 1000000;

	public static int rectHeight = 1000000;

	public static int regionWidth = 1000000;

	public static int regionHeight = 1000000;

	public static int maxLines = 0;

	public static bool gradient = false;

	public static Color gradientBottom = Color.white;

	public static Color gradientTop = Color.white;

	public static bool encoding = false;

	public static float spacingX = 0f;

	public static float spacingY = 0f;

	public static bool premultiply = false;

	public static SymbolStyle symbolStyle;

	public static int finalSize = 0;

	public static float finalSpacingX = 0f;

	public static float finalLineHeight = 0f;

	public static float baseline = 0f;

	public static bool useSymbols = false;

	[NonSerialized]
	private static StringBuilder mTempSB;

	private static Color mInvisible = new Color(0f, 0f, 0f, 0f);

	private static BetterList<Color> mColors = new BetterList<Color>();

	private static float mAlpha = 1f;

	private static CharacterInfo mTempChar;

	private static BetterList<float> mSizes = new BetterList<float>();

	[NonSerialized]
	private static StringBuilder mSB;

	private static Color s_c0;

	private static Color s_c1;

	private static float[] mBoldOffset = new float[8] { -0.25f, 0f, 0.25f, 0f, 0f, -0.25f, 0f, 0.25f };

	private static float symbolScale
	{
		get
		{
			NGUIFont nGUIFont = nguiFont as NGUIFont;
			if (nGUIFont == null)
			{
				return 1f;
			}
			return nGUIFont.symbolScale * (float)finalSize / (float)nGUIFont.defaultSize;
		}
	}

	private static float symbolOffset
	{
		get
		{
			NGUIFont nGUIFont = nguiFont as NGUIFont;
			if (nGUIFont == null)
			{
				return 1f;
			}
			return nGUIFont.symbolOffset;
		}
	}

	private static int symbolMaxHeight
	{
		get
		{
			NGUIFont nGUIFont = nguiFont as NGUIFont;
			if (nGUIFont == null)
			{
				return 0;
			}
			return nGUIFont.symbolMaxHeight;
		}
	}

	private static bool symbolCentered
	{
		get
		{
			NGUIFont nGUIFont = nguiFont as NGUIFont;
			if (nGUIFont == null)
			{
				return false;
			}
			return nGUIFont.symbolCentered;
		}
	}

	public static void Update()
	{
		Update(request: true);
	}

	public static void Update(bool request)
	{
		finalSize = Mathf.RoundToInt((float)fontSize / pixelDensity);
		finalSpacingX = spacingX * fontScale;
		finalLineHeight = ((float)fontSize + spacingY) * fontScale;
		useSymbols = nguiFont != null && encoding && symbolStyle != SymbolStyle.None;
		Font font = dynamicFont;
		if (!(font != null && request))
		{
			return;
		}
		font.RequestCharactersInTexture(")_-.", finalSize, fontStyle);
		if (!font.GetCharacterInfo(')', out mTempChar, finalSize, fontStyle) || (float)mTempChar.maxY == 0f)
		{
			font.RequestCharactersInTexture("A", finalSize, fontStyle);
			if (!font.GetCharacterInfo('A', out mTempChar, finalSize, fontStyle))
			{
				baseline = 0f;
				return;
			}
		}
		float num = mTempChar.maxY;
		float num2 = mTempChar.minY;
		baseline = Mathf.Round(num + ((float)finalSize - num + num2) * 0.5f);
	}

	public static void Prepare(string text)
	{
		if (!(dynamicFont != null))
		{
			return;
		}
		if (!encoding || symbolStyle == SymbolStyle.None)
		{
			dynamicFont.RequestCharactersInTexture(text, finalSize, NGUIText.fontStyle);
			return;
		}
		if (mTempSB == null)
		{
			mTempSB = new StringBuilder();
		}
		else
		{
			mTempSB.Length = 0;
		}
		bool bold = false;
		bool italic = false;
		bool underline = false;
		bool strike = false;
		bool ignoreColor = false;
		bool forceSpriteColor = false;
		FontStyle fontStyle = NGUIText.fontStyle;
		int sub = 0;
		float fontScaleMult = 0f;
		int i = 0;
		for (int length = text.Length; i < length; i++)
		{
			if (ParseSymbol(text, ref i, null, premultiply: false, ref sub, ref fontScaleMult, ref bold, ref italic, ref underline, ref strike, ref ignoreColor, ref forceSpriteColor))
			{
				i--;
				continue;
			}
			FontStyle fontStyle2 = NGUIText.fontStyle;
			if (bold && italic)
			{
				fontStyle2 = FontStyle.BoldAndItalic;
			}
			else if (bold)
			{
				fontStyle2 = FontStyle.Bold;
			}
			else if (italic)
			{
				fontStyle2 = FontStyle.Italic;
			}
			if (fontStyle != fontStyle2)
			{
				if (mTempSB.Length != 0)
				{
					dynamicFont.RequestCharactersInTexture(mTempSB.ToString(), finalSize, fontStyle);
				}
				fontStyle = fontStyle2;
				mTempSB.Clear();
			}
			mTempSB.Append(text[i]);
		}
		if (mTempSB.Length != 0)
		{
			string characters = mTempSB.ToString();
			dynamicFont.RequestCharactersInTexture(characters, finalSize, fontStyle);
			mTempSB.Clear();
		}
	}

	public static BMSymbol GetSymbol(ref string text, int index, int textLength)
	{
		if (nguiFont != null)
		{
			return nguiFont.MatchSymbol(ref text, index, textLength);
		}
		return null;
	}

	public static float GetGlyphWidth(int ch, int prev, float fontScale, bool bold, bool italic)
	{
		if (spaceWidth != 0 && ch == 32)
		{
			return Mathf.RoundToInt((float)spaceWidth * fontScale * pixelDensity * ((float)finalSize / (float)dynamicFont.fontSize));
		}
		if (dynamicFont != null)
		{
			FontStyle style = fontStyle;
			if (bold && italic)
			{
				style = FontStyle.BoldAndItalic;
			}
			else if (italic)
			{
				style = FontStyle.Italic;
			}
			else if (bold)
			{
				style = FontStyle.Bold;
			}
			if (dynamicFont.GetCharacterInfo((char)ch, out mTempChar, finalSize, style))
			{
				return (float)mTempChar.advance * fontScale * pixelDensity;
			}
		}
		else if (nguiFont != null)
		{
			bool flag = false;
			if (ch == 8201)
			{
				flag = true;
				ch = 32;
			}
			BMGlyph bMGlyph = null;
			if (nguiFont != null)
			{
				bMGlyph = nguiFont.bmFont.GetGlyph(ch);
			}
			if (bMGlyph != null)
			{
				int num = bMGlyph.advance;
				if (flag)
				{
					num >>= 1;
				}
				return fontScale * (float)((prev != 0) ? (num + bMGlyph.GetKerning(prev)) : bMGlyph.advance);
			}
		}
		return 0f;
	}

	public static GlyphInfo GetGlyph(int ch, int prev, bool bold, bool italic, float fontScale = 1f)
	{
		if (dynamicFont != null)
		{
			FontStyle style = fontStyle;
			if (bold && italic)
			{
				style = FontStyle.BoldAndItalic;
			}
			else if (italic)
			{
				style = FontStyle.Italic;
			}
			else if (bold)
			{
				style = FontStyle.Bold;
			}
			if (dynamicFont.GetCharacterInfo((char)ch, out mTempChar, finalSize, style))
			{
				int num = 0;
				NGUIFont nGUIFont = nguiFont as NGUIFont;
				if (nGUIFont != null)
				{
					num = nGUIFont.GetKerning(prev, ch);
					if (num != 0)
					{
						num = Mathf.RoundToInt((float)num * ((float)finalSize / (float)dynamicFont.fontSize));
					}
				}
				glyph.v0.x = mTempChar.minX + num;
				glyph.v1.x = mTempChar.maxX + num;
				glyph.v0.y = (float)mTempChar.maxY - baseline;
				glyph.v1.y = (float)mTempChar.minY - baseline;
				glyph.u0 = mTempChar.uvTopLeft;
				glyph.u1 = mTempChar.uvBottomLeft;
				glyph.u2 = mTempChar.uvBottomRight;
				glyph.u3 = mTempChar.uvTopRight;
				glyph.advance = mTempChar.advance + num;
				glyph.channel = 0;
				glyph.v0.x = Mathf.Round(glyph.v0.x);
				glyph.v0.y = Mathf.Round(glyph.v0.y);
				glyph.v1.x = Mathf.Round(glyph.v1.x);
				glyph.v1.y = Mathf.Round(glyph.v1.y);
				if (ch == 32 && spaceWidth != 0)
				{
					glyph.advance = Mathf.RoundToInt((float)spaceWidth * ((float)finalSize / (float)dynamicFont.fontSize));
				}
				float num2 = fontScale * pixelDensity;
				if (num2 != 1f)
				{
					glyph.v0 *= num2;
					glyph.v1 *= num2;
					glyph.advance *= num2;
				}
				return glyph;
			}
		}
		else if (nguiFont != null && nguiFont.bmFont != null)
		{
			bool flag = false;
			if (ch == 8201)
			{
				flag = true;
				ch = 32;
			}
			BMGlyph bMGlyph = nguiFont.bmFont.GetGlyph(ch);
			if (bMGlyph != null)
			{
				int num3 = ((prev != 0) ? bMGlyph.GetKerning(prev) : 0);
				glyph.v0.x = bMGlyph.offsetX + num3;
				glyph.v1.y = -bMGlyph.offsetY;
				glyph.v1.x = glyph.v0.x + (float)bMGlyph.width;
				glyph.v0.y = glyph.v1.y - (float)bMGlyph.height;
				glyph.u0.x = bMGlyph.x;
				glyph.u0.y = bMGlyph.y + bMGlyph.height;
				glyph.u2.x = bMGlyph.x + bMGlyph.width;
				glyph.u2.y = bMGlyph.y;
				glyph.u1.x = glyph.u0.x;
				glyph.u1.y = glyph.u2.y;
				glyph.u3.x = glyph.u2.x;
				glyph.u3.y = glyph.u0.y;
				int num4 = bMGlyph.advance;
				if (ch == 32 && spaceWidth != 0)
				{
					num4 = spaceWidth;
				}
				if (flag)
				{
					num4 >>= 1;
				}
				glyph.advance = num4 + num3;
				glyph.channel = bMGlyph.channel;
				if (fontScale != 1f)
				{
					glyph.v0 *= fontScale;
					glyph.v1 *= fontScale;
					glyph.advance *= fontScale;
				}
				return glyph;
			}
		}
		return null;
	}

	[DebuggerHidden]
	[DebuggerStepThrough]
	public static float ParseAlpha(string text, int index)
	{
		return Mathf.Clamp01((float)((NGUIMath.HexToDecimal(text[index + 1]) << 4) | NGUIMath.HexToDecimal(text[index + 2])) / 255f);
	}

	[DebuggerHidden]
	[DebuggerStepThrough]
	public static Color ParseColor(string text, int offset = 0)
	{
		return ParseColor24(text, offset);
	}

	[DebuggerHidden]
	[DebuggerStepThrough]
	public static Color ParseColor24(string text, int offset = 0)
	{
		int num = (NGUIMath.HexToDecimal(text[offset]) << 4) | NGUIMath.HexToDecimal(text[offset + 1]);
		int num2 = (NGUIMath.HexToDecimal(text[offset + 2]) << 4) | NGUIMath.HexToDecimal(text[offset + 3]);
		int num3 = (NGUIMath.HexToDecimal(text[offset + 4]) << 4) | NGUIMath.HexToDecimal(text[offset + 5]);
		float num4 = 0.003921569f;
		return new Color(num4 * (float)num, num4 * (float)num2, num4 * (float)num3);
	}

	[DebuggerHidden]
	[DebuggerStepThrough]
	public static bool ParseColor24(ref string text, int offset, out Color c)
	{
		int num = NGUIMath.HexToDecimal(text[offset], -1);
		int num2 = NGUIMath.HexToDecimal(text[offset + 1], -1);
		int num3 = NGUIMath.HexToDecimal(text[offset + 2], -1);
		int num4 = NGUIMath.HexToDecimal(text[offset + 3], -1);
		int num5 = NGUIMath.HexToDecimal(text[offset + 4], -1);
		int num6 = NGUIMath.HexToDecimal(text[offset + 5], -1);
		if ((num | num2 | num3 | num4 | num5 | num6) == -1)
		{
			c = Color.white;
			return false;
		}
		int num7 = (num << 4) | num2;
		int num8 = (num3 << 4) | num4;
		int num9 = (num5 << 4) | num6;
		float num10 = 0.003921569f;
		c = new Color(num10 * (float)num7, num10 * (float)num8, num10 * (float)num9);
		return true;
	}

	[DebuggerHidden]
	[DebuggerStepThrough]
	public static Color ParseColor32(string text, int offset)
	{
		int num = (NGUIMath.HexToDecimal(text[offset]) << 4) | NGUIMath.HexToDecimal(text[offset + 1]);
		int num2 = (NGUIMath.HexToDecimal(text[offset + 2]) << 4) | NGUIMath.HexToDecimal(text[offset + 3]);
		int num3 = (NGUIMath.HexToDecimal(text[offset + 4]) << 4) | NGUIMath.HexToDecimal(text[offset + 5]);
		int num4 = (NGUIMath.HexToDecimal(text[offset + 6]) << 4) | NGUIMath.HexToDecimal(text[offset + 7]);
		float num5 = 0.003921569f;
		return new Color(num5 * (float)num, num5 * (float)num2, num5 * (float)num3, num5 * (float)num4);
	}

	[DebuggerHidden]
	[DebuggerStepThrough]
	public static bool ParseColor32(ref string text, int offset, out Color c)
	{
		int num = NGUIMath.HexToDecimal(text[offset], -1);
		int num2 = NGUIMath.HexToDecimal(text[offset + 1], -1);
		int num3 = NGUIMath.HexToDecimal(text[offset + 2], -1);
		int num4 = NGUIMath.HexToDecimal(text[offset + 3], -1);
		int num5 = NGUIMath.HexToDecimal(text[offset + 4], -1);
		int num6 = NGUIMath.HexToDecimal(text[offset + 5], -1);
		int num7 = NGUIMath.HexToDecimal(text[offset + 6], -1);
		int num8 = NGUIMath.HexToDecimal(text[offset + 7], -1);
		if ((num | num2 | num3 | num4 | num5 | num6 | num7 | num8) == -1)
		{
			c = Color.white;
			return false;
		}
		int num9 = (num << 4) | num2;
		int num10 = (num3 << 4) | num4;
		int num11 = (num5 << 4) | num6;
		int num12 = (num7 << 4) | num8;
		float num13 = 0.003921569f;
		c = new Color(num13 * (float)num9, num13 * (float)num10, num13 * (float)num11, num13 * (float)num12);
		return true;
	}

	[DebuggerHidden]
	[DebuggerStepThrough]
	public static string EncodeColor(Color c)
	{
		return EncodeColor24(c);
	}

	[DebuggerHidden]
	[DebuggerStepThrough]
	public static string EncodeColor(string text, Color c)
	{
		return "[c][" + EncodeColor24(c) + "]" + text + "[-][/c]";
	}

	[DebuggerHidden]
	[DebuggerStepThrough]
	public static string EncodeAlpha(float a)
	{
		return NGUIMath.DecimalToHex8(Mathf.Clamp(Mathf.RoundToInt(a * 255f), 0, 255));
	}

	[DebuggerHidden]
	[DebuggerStepThrough]
	public static string EncodeColor24(Color c)
	{
		return NGUIMath.DecimalToHex24(0xFFFFFF & (NGUIMath.ColorToInt(c) >> 8));
	}

	[DebuggerHidden]
	[DebuggerStepThrough]
	public static string EncodeColor32(Color c)
	{
		return NGUIMath.DecimalToHex32(NGUIMath.ColorToInt(c));
	}

	public static bool ParseSymbol(string text, ref int index)
	{
		int sub = 0;
		bool bold = false;
		bool italic = false;
		bool underline = false;
		bool strike = false;
		bool ignoreColor = false;
		bool forceSpriteColor = false;
		float fontScaleMult = 0f;
		return ParseSymbol(text, ref index, null, premultiply: false, ref sub, ref fontScaleMult, ref bold, ref italic, ref underline, ref strike, ref ignoreColor, ref forceSpriteColor);
	}

	[DebuggerHidden]
	[DebuggerStepThrough]
	public static bool IsHex(char ch)
	{
		if ((ch < '0' || ch > '9') && (ch < 'a' || ch > 'f'))
		{
			if (ch >= 'A')
			{
				return ch <= 'F';
			}
			return false;
		}
		return true;
	}

	public static bool ParseSymbol(string text, ref int index, BetterList<Color> colors, bool premultiply, ref int sub, ref float fontScaleMult, ref bool bold, ref bool italic, ref bool underline, ref bool strike, ref bool ignoreColor, ref bool forceSpriteColor)
	{
		int length = text.Length;
		if (index + 3 > length || text[index] != '[')
		{
			return false;
		}
		char c = text[index + 1];
		char c2 = text[index + 2];
		switch (c2)
		{
		case ']':
			switch (c)
			{
			case '-':
				if (colors != null && colors.size > 1)
				{
					colors.RemoveAt(colors.size - 1);
				}
				index += 3;
				return true;
			case 'B':
			case 'b':
				index += 3;
				bold = true;
				return true;
			case 'I':
			case 'i':
				index += 3;
				italic = true;
				return true;
			case 'U':
			case 'u':
				index += 3;
				underline = true;
				return true;
			case 'S':
			case 's':
				index += 3;
				strike = true;
				return true;
			case 'C':
			case 'c':
				index += 3;
				ignoreColor = true;
				return true;
			case 'T':
			case 't':
				index += 3;
				forceSpriteColor = true;
				return true;
			}
			break;
		case '=':
			if (c == 'y' || c == 'Y')
			{
				int num = text.IndexOf(']', index + 4);
				if (num != -1 && float.TryParse(text.Substring(index + 3, num - (index + 3)), out fontScaleMult))
				{
					sub = 0;
					index = num + 1;
					return true;
				}
			}
			break;
		}
		if (index + 4 > length)
		{
			return false;
		}
		char c3 = text[index + 3];
		if (c3 == ']')
		{
			if (c == '/')
			{
				switch (c2)
				{
				case 'B':
				case 'b':
					index += 4;
					bold = false;
					return true;
				case 'I':
				case 'i':
					index += 4;
					italic = false;
					return true;
				case 'U':
				case 'u':
					index += 4;
					underline = false;
					return true;
				case 'S':
				case 's':
					index += 4;
					strike = false;
					return true;
				case 'C':
				case 'c':
					index += 4;
					ignoreColor = false;
					return true;
				case 'T':
				case 't':
					index += 4;
					forceSpriteColor = false;
					return true;
				case 'Y':
				case 'y':
					index += 4;
					sub = 0;
					fontScaleMult = 0f;
					return true;
				}
			}
			if (IsHex(c) && IsHex(c2))
			{
				mAlpha = (float)((NGUIMath.HexToDecimal(c) << 4) | NGUIMath.HexToDecimal(c2)) / 255f;
				index += 4;
				return true;
			}
		}
		if (index + 5 > length)
		{
			return false;
		}
		char c4 = text[index + 4];
		if ((c == 's' || c == 'S') && (c2 == 'u' || c2 == 'U'))
		{
			switch (c3)
			{
			case 'B':
			case 'b':
				switch (c4)
				{
				case ']':
					sub = 1;
					fontScaleMult = 0.75f;
					index += 5;
					return true;
				case '=':
				{
					int num3 = text.IndexOf(']', index + 4);
					if (num3 != -1 && float.TryParse(text.Substring(index + 5, num3 - (index + 5)), out fontScaleMult))
					{
						sub = 1;
						index = num3 + 1;
						return true;
					}
					break;
				}
				}
				break;
			case 'P':
			case 'p':
				switch (c4)
				{
				case ']':
					sub = 2;
					fontScaleMult = 0.75f;
					index += 5;
					return true;
				case '=':
				{
					int num2 = text.IndexOf(']', index + 4);
					if (num2 != -1 && float.TryParse(text.Substring(index + 5, num2 - (index + 5)), out fontScaleMult))
					{
						sub = 2;
						index = num2 + 1;
						return true;
					}
					break;
				}
				}
				break;
			}
		}
		if (index + 6 > length)
		{
			return false;
		}
		if (text[index + 5] == ']' && c == '/')
		{
			if ((c2 == 's' || c2 == 'S') && (c3 == 'u' || c3 == 'U'))
			{
				switch (c4)
				{
				case 'B':
				case 'b':
					sub = 0;
					fontScaleMult = 0f;
					index += 6;
					return true;
				case 'P':
				case 'p':
					sub = 0;
					fontScaleMult = 0f;
					index += 6;
					return true;
				}
			}
			else if ((c2 == 'u' || c2 == 'U') && (c3 == 'r' || c3 == 'R') && (c4 == 'l' || c4 == 'L'))
			{
				index += 6;
				return true;
			}
		}
		if ((c4 == '=' && c == 'u' && c2 == 'r' && c3 == 'l') || (c == 'U' && c2 == 'R' && c3 == 'L'))
		{
			int num4 = text.IndexOf(']', index + 4);
			if (num4 != -1)
			{
				index = num4 + 1;
				return true;
			}
			index = text.Length;
			return true;
		}
		if (index + 8 > length)
		{
			return false;
		}
		if (text[index + 7] == ']')
		{
			if (!ParseColor24(ref text, index + 1, out var c5))
			{
				return false;
			}
			if (colors != null && colors.size > 0)
			{
				c5.a = colors.buffer[colors.size - 1].a;
				if (premultiply && c5.a != 1f)
				{
					c5 = Color.Lerp(mInvisible, c5, c5.a);
				}
				colors.Add(c5);
			}
			index += 8;
			return true;
		}
		if (index + 10 > length)
		{
			return false;
		}
		if (text[index + 9] == ']')
		{
			if (!ParseColor32(ref text, index + 1, out var c6))
			{
				return false;
			}
			if (colors != null)
			{
				if (premultiply && c6.a != 1f)
				{
					c6 = Color.Lerp(mInvisible, c6, c6.a);
				}
				colors.Add(c6);
			}
			index += 10;
			return true;
		}
		return false;
	}

	public static string StripSymbols(string text)
	{
		if (text != null)
		{
			int num = 0;
			int length = text.Length;
			while (num < length)
			{
				if (text[num] == '[')
				{
					int sub = 0;
					bool bold = false;
					bool italic = false;
					bool underline = false;
					bool strike = false;
					bool ignoreColor = false;
					bool forceSpriteColor = false;
					int index = num;
					float fontScaleMult = 0f;
					if (ParseSymbol(text, ref index, null, premultiply: false, ref sub, ref fontScaleMult, ref bold, ref italic, ref underline, ref strike, ref ignoreColor, ref forceSpriteColor))
					{
						text = text.Remove(num, index - num);
						length = text.Length;
						continue;
					}
				}
				num++;
			}
		}
		return text;
	}

	public static void Align(List<Vector3> verts, int indexOffset, float printedWidth, int elements = 4)
	{
		switch (alignment)
		{
		case Alignment.Right:
		{
			float num12 = (float)rectWidth - printedWidth;
			if (!(num12 < 0f))
			{
				int j = indexOffset;
				for (int count3 = verts.Count; j < count3; j++)
				{
					Vector3 value3 = verts[j];
					value3.x += num12;
					verts[j] = value3;
				}
			}
			break;
		}
		case Alignment.Center:
		{
			float num9 = ((float)rectWidth - printedWidth) * 0.5f;
			if (!(num9 < 0f))
			{
				int num10 = Mathf.RoundToInt((float)rectWidth - printedWidth);
				int num11 = Mathf.RoundToInt(rectWidth);
				bool flag = (num10 & 1) == 1;
				bool flag2 = (num11 & 1) == 1;
				if ((flag && !flag2) || (!flag && flag2))
				{
					num9 += 0.5f * fontScale;
				}
				int i = indexOffset;
				for (int count2 = verts.Count; i < count2; i++)
				{
					Vector3 value2 = verts[i];
					value2.x += num9;
					verts[i] = value2;
				}
			}
			break;
		}
		case Alignment.Justified:
		{
			if (printedWidth < (float)rectWidth * 0.65f || ((float)rectWidth - printedWidth) * 0.5f < 1f)
			{
				break;
			}
			int num = (verts.Count - indexOffset) / elements;
			if (num < 1)
			{
				break;
			}
			float num2 = 1f / (float)(num - 1);
			float num3 = (float)rectWidth / printedWidth;
			int num4 = indexOffset + elements;
			int num5 = 1;
			int count = verts.Count;
			while (num4 < count)
			{
				float x = verts[num4].x;
				float x2 = verts[num4 + elements / 2].x;
				float num6 = x2 - x;
				float num7 = x * num3;
				float a = num7 + num6;
				float num8 = x2 * num3;
				float b = num8 - num6;
				float t = (float)num5 * num2;
				x2 = Mathf.Lerp(a, num8, t);
				x = Mathf.Lerp(num7, b, t);
				x = Mathf.Round(x);
				x2 = Mathf.Round(x2);
				switch (elements)
				{
				case 4:
				{
					Vector3 value = verts[num4];
					value.x = x;
					verts[num4++] = value;
					value = verts[num4];
					value.x = x;
					verts[num4++] = value;
					value = verts[num4];
					value.x = x2;
					verts[num4++] = value;
					value = verts[num4];
					value.x = x2;
					verts[num4++] = value;
					break;
				}
				case 2:
				{
					Vector3 value = verts[num4];
					value.x = x;
					verts[num4++] = value;
					value = verts[num4];
					value.x = x2;
					verts[num4++] = value;
					break;
				}
				case 1:
				{
					Vector3 value = verts[num4];
					value.x = x;
					verts[num4++] = value;
					break;
				}
				}
				num5++;
			}
			break;
		}
		}
	}

	public static int GetExactCharacterIndex(List<Vector3> verts, List<int> indices, Vector2 pos)
	{
		int i = 0;
		for (int count = indices.Count; i < count; i++)
		{
			int num = i << 1;
			int index = num + 1;
			float x = verts[num].x;
			if (pos.x < x)
			{
				continue;
			}
			float x2 = verts[index].x;
			if (pos.x > x2)
			{
				continue;
			}
			float y = verts[num].y;
			if (!(pos.y < y))
			{
				float y2 = verts[index].y;
				if (!(pos.y > y2))
				{
					return indices[i];
				}
			}
		}
		return 0;
	}

	public static int GetApproximateCharacterIndex(List<Vector3> verts, List<int> indices, Vector2 pos)
	{
		float num = float.MaxValue;
		float num2 = float.MaxValue;
		int index = 0;
		int i = 0;
		for (int count = verts.Count; i < count; i++)
		{
			float num3 = Mathf.Abs(pos.y - verts[i].y);
			if (!(num3 > num2))
			{
				float num4 = Mathf.Abs(pos.x - verts[i].x);
				if (num3 < num2)
				{
					num2 = num3;
					num = num4;
					index = i;
				}
				else if (num4 < num)
				{
					num = num4;
					index = i;
				}
			}
		}
		return indices[index];
	}

	[DebuggerHidden]
	[DebuggerStepThrough]
	public static bool IsSpace(int ch)
	{
		if (ch != 32 && ch != 8202 && ch != 8203)
		{
			return ch == 8201;
		}
		return true;
	}

	[DebuggerHidden]
	[DebuggerStepThrough]
	public static void EndLine(ref StringBuilder s)
	{
		int num = s.Length - 1;
		if (num > 0 && IsSpace(s[num]))
		{
			s[num] = '\n';
		}
		else
		{
			s.Append('\n');
		}
	}

	[DebuggerHidden]
	[DebuggerStepThrough]
	private static void ReplaceSpaceWithNewline(ref StringBuilder s)
	{
		int num = s.Length - 1;
		if (num > 0 && IsSpace(s[num]))
		{
			s[num] = '\n';
		}
	}

	public static Vector2 CalculatePrintedSize(string text, bool prepare = true)
	{
		Vector2 zero = Vector2.zero;
		if (!string.IsNullOrEmpty(text))
		{
			if (prepare)
			{
				Prepare(text);
			}
			mColors.Clear();
			int num = 0;
			int prev = 0;
			float num2 = 0f;
			float num3 = 0f;
			float num4 = 0f;
			float num5 = (float)regionWidth + 0.01f;
			int length = text.Length;
			int sub = 0;
			bool bold = false;
			bool italic = false;
			bool underline = false;
			bool strike = false;
			bool ignoreColor = false;
			bool forceSpriteColor = false;
			float num6 = symbolScale;
			int num7 = symbolMaxHeight;
			float fontScaleMult = 0f;
			for (int i = 0; i < length; i++)
			{
				num = text[i];
				if (encoding && ParseSymbol(text, ref i, mColors, premultiply, ref sub, ref fontScaleMult, ref bold, ref italic, ref underline, ref strike, ref ignoreColor, ref forceSpriteColor))
				{
					i--;
					continue;
				}
				if (num == 10)
				{
					if (num2 > num4)
					{
						num4 = num2;
					}
					num2 = 0f;
					num3 += finalLineHeight;
					prev = 0;
					continue;
				}
				if (num < 32)
				{
					prev = num;
					continue;
				}
				BMSymbol bMSymbol = (useSymbols ? GetSymbol(ref text, i, length) : null);
				float num8 = ((sub != 0) ? (fontScale * fontScaleMult) : ((fontScaleMult == 0f) ? fontScale : (fontScale * fontScaleMult)));
				if (bMSymbol != null)
				{
					int paddedHeight = bMSymbol.paddedHeight;
					if (!bMSymbol.pixelPerfect && num7 != 0 && paddedHeight > num7)
					{
						num8 *= (float)num7 / (float)paddedHeight;
					}
					float num9 = (bMSymbol.pixelPerfect ? ((float)bMSymbol.advance) : Mathf.Round((float)bMSymbol.advance * num8 * num6));
					float num10 = num2 + num9;
					if (num10 > num5)
					{
						if (num2 == 0f)
						{
							break;
						}
						if (num2 > num4)
						{
							num4 = num2;
						}
						num2 = 0f;
						num3 += finalLineHeight;
					}
					else if (num10 > num4)
					{
						num4 = num10;
					}
					num2 += num9 + finalSpacingX;
					i += bMSymbol.length - 1;
					prev = 0;
					continue;
				}
				GlyphInfo glyphInfo = GetGlyph(num, prev, bold, italic, num8);
				if (glyphInfo == null)
				{
					continue;
				}
				prev = num;
				float advance = glyphInfo.advance;
				switch (sub)
				{
				case 1:
				{
					float num12 = fontScale * (float)fontSize * 0.4f;
					glyphInfo.v0.y -= num12;
					glyphInfo.v1.y -= num12;
					break;
				}
				default:
				{
					float num11 = fontScale * (float)fontSize * 0.05f;
					glyphInfo.v0.y += num11;
					glyphInfo.v1.y += num11;
					break;
				}
				case 0:
					break;
				}
				advance += finalSpacingX;
				float num13 = num2 + advance;
				if (num13 > num5)
				{
					if (num2 == 0f)
					{
						continue;
					}
					num2 = 0f;
					num3 += finalLineHeight;
				}
				else if (num13 > num4)
				{
					num4 = num13;
				}
				if (IsSpace(num))
				{
					if (underline)
					{
						num = 95;
					}
					else if (strike)
					{
						num = 45;
					}
				}
				num2 = num13;
				if (sub != 0)
				{
					num2 = Mathf.Round(num2);
				}
				IsSpace(num);
			}
			zero.x = Mathf.Ceil((num2 > num4) ? (num2 - finalSpacingX) : num4);
			zero.y = Mathf.Ceil(num3 + finalLineHeight);
		}
		return zero;
	}

	public static int CalculateOffsetToFit(string text, bool prepare = true)
	{
		if (string.IsNullOrEmpty(text) || regionWidth < 1)
		{
			return 0;
		}
		if (prepare)
		{
			Prepare(text);
		}
		mColors.Clear();
		int length = text.Length;
		int prev = 0;
		int sub = 0;
		bool bold = false;
		bool italic = false;
		bool underline = false;
		bool strike = false;
		bool ignoreColor = false;
		bool forceSpriteColor = false;
		float num = symbolScale;
		int num2 = symbolMaxHeight;
		float fontScaleMult = 0f;
		int i = 0;
		for (int length2 = text.Length; i < length2; i++)
		{
			if (encoding && ParseSymbol(text, ref i, mColors, premultiply, ref sub, ref fontScaleMult, ref bold, ref italic, ref underline, ref strike, ref ignoreColor, ref forceSpriteColor))
			{
				i--;
				continue;
			}
			BMSymbol bMSymbol = (useSymbols ? GetSymbol(ref text, i, length) : null);
			float num3 = ((sub != 0) ? (fontScale * fontScaleMult) : ((fontScaleMult == 0f) ? fontScale : (fontScale * fontScaleMult)));
			if (bMSymbol == null)
			{
				char num4 = text[i];
				float glyphWidth = GetGlyphWidth(num4, prev, num3, bold, italic);
				if (glyphWidth != 0f)
				{
					mSizes.Add(finalSpacingX + glyphWidth);
				}
				prev = num4;
				continue;
			}
			int paddedHeight = bMSymbol.paddedHeight;
			if (!bMSymbol.pixelPerfect && num2 != 0 && paddedHeight > num2)
			{
				num3 *= (float)num2 / (float)paddedHeight;
			}
			mSizes.Add(finalSpacingX + (bMSymbol.pixelPerfect ? ((float)bMSymbol.advance) : Mathf.Round((float)bMSymbol.advance * num3 * num)));
			int j = 0;
			for (int num5 = bMSymbol.sequence.Length - 1; j < num5; j++)
			{
				mSizes.Add(0f);
			}
			i += bMSymbol.sequence.Length - 1;
			prev = 0;
		}
		float num6 = regionWidth;
		int num7 = mSizes.size;
		while (num7 > 0 && num6 > 0f)
		{
			num6 -= mSizes.buffer[--num7];
		}
		mSizes.Clear();
		if (num6 < 0f)
		{
			num7++;
		}
		return num7;
	}

	public static string GetEndOfLineThatFits(string text)
	{
		int length = text.Length;
		int num = CalculateOffsetToFit(text);
		return text.Substring(num, length - num);
	}

	public static bool WrapText(string text, out string finalText, bool wrapLineColors = false)
	{
		return WrapText(text, out finalText, keepCharCount: false, wrapLineColors);
	}

	public static bool WrapText(string text, out string finalText, bool keepCharCount, bool wrapLineColors, bool useEllipsis = false)
	{
		if (regionWidth < 1 || regionHeight < 1 || finalLineHeight < 1f)
		{
			finalText = "";
			return false;
		}
		float num = ((maxLines > 0) ? Mathf.Min(regionHeight, finalLineHeight * (float)maxLines) : ((float)regionHeight));
		int num2 = ((maxLines > 0) ? maxLines : 1000000);
		num2 = Mathf.FloorToInt(Mathf.Min(num2, num / finalLineHeight) + 0.01f);
		if (num2 == 0)
		{
			finalText = "";
			return false;
		}
		if (string.IsNullOrEmpty(text))
		{
			text = " ";
		}
		int length = text.Length;
		Prepare(text);
		mColors.Clear();
		if (mSB == null)
		{
			mSB = new StringBuilder();
		}
		else
		{
			mSB.Length = 0;
		}
		float num3 = regionWidth;
		float num4 = 0f;
		int i = 0;
		int j = 0;
		int num5 = 1;
		int prev = 0;
		bool flag = true;
		bool flag2 = true;
		bool flag3 = false;
		Color color = tint;
		int sub = 0;
		bool bold = false;
		bool italic = false;
		bool underline = false;
		bool strike = false;
		bool ignoreColor = false;
		bool forceSpriteColor = false;
		float num6 = (useEllipsis ? ((finalSpacingX + GetGlyphWidth(46, 46, fontScale, bold, italic)) * 3f) : finalSpacingX);
		float num7 = symbolScale;
		int num8 = symbolMaxHeight;
		int num9 = 0;
		float fontScaleMult = 0f;
		mColors.Add(color);
		if (!useSymbols)
		{
			wrapLineColors = false;
		}
		if (wrapLineColors)
		{
			mSB.Append("[");
			mSB.Append(EncodeColor(color));
			mSB.Append("]");
		}
		for (; j < length; j++)
		{
			char c = text[j];
			bool flag4 = IsSpace(c);
			if (c > '\u2fff')
			{
				flag3 = true;
			}
			if (c == '\n')
			{
				if (num5 == num2)
				{
					break;
				}
				num4 = 0f;
				if (i < j)
				{
					mSB.Append(text, i, j - i + 1);
				}
				else
				{
					mSB.Append(c);
				}
				if (wrapLineColors)
				{
					for (int k = 0; k < mColors.size; k++)
					{
						mSB.Insert(mSB.Length - 1, "[-]");
					}
					for (int l = 0; l < mColors.size; l++)
					{
						mSB.Append("[");
						mSB.Append(EncodeColor(mColors.buffer[l]));
						mSB.Append("]");
					}
				}
				flag = true;
				num5++;
				i = j + 1;
				prev = 0;
				continue;
			}
			bool flag5 = flag || num5 == num2;
			if (encoding && ParseSymbol(text, ref j, mColors, premultiply, ref sub, ref fontScaleMult, ref bold, ref italic, ref underline, ref strike, ref ignoreColor, ref forceSpriteColor))
			{
				if (num9 + 1 > j)
				{
					mSB.Append(text, i, j - i);
					i = j;
					num9 = j;
				}
				if (wrapLineColors)
				{
					if (ignoreColor)
					{
						color = mColors.buffer[mColors.size - 1];
						color.a *= mAlpha * tint.a;
					}
					else
					{
						color = tint * mColors.buffer[mColors.size - 1];
						color.a *= mAlpha;
					}
					int m = 0;
					for (int num10 = mColors.size - 2; m < num10; m++)
					{
						color.a *= mColors.buffer[m].a;
					}
				}
				if (i < j)
				{
					mSB.Append(text, i, j - i);
				}
				else
				{
					mSB.Append(c);
				}
				i = j--;
				num9 = i;
				continue;
			}
			BMSymbol bMSymbol = (useSymbols ? GetSymbol(ref text, j, length) : null);
			float num11 = ((sub != 0) ? (fontScale * fontScaleMult) : ((fontScaleMult == 0f) ? fontScale : (fontScale * fontScaleMult)));
			float num12;
			if (bMSymbol == null)
			{
				float glyphWidth = GetGlyphWidth(c, prev, num11, bold, italic);
				if (glyphWidth == 0f && !flag4)
				{
					continue;
				}
				num12 = finalSpacingX + glyphWidth;
			}
			else
			{
				int paddedHeight = bMSymbol.paddedHeight;
				if (!bMSymbol.pixelPerfect && num8 != 0 && paddedHeight > num8)
				{
					num11 *= (float)num8 / (float)paddedHeight;
				}
				num12 = finalSpacingX + (bMSymbol.pixelPerfect ? ((float)bMSymbol.advance) : Mathf.Round((float)bMSymbol.advance * num11 * num7));
			}
			if (sub != 0)
			{
				num12 = Mathf.Round(num12);
			}
			num4 += num12;
			prev = c;
			float num13 = ((useEllipsis && flag5) ? (num3 - num6) : num3);
			if (flag4 && !flag3 && i < j)
			{
				int num14 = j - i;
				if (num5 == num2 && num4 >= num13 && j < length)
				{
					char c2 = text[j];
					if (c2 < ' ' || IsSpace(c2))
					{
						num14--;
					}
				}
				if (flag5 && useEllipsis && i < num9 && num4 < num3 && num4 > num13)
				{
					if (num9 > i)
					{
						mSB.Append(text, i, num9 - i + 1);
					}
					if (sub != 0)
					{
						mSB.Append("[/sub]");
					}
					else if (fontScaleMult != 0f)
					{
						mSB.Append("[/y]");
					}
					mSB.Append("...");
					i = j;
					break;
				}
				mSB.Append(text, i, num14 + 1);
				flag = false;
				i = j + 1;
			}
			if (useEllipsis && !flag4 && num4 <= num13)
			{
				num9 = j;
			}
			if (num4 > num13)
			{
				if (!flag5)
				{
					for (; i < length && IsSpace(text[i]); i++)
					{
					}
					flag = true;
					num4 = 0f;
					j = i - 1;
					prev = 0;
					if (num5++ == num2)
					{
						break;
					}
					if (keepCharCount)
					{
						ReplaceSpaceWithNewline(ref mSB);
					}
					else
					{
						EndLine(ref mSB);
					}
					if (wrapLineColors)
					{
						for (int n = 0; n < mColors.size; n++)
						{
							mSB.Insert(mSB.Length - 1, "[-]");
						}
						for (int num15 = 0; num15 < mColors.size; num15++)
						{
							mSB.Append("[");
							mSB.Append(EncodeColor(mColors.buffer[num15]));
							mSB.Append("]");
						}
					}
					continue;
				}
				if (useEllipsis && j > 0)
				{
					if (num9 > i)
					{
						mSB.Append(text, i, num9 - i + 1);
					}
					if (sub != 0)
					{
						mSB.Append("[/sub]");
					}
					else if (fontScaleMult != 0f)
					{
						mSB.Append("[/y]");
					}
					if (symbolStyle == SymbolStyle.None)
					{
						mSB.Append("...");
					}
					else
					{
						mSB.Append("[-][ff]...");
					}
					i = j;
					break;
				}
				mSB.Append(text, i, Mathf.Max(0, j - i));
				if (!flag4 && !flag3)
				{
					flag2 = false;
				}
				if (wrapLineColors && mColors.size > 0)
				{
					mSB.Append("[-]");
				}
				if (num5++ == num2)
				{
					i = j;
					break;
				}
				if (keepCharCount)
				{
					ReplaceSpaceWithNewline(ref mSB);
				}
				else
				{
					EndLine(ref mSB);
				}
				if (wrapLineColors)
				{
					for (int num16 = 0; num16 < mColors.size; num16++)
					{
						mSB.Insert(mSB.Length - 1, "[-]");
					}
					for (int num17 = 0; num17 < mColors.size; num17++)
					{
						mSB.Append("[");
						mSB.Append(EncodeColor(mColors.buffer[num17]));
						mSB.Append("]");
					}
				}
				flag = true;
				if (flag4)
				{
					i = j + 1;
					num4 = 0f;
				}
				else
				{
					i = j;
					num4 = num12;
				}
				num9 = j;
				prev = 0;
			}
			if (bMSymbol != null)
			{
				j += bMSymbol.length - 1;
				prev = 0;
			}
		}
		if (i < j)
		{
			mSB.Append(text, i, j - i);
		}
		if (wrapLineColors && mColors.size > 0)
		{
			mSB.Append("[-]");
		}
		finalText = mSB.ToString();
		mColors.Clear();
		if (flag2)
		{
			if (j != length)
			{
				if (maxLines == 0)
				{
					return num5 == 0;
				}
				return num5 == num2;
			}
			return true;
		}
		return false;
	}

	public static void Print(string text, List<Vector3> verts, List<Vector2> uvs, List<Color> cols, List<Vector3> sverts = null, List<Vector2> suvs = null, List<Color> scols = null)
	{
		if (string.IsNullOrEmpty(text))
		{
			return;
		}
		int count = verts.Count;
		int indexOffset = sverts?.Count ?? 0;
		Prepare(text);
		mColors.Clear();
		mColors.Add(Color.white);
		mAlpha = 1f;
		int num = 0;
		int prev = 0;
		float num2 = 0f;
		float num3 = 0f;
		float num4 = 0f;
		Color a = tint * gradientBottom;
		Color b = tint * gradientTop;
		Color color = tint;
		int length = text.Length;
		Rect rect = default(Rect);
		float num5 = 0f;
		float num6 = 0f;
		float num7 = (float)finalSize * pixelDensity;
		float num8 = 0f;
		float num9 = (float)regionWidth + 0.01f;
		int sub = 0;
		bool bold = false;
		bool italic = false;
		bool underline = false;
		bool strike = false;
		bool ignoreColor = false;
		bool forceSpriteColor = false;
		Color item = new Color(0f, 0f, 0f, 0f);
		float num10 = symbolScale;
		float num11 = symbolOffset;
		int num12 = symbolMaxHeight;
		float fontScaleMult = 0f;
		if (dynamicFont == null && nguiFont != null)
		{
			rect = nguiFont.uvRect;
			num5 = rect.width / (float)nguiFont.texWidth;
			num6 = rect.height / (float)nguiFont.texHeight;
		}
		for (int i = 0; i < length; i++)
		{
			num = text[i];
			num8 = num2;
			if (encoding && ParseSymbol(text, ref i, mColors, premultiply, ref sub, ref fontScaleMult, ref bold, ref italic, ref underline, ref strike, ref ignoreColor, ref forceSpriteColor))
			{
				if (ignoreColor)
				{
					color = mColors.buffer[mColors.size - 1];
					color.a *= mAlpha * tint.a;
				}
				else
				{
					color = tint * mColors.buffer[mColors.size - 1];
					color.a *= mAlpha;
				}
				int j = 0;
				for (int num13 = mColors.size - 2; j < num13; j++)
				{
					color.a *= mColors.buffer[j].a;
				}
				if (gradient)
				{
					a = gradientBottom * color;
					b = gradientTop * color;
				}
				i--;
				continue;
			}
			if (num == 10)
			{
				if (num2 > num4)
				{
					num4 = num2;
				}
				if (alignment != Alignment.Left)
				{
					Align(verts, count, num2 - finalSpacingX);
					count = verts.Count;
					if (sverts != null)
					{
						Align(sverts, indexOffset, num2 - finalSpacingX);
						indexOffset = sverts.Count;
					}
				}
				num2 = 0f;
				num3 += finalLineHeight;
				prev = 0;
				continue;
			}
			if (num < 32)
			{
				prev = num;
				continue;
			}
			BMSymbol bMSymbol = (useSymbols ? GetSymbol(ref text, i, length) : null);
			float num14 = ((sub != 0) ? (fontScale * fontScaleMult) : ((fontScaleMult == 0f) ? fontScale : (fontScale * fontScaleMult)));
			float num17;
			float num18;
			float num20;
			float num19;
			if (bMSymbol != null)
			{
				int paddedHeight = bMSymbol.paddedHeight;
				float num15 = ((!bMSymbol.pixelPerfect && num12 != 0 && paddedHeight > num12) ? ((float)num12 / (float)paddedHeight) : 1f);
				float num16 = (bMSymbol.pixelPerfect ? 1f : (fontScale * num10 * num15));
				num17 = num2 + (float)bMSymbol.offsetX * num16;
				num18 = num17 + (float)bMSymbol.width * num16;
				num19 = 0f - (num3 + (float)bMSymbol.offsetY * num16) + num11 * num15;
				num20 = num19 - (float)bMSymbol.height * num16;
				float num21 = (bMSymbol.pixelPerfect ? ((float)bMSymbol.advance) : Mathf.Round((float)bMSymbol.advance * num14 * num10 * num15));
				if (symbolCentered)
				{
					int num22 = Mathf.RoundToInt((float)bMSymbol.height * num16);
					int num23 = Mathf.RoundToInt(fontScale * (float)fontSize);
					int num24 = (num22 - num23) / 2;
					num20 += (float)num24;
					num19 += (float)num24;
				}
				if (num2 + num21 > num9)
				{
					if (num2 == 0f)
					{
						return;
					}
					if (alignment != Alignment.Left && count < verts.Count)
					{
						Align(verts, count, num2 - finalSpacingX);
						count = verts.Count;
						if (sverts != null)
						{
							Align(sverts, indexOffset, num2 - finalSpacingX);
							indexOffset = sverts.Count;
						}
					}
					num17 -= num2;
					num18 -= num2;
					num20 -= finalLineHeight;
					num19 -= finalLineHeight;
					num2 = 0f;
					num3 += finalLineHeight;
					num8 = 0f;
				}
				verts.Add(new Vector3(num17, num20));
				verts.Add(new Vector3(num17, num19));
				verts.Add(new Vector3(num18, num19));
				verts.Add(new Vector3(num18, num20));
				if (sverts != null)
				{
					sverts.Add(new Vector3(num17, num20));
					sverts.Add(new Vector3(num17, num19));
					sverts.Add(new Vector3(num18, num19));
					sverts.Add(new Vector3(num18, num20));
				}
				num2 += num21 + finalSpacingX;
				i += bMSymbol.length - 1;
				prev = 0;
				if (uvs != null)
				{
					Rect uvRect = bMSymbol.uvRect;
					float xMin = uvRect.xMin;
					float yMin = uvRect.yMin;
					float xMax = uvRect.xMax;
					float yMax = uvRect.yMax;
					if (suvs != null)
					{
						uvs.Add(new Vector2(1f, 1f));
						uvs.Add(new Vector2(1f, 1f));
						uvs.Add(new Vector2(1f, 1f));
						uvs.Add(new Vector2(1f, 1f));
						suvs.Add(new Vector2(xMin, yMin));
						suvs.Add(new Vector2(xMin, yMax));
						suvs.Add(new Vector2(xMax, yMax));
						suvs.Add(new Vector2(xMax, yMin));
					}
					else
					{
						uvs.Add(new Vector2(xMin, yMin));
						uvs.Add(new Vector2(xMin, yMax));
						uvs.Add(new Vector2(xMax, yMax));
						uvs.Add(new Vector2(xMax, yMin));
					}
				}
				if (cols == null)
				{
					continue;
				}
				if (symbolStyle == SymbolStyle.Colored || (symbolStyle == SymbolStyle.Normal && (forceSpriteColor || bMSymbol.colored)))
				{
					if (scols != null)
					{
						for (int k = 0; k < 4; k++)
						{
							cols.Add(item);
							scols.Add(color);
						}
					}
					else
					{
						for (int l = 0; l < 4; l++)
						{
							cols.Add(color);
						}
					}
					continue;
				}
				Color white = Color.white;
				if (symbolStyle == SymbolStyle.NoOutline)
				{
					white.r = -1f;
					white.a = 0f;
				}
				else
				{
					white.a = color.a;
				}
				if (scols != null)
				{
					for (int m = 0; m < 4; m++)
					{
						cols.Add(item);
						scols.Add(white);
					}
				}
				else
				{
					for (int n = 0; n < 4; n++)
					{
						cols.Add(white);
					}
				}
				continue;
			}
			GlyphInfo glyphInfo = GetGlyph(num, prev, bold, italic, num14);
			if (glyphInfo == null)
			{
				continue;
			}
			prev = num;
			float advance = glyphInfo.advance;
			switch (sub)
			{
			case 1:
			{
				float num26 = fontScale * (float)fontSize * 0.4f;
				glyphInfo.v0.y -= num26;
				glyphInfo.v1.y -= num26;
				break;
			}
			default:
			{
				float num27 = fontScale * (float)fontSize * 0.05f;
				glyphInfo.v0.y += num27;
				glyphInfo.v1.y += num27;
				break;
			}
			case 0:
				if (fontScaleMult != 0f)
				{
					float num25 = fontScale * (1f - fontScaleMult) * (float)fontSize * 0.5f;
					glyphInfo.v0.y -= num25;
					glyphInfo.v1.y -= num25;
				}
				break;
			}
			advance += finalSpacingX;
			num17 = glyphInfo.v0.x + num2;
			num20 = glyphInfo.v0.y - num3;
			num18 = glyphInfo.v1.x + num2;
			num19 = glyphInfo.v1.y - num3;
			if (num2 + advance > num9)
			{
				if (num2 == 0f)
				{
					return;
				}
				if (alignment != Alignment.Left && count < verts.Count)
				{
					Align(verts, count, num2 - finalSpacingX);
					count = verts.Count;
					if (sverts != null)
					{
						Align(sverts, indexOffset, num2 - finalSpacingX);
						indexOffset = sverts.Count;
					}
				}
				num17 -= num2;
				num18 -= num2;
				num20 -= finalLineHeight;
				num19 -= finalLineHeight;
				num2 = 0f;
				num3 += finalLineHeight;
				num8 = 0f;
			}
			if (IsSpace(num))
			{
				if (underline)
				{
					num = 95;
				}
				else if (strike)
				{
					num = 45;
				}
			}
			num2 += advance;
			if (sub != 0)
			{
				num2 = Mathf.Round(num2);
			}
			if (IsSpace(num))
			{
				continue;
			}
			bool flag = bold && dynamicFont == null;
			if (uvs != null)
			{
				if (dynamicFont == null && nguiFont != null)
				{
					glyphInfo.u0.x = rect.xMin + num5 * glyphInfo.u0.x;
					glyphInfo.u2.x = rect.xMin + num5 * glyphInfo.u2.x;
					glyphInfo.u0.y = rect.yMax - num6 * glyphInfo.u0.y;
					glyphInfo.u2.y = rect.yMax - num6 * glyphInfo.u2.y;
					glyphInfo.u1.x = glyphInfo.u0.x;
					glyphInfo.u1.y = glyphInfo.u2.y;
					glyphInfo.u3.x = glyphInfo.u2.x;
					glyphInfo.u3.y = glyphInfo.u0.y;
				}
				int num28 = 0;
				for (int num29 = ((!flag) ? 1 : 4); num28 < num29; num28++)
				{
					uvs.Add(glyphInfo.u0);
					uvs.Add(glyphInfo.u1);
					uvs.Add(glyphInfo.u2);
					uvs.Add(glyphInfo.u3);
				}
			}
			if (cols != null)
			{
				if (glyphInfo.channel == 0 || glyphInfo.channel == 15)
				{
					if (gradient)
					{
						float num30 = num7 + glyphInfo.v0.y / fontScale;
						float num31 = num7 + glyphInfo.v1.y / fontScale;
						num30 /= num7;
						num31 /= num7;
						s_c0 = Color.Lerp(a, b, num30);
						s_c1 = Color.Lerp(a, b, num31);
						int num32 = 0;
						for (int num33 = ((!flag) ? 1 : 4); num32 < num33; num32++)
						{
							cols.Add(s_c0);
							cols.Add(s_c1);
							cols.Add(s_c1);
							cols.Add(s_c0);
						}
					}
					else
					{
						int num34 = 0;
						for (int num35 = (flag ? 16 : 4); num34 < num35; num34++)
						{
							cols.Add(color);
						}
					}
				}
				else
				{
					Color item2 = color;
					item2 *= 0.49f;
					switch (glyphInfo.channel)
					{
					case 1:
						item2.b += 0.51f;
						break;
					case 2:
						item2.g += 0.51f;
						break;
					case 4:
						item2.r += 0.51f;
						break;
					case 8:
						item2.a += 0.51f;
						break;
					}
					int num36 = 0;
					for (int num37 = (flag ? 16 : 4); num36 < num37; num36++)
					{
						cols.Add(item2);
					}
				}
			}
			if (dynamicFont != null)
			{
				verts.Add(new Vector3(num17, num20));
				verts.Add(new Vector3(num17, num19));
				verts.Add(new Vector3(num18, num19));
				verts.Add(new Vector3(num18, num20));
			}
			else if (!bold)
			{
				if (!italic)
				{
					verts.Add(new Vector3(num17, num20));
					verts.Add(new Vector3(num17, num19));
					verts.Add(new Vector3(num18, num19));
					verts.Add(new Vector3(num18, num20));
				}
				else
				{
					float num38 = (float)fontSize * 0.1f * ((num19 - num20) / (float)fontSize);
					verts.Add(new Vector3(num17 - num38, num20));
					verts.Add(new Vector3(num17 + num38, num19));
					verts.Add(new Vector3(num18 + num38, num19));
					verts.Add(new Vector3(num18 - num38, num20));
				}
			}
			else
			{
				for (int num39 = 0; num39 < 4; num39++)
				{
					float num40 = mBoldOffset[num39 * 2];
					float num41 = mBoldOffset[num39 * 2 + 1];
					float num42 = (italic ? ((float)fontSize * 0.1f * ((num19 - num20) / (float)fontSize)) : 0f);
					verts.Add(new Vector3(num17 + num40 - num42, num20 + num41));
					verts.Add(new Vector3(num17 + num40 + num42, num19 + num41));
					verts.Add(new Vector3(num18 + num40 + num42, num19 + num41));
					verts.Add(new Vector3(num18 + num40 - num42, num20 + num41));
				}
			}
			if (!(underline || strike))
			{
				continue;
			}
			GlyphInfo glyphInfo2 = GetGlyph(strike ? 45 : 95, 0, bold: false, italic: false, num14);
			if (glyphInfo2 == null)
			{
				continue;
			}
			if (uvs != null)
			{
				if (dynamicFont == null && nguiFont != null)
				{
					glyphInfo2.u0.x = rect.xMin + num5 * glyphInfo2.u0.x;
					glyphInfo2.u2.x = rect.xMin + num5 * glyphInfo2.u2.x;
					glyphInfo2.u0.y = rect.yMax - num6 * glyphInfo2.u0.y;
					glyphInfo2.u2.y = rect.yMax - num6 * glyphInfo2.u2.y;
				}
				float x = (glyphInfo2.u0.x + glyphInfo2.u2.x) * 0.5f;
				int num43 = 0;
				for (int num44 = ((!flag) ? 1 : 4); num43 < num44; num43++)
				{
					uvs.Add(new Vector2(x, glyphInfo2.u0.y));
					uvs.Add(new Vector2(x, glyphInfo2.u2.y));
					uvs.Add(new Vector2(x, glyphInfo2.u2.y));
					uvs.Add(new Vector2(x, glyphInfo2.u0.y));
				}
			}
			float num45 = Mathf.Round(glyphInfo2.v0.y - glyphInfo2.v1.y);
			num45 = Mathf.Max(num45 - 2f, 2f);
			num20 = 0f - num3 + glyphInfo2.v0.y - 1f;
			num19 = num20 - num45;
			if (flag)
			{
				for (int num46 = 0; num46 < 4; num46++)
				{
					float num47 = mBoldOffset[num46 * 2];
					float num48 = mBoldOffset[num46 * 2 + 1];
					verts.Add(new Vector3(num8 + num47, num20 + num48));
					verts.Add(new Vector3(num8 + num47, num19 + num48));
					verts.Add(new Vector3(num2 + num47, num19 + num48));
					verts.Add(new Vector3(num2 + num47, num20 + num48));
				}
			}
			else
			{
				verts.Add(new Vector3(num8, num20));
				verts.Add(new Vector3(num8, num19));
				verts.Add(new Vector3(num2, num19));
				verts.Add(new Vector3(num2, num20));
			}
			if (gradient)
			{
				float num49 = num7 + glyphInfo2.v0.y / num14;
				float num50 = num7 + glyphInfo2.v1.y / num14;
				num49 /= num7;
				num50 /= num7;
				s_c0 = Color.Lerp(a, b, num49);
				s_c1 = Color.Lerp(a, b, num50);
				int num51 = 0;
				for (int num52 = ((!flag) ? 1 : 4); num51 < num52; num51++)
				{
					cols.Add(s_c0);
					cols.Add(s_c1);
					cols.Add(s_c1);
					cols.Add(s_c0);
				}
			}
			else
			{
				int num53 = 0;
				for (int num54 = (flag ? 16 : 4); num53 < num54; num53++)
				{
					cols.Add(color);
				}
			}
		}
		if (alignment != Alignment.Left && count < verts.Count)
		{
			Align(verts, count, num2 - finalSpacingX);
			count = verts.Count;
			if (sverts != null)
			{
				Align(sverts, indexOffset, num2 - finalSpacingX);
				indexOffset = sverts.Count;
			}
		}
		mColors.Clear();
	}

	public static void PrintApproximateCharacterPositions(string text, List<Vector3> verts, List<int> indices)
	{
		if (string.IsNullOrEmpty(text))
		{
			text = " ";
		}
		Prepare(text);
		mColors.Clear();
		float num = 0f;
		float num2 = 0f;
		float num3 = (float)regionWidth + 0.01f;
		int length = text.Length;
		int count = verts.Count;
		int num4 = 0;
		int prev = 0;
		int sub = 0;
		bool bold = false;
		bool italic = false;
		bool underline = false;
		bool strike = false;
		bool ignoreColor = false;
		bool forceSpriteColor = false;
		float num5 = symbolScale;
		int num6 = symbolMaxHeight;
		float fontScaleMult = 0f;
		for (int i = 0; i < length; i++)
		{
			num4 = text[i];
			if (encoding && ParseSymbol(text, ref i, mColors, premultiply, ref sub, ref fontScaleMult, ref bold, ref italic, ref underline, ref strike, ref ignoreColor, ref forceSpriteColor))
			{
				i--;
				continue;
			}
			float num7 = ((sub != 0) ? (fontScale * fontScaleMult) : ((fontScaleMult == 0f) ? fontScale : (fontScale * fontScaleMult)));
			float num8 = num7 * 0.5f;
			verts.Add(new Vector3(num, 0f - num2 - num8));
			indices.Add(i);
			if (num4 == 10)
			{
				if (alignment != Alignment.Left)
				{
					Align(verts, count, num - finalSpacingX, 1);
					count = verts.Count;
				}
				num = 0f;
				num2 += finalLineHeight;
				prev = 0;
				continue;
			}
			if (num4 < 32)
			{
				prev = 0;
				continue;
			}
			BMSymbol bMSymbol = (useSymbols ? GetSymbol(ref text, i, length) : null);
			if (bMSymbol == null)
			{
				float glyphWidth = GetGlyphWidth(num4, prev, num7, bold, italic);
				if (glyphWidth == 0f)
				{
					continue;
				}
				glyphWidth += finalSpacingX;
				if (num + glyphWidth > num3)
				{
					if (num == 0f)
					{
						return;
					}
					if (alignment != Alignment.Left && count < verts.Count)
					{
						Align(verts, count, num - finalSpacingX, 1);
						count = verts.Count;
					}
					num = glyphWidth;
					num2 += finalLineHeight;
				}
				else
				{
					num += glyphWidth;
				}
				verts.Add(new Vector3(num, 0f - num2 - num8));
				indices.Add(i + 1);
				prev = num4;
				continue;
			}
			int paddedHeight = bMSymbol.paddedHeight;
			if (!bMSymbol.pixelPerfect && num6 != 0 && paddedHeight > num6)
			{
				num7 *= (float)num6 / (float)paddedHeight;
			}
			float num9 = (bMSymbol.pixelPerfect ? ((float)bMSymbol.advance + finalSpacingX) : Mathf.Round((float)bMSymbol.advance * num7 * num5 + finalSpacingX));
			if (num + num9 > num3)
			{
				if (num == 0f)
				{
					return;
				}
				if (alignment != Alignment.Left && count < verts.Count)
				{
					Align(verts, count, num - finalSpacingX, 1);
					count = verts.Count;
				}
				num = num9;
				num2 += finalLineHeight;
			}
			else
			{
				num += num9;
			}
			verts.Add(new Vector3(num, 0f - num2 - num8));
			indices.Add(i + 1);
			i += bMSymbol.sequence.Length - 1;
			prev = 0;
		}
		if (alignment != Alignment.Left && count < verts.Count)
		{
			Align(verts, count, num - finalSpacingX, 1);
		}
	}

	public static void PrintExactCharacterPositions(string text, List<Vector3> verts, List<int> indices)
	{
		if (string.IsNullOrEmpty(text))
		{
			text = " ";
		}
		Prepare(text);
		mColors.Clear();
		float num = 0f;
		float num2 = 0f;
		float num3 = (float)regionWidth + 0.01f;
		float num4 = (float)fontSize * fontScale;
		int length = text.Length;
		int count = verts.Count;
		int num5 = 0;
		int prev = 0;
		int sub = 0;
		bool bold = false;
		bool italic = false;
		bool underline = false;
		bool strike = false;
		bool ignoreColor = false;
		bool forceSpriteColor = false;
		float num6 = symbolScale;
		int num7 = symbolMaxHeight;
		float fontScaleMult = 0f;
		for (int i = 0; i < length; i++)
		{
			num5 = text[i];
			if (encoding && ParseSymbol(text, ref i, mColors, premultiply, ref sub, ref fontScaleMult, ref bold, ref italic, ref underline, ref strike, ref ignoreColor, ref forceSpriteColor))
			{
				i--;
				continue;
			}
			float num8 = ((sub != 0) ? (fontScale * fontScaleMult) : ((fontScaleMult == 0f) ? fontScale : (fontScale * fontScaleMult)));
			if (num5 == 10)
			{
				if (alignment != Alignment.Left)
				{
					Align(verts, count, num - finalSpacingX, 2);
					count = verts.Count;
				}
				num = 0f;
				num2 += finalLineHeight;
				prev = 0;
				continue;
			}
			if (num5 < 32)
			{
				prev = 0;
				continue;
			}
			BMSymbol bMSymbol = (useSymbols ? GetSymbol(ref text, i, length) : null);
			if (bMSymbol == null)
			{
				float glyphWidth = GetGlyphWidth(num5, prev, num8, bold, italic);
				if (glyphWidth == 0f)
				{
					continue;
				}
				float num9 = glyphWidth + finalSpacingX;
				if (num + num9 > num3)
				{
					if (num == 0f)
					{
						return;
					}
					if (alignment != Alignment.Left && count < verts.Count)
					{
						Align(verts, count, num - finalSpacingX, 2);
						count = verts.Count;
					}
					num = 0f;
					num2 += finalLineHeight;
					prev = 0;
					i--;
				}
				else
				{
					indices.Add(i);
					verts.Add(new Vector3(num, 0f - num2 - num4));
					verts.Add(new Vector3(num + num9, 0f - num2));
					prev = num5;
					num += num9;
				}
				continue;
			}
			int paddedHeight = bMSymbol.paddedHeight;
			if (!bMSymbol.pixelPerfect && num7 != 0 && paddedHeight > num7)
			{
				num8 *= (float)num7 / (float)paddedHeight;
			}
			float num10 = (bMSymbol.pixelPerfect ? ((float)bMSymbol.advance + finalSpacingX) : Mathf.Round((float)bMSymbol.advance * num8 * num6 + finalSpacingX));
			if (num + num10 > num3)
			{
				if (num == 0f)
				{
					return;
				}
				if (alignment != Alignment.Left && count < verts.Count)
				{
					Align(verts, count, num - finalSpacingX, 2);
					count = verts.Count;
				}
				num = 0f;
				num2 += finalLineHeight;
				prev = 0;
				i--;
			}
			else
			{
				indices.Add(i);
				verts.Add(new Vector3(num, 0f - num2 - num4));
				verts.Add(new Vector3(num + num10, 0f - num2));
				i += bMSymbol.sequence.Length - 1;
				num += num10;
				prev = 0;
			}
		}
		if (alignment != Alignment.Left && count < verts.Count)
		{
			Align(verts, count, num - finalSpacingX, 2);
		}
	}

	public static void PrintCaretAndSelection(string text, int start, int end, List<Vector3> caret, List<Vector3> highlight)
	{
		if (string.IsNullOrEmpty(text))
		{
			text = " ";
		}
		Prepare(text);
		mColors.Clear();
		int num = end;
		if (start > end)
		{
			end = start;
			start = num;
		}
		float num2 = 0f;
		float num3 = 0f;
		float num4 = (float)fontSize * fontScale;
		int indexOffset = caret?.Count ?? 0;
		int num5 = highlight?.Count ?? 0;
		int length = text.Length;
		int i = 0;
		int num6 = 0;
		int prev = 0;
		bool flag = false;
		bool flag2 = false;
		int sub = 0;
		bool bold = false;
		bool italic = false;
		bool underline = false;
		bool strike = false;
		bool ignoreColor = false;
		bool forceSpriteColor = false;
		float num7 = symbolScale;
		int num8 = symbolMaxHeight;
		float fontScaleMult = 0f;
		Vector2 vector = Vector2.zero;
		Vector2 vector2 = Vector2.zero;
		for (; i < length; i++)
		{
			if (encoding && ParseSymbol(text, ref i, mColors, premultiply, ref sub, ref fontScaleMult, ref bold, ref italic, ref underline, ref strike, ref ignoreColor, ref forceSpriteColor))
			{
				i--;
				continue;
			}
			float num9 = ((sub != 0) ? (fontScale * fontScaleMult) : ((fontScaleMult == 0f) ? fontScale : (fontScale * fontScaleMult)));
			if (caret != null && !flag2 && num <= i)
			{
				flag2 = true;
				caret.Add(new Vector3(num2 - 1f, 0f - num3 - num4));
				caret.Add(new Vector3(num2 - 1f, 0f - num3));
				caret.Add(new Vector3(num2 + 1f, 0f - num3));
				caret.Add(new Vector3(num2 + 1f, 0f - num3 - num4));
			}
			num6 = text[i];
			if (num6 == 10)
			{
				if (caret != null && flag2)
				{
					if (alignment != Alignment.Left)
					{
						Align(caret, indexOffset, num2 - finalSpacingX);
					}
					caret = null;
				}
				if (highlight != null)
				{
					if (flag)
					{
						flag = false;
						highlight.Add(vector2);
						highlight.Add(vector);
					}
					else if (start <= i && end > i)
					{
						highlight.Add(new Vector3(num2, 0f - num3 - num4));
						highlight.Add(new Vector3(num2, 0f - num3));
						highlight.Add(new Vector3(num2 + 2f, 0f - num3));
						highlight.Add(new Vector3(num2 + 2f, 0f - num3 - num4));
					}
					if (alignment != Alignment.Left && num5 < highlight.Count)
					{
						Align(highlight, num5, num2 - finalSpacingX);
						num5 = highlight.Count;
					}
				}
				num2 = 0f;
				num3 += finalLineHeight;
				prev = 0;
				continue;
			}
			if (num6 < 32)
			{
				prev = 0;
				continue;
			}
			BMSymbol bMSymbol = (useSymbols ? GetSymbol(ref text, i, length) : null);
			float num10;
			if (bMSymbol != null)
			{
				int paddedHeight = bMSymbol.paddedHeight;
				if (!bMSymbol.pixelPerfect && num8 != 0 && paddedHeight > num8)
				{
					num9 *= (float)num8 / (float)paddedHeight;
				}
				num10 = (bMSymbol.pixelPerfect ? ((float)bMSymbol.advance) : Mathf.Round((float)bMSymbol.advance * num9 * num7));
			}
			else
			{
				num10 = GetGlyphWidth(num6, prev, num9, bold, italic);
			}
			if (num10 == 0f)
			{
				continue;
			}
			float num11 = num2;
			float num12 = num2 + num10;
			float num13 = 0f - num3 - num4;
			float num14 = 0f - num3;
			if (num12 + finalSpacingX > (float)regionWidth)
			{
				if (num2 == 0f)
				{
					return;
				}
				if (caret != null && flag2)
				{
					if (alignment != Alignment.Left)
					{
						Align(caret, indexOffset, num2 - finalSpacingX);
					}
					caret = null;
				}
				if (highlight != null)
				{
					if (flag)
					{
						flag = false;
						highlight.Add(vector2);
						highlight.Add(vector);
					}
					else if (start <= i && end > i)
					{
						highlight.Add(new Vector3(num2, 0f - num3 - num4));
						highlight.Add(new Vector3(num2, 0f - num3));
						highlight.Add(new Vector3(num2 + 2f, 0f - num3));
						highlight.Add(new Vector3(num2 + 2f, 0f - num3 - num4));
					}
					if (alignment != Alignment.Left && num5 < highlight.Count)
					{
						Align(highlight, num5, num2 - finalSpacingX);
						num5 = highlight.Count;
					}
				}
				num11 -= num2;
				num12 -= num2;
				num13 -= finalLineHeight;
				num14 -= finalLineHeight;
				num2 = 0f;
				num3 += finalLineHeight;
			}
			num2 += num10 + finalSpacingX;
			if (highlight != null)
			{
				if (start > i || end <= i)
				{
					if (flag)
					{
						flag = false;
						highlight.Add(vector2);
						highlight.Add(vector);
					}
				}
				else if (!flag)
				{
					flag = true;
					highlight.Add(new Vector3(num11, num13));
					highlight.Add(new Vector3(num11, num14));
				}
			}
			vector = new Vector2(num12, num13);
			vector2 = new Vector2(num12, num14);
			prev = num6;
		}
		if (caret != null)
		{
			if (!flag2)
			{
				caret.Add(new Vector3(num2 - 1f, 0f - num3 - num4));
				caret.Add(new Vector3(num2 - 1f, 0f - num3));
				caret.Add(new Vector3(num2 + 1f, 0f - num3));
				caret.Add(new Vector3(num2 + 1f, 0f - num3 - num4));
			}
			if (alignment != Alignment.Left)
			{
				Align(caret, indexOffset, num2 - finalSpacingX);
			}
		}
		if (highlight != null)
		{
			if (flag)
			{
				highlight.Add(vector2);
				highlight.Add(vector);
			}
			else if (start < i && end == i)
			{
				highlight.Add(new Vector3(num2, 0f - num3 - num4));
				highlight.Add(new Vector3(num2, 0f - num3));
				highlight.Add(new Vector3(num2 + 2f, 0f - num3));
				highlight.Add(new Vector3(num2 + 2f, 0f - num3 - num4));
			}
			if (alignment != Alignment.Left && num5 < highlight.Count)
			{
				Align(highlight, num5, num2 - finalSpacingX);
			}
		}
		mColors.Clear();
	}

	public static bool ReplaceLink(ref string text, ref int index, string type, string prefix = null, string suffix = null)
	{
		if (index == -1)
		{
			return false;
		}
		index = text.IndexOf(type, index);
		if (index == -1)
		{
			return false;
		}
		if (index > 5)
		{
			for (int num = index - 5; num >= 0; num--)
			{
				if (text[num] == '[')
				{
					if (text[num + 1] == 'u' && text[num + 2] == 'r' && text[num + 3] == 'l' && text[num + 4] == '=')
					{
						index += type.Length;
						return ReplaceLink(ref text, ref index, type, prefix, suffix);
					}
					if (text[num + 1] == '/' && text[num + 2] == 'u' && text[num + 3] == 'r' && text[num + 4] == 'l')
					{
						break;
					}
				}
			}
		}
		int num2 = index + type.Length;
		int num3 = text.IndexOfAny(new char[5] { ' ', '\n', '\u200a', '\u200b', '\u2009' }, num2);
		if (num3 == -1)
		{
			num3 = text.Length;
		}
		int num4 = text.IndexOfAny(new char[2] { '/', ' ' }, num2);
		if (num4 == -1 || num4 == num2)
		{
			index += type.Length;
			return true;
		}
		string text2 = text.Substring(0, index);
		string text3 = text.Substring(index, num3 - index);
		string text4 = text.Substring(num3);
		string text5 = text.Substring(num2, num4 - num2);
		if (!string.IsNullOrEmpty(prefix))
		{
			text2 += prefix;
		}
		text = text2 + "[url=" + text3 + "][u]" + text5 + "[/u][/url]";
		index = text.Length;
		if (string.IsNullOrEmpty(suffix))
		{
			text += text4;
		}
		else
		{
			text = text + suffix + text4;
		}
		return true;
	}

	public static bool InsertHyperlink(ref string text, ref int index, string keyword, string link, string prefix = null, string suffix = null)
	{
		int num = text.IndexOf(keyword, index, StringComparison.CurrentCultureIgnoreCase);
		if (num == -1)
		{
			return false;
		}
		if (num > 5)
		{
			for (int num2 = num - 5; num2 >= 0; num2--)
			{
				if (text[num2] == '[')
				{
					if (text[num2 + 1] == 'u' && text[num2 + 2] == 'r' && text[num2 + 3] == 'l' && text[num2 + 4] == '=')
					{
						index = num + keyword.Length;
						return InsertHyperlink(ref text, ref index, keyword, link, prefix, suffix);
					}
					if (text[num2 + 1] == '/' && text[num2 + 2] == 'u' && text[num2 + 3] == 'r' && text[num2 + 4] == 'l')
					{
						break;
					}
				}
			}
		}
		string text2 = text.Substring(0, num);
		string text3 = "[url=" + link + "][u]";
		string text4 = text.Substring(num, keyword.Length);
		if (!string.IsNullOrEmpty(prefix))
		{
			text4 = prefix + text4;
		}
		if (!string.IsNullOrEmpty(suffix))
		{
			text4 += suffix;
		}
		string text5 = text.Substring(num + keyword.Length);
		text = text2 + text3 + text4 + "[/u][/url]";
		index = text.Length;
		text += text5;
		return true;
	}

	public static void ReplaceLinks(ref string text, string prefix = null, string suffix = null)
	{
		int index = 0;
		while (index < text.Length && ReplaceLink(ref text, ref index, "http://", prefix, suffix))
		{
		}
		int index2 = 0;
		while (index2 < text.Length && ReplaceLink(ref text, ref index2, "https://", prefix, suffix))
		{
		}
	}
}
