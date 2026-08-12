using System;
using System.Collections.Generic;
using TWDModel;
using UnityEngine;

public class ActorNotificationManager
{
	private ActorNotificationElement nextElement;

	private GameObject followTarget;

	private List<ActorNotificationElement> notificationList;

	private List<ActorNotificationElement> allNotificationsThisTurn;

	private float deltaTime;

	public ActorNotificationManager(Transform parentTransform)
	{
		notificationList = new List<ActorNotificationElement>();
		allNotificationsThisTurn = new List<ActorNotificationElement>();
		followTarget = new GameObject("Actor Notification Element position");
		followTarget.transform.parent = parentTransform;
		followTarget.transform.position = parentTransform.position + new Vector3(0f, 2f, 0f);
		deltaTime = 0f;
	}

	public void Update(float dt)
	{
		if (nextElement == null)
		{
			nextElement = GetNextElement();
		}
		if (nextElement != null)
		{
			deltaTime += (dt + Time.unscaledDeltaTime) / 2f;
			if (deltaTime >= nextElement.Time)
			{
				PlayNotification(nextElement);
			}
		}
	}

	public void AddNotification(ActorNotificationMessage notification, bool showLucky = false, ActorModel actorModel = null, Action onStarted = null, TimedEffectType timedEffectType = TimedEffectType.None)
	{
		if (!(CombatView.Instance != null) || !(CombatView.Instance.CombatHUD != null))
		{
			return;
		}
		ActorNotificationElement actorNotificationElement = CombatView.Instance.CombatHUD.CreateActorNotificationElement(notification.MessageType);
		if (!(actorNotificationElement != null))
		{
			return;
		}
		actorNotificationElement.SetManager(this);
		actorNotificationElement.FollowTarget(followTarget);
		actorNotificationElement.NotificationElement.text = notification.Message;
		actorNotificationElement.MessageType = notification.MessageType;
		actorNotificationElement.TimedEffectType = timedEffectType;
		actorNotificationElement.SourceTraitIdentifier = notification.SourceTraitIdentifier;
		if (onStarted != null)
		{
			actorNotificationElement.OnStartedPlaying = onStarted;
		}
		if (notification.MessageSize > 0)
		{
			actorNotificationElement.NotificationElement.fontSize = notification.MessageSize;
		}
		if (notification.MessageSound != NotificationSound.None)
		{
			actorNotificationElement.MessageSound = notification.MessageSound;
		}
		if ((actorNotificationElement.MessageType == ActorNotificationType.BattlePassCurrencyNotification || actorNotificationElement.MessageType == ActorNotificationType.ActionNotification || actorNotificationElement.MessageType == ActorNotificationType.TimedEffectNotification) && actorNotificationElement.NotificationIcon != null)
		{
			actorNotificationElement.NotificationIcon.spriteName = notification.Icon;
			if (actorNotificationElement.MessageType == ActorNotificationType.TimedEffectNotification)
			{
				(actorNotificationElement as ActorTimedEffectNotificationElement)?.Init();
			}
		}
		if (actorNotificationElement.MessageType == ActorNotificationType.ChargePoint && actorModel != null && actorModel.ChargeMeter.ChargeLevel == actorModel.ChargeMeter.MaxLevel)
		{
			actorNotificationElement.NotificationIcon.spriteName = "Ui_Charge_Point_Fill_Green";
		}
		if (actorNotificationElement.MessageType == ActorNotificationType.DamageFlame || actorNotificationElement.MessageType == ActorNotificationType.IgniteBoost)
		{
			Helpers.GameObjectSetActive(actorNotificationElement.NotificationIcon, value: false);
		}
		if (showLucky && actorNotificationElement.LuckyIcon != null)
		{
			actorNotificationElement.LuckyIcon.gameObject.SetActive(value: true);
		}
		EffectRumble componentInChildren = actorNotificationElement.gameObject.GetComponentInChildren<EffectRumble>();
		if (componentInChildren != null)
		{
			componentInChildren.EffectFinished += OnNotificationEffectFinished;
		}
		actorNotificationElement.gameObject.SetActive(value: false);
		notificationList.Add(actorNotificationElement);
		allNotificationsThisTurn.Add(actorNotificationElement);
	}

	public bool GetIsNotificationAlreadyInQueueForEffect(TimedEffectType timedEffectType)
	{
		if (new List<ActorNotificationElement>(allNotificationsThisTurn).Find((ActorNotificationElement x) => x.TimedEffectType == timedEffectType) != null)
		{
			return true;
		}
		return false;
	}

	public void StackNotificationMessage(ActorNotificationMessage message)
	{
		foreach (ActorNotificationElement item in new List<ActorNotificationElement>(notificationList))
		{
			if (item.MessageType == message.MessageType)
			{
				notificationList.Remove(item);
				message.StackedNotificationCount++;
			}
		}
		if (message.StackedNotificationCount > 1)
		{
			message.Message = message.Message + " x" + message.StackedNotificationCount;
		}
	}

	public void WipeNotificationList()
	{
		allNotificationsThisTurn.Clear();
	}

	public void ClearSameNotificationTypes(ActorNotificationType type)
	{
		notificationList.RemoveAll((ActorNotificationElement x) => x.MessageType == type);
	}

	public int GetTotalPendingNotifications()
	{
		return notificationList.Count;
	}

	public void RemoveNotification(ActorNotificationElement notification)
	{
		notificationList.Remove(notification);
		UnityEngine.Object.Destroy(notification.gameObject);
	}

	public void RemoveNotificationsForTrait(string traitIdentifier)
	{
		if (traitIdentifier == "")
		{
			return;
		}
		foreach (ActorNotificationElement item in new List<ActorNotificationElement>(notificationList))
		{
			if (item.SourceTraitIdentifier == traitIdentifier)
			{
				RemoveNotification(item);
			}
		}
	}

	private void PlayNotification(ActorNotificationElement notification)
	{
		notification.gameObject.SetActive(value: true);
		notification.IsPlaying = true;
		notification.OnStarted();
		deltaTime = 0f;
		TweenManager.PlayTweenGroup(notification.gameObject, 0, forward: true, notification.OnFinished);
		EffectRumble componentInChildren = notification.gameObject.GetComponentInChildren<EffectRumble>();
		if (componentInChildren != null)
		{
			componentInChildren.ResetTimer();
		}
		if (SingularityMonoBehaviour<AudioManager>.Instance != null)
		{
			SingularityMonoBehaviour<AudioManager>.Instance.PlayNotificationSound(notification.MessageSound);
		}
		nextElement = null;
	}

	private ActorNotificationElement GetNextElement()
	{
		for (int i = 0; i < notificationList.Count; i++)
		{
			if (!notificationList[i].IsPlaying)
			{
				return notificationList[i];
			}
		}
		return null;
	}

	private void OnNotificationEffectFinished(GameObject rumbleObject)
	{
		ActorNotificationElement component = rumbleObject.transform.parent.GetComponent<ActorNotificationElement>();
		if (component != null)
		{
			RemoveNotification(component);
		}
	}
}
