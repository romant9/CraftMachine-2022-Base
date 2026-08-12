using System;
using System.Collections.Generic;
using UnityEngine;

public class TweenLetters : UITweener
{
	[DoNotObfuscateNGUI]
	public enum AnimationLetterOrder
	{
		Forward = 0,
		Reverse = 1,
		Random = 2
	}

	private struct LetterProperties
	{
		public float start;

		public float duration;

		public Vector3 pos;

		public Quaternion rot;

		public Vector3 scale;
	}

	[Serializable]
	public class AnimationProperties
	{
		[Tooltip("If set, overrides the tween's animation duration")]
		public float duration;

		public AnimationLetterOrder animationOrder = AnimationLetterOrder.Random;

		[Range(0f, 1f)]
		public float overlap = 0.5f;

		[Tooltip("If set, each letter will animate with a random duration")]
		public bool randomDurations;

		[MinMaxRange(0f, 1f)]
		public Vector2 randomness = new Vector2(0.25f, 0.75f);

		[HideInInspector]
		public bool upgraded;

		[HideInInspector]
		public Vector2 offsetRange = Vector2.zero;

		[HideInInspector]
		public Vector3 pos = Vector3.zero;

		[HideInInspector]
		public Vector3 rot = Vector3.zero;

		[HideInInspector]
		public Vector3 scale = Vector3.one;

		public Vector3 pos1 = Vector3.zero;

		public Vector3 pos2 = Vector3.zero;

		public Vector3 rot1 = Vector3.zero;

		public Vector3 rot2 = Vector3.zero;

		public Vector3 scale1 = Vector3.one;

		public Vector3 scale2 = Vector3.one;

		[Range(0f, 1f)]
		[Tooltip("Starting or finishing alpha")]
		public float alpha;

		public void Upgrade()
		{
			upgraded = true;
			pos1 = pos - new Vector3(offsetRange.x, offsetRange.y, 0f);
			pos2 = pos + new Vector3(offsetRange.x, offsetRange.y, 0f);
			rot1 = rot;
			rot2 = rot;
			scale1 = scale;
			scale2 = scale;
		}
	}

	public AnimationProperties hoverOver = new AnimationProperties();

	public AnimationProperties hoverOut = new AnimationProperties();

	private UILabel mLabel;

	private int mVertexCount = -1;

	private int[] mLetterOrder;

	private LetterProperties[] mLetter;

	private AnimationProperties mCurrent;

	protected void OnValidate()
	{
		if (hoverOver != null && !hoverOver.upgraded)
		{
			hoverOver.Upgrade();
			NGUITools.SetDirty(this, "Upgraded TweenLetters");
		}
		if (hoverOut != null && !hoverOut.upgraded)
		{
			hoverOut.Upgrade();
			NGUITools.SetDirty(this, "Upgraded TweenLetters");
		}
	}

	private void OnEnable()
	{
		mVertexCount = -1;
		mLabel = GetComponent<UILabel>();
		UILabel uILabel = mLabel;
		uILabel.onPostFill = (UIWidget.OnPostFillCallback)Delegate.Combine(uILabel.onPostFill, new UIWidget.OnPostFillCallback(OnPostFill));
		mCurrent = hoverOver;
	}

	protected override void OnDisable()
	{
		base.OnDisable();
		UILabel uILabel = mLabel;
		uILabel.onPostFill = (UIWidget.OnPostFillCallback)Delegate.Remove(uILabel.onPostFill, new UIWidget.OnPostFillCallback(OnPostFill));
	}

	public override void Play(bool forward)
	{
		base.enabled = true;
		mCurrent = (forward ? hoverOver : hoverOut);
		if (mCurrent.duration != 0f)
		{
			duration = mCurrent.duration;
		}
		base.Play(forward);
	}

