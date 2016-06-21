using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.ScriptControl), HutongGames.PlayMaker.Tooltip("Start a Coroutine in a Behaviour on a Game Object. See Unity StartCoroutine docs.")]
	public class StartCoroutine : FsmStateAction
	{
		[RequiredField, HutongGames.PlayMaker.Tooltip("The game object that owns the Behaviour.")]
		public FsmOwnerDefault gameObject;

		[RequiredField, HutongGames.PlayMaker.Tooltip("The Behaviour that contains the method to start as a coroutine."), UIHint(UIHint.Behaviour)]
		public FsmString behaviour;

		[RequiredField, HutongGames.PlayMaker.Tooltip("The name of the coroutine method."), UIHint(UIHint.Coroutine)]
		public FunctionCall functionCall;

		[HutongGames.PlayMaker.Tooltip("Stop the coroutine when the state is exited.")]
		public bool stopOnExit;

		private MonoBehaviour component;

		public override void Reset()
		{
			this.gameObject = null;
			this.behaviour = null;
			this.functionCall = null;
			this.stopOnExit = false;
		}

		public override void OnEnter()
		{
			this.DoStartCoroutine();
			base.Finish();
		}

		private void DoStartCoroutine()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (ownerDefaultTarget == null)
			{
				return;
			}
			this.component = (ownerDefaultTarget.GetComponent(this.behaviour.Value) as MonoBehaviour);
			if (this.component == null)
			{
				this.LogWarning("StartCoroutine: " + ownerDefaultTarget.name + " missing behaviour: " + this.behaviour.Value);
				return;
			}
			if (!ownerDefaultTarget.activeSelf)
			{
				return;
			}
			string parameterType = this.functionCall.ParameterType;
			switch (parameterType)
			{
			case "None":
				this.component.StartCoroutine(this.functionCall.FunctionName);
				return;
			case "int":
				this.component.StartCoroutine(this.functionCall.FunctionName, this.functionCall.IntParameter.Value);
				return;
			case "float":
				this.component.StartCoroutine(this.functionCall.FunctionName, this.functionCall.FloatParameter.Value);
				return;
			case "string":
				this.component.StartCoroutine(this.functionCall.FunctionName, this.functionCall.StringParameter.Value);
				return;
			case "bool":
				this.component.StartCoroutine(this.functionCall.FunctionName, this.functionCall.BoolParameter.Value);
				return;
			case "Vector2":
				this.component.StartCoroutine(this.functionCall.FunctionName, this.functionCall.Vector2Parameter.Value);
				return;
			case "Vector3":
				this.component.StartCoroutine(this.functionCall.FunctionName, this.functionCall.Vector3Parameter.Value);
				return;
			case "Rect":
				this.component.StartCoroutine(this.functionCall.FunctionName, this.functionCall.RectParamater.Value);
				return;
			case "GameObject":
				this.component.StartCoroutine(this.functionCall.FunctionName, this.functionCall.GameObjectParameter.Value);
				return;
			case "Material":
				this.component.StartCoroutine(this.functionCall.FunctionName, this.functionCall.MaterialParameter.Value);
				break;
			case "Texture":
				this.component.StartCoroutine(this.functionCall.FunctionName, this.functionCall.TextureParameter.Value);
				break;
			case "Quaternion":
				this.component.StartCoroutine(this.functionCall.FunctionName, this.functionCall.QuaternionParameter.Value);
				break;
			case "Object":
				this.component.StartCoroutine(this.functionCall.FunctionName, this.functionCall.ObjectParameter.Value);
				return;
			}
		}

		public override void OnExit()
		{
			if (this.component == null)
			{
				return;
			}
			if (this.stopOnExit)
			{
				this.component.StopCoroutine(this.functionCall.FunctionName);
			}
		}
	}
}
