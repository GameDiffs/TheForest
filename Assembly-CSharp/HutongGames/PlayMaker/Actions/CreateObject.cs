using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.GameObject), HutongGames.PlayMaker.Tooltip("Creates a Game Object, usually from a Prefab.")]
	public class CreateObject : FsmStateAction
	{
		[RequiredField, HutongGames.PlayMaker.Tooltip("GameObject to create. Usually a Prefab.")]
		public FsmGameObject gameObject;

		[HutongGames.PlayMaker.Tooltip("Optional Spawn Point.")]
		public FsmGameObject spawnPoint;

		[HutongGames.PlayMaker.Tooltip("Position. If a Spawn Point is defined, this is used as a local offset from the Spawn Point position.")]
		public FsmVector3 position;

		[HutongGames.PlayMaker.Tooltip("Rotation. NOTE: Overrides the rotation of the Spawn Point.")]
		public FsmVector3 rotation;

		[HutongGames.PlayMaker.Tooltip("Optionally store the created object."), UIHint(UIHint.Variable)]
		public FsmGameObject storeObject;

		[HutongGames.PlayMaker.Tooltip("Use Network.Instantiate to create a Game Object on all clients in a networked game.")]
		public FsmBool networkInstantiate;

		[HutongGames.PlayMaker.Tooltip("Usually 0. The group number allows you to group together network messages which allows you to filter them if so desired.")]
		public FsmInt networkGroup;

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
			this.networkInstantiate = false;
			this.networkGroup = 0;
		}

		public override void OnEnter()
		{
			GameObject value = this.gameObject.Value;
			if (value != null)
			{
				Vector3 a = Vector3.zero;
				Vector3 euler = Vector3.up;
				if (this.spawnPoint.Value != null)
				{
					a = this.spawnPoint.Value.transform.position;
					if (!this.position.IsNone)
					{
						a += this.position.Value;
					}
					euler = (this.rotation.IsNone ? this.spawnPoint.Value.transform.eulerAngles : this.rotation.Value);
				}
				else
				{
					if (!this.position.IsNone)
					{
						a = this.position.Value;
					}
					if (!this.rotation.IsNone)
					{
						euler = this.rotation.Value;
					}
				}
				GameObject value2;
				if (!this.networkInstantiate.Value)
				{
					value2 = (GameObject)UnityEngine.Object.Instantiate(value, a, Quaternion.Euler(euler));
				}
				else
				{
					value2 = (GameObject)Network.Instantiate(value, a, Quaternion.Euler(euler), this.networkGroup.Value);
				}
				this.storeObject.Value = value2;
			}
			base.Finish();
		}
	}
}
