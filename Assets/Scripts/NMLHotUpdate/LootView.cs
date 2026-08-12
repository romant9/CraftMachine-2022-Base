using System.Collections;
using BaseModel;
using TWDModel;
using UnityEngine;

public class LootView : ModelView<LootModel>, IRunLocationItem
{
	public GameObject notificationParent;

	public bool rotateOnLoot;

	public Vector3 rotationAngles = new Vector3(-180f, 0f, 0f);

	public float rotationDuration = 1f;

	public Transform rotationTarget;

	public GameObject primaryIndicatorPrefab;

	private ActorNotificationManager notificationManager;

	private Quaternion targetRotation;

	private float rotationTime;

	private Vector3 origAngles;

	public float textNotificationDelay = 0.4f;

	public float lootHUDNotificationDelay = 1f;

	public bool hideOnLoot;

	public override bool AutoGenerateViewID => true;

	public override void Initialize(ModelObject model)
	{
		base.Initialize(model);
		model.Changed += OnModelChanged;
		notificationManager = new ActorNotificationManager((notificationParent != null) ? notificationParent.transform : base.transform);
		rotationTime = (base.Model.IsOpened ? 0f : rotationDuration);
		if (rotationTarget != null)
		{
			origAngles = rotationTarget.transform.localEulerAngles;
		}
	}

	private void Update()
	{
		if (!base.IsInitialized)
		{
			return;
		}
		notificationManager.Update(Time.deltaTime);
		if (rotationTime < rotationDuration)
		{
			rotationTime += Time.deltaTime;
			float t = Mathf.Clamp(rotationTime / rotationDuration, 0f, 1f);
			if (rotationTarget != null)
			{
				rotationTarget.transform.localEulerAngles = Vector3.Lerp(origAngles, origAngles + rotationAngles, t);
			}
		}
	}

	public void OnModelChanged(ModelObject model, string changed, object args)
	{
		ActorModel actor = args as ActorModel;
		VisualizationQueue.Instance.Add(new DelayedNotificationVisualizationTask(actor, DelayedNotification));
	}

	private IEnumerator DelayedTextNotification()
	{
		yield return new WaitForSeconds(textNotificationDelay);
		if (base.Model.ContainsKey)
		{
			notificationManager.AddNotification(new ActorNotificationMessage("", ActorNotificationType.CurrencyKey, -1, NotificationSound.CurrencyKey));
		}
		else
		{
			notificationManager.AddNotification(new ActorNotificationMessage(LocalizationManager.GetText("Loot.Empty"), ActorNotificationType.Generic, -1, NotificationSound.NothingUseful));
		}
		if (!hideOnLoot)
		{
			yield break;
		}
		MeshRenderer[] componentsInChildren = base.gameObject.GetComponentsInChildren<MeshRenderer>();
		if (componentsInChildren != null && componentsInChildren.Length != 0)
		{
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				componentsInChildren[i].enabled = false;
			}
		}
	}

	public void DelayedNotification()
	{
		if (!(this == null) && (object)this != null)
		{
			StartCoroutine(DelayedTextNotification());
			if (rotateOnLoot)
			{
				rotationTime = 0f;
			}
			InteractiveObjectView component = base.gameObject.transform.parent.GetComponent<InteractiveObjectView>();
			if (component != null)
			{
				component.PlayInteractionSoundEvent();
			}
		}
	}

	public TWDModelObject Apply(IRunLocationItemContainer runLocation, IRunLocationErrorContext errors)
	{
		LootModel lootModel = new LootModel(ViewId);
		runLocation.AddModelObject(lootModel);
		return lootModel;
	}
}
