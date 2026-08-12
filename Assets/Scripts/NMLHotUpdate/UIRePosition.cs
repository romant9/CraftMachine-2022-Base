using System.Collections.Generic;
using UnityEngine;

public class UIRePosition : MonoBehaviour
{
	[SerializeField]
	private List<GameObject> PreGoList;

	[SerializeField]
	private Vector3 AdjustP = Vector3.zero;

	private Vector3 OriginLocalPosition = Vector3.zero;

	private void OnEnable()
	{
	}

	private void OnDisable()
	{
	}

	private void Start()
	{
		OriginLocalPosition = base.transform.localPosition;
	}

	public void Update()
	{
		UpdateUi();
	}

	private void UpdateUi()
	{
		if (PreGoList == null || PreGoList.Count <= 0)
		{
			return;
		}
		base.transform.localPosition = OriginLocalPosition;
		for (int i = 0; i < PreGoList.Count; i++)
		{
			if (!PreGoList[i].activeInHierarchy)
			{
				base.transform.localPosition = base.transform.localPosition + AdjustP;
			}
		}
	}
}
