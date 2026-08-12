using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LogPopup : MonoBehaviour
{
	public UITextList LogList;
	public List<string> LogStringList { get; private set; } = new List<string>();

    public GameObject toggleSet;
	public UIScrollView logScrollView;
	public GameObject logTypeItemPrefab;
	public UILabel logLabel;
	public UIButton bt_dropdown;

	public bool IsUseLabel = true;

	void Start()
	{
		IsUseLabel = false;
    }

    void Update()
	{
	}

	void OnEnable()
	{
		LogList.gameObject.SetActive(!IsUseLabel);
		logLabel.transform.parent.gameObject.SetActive(IsUseLabel);

		DebugTWD.On_Debug += OnDebug;
		LogListInit();
	}

	public void OnClickClose()
	{
		this.gameObject.SetActive(false);
	}

	public void AddLog(string text)
	{
		logLabel.text += text + '\n';
	}

	void LogListInit()
	{
		if (IsUseLabel)
		{
			logLabel.text = "";
			LogStringList.Clear();

            foreach (var log in DebugTWD.DebugItems)
			{
				if (GetLogType(log))
				{
                    AddLog("(" + DebugTWD.DebugItems.IndexOf(log) + ") " + log.DebugMessage);
                }
            }
		}
		else
		{
			LogList.Clear();
			LogStringList.Clear();

            foreach (var log in DebugTWD.DebugItems)
			{
                if (GetLogType(log))
				{
                    var logStr = "(" + DebugTWD.DebugItems.IndexOf(log) + ") " + log.DebugMessage;
                    LogList.Add(logStr);
					LogStringList.Add(logStr);
                }
            }
        }
	}

	private bool GetLogType(DebugItem item)
	{
		if (DebugTWD.LogUserTypesSelected.Contains(DebugType.None)) return false;
		if (DebugTWD.LogUserTypesAll.Contains(item.DebugTypeLog) && (DebugTWD.LogUserTypesSelected.Contains(DebugType.All) || DebugTWD.LogUserTypesSelected.Contains(item.DebugTypeLog)))
		{
			return true;
		}
		return false;
	}

	private void OnDisable()
	{
		DebugTWD.On_Debug -= OnDebug;
	}

	public void OnDebug(DebugItem item)
	{
		if (GetLogType(item))
		{
			if (IsUseLabel)
			{
				AddLog(item.DebugMessage);
			}
			else
			{
                var logStr = "(" + (LogStringList.Count + 1) + ") " + item.DebugMessage;
                LogStringList.Add(logStr);
                LogList.Add(logStr);
			}
		}
	}

	public void ClearLog()
	{
		if (IsUseLabel)
		{
			logLabel.text = "";
		}
		else
		{
			LogList.Clear();
		}
		DebugTWD.DebugItems.Clear();
        LogStringList.Clear();
    }

    public void SaveLogToClipboard()
	{
		TextEditor te = new TextEditor();
		te.text = IsUseLabel ? logLabel.text : string.Join('\n', LogStringList);// LogList.GetAllStrings();
		te.SelectAll();
		te.Copy();
	}

	public void ChangeLogType(UIButtonExtended bt)
	{
		var toggles = logScrollView.transform.GetComponentsInChildren<UIButtonToggle>().ToList();
		var index = toggles.IndexOf((UIButtonToggle)bt);
		if (toggles[index].IsToggled && index > 0)
		{
			if (DebugTWD.LogUserTypesSelected.Contains(DebugType.All))
			{
				DebugTWD.LogUserTypesSelected.Remove(DebugType.All);
				toggles[0].SetToggled(false);
			}

			if (!DebugTWD.LogUserTypesSelected.Contains(DebugTWD.LogUserTypesAll[index]))
				DebugTWD.LogUserTypesSelected.Add(DebugTWD.LogUserTypesAll[index]);
		}
		else
		{
			if (DebugTWD.LogUserTypesSelected.Contains(DebugTWD.LogUserTypesAll[index]))
				DebugTWD.LogUserTypesSelected.Remove(DebugTWD.LogUserTypesAll[index]);

			if (DebugTWD.LogUserTypesSelected.Count > 0 && !DebugTWD.LogUserTypesSelected.Contains(DebugType.All) && index == 0)
			{
				for (int i = 1; i < toggles.Count; i++)
				{
					if (DebugTWD.LogUserTypesSelected.Contains(DebugTWD.LogUserTypesAll[i]))
					{
						DebugTWD.LogUserTypesSelected.Remove(DebugTWD.LogUserTypesAll[i]);
						toggles[i].SetToggled(false);
					}
				}
				DebugTWD.LogUserTypesSelected.Add(DebugType.All);
				toggles[0].SetToggled(true);
			}

			if (toggles.All(x => !x.IsToggled))
			{
				DebugTWD.LogUserTypesSelected.Add(DebugType.All);
				toggles[0].SetToggled(true);
			}
		}
		LogListInit();
	}

	public void OnOpenLogDropdown()
	{
		if (toggleSet.activeSelf)
		{
			StopCoroutine(CloseIfUnselected());
			toggleSet.SetActive(false);
			return;
		}

        List<DebugType> logTypeList = DebugTWD.LogUserTypesAll;

		if (logScrollView.transform.childCount > 0)
		{
			Helpers.DestroyAllChildren(logScrollView.gameObject);
		}

		for (int j = 0; j < logTypeList.Count; j++)
		{
			GameObject gameObject = Helpers.InstantiateToParent(logTypeItemPrefab, logScrollView.gameObject);
			gameObject.name = gameObject.name.Replace("(Clone)", $" ({j})");
			gameObject.GetComponent<UILabel>().text = logTypeList[j].ToString();
			var tg = gameObject.GetComponent<UIButtonToggle>();

			tg.SetClickCallback(ChangeLogType);
		}

		toggleSet.SetActive(true);

		StartCoroutine(WaitForEnable());
	}

	private IEnumerator WaitForEnable()
	{
		yield return new WaitUntil(() => logScrollView.transform.childCount > 0 && logScrollView.transform.GetChild(0).GetComponent<UIButtonToggle>().isActiveAndEnabled);

		var logTypeList = DebugTWD.LogUserTypesAll;

		for (int i = 0; i < logTypeList.Count; i++)
		{
			var tg = logScrollView.transform.GetChild(i).GetComponent<UIButtonToggle>();
			if (DebugTWD.LogUserTypesSelected.Contains(logTypeList[i]))
			{
				if (i > 0 && DebugTWD.LogUserTypesSelected.Contains(DebugType.All))
				{
					tg.SetToggled(false);
				}
				else
				{
					//DebugTWD.Log(logTypeList[i].ToString() + "(" + i + ") " + tg.name + " is toggled TRUE", DebugType.UI);
					tg.SetToggled(true);
				}
			}
			else
			{
				//DebugTWD.Log(logTypeList[i].ToString() + "(" + i + ") " + tg.name + " is toggled FALSE", DebugType.UI);
				tg.SetToggled(false);
			}
		}

		logScrollView.GetComponent<UITable>().Reposition();
		logScrollView.ResetPosition();
		StartCoroutine(CloseIfUnselected());
	}

	private IEnumerator CloseIfUnselected()
	{
		yield return new WaitUntil(() => UICamera.selectedObject == null || (UICamera.selectedObject != bt_dropdown.gameObject && !NGUITools.IsChild(toggleSet.transform, UICamera.selectedObject.transform)));
		toggleSet.SetActive(false);
	}
}
