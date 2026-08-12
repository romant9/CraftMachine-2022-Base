using System;
using System.Collections.Generic;
using System.Linq;
using TwdCustomMod;
using UnityEngine;

public class UIToggleMenu : UIButtonToggleSet
{
	[SerializeField]
	private GameObject[] TogglePanels;

	[SerializeField]
	private Transform ContentParent;

	private List<UIToggleContent> ToggleContent;

	private const string LogString = "UIToggleMenu: ";

	public override void Start()
	{
		if (ContentParent != null)
		{
			if (ToggleContent == null)
			{
				ToggleContent = new List<UIToggleContent>();
				SetContent();
			}
		}
		else
		{
			DebugTWD.LogWarning("UIToggleMenu: ContentParent is NULL!");
		}
		base.Start();
	}

	private void SetContent()
	{
		for (int i = 0; i < TogglePanels.Length; i++)
		{
			if (!(TogglePanels[i] != null) || !(TogglePanels[i].gameObject != null))
			{
				continue;
			}
			GameObject gameObject = ((!TogglePanels[i].transform.IsPrefab()) ? TogglePanels[i] : UnityEngine.Object.Instantiate(TogglePanels[i], Vector3.zero, Quaternion.identity));
			if (gameObject != null)
			{
				gameObject.transform.SetParent(ContentParent);
				gameObject.transform.localPosition = Vector3.zero;
				gameObject.transform.localScale = Vector3.one;
				UIToggleContent component = gameObject.GetComponent<UIToggleContent>();
				if (component != null)
				{
					component.Added(this);
					ToggleContent.Add(component);
				}
				else
				{
					DebugTWD.LogWarning("UIToggleMenu: Could not find Component UIToggleContent!");
				}
			}
			else
			{
				DebugTWD.LogWarning("UIToggleMenu: Could not Instantiate Content!");
			}
		}
	}

	public void OpenContentByToggle(UIButtonToggle tg)
	{
		int index = GetUIButtonToggleList.ToList().IndexOf(tg);

		OpenContentByIndex(index);
	}

	public void OpenContentByIndex(int index)
	{
		if (ToggleContent == null)
		{
			Start();
		}
		if (OfflineManager.IsLoadDataManager && ToggleContent.Count == 0 && TogglePanels.Length > 0)
		{
			SetContent();
		}
		for (int i = 0; i < ToggleContent.Count; i++)
		{
			if (index == i)
			{
				if (OfflineManager.IsLoadDataManager && isProUserLimit)
				{
					if (proUserPanels.Contains(i))
					{
						if (!DataManager.Instance.ProGuild) 
						{
							string message;
							if (DataManager.Instance.language == DataManager.Language.Ru)
							{
								message = "Необходим статус: PRO-GUILD!";
							}
							else
							{
								message = "Required status: PRO-GUILD!";
							}
							MyTools.OpenAlert(message); 
							DebugTWD.Log(message); 
							return; 
						}
					}
				}
				ActivateContentByIndex(i);
			}
			else
			{
				DeactivateContentByIndex(i);
			}
		}
		if (OfflineManager.IsLoadDataManager) OpenHideGameobjectsList(index);

		base.UpdateStates(index.ToString());
	}

	public override void Clear()
	{
		base.Clear();
	}

	public UIToggleContent GetContentByIndex(int index)
	{
		if (ToggleContent != null && ToggleContent.Count > index && ToggleContent[index] != null)
		{
			return ToggleContent[index];
		}
		return null;
	}

	protected override void UpdateStates(string overrideId = "", bool originOnClick = false, bool hasTabChanged = true)
	{
		if (OfflineManager.IsLoadDataManager && ToggleContent.Count == 0 && TogglePanels.Length > 0)
		{
			SetContent();
		}
		for (int i = 0; i < TogglePanels.Length; i++)
		{
			if (CurrentToggle != null && int.Parse(CurrentToggle.id) == i)
			{
				if (OfflineManager.IsLoadDataManager && isProUserLimit)
				{
					if (proUserPanels.Contains(i))
					{
						if (!DataManager.Instance.ProGuild) 
						{
							string message;
							if (DataManager.Instance.language == DataManager.Language.Ru)
							{
								message = "Необходим статус: PRO-GUILD!";
							}
							else
							{
								message = "Required status: PRO-GUILD!";
							}
							MyTools.OpenAlert(message); 
							return; 
						}
					}
				}
				ActivateContentByIndex(i);
			}
			else
			{
				DeactivateContentByIndex(i);
			}
		}
		base.UpdateStates(overrideId, originOnClick);
	}

	private void ActivateContentByIndex(int index)
	{
		if (GetContentByIndex(index) != null && !GetContentByIndex(index).gameObject.activeSelf)
		{
			GetContentByIndex(index).Activate();
		}
	}

	private void DeactivateContentByIndex(int index)
	{
		if (GetContentByIndex(index) != null)
		{
			GetContentByIndex(index).Deactivate();
		}
	}



	#region myparams
	public bool isProUserLimit = false;

	public List<int> proUserPanels;
	public List<ProObject> proObjsList;

	[Serializable]
	public class ProObject
	{
		public List<int> PanelIndex;
		public GameObject go;
	}
	#endregion

	#region mycode
		void OpenHideGameobjectsList(int index)
	{
		if (proObjsList != null && proObjsList.Count > 0)
		{
			foreach (var go in proObjsList)
			{
				go.go.SetActive(go.PanelIndex.Contains(index));
			}
		}
	}
	#endregion

}
