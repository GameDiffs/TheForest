using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("ArrayMaker/ArrayList"), HutongGames.PlayMaker.Tooltip("Set a mesh vertex colors based on colors found in an arrayList")]
	public class ArrayListSetVertexColors : ArrayListActions
	{
		[ActionSection("Set up"), CheckForComponent(typeof(PlayMakerArrayListProxy)), RequiredField, HutongGames.PlayMaker.Tooltip("The gameObject with the PlayMaker ArrayList Proxy component")]
		public FsmOwnerDefault gameObject;

		[HutongGames.PlayMaker.Tooltip("Author defined Reference of the PlayMaker ArrayList Proxy component ( necessary if several component coexists on the same GameObject")]
		public FsmString reference;

		[ActionSection("Target"), CheckForComponent(typeof(MeshFilter)), HutongGames.PlayMaker.Tooltip("The GameObject to set the mesh colors to")]
		public FsmGameObject mesh;

		public bool everyFrame;

		private Mesh _mesh;

		private Color[] _colors;

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
				this.SetVertexColors();
			}
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		public override void OnUpdate()
		{
			this.SetVertexColors();
		}

		public void SetVertexColors()
		{
			if (!base.isProxyValid())
			{
				return;
			}
			this._colors = new Color[this.proxy.arrayList.Count];
			int num = 0;
			foreach (Color color in this.proxy.arrayList)
			{
				this._colors[num] = color;
				num++;
			}
			this._mesh.colors = this._colors;
		}
	}
}
