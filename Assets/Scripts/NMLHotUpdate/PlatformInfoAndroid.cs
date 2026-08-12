using System.Text.RegularExpressions;
using UnityEngine;

public class PlatformInfoAndroid : PlatformInfoBase
{
	private static int m_memory = 512;

	private static int m_nativeWidth = 0;

	private static int m_nativeHeight = 0;

	private static int m_limitedWidth = 0;

	private static int m_limitedHeight = 0;

	private static bool m_es2 = false;

	private static bool m_slowEs3 = false;

	private static bool m_tegra4 = false;

	public PlatformInfoAndroid()
	{
		m_memory = SystemInfo.systemMemorySize + SystemInfo.graphicsMemorySize;
		m_es2 = SystemInfo.graphicsDeviceVersion.Contains("ES 2");
		m_nativeWidth = Screen.width;
		m_nativeHeight = Screen.height;
		int num = 0;
		bool flag = SystemInfo.graphicsDeviceName.ToLower().Contains("videocore");
		bool flag2 = SystemInfo.graphicsDeviceName.ToLower().Contains("powervr");
		bool flag3 = SystemInfo.graphicsDeviceName.ToLower().Contains("mali");
		bool flag4 = SystemInfo.graphicsDeviceName.ToLower().Contains("adreno");
		bool flag5 = SystemInfo.graphicsDeviceName.ToLower().Contains("nvidia") && SystemInfo.graphicsDeviceName.ToLower().Contains("tegra");
		bool num2 = SystemInfo.graphicsDeviceName.ToLower().Contains("vivante");
		int result = 0;
		int.TryParse(Regex.Match(SystemInfo.graphicsDeviceName, "\\d+").Value, out result);
		if (num2)
		{
			if (result <= 7000)
			{
				m_slowEs3 = !m_es2;
				num = Mathf.Max(m_nativeHeight / 2, 540);
			}
		}
		else if (flag)
		{
			num = Mathf.Max(m_nativeHeight / 2, 540);
		}
		else if (flag2)
		{
			if (SystemInfo.graphicsDeviceName.ToLower().Contains("rogue han"))
			{
				result = 6200;
			}
			else if (SystemInfo.graphicsDeviceName.ToLower().Contains("rogue hood"))
			{
				result = 6400;
			}
			switch (result)
			{
			case 6100:
				num = 720;
				break;
			case 6200:
			case 6230:
				num = 800;
				break;
			case 6400:
			case 6430:
				num = 1080;
				break;
			case 6630:
				num = 1440;
				break;
			case 535:
			case 536:
			case 537:
			case 538:
			case 539:
			case 540:
			case 541:
			case 542:
			case 543:
			case 544:
				num = Mathf.Max(m_nativeHeight / 2, 480);
				break;
			}
		}
		else if (flag3)
		{
			if (result != 0)
			{
				if (result < 604)
				{
					num = 640;
				}
				else if (result <= 604)
				{
					m_slowEs3 = true;
					num = 800;
				}
				else if (result <= 622)
				{
					num = 800;
				}
				else if (result <= 624)
				{
					num = 800;
				}
				else if (result <= 628)
				{
					num = 1080;
				}
				else if (result <= 658)
				{
					num = 1080;
				}
				else if (result <= 678)
				{
					num = 1080;
				}
				else if (result <= 720)
				{
					num = 1080;
				}
				else if (result <= 760)
				{
					num = 1600;
				}
				else if (result <= 820)
				{
					num = 1080;
				}
				else if (result <= 880)
				{
					num = 1080;
				}
			}
			if (SystemInfo.graphicsDeviceName.ToLower().StartsWith("mali-g") && result > 0)
			{
				num = 1080;
			}
		}
		else if (flag4)
		{
			if (result != 0)
			{
				if (result < 220)
				{
					num = 320;
				}
				else if (result <= 220)
				{
					num = 540;
				}
				else if (result <= 225)
				{
					num = 640;
				}
				else if (result <= 304)
				{
					m_slowEs3 = true;
					num = 640;
				}
				else if (result <= 309)
				{
					m_slowEs3 = true;
					num = 720;
				}
				else if (result <= 320)
				{
					num = 900;
				}
				else if (result <= 330)
				{
					num = 1024;
				}
				else if (result <= 405)
				{
					num = 1080;
				}
				else if (result <= 418)
				{
					num = 1080;
				}
				else if (result <= 420)
				{
					num = 1080;
				}
				else if (result <= 430)
				{
					num = 1440;
				}
				else if (result > 430)
				{
					num = 1440;
				}
			}
		}
		else if (flag5)
		{
			m_tegra4 = m_es2;
			if (!m_tegra4 && m_es2)
			{
				num = 600;
			}
		}
		if (m_es2 && num == 0)
		{
			num = 720;
		}
		if (num > 0 && (double)num < (double)m_nativeHeight * 0.93)
		{
			int limitedHeight = num;
			m_limitedWidth = (int)(Mathf.Ceil((float)num / (float)m_nativeHeight * (float)m_nativeWidth / 16f) * 16f);
			m_limitedHeight = limitedHeight;
		}
	}

	protected override bool IsLowMemoryDevice()
	{
		if (m_memory > 768)
		{
			if (m_es2)
			{
				return m_memory <= 1024;
			}
			return false;
		}
		return true;
	}

	protected override bool IsSDResolutionDevice()
	{
		int num = m_limitedWidth * m_limitedHeight;
		if (num == 0)
		{
			num = Screen.width * Screen.height;
		}
		if (num > 786432 && m_memory > 768)
		{
			return SystemInfo.maxTextureSize <= 2048;
		}
		return true;
	}

	protected override bool IsSlowCPUDevice()
	{
		if (SystemInfo.processorCount > 2)
		{
			return m_memory <= 768;
		}
		return true;
	}

	protected override bool IsSlowGPUDevice()
	{
		if ((!m_es2 || m_tegra4) && !m_slowEs3 && m_memory > 768 && SystemInfo.supportsShadows)
		{
			return SystemInfo.maxTextureSize <= 2048;
		}
		return true;
	}

	public override bool GetLimitedScreenSize(out int w, out int h)
	{
		w = m_limitedWidth;
		h = m_limitedHeight;
		if (w > 0)
		{
			return h > 0;
		}
		return false;
	}

	protected override bool SupportsStencil()
	{
		return !m_tegra4;
	}
}
