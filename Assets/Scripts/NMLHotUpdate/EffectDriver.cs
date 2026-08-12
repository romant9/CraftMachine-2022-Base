using UnityEngine;

[ExecuteInEditMode]
public class EffectDriver : MonoBehaviour
{
	public enum DriverChannelType
	{
		Position = 0,
		PositionX = 1,
		PositionY = 2,
		PositionZ = 3,
		Rotation = 4,
		RotationX = 5,
		RotationY = 6,
		RotationZ = 7,
		AngleFromZero = 8,
		Scale = 9,
		ScaleX = 10,
		ScaleY = 11,
		ScaleZ = 12
	}

	public enum DrivenChannelType
	{
		Position = 0,
		PositionX = 1,
		PositionY = 2,
		PositionZ = 3,
		Rotation = 4,
		RotationX = 5,
		RotationY = 6,
		RotationZ = 7,
		Scale = 8,
		ScaleX = 9,
		ScaleY = 10,
		ScaleZ = 11,
		MaterialColor = 12,
		MaterialAlpha = 13
	}

	public enum ModulatorType
	{
		MADD = 0,
		FIT = 1,
		HALFSIN = 2,
		SIN = 3
	}

	public GameObject driverObj;

	public DriverChannelType DriverChannel;

	public ModulatorType Modulator;

	public DrivenChannelType DrivenChannel;

	public Vector3 PreAddMulPostAdd = new Vector3(0f, 1f, 0f);

	public Vector4 FromLowHighToLowHigh = new Vector4(0f, 1f, 0f, 1f);

	public bool Clamp;

	public Vector2 ClampMinMax = new Vector2(0f, 90f);

	public float SinWavelength = 1f;

	public float SinFreqOffset;

	public float SinMultiplier = 1f;

	public float SinValueGamma = 1f;

	public float SinPhaseGamma = 1f;

	private Transform driver;

	private Transform driven;

	private Transform secondary;

	private Renderer drivenRenderer;

	private Material drivenMaterial;

	private void Start()
	{
		driver = driverObj.GetComponent<Renderer>().transform;
		if (driver == null)
		{
			Debug.LogWarning("Driver Transform not found");
		}
		driven = base.transform;
		if (driven == null)
		{
			Debug.LogWarning("Driven Transform not found");
		}
		if (DrivenChannel == DrivenChannelType.MaterialAlpha || DrivenChannel == DrivenChannelType.MaterialColor)
		{
			drivenMaterial = base.gameObject.GetComponentInChildren<Renderer>().material;
			_ = drivenMaterial == null;
		}
	}

	private void Update()
	{
		float num = 0f;
		Vector3 localPosition = new Vector3(0f, 0f, 0f);
		Quaternion rotation = Quaternion.identity;
		switch (DriverChannel)
		{
		case DriverChannelType.Position:
			localPosition = driver.localPosition;
			break;
		case DriverChannelType.PositionX:
			num = driver.localPosition.x;
			break;
		case DriverChannelType.PositionY:
			num = driver.localPosition.y;
			break;
		case DriverChannelType.PositionZ:
			num = driver.localPosition.z;
			break;
		case DriverChannelType.AngleFromZero:
			num = Quaternion.Angle(driver.localRotation, Quaternion.identity);
			break;
		case DriverChannelType.Rotation:
			rotation = driver.rotation;
			break;
		case DriverChannelType.RotationX:
			num = driver.localEulerAngles.x;
			break;
		case DriverChannelType.RotationY:
			num = driver.localEulerAngles.y;
			break;
		case DriverChannelType.RotationZ:
			num = driver.localEulerAngles.z;
			break;
		}
		switch (Modulator)
		{
		case ModulatorType.MADD:
			num += PreAddMulPostAdd.x;
			num *= PreAddMulPostAdd.y;
			num += PreAddMulPostAdd.z;
			break;
		case ModulatorType.FIT:
			num = (num - FromLowHighToLowHigh.x) * (FromLowHighToLowHigh.w - FromLowHighToLowHigh.z) / (FromLowHighToLowHigh.y - FromLowHighToLowHigh.x) + FromLowHighToLowHigh.z;
			break;
		case ModulatorType.HALFSIN:
		{
			float num2 = Mathf.Clamp(num / (0.5f * SinWavelength) + SinFreqOffset, 0f, 3.141592f);
			num2 = 3.141592f * Mathf.Pow(num2 / 3.141592f, SinPhaseGamma);
			num = SinMultiplier * Mathf.Pow(Mathf.Sin(num2), SinValueGamma);
			break;
		}
		}
		if (Clamp)
		{
			num = Mathf.Clamp(num, ClampMinMax.x, ClampMinMax.y);
		}
		switch (DrivenChannel)
		{
		case DrivenChannelType.Position:
			driven.localPosition = localPosition;
			break;
		case DrivenChannelType.PositionX:
			driven.localPosition = new Vector3(num, driven.localPosition.y, driven.localPosition.z);
			break;
		case DrivenChannelType.PositionY:
			driven.localPosition = new Vector3(driven.localPosition.x, num, driven.localPosition.z);
			break;
		case DrivenChannelType.PositionZ:
			driven.localPosition = new Vector3(driven.localPosition.x, driven.localPosition.y, num);
			break;
		case DrivenChannelType.Rotation:
			driven.rotation = rotation;
			break;
		case DrivenChannelType.RotationX:
			driven.localEulerAngles = new Vector3(num, driven.localEulerAngles.y, driven.localEulerAngles.z);
			break;
		case DrivenChannelType.RotationY:
			driven.localEulerAngles = new Vector3(driven.localEulerAngles.x, num, driven.localEulerAngles.z);
			break;
		case DrivenChannelType.RotationZ:
			driven.localEulerAngles = new Vector3(driven.localEulerAngles.x, driven.localEulerAngles.y, num);
			break;
		case DrivenChannelType.ScaleX:
			driven.localScale = new Vector3(num, driven.localScale.y, driven.localScale.z);
			break;
		case DrivenChannelType.ScaleY:
			driven.localScale = new Vector3(driven.localScale.x, num, driven.localScale.z);
			break;
		case DrivenChannelType.ScaleZ:
			driven.localScale = new Vector3(driven.localScale.x, driven.localScale.y, num);
			break;
		case DrivenChannelType.MaterialAlpha:
			drivenMaterial.color = new Color(1f, 1f, 1f, num);
			break;
		case DrivenChannelType.Scale:
		case DrivenChannelType.MaterialColor:
			break;
		}
	}
}
