using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Material), HutongGames.PlayMaker.Tooltip("Sets the material on a game object.")]
	public class SetMaterial : ComponentAction<Renderer>
	{
		[CheckForComponent(typeof(Renderer)), RequiredField]
		public FsmOwnerDefault gameObject;

		public FsmInt materialIndex;

		[RequiredField]
		public FsmMaterial material;

		public override void Reset()
		{
			this.gameObject = null;
			this.material = null;
			this.materialIndex = 0;
		}

		public override void OnEnter()
		{
			this.DoSetMaterial();
			base.Finish();
		}

		private void DoSetMaterial()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (!base.UpdateCache(ownerDefaultTarget))
			{
				return;
			}
			if (this.materialIndex.Value == 0)
			{
				base.renderer.material = this.material.Value;
			}
			else if (base.renderer.materials.Length > this.materialIndex.Value)
			{
				Material[] materials = base.renderer.materials;
				materials[this.materialIndex.Value] = this.material.Value;
				base.renderer.materials = materials;
			}
		}
	}
}
