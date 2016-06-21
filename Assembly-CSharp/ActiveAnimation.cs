using AnimationOrTween;
using System;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("NGUI/Internal/Active Animation")]
public class ActiveAnimation : MonoBehaviour
{
	public static ActiveAnimation current;

	public List<EventDelegate> onFinished = new List<EventDelegate>();

	[HideInInspector]
	public GameObject eventReceiver;

	[HideInInspector]
	public string callWhenFinished;

	private Animation mAnim;

	private Direction mLastDirection;

	private Direction mDisableDirection;

	private bool mNotify;

	private Animator mAnimator;

	private string mClip = string.Empty;

	private float playbackTime
	{
		get
		{
			return Mathf.Clamp01(this.mAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime);
		}
	}

	public bool isPlaying
	{
		get
		{
			if (!(this.mAnim == null))
			{
				foreach (AnimationState animationState in this.mAnim)
				{
					if (this.mAnim.IsPlaying(animationState.name))
					{
						if (this.mLastDirection == Direction.Forward)
						{
							if (animationState.time < animationState.length)
							{
								bool result = true;
								return result;
							}
						}
						else
						{
							if (this.mLastDirection != Direction.Reverse)
							{
								bool result = true;
								return result;
							}
							if (animationState.time > 0f)
							{
								bool result = true;
								return result;
							}
						}
					}
				}
				return false;
			}
			if (this.mAnimator != null)
			{
				if (this.mLastDirection == Direction.Reverse)
				{
					if (this.playbackTime == 0f)
					{
						return false;
					}
				}
				else if (this.playbackTime == 1f)
				{
					return false;
				}
				return true;
			}
			return false;
		}
	}

	public void Finish()
	{
		if (this.mAnim != null)
		{
			foreach (AnimationState animationState in this.mAnim)
			{
				if (this.mLastDirection == Direction.Forward)
				{
					animationState.time = animationState.length;
				}
				else if (this.mLastDirection == Direction.Reverse)
				{
					animationState.time = 0f;
				}
			}
			this.mAnim.Sample();
		}
		else if (this.mAnimator != null)
		{
			this.mAnimator.Play(this.mClip, 0, (this.mLastDirection != Direction.Forward) ? 0f : 1f);
		}
	}

	public void Reset()
	{
		if (this.mAnim != null)
		{
			foreach (AnimationState animationState in this.mAnim)
			{
				if (this.mLastDirection == Direction.Reverse)
				{
					animationState.time = animationState.length;
				}
				else if (this.mLastDirection == Direction.Forward)
				{
					animationState.time = 0f;
				}
			}
		}
		else if (this.mAnimator != null)
		{
			this.mAnimator.Play(this.mClip, 0, (this.mLastDirection != Direction.Reverse) ? 0f : 1f);
		}
	}

	private void Start()
	{
		if (this.eventReceiver != null && EventDelegate.IsValid(this.onFinished))
		{
			this.eventReceiver = null;
			this.callWhenFinished = null;
		}
	}

	private void Update()
	{
		float deltaTime = RealTime.deltaTime;
		if (deltaTime == 0f)
		{
			return;
		}
		if (this.mAnimator != null)
		{
			this.mAnimator.Update((this.mLastDirection != Direction.Reverse) ? deltaTime : (-deltaTime));
			if (this.isPlaying)
			{
				return;
			}
			this.mAnimator.enabled = false;
			base.enabled = false;
		}
		else
		{
			if (!(this.mAnim != null))
			{
				base.enabled = false;
				return;
			}
			bool flag = false;
			foreach (AnimationState animationState in this.mAnim)
			{
				if (this.mAnim.IsPlaying(animationState.name))
				{
					float num = animationState.speed * deltaTime;
					animationState.time += num;
					if (num < 0f)
					{
						if (animationState.time > 0f)
						{
							flag = true;
						}
						else
						{
							animationState.time = 0f;
						}
					}
					else if (animationState.time < animationState.length)
					{
						flag = true;
					}
					else
					{
						animationState.time = animationState.length;
					}
				}
			}
			this.mAnim.Sample();
			if (flag)
			{
				return;
			}
			base.enabled = false;
		}
		if (this.mNotify)
		{
			this.mNotify = false;
			if (ActiveAnimation.current == null)
			{
				ActiveAnimation.current = this;
				EventDelegate.Execute(this.onFinished);
				if (this.eventReceiver != null && !string.IsNullOrEmpty(this.callWhenFinished))
				{
					this.eventReceiver.SendMessage(this.callWhenFinished, SendMessageOptions.DontRequireReceiver);
				}
				ActiveAnimation.current = null;
			}
			if (this.mDisableDirection != Direction.Toggle && this.mLastDirection == this.mDisableDirection)
			{
				NGUITools.SetActive(base.gameObject, false);
			}
		}
	}

