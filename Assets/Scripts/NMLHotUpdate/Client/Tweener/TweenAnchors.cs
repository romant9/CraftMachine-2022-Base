using System;
using UnityEngine;

namespace Client.Tweener
{
	public class TweenAnchors : MonoBehaviour
	{
		public string id;

		public TweenAnchorsData fromData;

		public TweenAnchorsData toData;

		public float duration;

		public Easing.All easing;

		public bool leftEnabled;

		public bool rightEnabled;

		public bool bottomEnabled;

		public bool topEnabled;

		public bool alphaEnabled;

		public bool dynamicFrom;

		private UIWidget widgetRef;

		private Tweener tweenPosition;

		private Tweener tweenAlpha;

		private Vector4 positionFrom;

		private Vector4 positionTo;

		private Vector4 alphaFrom;

		private Vector4 alphaTo;

		private Tweener.TweenDelegate easingFunction;

		public UIWidget Widget
		{
			get
			{
				if (widgetRef == null)
				{
					widgetRef = GetComponent<UIWidget>();
				}
				return widgetRef;
			}
			set
			{
				widgetRef = value;
			}
		}

		private void OnDisable()
		{
			Reset();
		}

		private void Update()
		{
			if (tweenPosition == null || tweenAlpha == null || !(Widget != null))
			{
				return;
			}
			tweenPosition.update();
			if (tweenAlpha != null)
			{
				tweenAlpha.update();
			}
			if (tweenPosition != null && tweenAlpha != null)
			{
				if (alphaEnabled)
				{
					Widget.alpha = tweenAlpha.progression.w;
				}
				if (leftEnabled)
				{
					Widget.leftAnchor.absolute = (int)tweenPosition.progression.x;
				}
				if (rightEnabled)
				{
					Widget.rightAnchor.absolute = (int)tweenPosition.progression.y;
				}
				if (bottomEnabled)
				{
					Widget.bottomAnchor.absolute = (int)tweenPosition.progression.z;
				}
				if (topEnabled)
				{
					Widget.topAnchor.absolute = (int)tweenPosition.progression.w;
				}
			}
			if (tweenPosition == null || (tweenPosition != null && !tweenPosition.animating))
			{
				ClearTween();
				base.enabled = false;
			}
		}

		[ContextMenu("Play Forward")]
		public void PlayForward()
		{
			CreateTween();
		}

		[ContextMenu("Play Backwards")]
		public void PlayBackwards()
		{
			CreateTween(forward: false);
		}

		[ContextMenu("Reset")]
		public void Reset()
		{
			ClearTween();
			widgetRef = null;
			base.enabled = false;
		}

		public void SetCallback(Tweener.CallBackDelegate callback)
		{
			if (tweenPosition != null)
			{
				Tweener tweener = tweenPosition;
				tweener.currectCallback = (Tweener.CallBackDelegate)Delegate.Remove(tweener.currectCallback, callback);
				Tweener tweener2 = tweenPosition;
				tweener2.currectCallback = (Tweener.CallBackDelegate)Delegate.Combine(tweener2.currectCallback, callback);
			}
		}

		private void CreateTween(bool forward = true)
		{
			if (base.gameObject == null)
			{
				Debug.LogWarning("Cant create tween gameobject NULL");
				return;
			}
			if (tweenPosition == null)
			{
				tweenPosition = new Tweener();
				tweenAlpha = new Tweener();
			}
			if (dynamicFrom)
			{
				GetCurrectVector4(base.gameObject, Widget, out positionFrom, out alphaFrom);
			}
			else
			{
				fromData.GetAsVector4s(out positionFrom, out alphaFrom);
			}
			toData.GetAsVector4s(out positionTo, out alphaTo);
			easingFunction = TweenerHelpers.getGetByEnum(easing);
			base.enabled = true;
			if (forward)
			{
				tweenPosition.easeFromTo(positionFrom, positionTo, duration, easingFunction);
				tweenAlpha.easeFromTo(alphaFrom, alphaTo, duration, easingFunction);
			}
			else
			{
				tweenPosition.easeFromTo(positionTo, positionFrom, duration, easingFunction);
				tweenAlpha.easeFromTo(alphaTo, alphaFrom, duration, easingFunction);
			}
		}

		private void ClearTween()
		{
			tweenPosition = null;
			tweenAlpha = null;
			positionFrom = Helpers.staticVector4Zero;
			positionTo = Helpers.staticVector4Zero;
			alphaFrom = Helpers.staticVector4Zero;
			alphaTo = Helpers.staticVector4Zero;
			easingFunction = null;
		}

		private static void GetCurrectVector4(GameObject obj, UIWidget widget, out Vector4 positionData, out Vector4 alphaData)
		{
			positionData.x = 0f;
			positionData.y = 0f;
			positionData.z = 0f;
			positionData.w = 0f;
			alphaData.x = 0f;
			alphaData.y = 0f;
			alphaData.z = 0f;
			alphaData.w = 0f;
			if (widget != null)
			{
				positionData.x = widget.leftAnchor.absolute;
				positionData.y = widget.rightAnchor.absolute;
				positionData.z = widget.bottomAnchor.absolute;
				positionData.w = widget.topAnchor.absolute;
				alphaData.x = widget.alpha;
				alphaData.y = widget.alpha;
				alphaData.z = widget.alpha;
				alphaData.w = widget.alpha;
			}
		}
	}
}
