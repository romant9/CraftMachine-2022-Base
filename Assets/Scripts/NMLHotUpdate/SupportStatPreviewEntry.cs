using TWDModel;
using UnityEngine;

public class SupportStatPreviewEntry : MonoBehaviour
{
	[SerializeField]
	private UILabel statNameLabel;

	[SerializeField]
	private UILabel currentValueLabel;

	[SerializeField]
	private UILabel nextValueLabel;

	[SerializeField]
	private GameObject[] changePreviewObjects;

	private SupportModel supportModel;

	private int statIndex;

	private bool active;

	public void Set(SupportModel support, int index)
	{
		supportModel = support;
		statIndex = index;
		string supportStatName = HelpersLocalization.GetSupportStatName(support.SupportId, statIndex);
		active = statIndex < supportModel.ParameterCount && !string.IsNullOrEmpty(supportStatName);
		statNameLabel.text = supportStatName;
		base.gameObject.SetActive(active);
		if (active)
		{
			FixedPoint parameter = supportModel.GetParameter(statIndex);
			FixedPoint parameterNextLevel = supportModel.GetParameterNextLevel(statIndex);
			bool flag = parameter != parameterNextLevel;
			currentValueLabel.text = parameter.ToString();
			GameObject[] array = changePreviewObjects;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].SetActive(flag);
			}
			if (flag)
			{
				nextValueLabel.text = parameterNextLevel.ToString();
			}
		}
	}
}
