using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractiveCarousel : MonoBehaviour
{
	[Header("UI组件引用")]
	public UIScrollView scrollView;

	public UIPanel panel;

	public UIGrid grid;

	[Header("轮播图设置")]
	public float pageWidth = 150f;

	public float autoSwitchTime = 5f;

	public bool loop = true;

	public float switchSpeed = 10f;

	[Header("拖拽设置")]
	[Tooltip("拖拽距离超过此阈值才会触发页面切换（页面宽度的百分比）")]
	[Range(0.1f, 0.5f)]
	public float dragThreshold = 0.2f;

	[Tooltip("触发惯性滚动的最小速度")]
	public float minSwipeSpeed = 800f;

	[Tooltip("拖拽灵敏度，值越小越不灵敏")]
	[Range(0.1f, 1f)]
	public float dragSensitivity = 0.5f;

	[Header("指示点")]
	public GameObject dotPrefab;

	public Transform dotsContainer;

	public Color activeDotColor = Color.white;

	public Color inactiveDotColor = Color.gray;

	private List<Transform> items = new List<Transform>();

	private List<GameObject> dots = new List<GameObject>();

	private int currentPage;

	private float lastSwitchTime;

	private SpringPanel springPanel;

	public bool isSwitching;

	private bool isDragging;

	private Vector3 dragStartPos;

	private float dragStartTime;

	private Vector3 dragVelocity;

	private Vector3 lastDragPos;

	private float lastDragTime;

	private List<Vector3> dragPositions = new List<Vector3>();

	private List<float> dragTimes = new List<float>();

	private const int SAMPLE_COUNT = 5;

	private Coroutine autoSwitchCoroutine;

	private Coroutine switchCompleteCoroutine;

	public void Initialize(List<Transform> itemList)
	{
		items.Clear();
		foreach (Transform item in itemList)
		{
			items.Add(item);
		}
		grid.cellWidth = pageWidth;
		grid.Reposition();
		springPanel = grid.GetComponent<SpringPanel>();
		if (springPanel == null)
		{
			springPanel = grid.gameObject.AddComponent<SpringPanel>();
		}
		springPanel.enabled = false;
		if (scrollView != null)
		{
			scrollView.enabled = false;
		}
		ResetPosition();
		CreateDots();
		StartAutoSwitch();
		SetupDragEvents();
	}

	private void SetupDragEvents()
	{
		if (panel != null)
		{
			UIEventListener.Get(panel.gameObject).onPress = delegate(GameObject go, bool isPressed)
			{
				OnDragPress(isPressed);
			};
			UIEventListener.Get(panel.gameObject).onDrag = delegate(GameObject go, Vector2 delta)
			{
				OnDrag(delta);
			};
		}
	}

	private void ResetPosition()
	{
		Vector3 localPosition = grid.transform.localPosition;
		localPosition.x = 0f;
		grid.transform.localPosition = localPosition;
	}

	private void CreateDots()
	{
		if (dotPrefab == null || dotsContainer == null)
		{
			return;
		}
		ClearDotEntries();
		for (int i = 0; i < items.Count; i++)
		{
			GameObject gameObject = dotsContainer.gameObject.AddChild(dotPrefab);
			UISprite component = gameObject.GetComponent<UISprite>();
			if (component != null)
			{
				component.color = ((i == currentPage) ? activeDotColor : inactiveDotColor);
				dots.Add(gameObject);
			}
		}
		UIGrid component2 = dotsContainer.GetComponent<UIGrid>();
		if (component2 != null)
		{
			component2.Reposition();
		}
	}

	private void StartAutoSwitch()
	{
		lastSwitchTime = Time.time;
		if (autoSwitchCoroutine != null)
		{
			StopCoroutine(autoSwitchCoroutine);
		}
		autoSwitchCoroutine = StartCoroutine(AutoSwitchCoroutine());
	}

	private IEnumerator AutoSwitchCoroutine()
	{
		while (true)
		{
			if (Time.time - lastSwitchTime >= autoSwitchTime && !isSwitching && !isDragging)
			{
				SwitchToNextPage();
			}
			yield return new WaitForSeconds(0.5f);
		}
	}

	private void SwitchToPage(int pageIndex)
	{
		if (items.Count != 0 && !isSwitching)
		{
			isSwitching = true;
			pageIndex = ((!loop) ? Mathf.Clamp(pageIndex, 0, items.Count - 1) : ((pageIndex + items.Count) % items.Count));
			currentPage = pageIndex;
			float x = (float)(-currentPage) * pageWidth;
			Vector3 pos = new Vector3(x, grid.transform.localPosition.y, grid.transform.localPosition.z);
			if (springPanel != null && springPanel.enabled)
			{
				springPanel.enabled = false;
			}
			SpringPanel.Begin(grid.gameObject, pos, switchSpeed);
			UpdateDots();
			lastSwitchTime = Time.time;
			if (switchCompleteCoroutine != null)
			{
				StopCoroutine(switchCompleteCoroutine);
			}
			switchCompleteCoroutine = StartCoroutine(WaitForSwitchComplete());
		}
	}

	private IEnumerator WaitForSwitchComplete()
	{
		yield return new WaitForSeconds(0.1f);
		while (springPanel != null && springPanel.enabled)
		{
			yield return null;
		}
		isSwitching = false;
		switchCompleteCoroutine = null;
	}

	private void SwitchToNextPage()
	{
		SwitchToPage(currentPage + 1);
	}

	private void UpdateDots()
	{
		for (int i = 0; i < dots.Count; i++)
		{
			if (i < dots.Count)
			{
				dots[i].GetComponent<UISprite>().color = ((i == currentPage) ? activeDotColor : inactiveDotColor);
			}
		}
	}

	private void OnDragPress(bool isPressed)
	{
		if (isPressed)
		{
			isDragging = true;
			dragStartPos = UICamera.currentTouch.pos;
			lastDragPos = dragStartPos;
			dragStartTime = Time.time;
			lastDragTime = dragStartTime;
			dragPositions.Clear();
			dragTimes.Clear();
			dragPositions.Add(dragStartPos);
			dragTimes.Add(dragStartTime);
			lastSwitchTime = Time.time;
			if (springPanel != null && springPanel.enabled)
			{
				springPanel.enabled = false;
			}
			dragVelocity = Vector3.zero;
		}
		else
		{
			OnDragEnd();
		}
	}

	private void OnDrag(Vector2 delta)
	{
		if (!isDragging || isSwitching)
		{
			return;
		}
		Vector3 vector = UICamera.currentTouch.pos;
		float time = Time.time;
		dragPositions.Add(vector);
		dragTimes.Add(time);
		if (dragPositions.Count > 5)
		{
			dragPositions.RemoveAt(0);
			dragTimes.RemoveAt(0);
		}
		if (dragPositions.Count >= 2)
		{
			Vector3 vector2 = vector - dragPositions[0];
			float num = time - dragTimes[0];
			if (num > 0f)
			{
				dragVelocity = vector2 / num;
			}
		}
		lastDragPos = vector;
		lastDragTime = time;
		Vector3 localPosition = grid.transform.localPosition;
		localPosition.x += delta.x * dragSensitivity;
		if (!loop)
		{
			float min = (float)(-(items.Count - 1)) * pageWidth;
			localPosition.x = Mathf.Clamp(localPosition.x, min, 0f);
		}
		grid.transform.localPosition = localPosition;
	}

	private void OnDragEnd()
	{
		if (isDragging)
		{
			isDragging = false;
			float num = UICamera.currentTouch.pos.x - dragStartPos.x;
			float num2 = Mathf.Abs(num) / ((float)Screen.width * 0.5f);
			float num3 = Mathf.Abs(dragVelocity.x);
			bool flag = false;
			int num4 = 0;
			if (num2 > dragThreshold)
			{
				flag = true;
				num4 = ((!(num > 0f)) ? 1 : (-1));
			}
			else if (num3 > minSwipeSpeed)
			{
				flag = true;
				num4 = ((!(dragVelocity.x > 0f)) ? 1 : (-1));
			}
			else if (Mathf.Abs(grid.transform.localPosition.x + (float)currentPage * pageWidth) > pageWidth * 0.3f)
			{
				flag = true;
				num4 = ((!(grid.transform.localPosition.x + (float)currentPage * pageWidth > 0f)) ? 1 : (-1));
			}
			if (flag)
			{
				SwitchToPage(currentPage + num4);
			}
			else
			{
				SwitchToPage(currentPage);
			}
			dragPositions.Clear();
			dragTimes.Clear();
		}
	}

	public void GoToPage(int pageIndex)
	{
		SwitchToPage(pageIndex);
	}

	public void GoToNextPage()
	{
		SwitchToNextPage();
	}

	public void GoToPreviousPage()
	{
		SwitchToPage(currentPage - 1);
	}

	public int GetCurrentPage()
	{
		return currentPage;
	}

	public int GetTotalPages()
	{
		return items.Count;
	}

	public void SetDragSensitivity(float sensitivity)
	{
		dragSensitivity = Mathf.Clamp(sensitivity, 0.1f, 1f);
	}

	public void SetDragThreshold(float threshold)
	{
		dragThreshold = Mathf.Clamp(threshold, 0.1f, 0.5f);
	}

	private void OnEnable()
	{
		if (items.Count > 0)
		{
			lastSwitchTime = Time.time;
		}
	}

	private void OnDisable()
	{
		if (autoSwitchCoroutine != null)
		{
			StopCoroutine(autoSwitchCoroutine);
			autoSwitchCoroutine = null;
		}
		if (switchCompleteCoroutine != null)
		{
			StopCoroutine(switchCompleteCoroutine);
			switchCompleteCoroutine = null;
		}
	}

	private void ClearDotEntries()
	{
		for (int i = 0; i < dots.Count; i++)
		{
			NGUITools.Destroy(dots[i]);
		}
		dots.Clear();
	}
}
