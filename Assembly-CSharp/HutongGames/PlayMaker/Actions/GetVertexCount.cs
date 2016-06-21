using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Mesh"), HutongGames.PlayMaker.Tooltip("Gets the number of vertices in a GameObject's mesh. Useful in conjunction with GetVertexPosition.")]
	public class GetVertexCount : FsmStateAction
	{
		[CheckForComponent(typeof(MeshFilter)), RequiredField, HutongGames.PlayMaker.Tooltip("The GameObject to check.")]
		public FsmOwnerDefault gameObject;

		[RequiredField, HutongGames.PlayMaker.Tooltip("Store the vertex count in a variable."), UIHint(UIHint.Variable)]
		public FsmInt storeCount;

		public bool everyFrame;

		public override void Reset()
		{
			this.gameObject = null;
			this.storeCount = null;
			this.everyFrame = false;
		}

		public override void OnEnter()
		{
			this.DoGetVertexCount();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		public override void OnUpdate()
		{
			this.DoGetVertexCount();
		}

		private void DoGetVertexCount()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (ownerDefaultTarget != null)
			{
				MeshFilter component = ownerDefaultTarget.GetComponent<MeshFilter>();
				if (component == null)
				{
					this.LogError("Missing MeshFilter!");
					return;
				}
				this.storeCount.Value = component.mesh.vertexCount;
			}
		}
	}
}
