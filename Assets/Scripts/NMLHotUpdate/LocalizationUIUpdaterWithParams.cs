using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(UILabel))]
public class LocalizationUIUpdaterWithParams : LocalizationUIUpdater
{
	[SerializeField]
	protected string[] parameters;

	protected override string GetLocalizedText(params object[] tempParams)
	{
		return SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText(localizationKey, ((IEnumerable<object>)parameters).ToArray());
	}

	public void UpdateParameters(params string[] newParameters)
	{
		parameters = newParameters;
		UpdateContent();
	}
}
