using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.ScriptControl), HutongGames.PlayMaker.Tooltip("Invokes a Method in a Behaviour attached to a Game Object. See Unity InvokeMethod docs.")]
	public class InvokeMethod : FsmStateAction
	{
		[RequiredField]
		public FsmOwnerDefault gameObject;

		[RequiredField, UIHint(UIHint.Script)]
		public FsmString behaviour;

		[RequiredField, UIHint(UIHint.Method)]
		public FsmString methodName;

		[HasFloatSlider(0f, 10f)]
		public FsmFloat delay;

		public FsmBool repeating;

		[HasFloatSlider(0f, 10f)]
		public FsmFloat repeatDelay;

		public FsmBool cancelOnExit;

		private MonoBehaviour component;

		public override void Reset()
		{
			this.gameObject = null;
			this.behaviour = null;
			this.methodName = string.Empty;
			this.delay = null;
			this.repeating = false;
			this.repeatDelay = 1f;
			this.cancelOnExit = false;
		}

		public override void OnEnter()
		{
			this.DoInvokeMethod(base.Fsm.GetOwnerDefaultTarget(this.gameObject));
			base.Finish();
		}

		private void DoInvokeMethod(GameObject go)
		{
			if (go == null)
			{
				return;
			}
			this.component = (go.GetComponent(this.behaviour.Value) as MonoBehaviour);
			if (this.component == null)
			{
				this.LogWarning("InvokeMethod: " + go.name + " missing behaviour: " + this.behaviour.Value);
				return;
			}
			if (this.repeating.Value)
			{
				this.component.InvokeRepeating(this.methodName.Value, this.delay.Value, this.repeatDelay.Value);
			}
			else
			{
				this.component.Invoke(this.methodName.Value, this.delay.Value);
			}
		}

		public override void OnExit()
		{
			if (this.component == null)
			{
				return;
			}
			if (this.cancelOnExit.Value)
			{
				this.component.CancelInvoke(this.methodName.Value);
			}
		}
	}
}
