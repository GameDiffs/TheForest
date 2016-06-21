using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.ScriptControl), HutongGames.PlayMaker.Tooltip("Sends a Message to a Game Object. See Unity docs for SendMessage.")]
	public class SendMessage : FsmStateAction
	{
		public enum MessageType
		{
			SendMessage,
			SendMessageUpwards,
			BroadcastMessage
		}

		[RequiredField, HutongGames.PlayMaker.Tooltip("GameObject that sends the message.")]
		public FsmOwnerDefault gameObject;

		[HutongGames.PlayMaker.Tooltip("Where to send the message.\nSee Unity docs.")]
		public SendMessage.MessageType delivery;

		[HutongGames.PlayMaker.Tooltip("Send options.\nSee Unity docs.")]
		public SendMessageOptions options;

		[RequiredField]
		public FunctionCall functionCall;

		public override void Reset()
		{
			this.gameObject = null;
			this.delivery = SendMessage.MessageType.SendMessage;
			this.options = SendMessageOptions.DontRequireReceiver;
			this.functionCall = null;
		}

		public override void OnEnter()
		{
			this.DoSendMessage();
			base.Finish();
		}

		private void DoSendMessage()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (ownerDefaultTarget == null)
			{
				return;
			}
			object obj = null;
			string parameterType = this.functionCall.ParameterType;
			switch (parameterType)
			{
			case "bool":
				obj = this.functionCall.BoolParameter.Value;
				break;
			case "int":
				obj = this.functionCall.IntParameter.Value;
				break;
			case "float":
				obj = this.functionCall.FloatParameter.Value;
				break;
			case "string":
				obj = this.functionCall.StringParameter.Value;
				break;
			case "Vector2":
				obj = this.functionCall.Vector2Parameter.Value;
				break;
			case "Vector3":
				obj = this.functionCall.Vector3Parameter.Value;
				break;
			case "Rect":
				obj = this.functionCall.RectParamater.Value;
				break;
			case "GameObject":
				obj = this.functionCall.GameObjectParameter.Value;
				break;
			case "Material":
				obj = this.functionCall.MaterialParameter.Value;
				break;
			case "Texture":
				obj = this.functionCall.TextureParameter.Value;
				break;
			case "Color":
				obj = this.functionCall.ColorParameter.Value;
				break;
			case "Quaternion":
				obj = this.functionCall.QuaternionParameter.Value;
				break;
			case "Object":
				obj = this.functionCall.ObjectParameter.Value;
				break;
			}
			switch (this.delivery)
			{
			case SendMessage.MessageType.SendMessage:
				ownerDefaultTarget.SendMessage(this.functionCall.FunctionName, obj, this.options);
				return;
			case SendMessage.MessageType.SendMessageUpwards:
				ownerDefaultTarget.SendMessageUpwards(this.functionCall.FunctionName, obj, this.options);
				return;
			case SendMessage.MessageType.BroadcastMessage:
				ownerDefaultTarget.BroadcastMessage(this.functionCall.FunctionName, obj, this.options);
				return;
			default:
				return;
			}
		}
	}
}
