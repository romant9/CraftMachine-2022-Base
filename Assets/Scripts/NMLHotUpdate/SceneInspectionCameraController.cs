using UnityEngine;

public class SceneInspectionCameraController : MonoBehaviour
{
	[SerializeField]
	private float zoomSpeed = 1f;

	private const float pinchSpeed = 0.001f;

	private Vector3 prevMousePosition;

	private bool mouseDown;

	private bool touchDown;

	private int firstTouchId = -1;

	[SerializeField]
	private float cameraDistance = 21f;

	[SerializeField]
	private float elevationAngle = 65f;

	[SerializeField]
	private float rotationAngle = 90f;

	private Vector3 cameraInterestPoint;

	private void Start()
	{
		Vector3 direction = GetComponent<Camera>().transform.TransformDirection(new Vector3(0f, 0f, 1f));
		Plane plane = new Plane(Vector3.up, 0f);
		Ray ray = new Ray(GetComponent<Camera>().transform.position, direction);
		plane.Raycast(ray, out var enter);
		cameraInterestPoint = ray.origin + ray.direction * enter;
	}

	private void Update()
	{
		Plane plane = new Plane(Vector3.up, 0f);
		float num = 0f;
		if (Input.touchCount == 2)
		{
			Touch touch = Input.GetTouch(0);
			Touch touch2 = Input.GetTouch(1);
			Vector3 vector = touch.deltaPosition;
			Vector3 rhs = touch2.deltaPosition;
			float num2 = Vector3.Dot(vector, rhs);
			if (num2 < 0f)
			{
				num = ((!(Vector3.Dot(touch2.position - touch.position, vector) > 0f)) ? (num2 * 0.001f) : ((0f - num2) * 0.001f));
			}
		}
		else
		{
			num = Input.GetAxis("Mouse ScrollWheel");
		}
		cameraDistance += num * zoomSpeed;
		if (Input.touchCount == 1)
		{
			touchDown = true;
			Touch touch3 = Input.GetTouch(0);
			if (touch3.phase == TouchPhase.Began || firstTouchId != touch3.fingerId)
			{
				prevMousePosition = Input.GetTouch(0).position;
				firstTouchId = touch3.fingerId;
			}
		}
		else
		{
			touchDown = false;
			firstTouchId = -1;
		}
		if (Input.touchCount == 0)
		{
			if (Input.GetMouseButtonDown(0))
			{
				mouseDown = true;
				prevMousePosition = Input.mousePosition;
			}
			else if (Input.GetMouseButtonUp(0))
			{
				mouseDown = false;
			}
		}
		if (mouseDown || touchDown)
		{
			Vector3 mousePosition = Input.mousePosition;
			float enter = 0f;
			float enter2 = 0f;
			Ray ray = GetComponent<Camera>().ScreenPointToRay(mousePosition);
			Ray ray2 = GetComponent<Camera>().ScreenPointToRay(prevMousePosition);
			plane.Raycast(ray, out enter);
			plane.Raycast(ray2, out enter2);
			Vector3 vector2 = ray.origin + ray.direction * enter - (ray2.origin + ray2.direction * enter2);
			cameraInterestPoint -= vector2;
			cameraInterestPoint.y = 0f;
			prevMousePosition = mousePosition;
		}
		GetComponent<Camera>().transform.rotation = Quaternion.Euler(new Vector3(elevationAngle, rotationAngle, 0f));
		Vector3 vector3 = GetComponent<Camera>().transform.TransformDirection(Vector3.forward);
		GetComponent<Camera>().transform.position = cameraInterestPoint - vector3 * cameraDistance;
	}
}
