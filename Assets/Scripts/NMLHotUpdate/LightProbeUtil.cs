using System;
using UnityEngine;
using UnityEngine.Rendering;

public class LightProbeUtil
{
	private static SphericalHarmonicsL2 aSample;

	private static Vector4[] avCoeff = new Vector4[7];

	private static Vector3 vRGB = default(Vector3);

	private static float s_fSqrtPI = Mathf.Sqrt(MathF.PI);

	private static float fC0 = 1f / (2f * s_fSqrtPI);

	private static float fC1 = Mathf.Sqrt(3f) / (3f * s_fSqrtPI);

	private static float fC2 = Mathf.Sqrt(15f) / (8f * s_fSqrtPI);

	private static float fC3 = Mathf.Sqrt(5f) / (16f * s_fSqrtPI);

	private static float fC4 = 0.5f * fC2;

	public static Vector3 SampleLightProbes(Vector3 vPos, Renderer r, Vector3 vNormal3)
	{
		Vector4 b = default(Vector4);
		b.x = vNormal3.x;
		b.y = vNormal3.y;
		b.z = vNormal3.z;
		b.w = 1f;
		if (LightmapSettings.lightProbes != null)
		{
			LightProbes.GetInterpolatedProbe(vPos, r, out aSample);
			for (int i = 0; i < 3; i++)
			{
				avCoeff[i].x = (0f - fC1) * aSample[i, 3];
				avCoeff[i].y = (0f - fC1) * aSample[i, 1];
				avCoeff[i].z = fC1 * aSample[i, 2];
				avCoeff[i].w = fC0 * aSample[i, 0] - fC3 * aSample[i, 6];
			}
			for (int j = 0; j < 3; j++)
			{
				avCoeff[j + 3].x = fC2 * aSample[j, 4];
				avCoeff[j + 3].y = (0f - fC2) * aSample[j, 5];
				avCoeff[j + 3].z = 3f * fC3 * aSample[j, 6];
				avCoeff[j + 3].w = (0f - fC2) * aSample[j, 7];
			}
			avCoeff[6].x = fC4 * aSample[0, 8];
			avCoeff[6].y = fC4 * aSample[1, 8];
			avCoeff[6].z = fC4 * aSample[2, 8];
			avCoeff[6].w = 1f;
			vRGB.x = Vector4.Dot(avCoeff[0], b);
			vRGB.y = Vector4.Dot(avCoeff[1], b);
			vRGB.z = Vector4.Dot(avCoeff[2], b);
			Vector4 b2 = default(Vector4);
			b2.x = b.x * b.y;
			b2.y = b.y * b.z;
			b2.z = b.z * b.z;
			b2.w = b.z * b.x;
			vRGB.x += Vector4.Dot(avCoeff[3], b2);
			vRGB.y += Vector4.Dot(avCoeff[4], b2);
			vRGB.z += Vector4.Dot(avCoeff[5], b2);
			float num = b.x * b.x - b.y * b.y;
			vRGB.x += num * avCoeff[6].x;
			vRGB.y += num * avCoeff[6].y;
			vRGB.z += num * avCoeff[6].z;
			vRGB.x += RenderSettings.ambientLight.r;
			vRGB.y += RenderSettings.ambientLight.g;
			vRGB.z += RenderSettings.ambientLight.b;
			return vRGB;
		}
		return Vector3.one;
	}

	public static Vector3 SampleLightProbesUp(Vector3 vPos, Renderer r)
	{
		if (LightmapSettings.lightProbes != null)
		{
			LightProbes.GetInterpolatedProbe(vPos, r, out aSample);
			for (int i = 0; i < 3; i++)
			{
				avCoeff[i].y = (0f - fC1) * aSample[i, 1];
				avCoeff[i].w = fC0 * aSample[i, 0] - fC3 * aSample[i, 6];
			}
			avCoeff[6].x = fC4 * aSample[0, 8];
			avCoeff[6].y = fC4 * aSample[1, 8];
			avCoeff[6].z = fC4 * aSample[2, 8];
			vRGB.x = avCoeff[0].y + avCoeff[0].w;
			vRGB.y = avCoeff[1].y + avCoeff[1].w;
			vRGB.z = avCoeff[2].y + avCoeff[2].w;
			vRGB.x -= avCoeff[6].x;
			vRGB.y -= avCoeff[6].y;
			vRGB.z -= avCoeff[6].z;
			vRGB.x += RenderSettings.ambientLight.r;
			vRGB.y += RenderSettings.ambientLight.g;
			vRGB.z += RenderSettings.ambientLight.b;
			return vRGB;
		}
		return Vector3.one;
	}
}
