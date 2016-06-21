using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[HutongGames.PlayMaker.Tooltip("Ease base action - don't use!")]
	public abstract class EaseFsmAction : FsmStateAction
	{
		public enum EaseType
		{
			easeInQuad,
			easeOutQuad,
			easeInOutQuad,
			easeInCubic,
			easeOutCubic,
			easeInOutCubic,
			easeInQuart,
			easeOutQuart,
			easeInOutQuart,
			easeInQuint,
			easeOutQuint,
			easeInOutQuint,
			easeInSine,
			easeOutSine,
			easeInOutSine,
			easeInExpo,
			easeOutExpo,
			easeInOutExpo,
			easeInCirc,
			easeOutCirc,
			easeInOutCirc,
			linear,
			spring,
			bounce,
			easeInBack,
			easeOutBack,
			easeInOutBack,
			elastic,
			punch
		}

		protected delegate float EasingFunction(float start, float end, float value);

		[RequiredField]
		public FsmFloat time;

		public FsmFloat speed;

		public FsmFloat delay;

		public EaseFsmAction.EaseType easeType = EaseFsmAction.EaseType.linear;

		public FsmBool reverse;

		[HutongGames.PlayMaker.Tooltip("Optionally send an Event when the animation finishes.")]
		public FsmEvent finishEvent;

		[HutongGames.PlayMaker.Tooltip("Ignore TimeScale. Useful if the game is paused.")]
		public bool realTime;

		protected EaseFsmAction.EasingFunction ease;

		protected float runningTime;

		protected float lastTime;

		protected float startTime;

		protected float deltaTime;

		protected float delayTime;

		protected float percentage;

		protected float[] fromFloats = new float[0];

		protected float[] toFloats = new float[0];

		protected float[] resultFloats = new float[0];

		protected bool finishAction;

		protected bool start;

		protected bool finished;

		protected bool isRunning;

		public override void Reset()
		{
			this.easeType = EaseFsmAction.EaseType.linear;
			this.time = new FsmFloat
			{
				Value = 1f
			};
			this.delay = new FsmFloat
			{
				UseVariable = true
			};
			this.speed = new FsmFloat
			{
				UseVariable = true
			};
			this.reverse = new FsmBool
			{
				Value = false
			};
			this.realTime = false;
			this.finishEvent = null;
			this.ease = null;
			this.runningTime = 0f;
			this.lastTime = 0f;
			this.percentage = 0f;
			this.fromFloats = new float[0];
			this.toFloats = new float[0];
			this.resultFloats = new float[0];
			this.finishAction = false;
			this.start = false;
			this.finished = false;
			this.isRunning = false;
		}

		public override void OnEnter()
		{
			this.finished = false;
			this.isRunning = false;
			this.SetEasingFunction();
			this.runningTime = 0f;
			this.percentage = ((!this.reverse.IsNone) ? ((!this.reverse.Value) ? 0f : 1f) : 0f);
			this.finishAction = false;
			this.startTime = FsmTime.RealtimeSinceStartup;
			this.lastTime = FsmTime.RealtimeSinceStartup - this.startTime;
			this.delayTime = ((!this.delay.IsNone) ? (this.delayTime = this.delay.Value) : 0f);
			this.start = true;
		}

		public override void OnExit()
		{
		}

		public override void OnUpdate()
		{
			if (this.start && !this.isRunning)
			{
				if (this.delayTime >= 0f)
				{
					if (this.realTime)
					{
						this.deltaTime = FsmTime.RealtimeSinceStartup - this.startTime - this.lastTime;
						this.lastTime = FsmTime.RealtimeSinceStartup - this.startTime;
						this.delayTime -= this.deltaTime;
					}
					else
					{
						this.delayTime -= Time.deltaTime;
					}
				}
				else
				{
					this.isRunning = true;
					this.start = false;
					this.startTime = FsmTime.RealtimeSinceStartup;
					this.lastTime = FsmTime.RealtimeSinceStartup - this.startTime;
				}
			}
			if (this.isRunning && !this.finished)
			{
				if (this.reverse.IsNone || !this.reverse.Value)
				{
					this.UpdatePercentage();
					if (this.percentage < 1f)
					{
						for (int i = 0; i < this.fromFloats.Length; i++)
						{
							this.resultFloats[i] = this.ease(this.fromFloats[i], this.toFloats[i], this.percentage);
						}
					}
					else
					{
						this.finishAction = true;
						this.finished = true;
						this.isRunning = false;
					}
				}
				else
				{
					this.UpdatePercentage();
					if (this.percentage > 0f)
					{
						for (int j = 0; j < this.fromFloats.Length; j++)
						{
							this.resultFloats[j] = this.ease(this.fromFloats[j], this.toFloats[j], this.percentage);
						}
					}
					else
					{
						this.finishAction = true;
						this.finished = true;
						this.isRunning = false;
					}
				}
			}
		}

		protected void UpdatePercentage()
		{
			if (this.realTime)
			{
				this.deltaTime = FsmTime.RealtimeSinceStartup - this.startTime - this.lastTime;
				this.lastTime = FsmTime.RealtimeSinceStartup - this.startTime;
				if (!this.speed.IsNone)
				{
					this.runningTime += this.deltaTime * this.speed.Value;
				}
				else
				{
					this.runningTime += this.deltaTime;
				}
			}
			else if (!this.speed.IsNone)
			{
				this.runningTime += Time.deltaTime * this.speed.Value;
			}
			else
			{
				this.runningTime += Time.deltaTime;
			}
			if (!this.reverse.IsNone && this.reverse.Value)
			{
				this.percentage = 1f - this.runningTime / this.time.Value;
			}
			else
			{
				this.percentage = this.runningTime / this.time.Value;
			}
		}

		protected void SetEasingFunction()
		{
			switch (this.easeType)
			{
			case EaseFsmAction.EaseType.easeInQuad:
				this.ease = new EaseFsmAction.EasingFunction(this.easeInQuad);
				break;
			case EaseFsmAction.EaseType.easeOutQuad:
				this.ease = new EaseFsmAction.EasingFunction(this.easeOutQuad);
				break;
			case EaseFsmAction.EaseType.easeInOutQuad:
				this.ease = new EaseFsmAction.EasingFunction(this.easeInOutQuad);
				break;
			case EaseFsmAction.EaseType.easeInCubic:
				this.ease = new EaseFsmAction.EasingFunction(this.easeInCubic);
				break;
			case EaseFsmAction.EaseType.easeOutCubic:
				this.ease = new EaseFsmAction.EasingFunction(this.easeOutCubic);
				break;
			case EaseFsmAction.EaseType.easeInOutCubic:
				this.ease = new EaseFsmAction.EasingFunction(this.easeInOutCubic);
				break;
			case EaseFsmAction.EaseType.easeInQuart:
				this.ease = new EaseFsmAction.EasingFunction(this.easeInQuart);
				break;
			case EaseFsmAction.EaseType.easeOutQuart:
				this.ease = new EaseFsmAction.EasingFunction(this.easeOutQuart);
				break;
			case EaseFsmAction.EaseType.easeInOutQuart:
				this.ease = new EaseFsmAction.EasingFunction(this.easeInOutQuart);
				break;
			case EaseFsmAction.EaseType.easeInQuint:
				this.ease = new EaseFsmAction.EasingFunction(this.easeInQuint);
				break;
			case EaseFsmAction.EaseType.easeOutQuint:
				this.ease = new EaseFsmAction.EasingFunction(this.easeOutQuint);
				break;
			case EaseFsmAction.EaseType.easeInOutQuint:
				this.ease = new EaseFsmAction.EasingFunction(this.easeInOutQuint);
				break;
			case EaseFsmAction.EaseType.easeInSine:
				this.ease = new EaseFsmAction.EasingFunction(this.easeInSine);
				break;
			case EaseFsmAction.EaseType.easeOutSine:
				this.ease = new EaseFsmAction.EasingFunction(this.easeOutSine);
				break;
			case EaseFsmAction.EaseType.easeInOutSine:
				this.ease = new EaseFsmAction.EasingFunction(this.easeInOutSine);
				break;
			case EaseFsmAction.EaseType.easeInExpo:
				this.ease = new EaseFsmAction.EasingFunction(this.easeInExpo);
				break;
			case EaseFsmAction.EaseType.easeOutExpo:
				this.ease = new EaseFsmAction.EasingFunction(this.easeOutExpo);
				break;
			case EaseFsmAction.EaseType.easeInOutExpo:
				this.ease = new EaseFsmAction.EasingFunction(this.easeInOutExpo);
				break;
			case EaseFsmAction.EaseType.easeInCirc:
				this.ease = new EaseFsmAction.EasingFunction(this.easeInCirc);
				break;
			case EaseFsmAction.EaseType.easeOutCirc:
				this.ease = new EaseFsmAction.EasingFunction(this.easeOutCirc);
				break;
			case EaseFsmAction.EaseType.easeInOutCirc:
				this.ease = new EaseFsmAction.EasingFunction(this.easeInOutCirc);
				break;
			case EaseFsmAction.EaseType.linear:
				this.ease = new EaseFsmAction.EasingFunction(this.linear);
				break;
			case EaseFsmAction.EaseType.spring:
				this.ease = new EaseFsmAction.EasingFunction(this.spring);
				break;
			case EaseFsmAction.EaseType.bounce:
				this.ease = new EaseFsmAction.EasingFunction(this.bounce);
				break;
			case EaseFsmAction.EaseType.easeInBack:
				this.ease = new EaseFsmAction.EasingFunction(this.easeInBack);
				break;
			case EaseFsmAction.EaseType.easeOutBack:
				this.ease = new EaseFsmAction.EasingFunction(this.easeOutBack);
				break;
			case EaseFsmAction.EaseType.easeInOutBack:
				this.ease = new EaseFsmAction.EasingFunction(this.easeInOutBack);
				break;
			case EaseFsmAction.EaseType.elastic:
				this.ease = new EaseFsmAction.EasingFunction(this.elastic);
				break;
			}
		}

		protected float linear(float start, float end, float value)
		{
			return Mathf.Lerp(start, end, value);
		}

		protected float clerp(float start, float end, float value)
		{
			float num = 0f;
			float num2 = 360f;
			float num3 = Mathf.Abs((num2 - num) / 2f);
			float result;
			if (end - start < -num3)
			{
				float num4 = (num2 - start + end) * value;
				result = start + num4;
			}
			else if (end - start > num3)
			{
				float num4 = -(num2 - end + start) * value;
				result = start + num4;
			}
			else
			{
				result = start + (end - start) * value;
			}
			return result;
		}

		protected float spring(float start, float end, float value)
		{
			value = Mathf.Clamp01(value);
			value = (Mathf.Sin(value * 3.14159274f * (0.2f + 2.5f * value * value * value)) * Mathf.Pow(1f - value, 2.2f) + value) * (1f + 1.2f * (1f - value));
			return start + (end - start) * value;
		}

		protected float easeInQuad(float start, float end, float value)
		{
			end -= start;
			return end * value * value + start;
		}

		protected float easeOutQuad(float start, float end, float value)
		{
			end -= start;
			return -end * value * (value - 2f) + start;
		}

		protected float easeInOutQuad(float start, float end, float value)
		{
			value /= 0.5f;
			end -= start;
			if (value < 1f)
			{
				return end / 2f * value * value + start;
			}
			value -= 1f;
			return -end / 2f * (value * (value - 2f) - 1f) + start;
		}

		protected float easeInCubic(float start, float end, float value)
		{
			end -= start;
			return end * value * value * value + start;
		}

		protected float easeOutCubic(float start, float end, float value)
		{
			value -= 1f;
			end -= start;
			return end * (value * value * value + 1f) + start;
		}

		protected float easeInOutCubic(float start, float end, float value)
		{
			value /= 0.5f;
			end -= start;
			if (value < 1f)
			{
				return end / 2f * value * value * value + start;
			}
			value -= 2f;
			return end / 2f * (value * value * value + 2f) + start;
		}

		protected float easeInQuart(float start, float end, float value)
		{
			end -= start;
			return end * value * value * value * value + start;
		}

		protected float easeOutQuart(float start, float end, float value)
		{
			value -= 1f;
			end -= start;
			return -end * (value * value * value * value - 1f) + start;
		}

		protected float easeInOutQuart(float start, float end, float value)
		{
			value /= 0.5f;
			end -= start;
			if (value < 1f)
			{
				return end / 2f * value * value * value * value + start;
			}
			value -= 2f;
			return -end / 2f * (value * value * value * value - 2f) + start;
		}

		protected float easeInQuint(float start, float end, float value)
		{
			end -= start;
			return end * value * value * value * value * value + start;
		}

		protected float easeOutQuint(float start, float end, float value)
		{
			value -= 1f;
			end -= start;
			return end * (value * value * value * value * value + 1f) + start;
		}

		protected float easeInOutQuint(float start, float end, float value)
		{
			value /= 0.5f;
			end -= start;
			if (value < 1f)
			{
				return end / 2f * value * value * value * value * value + start;
			}
			value -= 2f;
			return end / 2f * (value * value * value * value * value + 2f) + start;
		}

		protected float easeInSine(float start, float end, float value)
		{
			end -= start;
			return -end * Mathf.Cos(value / 1f * 1.57079637f) + end + start;
		}

		protected float easeOutSine(float start, float end, float value)
		{
			end -= start;
			return end * Mathf.Sin(value / 1f * 1.57079637f) + start;
		}

		protected float easeInOutSine(float start, float end, float value)
		{
			end -= start;
			return -end / 2f * (Mathf.Cos(3.14159274f * value / 1f) - 1f) + start;
		}

		protected float easeInExpo(float start, float end, float value)
		{
			end -= start;
			return end * Mathf.Pow(2f, 10f * (value / 1f - 1f)) + start;
		}

		protected float easeOutExpo(float start, float end, float value)
		{
			end -= start;
			return end * (-Mathf.Pow(2f, -10f * value / 1f) + 1f) + start;
		}

		protected float easeInOutExpo(float start, float end, float value)
		{
			value /= 0.5f;
			end -= start;
			if (value < 1f)
			{
				return end / 2f * Mathf.Pow(2f, 10f * (value - 1f)) + start;
			}
			value -= 1f;
			return end / 2f * (-Mathf.Pow(2f, -10f * value) + 2f) + start;
		}

		protected float easeInCirc(float start, float end, float value)
		{
			end -= start;
			return -end * (Mathf.Sqrt(1f - value * value) - 1f) + start;
		}

		protected float easeOutCirc(float start, float end, float value)
		{
			value -= 1f;
			end -= start;
			return end * Mathf.Sqrt(1f - value * value) + start;
		}

		protected float easeInOutCirc(float start, float end, float value)
		{
			value /= 0.5f;
			end -= start;
			if (value < 1f)
			{
				return -end / 2f * (Mathf.Sqrt(1f - value * value) - 1f) + start;
			}
			value -= 2f;
			return end / 2f * (Mathf.Sqrt(1f - value * value) + 1f) + start;
		}

		protected float bounce(float start, float end, float value)
		{
			value /= 1f;
			end -= start;
			if (value < 0.363636374f)
			{
				return end * (7.5625f * value * value) + start;
			}
			if (value < 0.727272749f)
			{
				value -= 0.545454562f;
				return end * (7.5625f * value * value + 0.75f) + start;
			}
			if ((double)value < 0.90909090909090906)
			{
				value -= 0.8181818f;
				return end * (7.5625f * value * value + 0.9375f) + start;
			}
			value -= 0.954545438f;
			return end * (7.5625f * value * value + 0.984375f) + start;
		}

		protected float easeInBack(float start, float end, float value)
		{
			end -= start;
			value /= 1f;
			float num = 1.70158f;
			return end * value * value * ((num + 1f) * value - num) + start;
		}

		protected float easeOutBack(float start, float end, float value)
		{
			float num = 1.70158f;
			end -= start;
			value = value / 1f - 1f;
			return end * (value * value * ((num + 1f) * value + num) + 1f) + start;
		}

		protected float easeInOutBack(float start, float end, float value)
		{
			float num = 1.70158f;
			end -= start;
			value /= 0.5f;
			if (value < 1f)
			{
				num *= 1.525f;
				return end / 2f * (value * value * ((num + 1f) * value - num)) + start;
			}
			value -= 2f;
			num *= 1.525f;
			return end / 2f * (value * value * ((num + 1f) * value + num) + 2f) + start;
		}

		protected float punch(float amplitude, float value)
		{
			if (value == 0f)
			{
				return 0f;
			}
			if (value == 1f)
			{
				return 0f;
			}
			float num = 0.3f;
			float num2 = num / 6.28318548f * Mathf.Asin(0f);
			return amplitude * Mathf.Pow(2f, -10f * value) * Mathf.Sin((value * 1f - num2) * 6.28318548f / num);
		}

		protected float elastic(float start, float end, float value)
		{
			end -= start;
			float num = 1f;
			float num2 = num * 0.3f;
			float num3 = 0f;
			if (value == 0f)
			{
				return start;
			}
			if ((value /= num) == 1f)
			{
				return start + end;
			}
			float num4;
			if (num3 == 0f || num3 < Mathf.Abs(end))
			{
				num3 = end;
				num4 = num2 / 4f;
			}
			else
			{
				num4 = num2 / 6.28318548f * Mathf.Asin(end / num3);
			}
			return num3 * Mathf.Pow(2f, -10f * value) * Mathf.Sin((value * num - num4) * 6.28318548f / num2) + end + start;
		}
	}
}
