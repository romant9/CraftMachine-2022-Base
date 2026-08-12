using System;
using System.Collections.Generic;
using UnityEngine;

[HideInInspector]
public class UILabelSymbols : UIWidget
{
	[NonSerialized]
	public UILabel label;

	[NonSerialized]
	public int fillFrame = -1;

	[NonSerialized]
	public List<Vector3> cacheVerts = new List<Vector3>();

	[NonSerialized]
	public List<Vector2> cacheUVs = new List<Vector2>();

	[NonSerialized]
	public List<Color> cacheCols = new List<Color>();

	[NonSerialized]
	public List<Vector3> symbolVerts = new List<Vector3>();

	[NonSerialized]
	public List<Vector2> symbolUVs = new List<Vector2>();

	[NonSerialized]
	public List<Color> symbolCols = new List<Color>();

	public override bool isSelectable => false;

	public override Material material
	{
		get
		{
			if (label != null && label.separateSymbols)
			{
				NGUIFont nGUIFont = label.font as NGUIFont;
				if (!(nGUIFont != null))
				{
					return null;
				}
				return nGUIFont.symbolMaterial;
			}
			return null;
		}
	}

	public void ClearCache()
	{
		cacheVerts.Clear();
		cacheUVs.Clear();
		cacheCols.Clear();
		symbolVerts.Clear();
		symbolUVs.Clear();
		symbolCols.Clear();
		fillFrame = -1;
	}

	protected override void OnDisable()
	{
		base.OnDisable();
		ClearCache();
	}

	public override void OnFill(List<Vector3> verts, List<Vector2> uvs, List<Color> cols)
	{
		int frameCount = Time.frameCount;
		if (frameCount != fillFrame)
		{
			ClearCache();
			if (label != null)
			{
				label.Fill(cacheVerts, cacheUVs, cacheCols, verts, uvs, cols);
			}
			fillFrame = frameCount;
		}
		else if (symbolVerts.Count != 0)
		{
			verts.AddRange(symbolVerts);
			uvs.AddRange(symbolUVs);
			cols.AddRange(symbolCols);
			ClearCache();
		}
	}
}
