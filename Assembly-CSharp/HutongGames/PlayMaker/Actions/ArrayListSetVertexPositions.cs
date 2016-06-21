using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("ArrayMaker/ArrayList"), HutongGames.PlayMaker.Tooltip("Set mesh vertex positions based on vector3 found in an arrayList")]
	public class ArrayListSetVertexPositions : ArrayListActions
	{
		[ActionSection("Set up"), CheckForComponent(typeof(PlayMakerArrayListProxy)), RequiredField, HutongGames.PlayMaker.Tooltip("The gameObject with the PlayMaker ArrayList Proxy component")]
		public FsmOwnerDefault gameObject;

		[HutongGames.PlayMaker.Tooltip("Author defined Reference of the PlayMaker ArrayList Proxy component ( necessary if several component coexists on the same GameObject")]
		public FsmString reference;

		[ActionSection("Target"), CheckForComponent(typeof(MeshFilter)), HutongGames.PlayMaker.Tooltip("The GameObject to set the mesh vertex positions to")]
		public FsmGameObject mesh;

		public bool everyFrame;

		private Mesh _mesh;

		private Vector3[] _vertices;

		public override void Reset()
		{
			this.gameObject = null;
			this.reference = null;
			this.mesh = null;
			this.everyFrame = false;
		}

		public override void OnEnter()
		{
			GameObject value = this.mesh.Value;
			if (value == null)
			{
				base.Finish();
				return;
			}
			MeshFilter component = value.GetComponent<MeshFilter>();
			if (component == null)
			{
				base.Finish();
				return;
			}
			this._mesh = component.mesh;
			if (base.SetUpArrayListProxyPointer(base.Fsm.GetOwnerDefaultTarget(this.gameObject), this.reference.Value))
			{
				this.SetVertexPositions();
			}
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		public override void OnUpdate()
		{
			this.SetVertexPositions();
		}

		public void SetVertexPositions()
		{
			if (!base.isProxyValid())
			{
				return;
			}
			this._vertices = new Vector3[this.proxy.arrayList.Count];
			int num = 0;
			foreach (Vector3 vector in this.proxy.arrayList)
			{
				this._vertices[num] = vector;
				num++;
			}
			this._mesh.vertices = this._vertices;
		}
	}
}
