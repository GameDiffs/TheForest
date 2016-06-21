using System;
using System.Reflection;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.ScriptControl)]
	public class CallMethod : FsmStateAction
	{
		[ObjectType(typeof(MonoBehaviour)), HutongGames.PlayMaker.Tooltip("Store the component in an Object variable.\nNOTE: Set theObject variable's Object Type to get a component of that type. E.g., set Object Type to UnityEngine.AudioListener to get the AudioListener component on the camera.")]
		public FsmObject behaviour;

		[HutongGames.PlayMaker.Tooltip("Name of the method to call on the component")]
		public FsmString methodName;

		[HutongGames.PlayMaker.Tooltip("Method paramters. NOTE: these must match the method's signature!")]
		public FsmVar[] parameters;

		[ActionSection("Store Result"), HutongGames.PlayMaker.Tooltip("Store the result of the method call."), UIHint(UIHint.Variable)]
		public FsmVar storeResult;

		[HutongGames.PlayMaker.Tooltip("Repeat every frame.")]
		public bool everyFrame;

		private UnityEngine.Object cachedBehaviour;

		private Type cachedType;

		private MethodInfo cachedMethodInfo;

		private ParameterInfo[] cachedParameterInfo;

		private object[] parametersArray;

		private string errorString;

		public override void OnEnter()
		{
			this.parametersArray = new object[this.parameters.Length];
			this.DoMethodCall();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		public override void OnUpdate()
		{
			this.DoMethodCall();
		}

		private void DoMethodCall()
		{
			if (this.behaviour.Value == null)
			{
				base.Finish();
				return;
			}
			if (this.cachedBehaviour != this.behaviour.Value)
			{
				this.errorString = string.Empty;
				if (!this.DoCache())
				{
					Debug.LogError(this.errorString);
					base.Finish();
					return;
				}
			}
			object value;
			if (this.cachedParameterInfo.Length == 0)
			{
				value = this.cachedMethodInfo.Invoke(this.cachedBehaviour, null);
			}
			else
			{
				for (int i = 0; i < this.parameters.Length; i++)
				{
					FsmVar fsmVar = this.parameters[i];
					fsmVar.UpdateValue();
					this.parametersArray[i] = fsmVar.GetValue();
				}
				value = this.cachedMethodInfo.Invoke(this.cachedBehaviour, this.parametersArray);
			}
			this.storeResult.SetValue(value);
		}

		private bool DoCache()
		{
			this.cachedBehaviour = (this.behaviour.Value as MonoBehaviour);
			if (this.cachedBehaviour == null)
			{
				this.errorString += "Behaviour is invalid!\n";
				base.Finish();
				return false;
			}
			this.cachedType = this.behaviour.Value.GetType();
			this.cachedMethodInfo = this.cachedType.GetMethod(this.methodName.Value);
			if (this.cachedMethodInfo == null)
			{
				this.errorString = this.errorString + "Method Name is invalid: " + this.methodName.Value + "\n";
				base.Finish();
				return false;
			}
			this.cachedParameterInfo = this.cachedMethodInfo.GetParameters();
			return true;
		}

		public override string ErrorCheck()
		{
			this.errorString = string.Empty;
			this.DoCache();
			if (!string.IsNullOrEmpty(this.errorString))
			{
				return this.errorString;
			}
			if (this.parameters.Length != this.cachedParameterInfo.Length)
			{
				return string.Concat(new object[]
				{
					"Parameter count does not match method.\nMethod has ",
					this.cachedParameterInfo.Length,
					" parameters.\nYou specified ",
					this.parameters.Length,
					" paramaters."
				});
			}
			for (int i = 0; i < this.parameters.Length; i++)
			{
				FsmVar fsmVar = this.parameters[i];
				Type realType = fsmVar.RealType;
				Type parameterType = this.cachedParameterInfo[i].ParameterType;
				if (!object.ReferenceEquals(realType, parameterType))
				{
					return string.Concat(new object[]
					{
						"Parameters do not match method signature.\nParameter ",
						i + 1,
						" (",
						realType,
						") should be of type: ",
						parameterType
					});
				}
			}
			if (object.ReferenceEquals(this.cachedMethodInfo.ReturnType, typeof(void)))
			{
				if (!string.IsNullOrEmpty(this.storeResult.variableName))
				{
					return "Method does not have return.\nSpecify 'none' in Store Result.";
				}
			}
			else if (!object.ReferenceEquals(this.cachedMethodInfo.ReturnType, this.storeResult.RealType))
			{
				return "Store Result is of the wrong type.\nIt should be of type: " + this.cachedMethodInfo.ReturnType;
			}
			return string.Empty;
		}
	}
}
