using System;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.StateMachine), Tooltip("Gets info on the last event that caused a state change. See also Set Event Data action.")]
	public class GetEventInfo : FsmStateAction
	{
		[UIHint(UIHint.Variable)]
		public FsmGameObject sentByGameObject;

		[UIHint(UIHint.Variable)]
		public FsmString fsmName;

		[UIHint(UIHint.Variable)]
		public FsmBool getBoolData;

		[UIHint(UIHint.Variable)]
		public FsmInt getIntData;

		[UIHint(UIHint.Variable)]
		public FsmFloat getFloatData;

		[UIHint(UIHint.Variable)]
		public FsmVector2 getVector2Data;

		[UIHint(UIHint.Variable)]
		public FsmVector3 getVector3Data;

		[UIHint(UIHint.Variable)]
		public FsmString getStringData;

		[UIHint(UIHint.Variable)]
		public FsmGameObject getGameObjectData;

		[UIHint(UIHint.Variable)]
		public FsmRect getRectData;

		[UIHint(UIHint.Variable)]
		public FsmQuaternion getQuaternionData;

		[UIHint(UIHint.Variable)]
		public FsmMaterial getMaterialData;

		[UIHint(UIHint.Variable)]
		public FsmTexture getTextureData;

		[UIHint(UIHint.Variable)]
		public FsmColor getColorData;

		[UIHint(UIHint.Variable)]
		public FsmObject getObjectData;

		public override void Reset()
		{
			this.sentByGameObject = null;
			this.fsmName = null;
			this.getBoolData = null;
			this.getIntData = null;
			this.getFloatData = null;
			this.getVector2Data = null;
			this.getVector3Data = null;
			this.getStringData = null;
			this.getGameObjectData = null;
			this.getRectData = null;
			this.getQuaternionData = null;
			this.getMaterialData = null;
			this.getTextureData = null;
			this.getColorData = null;
			this.getObjectData = null;
		}

		public override void OnEnter()
		{
			if (Fsm.EventData.SentByFsm != null)
			{
				this.sentByGameObject.Value = Fsm.EventData.SentByFsm.GameObject;
				this.fsmName.Value = Fsm.EventData.SentByFsm.Name;
			}
			else
			{
				this.sentByGameObject.Value = null;
				this.fsmName.Value = string.Empty;
			}
			this.getBoolData.Value = Fsm.EventData.BoolData;
			this.getIntData.Value = Fsm.EventData.IntData;
			this.getFloatData.Value = Fsm.EventData.FloatData;
			this.getVector2Data.Value = Fsm.EventData.Vector2Data;
			this.getVector3Data.Value = Fsm.EventData.Vector3Data;
			this.getStringData.Value = Fsm.EventData.StringData;
			this.getGameObjectData.Value = Fsm.EventData.GameObjectData;
			this.getRectData.Value = Fsm.EventData.RectData;
			this.getQuaternionData.Value = Fsm.EventData.QuaternionData;
			this.getMaterialData.Value = Fsm.EventData.MaterialData;
			this.getTextureData.Value = Fsm.EventData.TextureData;
			this.getColorData.Value = Fsm.EventData.ColorData;
			this.getObjectData.Value = Fsm.EventData.ObjectData;
			base.Finish();
		}
	}
}
