using System;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.StateMachine), Tooltip("Sets Event Data before sending an event. Get the Event Data, along with sender information, using Get Event Info action.")]
	public class SetEventData : FsmStateAction
	{
		public FsmGameObject setGameObjectData;

		public FsmInt setIntData;

		public FsmFloat setFloatData;

		public FsmString setStringData;

		public FsmBool setBoolData;

		public FsmVector2 setVector2Data;

		public FsmVector3 setVector3Data;

		public FsmRect setRectData;

		public FsmQuaternion setQuaternionData;

		public FsmColor setColorData;

		public FsmMaterial setMaterialData;

		public FsmTexture setTextureData;

		public FsmObject setObjectData;

		public override void Reset()
		{
			this.setGameObjectData = new FsmGameObject
			{
				UseVariable = true
			};
			this.setIntData = new FsmInt
			{
				UseVariable = true
			};
			this.setFloatData = new FsmFloat
			{
				UseVariable = true
			};
			this.setStringData = new FsmString
			{
				UseVariable = true
			};
			this.setBoolData = new FsmBool
			{
				UseVariable = true
			};
			this.setVector2Data = new FsmVector2
			{
				UseVariable = true
			};
			this.setVector3Data = new FsmVector3
			{
				UseVariable = true
			};
			this.setRectData = new FsmRect
			{
				UseVariable = true
			};
			this.setQuaternionData = new FsmQuaternion
			{
				UseVariable = true
			};
			this.setColorData = new FsmColor
			{
				UseVariable = true
			};
			this.setMaterialData = new FsmMaterial
			{
				UseVariable = true
			};
			this.setTextureData = new FsmTexture
			{
				UseVariable = true
			};
			this.setObjectData = new FsmObject
			{
				UseVariable = true
			};
		}

		public override void OnEnter()
		{
			Fsm.EventData.BoolData = this.setBoolData.Value;
			Fsm.EventData.IntData = this.setIntData.Value;
			Fsm.EventData.FloatData = this.setFloatData.Value;
			Fsm.EventData.Vector2Data = this.setVector2Data.Value;
			Fsm.EventData.Vector3Data = this.setVector3Data.Value;
			Fsm.EventData.StringData = this.setStringData.Value;
			Fsm.EventData.GameObjectData = this.setGameObjectData.Value;
			Fsm.EventData.RectData = this.setRectData.Value;
			Fsm.EventData.QuaternionData = this.setQuaternionData.Value;
			Fsm.EventData.ColorData = this.setColorData.Value;
			Fsm.EventData.MaterialData = this.setMaterialData.Value;
			Fsm.EventData.TextureData = this.setTextureData.Value;
			Fsm.EventData.ObjectData = this.setObjectData.Value;
			base.Finish();
		}
	}
}