	private void Play(string clipName, Direction playDirection)
	{
		if (playDirection == Direction.Toggle)
		{
			playDirection = ((this.mLastDirection == Direction.Forward) ? Direction.Reverse : Direction.Forward);
		}
		if (this.mAnim != null)
		{
			base.enabled = true;
			this.mAnim.enabled = false;
			bool flag = string.IsNullOrEmpty(clipName);
			if (flag)
			{
				if (!this.mAnim.isPlaying)
				{
					this.mAnim.Play();
				}
			}
			else if (!this.mAnim.IsPlaying(clipName))
			{
				this.mAnim.Play(clipName);
			}
			foreach (AnimationState animationState in this.mAnim)
			{
				if (string.IsNullOrEmpty(clipName) || animationState.name == clipName)
				{
					float num = Mathf.Abs(animationState.speed);
					animationState.speed = num * (float)playDirection;
					if (playDirection == Direction.Reverse && animationState.time == 0f)
					{
						animationState.time = animationState.length;
					}
					else if (playDirection == Direction.Forward && animationState.time == animationState.length)
					{
						animationState.time = 0f;
					}
				}
			}
			this.mLastDirection = playDirection;
			this.mNotify = true;
			this.mAnim.Sample();
		}
		else if (this.mAnimator != null)
		{
			if (base.enabled && this.isPlaying && this.mClip == clipName)
			{
				this.mLastDirection = playDirection;
				return;
			}
			base.enabled = true;
			this.mNotify = true;
			this.mLastDirection = playDirection;
			this.mClip = clipName;
			this.mAnimator.Play(this.mClip, 0, (playDirection != Direction.Forward) ? 1f : 0f);
		}
	}

	public static ActiveAnimation Play(Animation anim, string clipName, Direction playDirection, EnableCondition enableBeforePlay, DisableCondition disableCondition)
	{
		if (!NGUITools.GetActive(anim.gameObject))
		{
			if (enableBeforePlay != EnableCondition.EnableThenPlay)
			{
				return null;
			}
			NGUITools.SetActive(anim.gameObject, true);
			UIPanel[] componentsInChildren = anim.gameObject.GetComponentsInChildren<UIPanel>();
			int i = 0;
			int num = componentsInChildren.Length;
			while (i < num)
			{
				componentsInChildren[i].Refresh();
				i++;
			}
		}
		ActiveAnimation activeAnimation = anim.GetComponent<ActiveAnimation>();
		if (activeAnimation == null)
		{
			activeAnimation = anim.gameObject.AddComponent<ActiveAnimation>();
		}
		activeAnimation.mAnim = anim;
		activeAnimation.mDisableDirection = (Direction)disableCondition;
		activeAnimation.onFinished.Clear();
		activeAnimation.Play(clipName, playDirection);
		if (activeAnimation.mAnim != null)
		{
			activeAnimation.mAnim.Sample();
		}
		else if (activeAnimation.mAnimator != null)
		{
			activeAnimation.mAnimator.Update(0f);
		}
		return activeAnimation;
	}

	public static ActiveAnimation Play(Animation anim, string clipName, Direction playDirection)
	{
		return ActiveAnimation.Play(anim, clipName, playDirection, EnableCondition.DoNothing, DisableCondition.DoNotDisable);
	}

	public static ActiveAnimation Play(Animation anim, Direction playDirection)
	{
		return ActiveAnimation.Play(anim, null, playDirection, EnableCondition.DoNothing, DisableCondition.DoNotDisable);
	}

	public static ActiveAnimation Play(Animator anim, string clipName, Direction playDirection, EnableCondition enableBeforePlay, DisableCondition disableCondition)
	{
		if (enableBeforePlay != EnableCondition.IgnoreDisabledState && !NGUITools.GetActive(anim.gameObject))
		{
			if (enableBeforePlay != EnableCondition.EnableThenPlay)
			{
				return null;
			}
			NGUITools.SetActive(anim.gameObject, true);
			UIPanel[] componentsInChildren = anim.gameObject.GetComponentsInChildren<UIPanel>();
			int i = 0;
			int num = componentsInChildren.Length;
			while (i < num)
			{
				componentsInChildren[i].Refresh();
				i++;
			}
		}
		ActiveAnimation activeAnimation = anim.GetComponent<ActiveAnimation>();
		if (activeAnimation == null)
		{
			activeAnimation = anim.gameObject.AddComponent<ActiveAnimation>();
		}
		activeAnimation.mAnimator = anim;
		activeAnimation.mDisableDirection = (Direction)disableCondition;
		activeAnimation.onFinished.Clear();
		activeAnimation.Play(clipName, playDirection);
		if (activeAnimation.mAnim != null)
		{
			activeAnimation.mAnim.Sample();
		}
		else if (activeAnimation.mAnimator != null)
		{
			activeAnimation.mAnimator.Update(0f);
		}
		return activeAnimation;
	}
}
