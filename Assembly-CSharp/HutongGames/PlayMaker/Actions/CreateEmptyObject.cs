using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.GameObject), HutongGames.PlayMaker.Tooltip("Creates a Game Object at a spawn point.\nUse a Game Object and/or Position/Rotation for the Spawn Point. If you specify a Game Object, Position is used as a local offset, and Rotation will override the object's rotation.")]
	public class CreateEmptyObject : FsmStateAction
	{
		public FsmGameObject gameObject;

		public FsmGameObject spawnPoint;

		public FsmVector3 position;

		public FsmVector3 rotation;

		[HutongGames.PlayMaker.Tooltip("Optionally store the created object."), UIHint(UIHint.Variable)]
		public FsmGameObject storeObject;

		public override void Reset()
		{
			this.gameObject = null;
			this.spawnPoint = null;
			this.position = new FsmVector3
			{
				UseVariable = true
			};
			this.rotation = new FsmVector3
			{
				UseVariable = true
			};
			this.storeObject = null;
		}

		public override void OnEnter()
		{
			GameObject value = this.gameObject.Value;
			Vector3 a = Vector3.zero;
			Vector3 eulerAngles = Vector3.up;
			if (this.spawnPoint.Value != null)
			{
				a = this.spawnPoint.Value.transform.position;
				if (!this.position.IsNone)
				{
					a += this.position.Value;
				}
				if (!this.rotation.IsNone)
				{
					eulerAngles = this.rotation.Value;
				}
				else
				{
					eulerAngles = this.spawnPoint.Value.transform.eulerAngles;
				}
			}
			else
			{
				if (!this.position.IsNone)
				{
					a = this.position.Value;
				}
				if (!this.rotation.IsNone)
				{
					eulerAngles = this.rotation.Value;
				}
			}
			GameObject gameObject = this.storeObject.Value;
			if (value != null)
			{
				gameObject = UnityEngine.Object.Instantiate<GameObject>(value);
				this.storeObject.Value = gameObject;
			}
			else
			{
				gameObject = new GameObject("EmptyObjectFromNull");
				this.storeObject.Value = gameObject;
			}
			if (gameObject != null)
			{
				gameObject.transform.position = a;
				gameObject.transform.eulerAngles = eulerAngles;
			}
			base.Finish();
		}
	}
}
