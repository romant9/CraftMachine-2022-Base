using BaseModel;
using Client.Framework.Audio;
using TWDModel;
using UnityEngine;

public class DoorView : ModelView<DoorModel>, IRunLocationItem
{
	[Tooltip("When this door is operated, should all visualization be pending on the door operation.")]
	public bool BlockAllVisualization;

	public float TurnAngle = 180f;

	public float AnimationDuration = 1f;

	public float SlideDistance;

	public Vector3 SlideAxis = new Vector3(0f, 1f, 0f);

	private Quaternion closedRotation;

	private Quaternion openRotationHalf;

	private Quaternion openRotation;

	private Vector3 closedPosition;

	private Vector3 openPosition;

	private float currentBlendValue;

	private float targetBlendValue;

	public SoundType InteractionSound = SoundType.InteractiveObject_Door_Wood;

	public bool playSound = true;

	private bool IsOver180 => Mathf.Abs(TurnAngle) >= 180f;

	private bool IsOpen { get; set; }

	public override bool AutoGenerateViewID => true;

	public override void Initialize(ModelObject model)
	{
		base.Initialize(model);
		model.Changed += OnModelChanged;
		targetBlendValue = (base.Model.IsOpen ? 1 : 0);
		currentBlendValue = targetBlendValue;
		IsOpen = base.Model.IsOpen;
		if (base.Model.IsHidden)
		{
			SetChildrenVisible(visible: false);
		}
	}

	public TWDModelObject Apply(IRunLocationItemContainer runLocation, IRunLocationErrorContext errors)
	{
		DoorModel doorModel = new DoorModel(ViewId);
		runLocation.AddModelObject(doorModel);
		return doorModel;
	}

	public void Start()
	{
		closedPosition = base.transform.position;
		openPosition = closedPosition + SlideAxis.normalized * SlideDistance;
		closedRotation = base.transform.localRotation;
		openRotation = closedRotation * Quaternion.AngleAxis(TurnAngle, SlideAxis);
		if (IsOver180)
		{
			openRotationHalf = closedRotation * Quaternion.AngleAxis(TurnAngle * 0.5f, SlideAxis);
		}
	}

	private void SetChildrenVisible(bool visible)
	{
		Renderer[] componentsInChildren = GetComponentsInChildren<Renderer>(includeInactive: true);
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].enabled = visible;
		}
	}

	private void Update()
	{
		if (!base.IsInitialized)
		{
			return;
		}
		float maxDelta = ((AnimationDuration == 0f) ? 1f : (Time.deltaTime / AnimationDuration));
		currentBlendValue = Mathf.MoveTowards(currentBlendValue, targetBlendValue, maxDelta);
		if (IsOver180)
		{
			if (currentBlendValue < 0.5f)
			{
				base.transform.localRotation = Quaternion.Lerp(closedRotation, openRotationHalf, currentBlendValue * 2f);
			}
			else
			{
				base.transform.localRotation = Quaternion.Lerp(openRotationHalf, openRotation, (currentBlendValue - 0.5f) * 2f);
			}
		}
		else
		{
			base.transform.localRotation = Quaternion.Lerp(closedRotation, openRotation, currentBlendValue);
		}
		base.transform.position = Vector3.Lerp(closedPosition, openPosition, currentBlendValue);
	}

	public void OnModelChanged(ModelObject model, string changed, object args)
	{
		if (changed == "IsHidden")
		{
			bool isHidden = ((DoorModel)model).IsHidden;
			SetChildrenVisible(!isHidden);
		}
		else
		{
			ActorModel actorModel = args as ActorModel;
			VisualizationQueue.Instance.Add(new DelayedNotificationVisualizationTask(BlockAllVisualization ? null : actorModel, DelayedNotification));
		}
	}

	public void DelayedNotification()
	{
		if (base.Model.IsOpen != IsOpen && this != null)
		{
			IsOpen = base.Model.IsOpen;
			targetBlendValue = (IsOpen ? 1 : 0);
			string audioEvent = AudioEvents.GetAudioEvent(InteractionSound);
			string text = "combat_level/door_metal_open";
			string eventName = (string.IsNullOrEmpty(audioEvent) ? text : audioEvent);
			if (SingularityMonoBehaviour<AudioManager>.Instance != null && playSound)
			{
				SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent(eventName, base.gameObject);
			}
		}
	}
}