	private void OnPostFill(UIWidget widget, int bufferOffset, List<Vector3> verts, List<Vector2> uvs, List<Color> cols)
	{
		if (verts == null)
		{
			return;
		}
		int count = verts.Count;
		if (verts == null || count == 0 || mLabel == null)
		{
			return;
		}
		try
		{
			int quadsPerCharacter = mLabel.quadsPerCharacter;
			int num = count / quadsPerCharacter / 4;
			_ = mLabel.printedText;
			if (mVertexCount != count)
			{
				mVertexCount = count;
				SetLetterOrder(num);
				GetLetterDuration(num);
			}
			Matrix4x4 identity = Matrix4x4.identity;
			Vector3 zero = Vector3.zero;
			Quaternion identity2 = Quaternion.identity;
			Vector3 one = Vector3.one;
			float num2 = 1f;
			Vector3 zero2 = Vector3.zero;
			Vector3 zero3 = Vector3.zero;
			Color clear = Color.clear;
			float num3 = base.tweenFactor * duration;
			for (int i = 0; i < quadsPerCharacter; i++)
			{
				for (int j = 0; j < num; j++)
				{
					int num4 = mLetterOrder[j];
					int num5 = i * num * 4 + num4 * 4;
					if (num5 < count)
					{
						float start = mLetter[num4].start;
						float time = Mathf.Clamp(num3 - start, 0f, mLetter[num4].duration) / mLetter[num4].duration;
						time = animationCurve.Evaluate(time);
						zero2 = GetCenter(verts, num5, 4);
						zero = Vector3.LerpUnclamped(mLetter[num4].pos, Vector3.zero, time);
						identity2 = Quaternion.SlerpUnclamped(mLetter[num4].rot, Quaternion.identity, time);
						one = Vector3.LerpUnclamped(mLetter[num4].scale, Vector3.one, time);
						num2 = Mathf.LerpUnclamped(mCurrent.alpha, 1f, time);
						identity.SetTRS(zero, identity2, one);
						for (int k = num5; k < num5 + 4; k++)
						{
							zero3 = verts[k];
							zero3 -= zero2;
							zero3 = identity.MultiplyPoint3x4(zero3);
							zero3 += zero2;
							verts[k] = zero3;
							clear = cols[k];
							clear.a *= num2;
							cols[k] = clear;
						}
					}
				}
			}
		}
		catch (Exception)
		{
			base.enabled = false;
		}
	}

	protected override void OnUpdate(float factor, bool isFinished)
	{
		if ((bool)mLabel)
		{
			mLabel.enabled = !isFinished || mCurrent == null || mCurrent != hoverOut || mCurrent.alpha != 0f;
			mLabel.MarkAsChanged();
		}
	}

	private void SetLetterOrder(int letterCount)
	{
		if (letterCount == 0)
		{
			mLetter = null;
			mLetterOrder = null;
			return;
		}
		mLetterOrder = new int[letterCount];
		mLetter = new LetterProperties[letterCount];
		for (int i = 0; i < letterCount; i++)
		{
			mLetterOrder[i] = ((mCurrent.animationOrder == AnimationLetterOrder.Reverse) ? (letterCount - 1 - i) : i);
			LetterProperties letterProperties = default(LetterProperties);
			letterProperties.pos = new Vector3(UnityEngine.Random.Range(mCurrent.pos1.x, mCurrent.pos2.x), UnityEngine.Random.Range(mCurrent.pos1.y, mCurrent.pos2.y), UnityEngine.Random.Range(mCurrent.pos1.z, mCurrent.pos2.z));
			letterProperties.rot = Quaternion.Euler(new Vector3(UnityEngine.Random.Range(mCurrent.rot1.x, mCurrent.rot2.x), UnityEngine.Random.Range(mCurrent.rot1.y, mCurrent.rot2.y), UnityEngine.Random.Range(mCurrent.rot1.z, mCurrent.rot2.z)));
			letterProperties.scale = new Vector3(UnityEngine.Random.Range(mCurrent.scale1.x, mCurrent.scale2.x), UnityEngine.Random.Range(mCurrent.scale1.y, mCurrent.scale2.y), UnityEngine.Random.Range(mCurrent.scale1.z, mCurrent.scale2.z));
			mLetter[mLetterOrder[i]] = letterProperties;
		}
		if (mCurrent.animationOrder == AnimationLetterOrder.Random)
		{
			System.Random random = new System.Random();
			int num = letterCount;
			while (num > 1)
			{
				int num2 = random.Next(--num + 1);
				int num3 = mLetterOrder[num2];
				mLetterOrder[num2] = mLetterOrder[num];
				mLetterOrder[num] = num3;
			}
		}
	}

	private void GetLetterDuration(int letterCount)
	{
		if (mCurrent.randomDurations)
		{
			for (int i = 0; i < mLetter.Length; i++)
			{
				mLetter[i].start = UnityEngine.Random.Range(0f, mCurrent.randomness.x * duration);
				float num = UnityEngine.Random.Range(mCurrent.randomness.y * duration, duration);
				mLetter[i].duration = num - mLetter[i].start;
			}
			return;
		}
		float num2 = duration / (float)letterCount;
		float num3 = 1f - mCurrent.overlap;
		float num4 = num2 * (float)letterCount * num3;
		float num5 = ScaleRange(num2, num4 + num2 * mCurrent.overlap, duration);
		float num6 = 0f;
		for (int j = 0; j < mLetter.Length; j++)
		{
			int num7 = mLetterOrder[j];
			mLetter[num7].start = num6;
			mLetter[num7].duration = num5;
			num6 += mLetter[num7].duration * num3;
		}
	}

	private float ScaleRange(float value, float baseMax, float limitMax)
	{
		return limitMax * value / baseMax;
	}

	private static Vector3 GetCenter(List<Vector3> verts, int firstVert, int length)
	{
		Vector3 vector = verts[firstVert];
		for (int i = firstVert + 1; i < firstVert + length; i++)
		{
			vector += verts[i];
		}
		return vector / length;
	}
}
