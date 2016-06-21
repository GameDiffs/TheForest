using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Debug), HutongGames.PlayMaker.Tooltip("Draws a line from a Start point to an End point. Specify the points as Game Objects or Vector3 world positions. If both are specified, position is used as a local offset from the Object's position.")]
	public class DrawDebugLine : FsmStateAction
	{
		[HutongGames.PlayMaker.Tooltip("Draw line from a GameObject.")]
		public FsmGameObject fromObject;

		[HutongGames.PlayMaker.Tooltip("Draw line from a world position, or local offset from GameObject if provided.")]
		public FsmVector3 fromPosition;

		[HutongGames.PlayMaker.Tooltip("Draw line to a GameObject.")]
		public FsmGameObject toObject;

		[HutongGames.PlayMaker.Tooltip("Draw line to a world position, or local offset from GameObject if provided.")]
		public FsmVector3 toPosition;

		[HutongGames.PlayMaker.Tooltip("The color of the line.")]
		public FsmColor color;

		public override void Reset()
		{
			this.fromObject = new FsmGameObject
			{
				UseVariable = true
			};
			this.fromPosition = new FsmVector3
			{
				UseVariable = true
			};
			this.toObject = new FsmGameObject
			{
				UseVariable = true
			};
			this.toPosition = new FsmVector3
			{
				UseVariable = true
			};
			this.color = Color.white;
		}

		public override void OnUpdate()
		{
			Vector3 position = ActionHelpers.GetPosition(this.fromObject, this.fromPosition);
			Vector3 position2 = ActionHelpers.GetPosition(this.toObject, this.toPosition);
			Debug.DrawLine(position, position2, this.color.Value);
		}
	}
}
