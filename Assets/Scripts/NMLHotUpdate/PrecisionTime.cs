using Client.Constants;
using UnityEngine;

public class PrecisionTime : MonoBehaviour
{
	public int m_baseFrequency = 25;

	public int m_seconds1 = 1420;

	public int m_seconds2 = 20;

	public int m_seconds3 = 20;

	public int m_seconds4 = 20;

	private int m_time1;

	private int m_time2;

	private int m_time3;

	private int m_time4;

	private void Start()
	{
		if (m_baseFrequency != (int)Mathf.Round(1f / Time.fixedDeltaTime))
		{
			Debug.LogError("Base frequency does not match Fixed Time settings");
		}
	}

	private void FixedUpdate()
	{
		m_time1++;
		if (m_time1 >= m_seconds1 * m_baseFrequency)
		{
			m_time1 = 0;
		}
		m_time2++;
		if (m_time2 >= m_seconds2 * m_baseFrequency)
		{
			m_time2 = 0;
		}
		m_time3++;
		if (m_time3 >= m_seconds3 * m_baseFrequency)
		{
			m_time3 = 0;
		}
		m_time4++;
		if (m_time4 >= m_seconds4 * m_baseFrequency)
		{
			m_time4 = 0;
		}
	}

	private void Update()
	{
		float num = 1f / (float)m_baseFrequency;
		Shader.SetGlobalVector(MaterialParameters.PrecisionTime, new Vector4((float)m_time1 * num, (float)m_time2 * num, (float)m_time3 * num, (float)m_time4 * num));
	}
}
