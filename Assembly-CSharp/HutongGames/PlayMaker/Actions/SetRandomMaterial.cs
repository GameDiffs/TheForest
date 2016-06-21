using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Material), HutongGames.PlayMaker.Tooltip("Sets a Game Object's material randomly from an array of Materials.")]
	public class SetRandomMaterial : ComponentAction<Renderer>
	{
		[CheckForComponent(typeof(Renderer)), RequiredField]
		public FsmOwnerDefault gameObject;

		public FsmInt materialIndex;

		public FsmMaterial[] materials;

		public override void Reset()
		{
			this.gameObject = null;
			this.materialIndex = 0;
			this.materials = new FsmMaterial[3];
		}

		public override void OnEnter()
		{
			this.DoSetRandomMaterial();
			base.Finish();
		}

		private void DoSetRandomMaterial()
		{
			if (this.materials == null)
			{
				return;
			}
			if (this.materials.Length == 0)
			{
				return;
			}
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (!base.UpdateCache(ownerDefaultTarget))
			{
				return;
			}
			if (base.renderer.material == null)
			{
				this.LogError("Missing Material!");
				return;
			}
			if (this.materialIndex.Value == 0)
			{
				base.renderer.material = this.materials[UnityEngine.Random.Range(0, this.materials.Length)].Value;
			}
			else if (base.renderer.materials.Length > this.materialIndex.Value)
			{
				Material[] array = base.renderer.materials;
				array[this.materialIndex.Value] = this.materials[UnityEngine.Random.Range(0, this.materials.Length)].Value;
				base.renderer.materials = array;
			}
		}
	}
}
