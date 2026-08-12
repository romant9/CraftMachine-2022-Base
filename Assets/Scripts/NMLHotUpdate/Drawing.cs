using System.Reflection;
using UnityEngine;

public static class Drawing
{
	private static Texture2D aaLineTex;

	private static Texture2D lineTex;

	private static Material blitMaterial;

	private static Material blendMaterial;

	private static Rect lineRect;

	public static void DrawLine(Vector2 pointA, Vector2 pointB, Color color, float width, bool antiAlias)
	{
		float num = pointB.x - pointA.x;
		float num2 = pointB.y - pointA.y;
		float num3 = Mathf.Sqrt(num * num + num2 * num2);
		if (!(num3 < 0.001f))
		{
			Texture2D texture;
			Material mat;
			if (antiAlias)
			{
				width *= 3f;
				texture = aaLineTex;
				mat = blendMaterial;
			}
			else
			{
				texture = lineTex;
				mat = blitMaterial;
			}
			float num4 = width * num2 / num3;
			float num5 = width * num / num3;
			Matrix4x4 identity = Matrix4x4.identity;
			identity.m00 = num;
			identity.m01 = 0f - num4;
			identity.m03 = pointA.x + 0.5f * num4;
			identity.m10 = num2;
			identity.m11 = num5;
			identity.m13 = pointA.y - 0.5f * num5;
			GL.PushMatrix();
			GL.MultMatrix(identity);
			Graphics.DrawTexture(lineRect, texture, lineRect, 0, 0, 0, 0, color, mat);
			GL.PopMatrix();
		}
	}

	public static void DrawBezierLine(Vector2 start, Vector2 startTangent, Vector2 end, Vector2 endTangent, Color color, float width, bool antiAlias, int segments)
	{
		Vector2 pointA = CubeBezier(start, startTangent, end, endTangent, 0f);
		for (int i = 1; i < segments; i++)
		{
			Vector2 vector = CubeBezier(start, startTangent, end, endTangent, (float)i / (float)segments);
			DrawLine(pointA, vector, color, width, antiAlias);
			pointA = vector;
		}
	}

	private static Vector2 CubeBezier(Vector2 s, Vector2 st, Vector2 e, Vector2 et, float t)
	{
		float num = 1f - t;
		return num * num * num * s + 3f * num * num * t * st + 3f * num * t * t * et + t * t * t * e;
	}

	static Drawing()
	{
		aaLineTex = null;
		lineTex = null;
		blitMaterial = null;
		blendMaterial = null;
		lineRect = new Rect(0f, 0f, 1f, 1f);
		Initialize();
	}

	private static void Initialize()
	{
		if (lineTex == null)
		{
			lineTex = new Texture2D(1, 1, TextureFormat.ARGB32, mipChain: false);
			lineTex.SetPixel(0, 1, Color.white);
			lineTex.Apply();
		}
		if (aaLineTex == null)
		{
			aaLineTex = new Texture2D(1, 3, TextureFormat.ARGB32, mipChain: false);
			aaLineTex.SetPixel(0, 0, new Color(1f, 1f, 1f, 0f));
			aaLineTex.SetPixel(0, 1, Color.white);
			aaLineTex.SetPixel(0, 2, new Color(1f, 1f, 1f, 0f));
			aaLineTex.Apply();
		}
		blitMaterial = (Material)typeof(GUI).GetMethod("get_blitMaterial", BindingFlags.Static | BindingFlags.NonPublic).Invoke(null, null);
		blendMaterial = (Material)typeof(GUI).GetMethod("get_blendMaterial", BindingFlags.Static | BindingFlags.NonPublic).Invoke(null, null);
	}
}
