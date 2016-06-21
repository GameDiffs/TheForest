using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Material), HutongGames.PlayMaker.Tooltip("Sets a named texture in a game object's material.")]
	public class SetMaterialTexture : ComponentAction<Renderer>
	{
		[CheckForComponent(typeof(Renderer)), HutongGames.PlayMaker.Tooltip("The GameObject that the material is applied to.")]
		public FsmOwnerDefault gameObject;

		[HutongGames.PlayMaker.Tooltip("GameObjects can have multiple materials. Specify an index to target a specific material.")]
		public FsmInt materialIndex;

		[HutongGames.PlayMaker.Tooltip("Alternatively specify a Material instead of a GameObject and Index.")]
		public FsmMaterial material;

		[HutongGames.PlayMaker.Tooltip("A named parameter in the shader."), UIHint(UIHint.NamedTexture)]
		public FsmString namedTexture;

		public FsmTexture texture;

		public override void Reset()
		{
			this.gameObject = null;
			this.materialIndex = 0;
			this.material = null;
			this.namedTexture = "_MainTex";
			this.texture = null;
		}

		public override void OnEnter()
		{
			this.DoSetMaterialTexture();
			base.Finish();
		}

		private void DoSetMaterialTexture()
		{
			string text = this.namedTexture.Value;
			if (text == string.Empty)
			{
				text = "_MainTex";
			}
			if (this.material.Value != null)
			{
				this.material.Value.SetTexture(text, this.texture.Value);
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
				base.renderer.material.SetTexture(text, this.texture.Value);
			}
			else if (base.renderer.materials.Length > this.materialIndex.Value)
			{
				Material[] materials = base.renderer.materials;
				materials[this.materialIndex.Value].SetTexture(text, this.texture.Value);
				base.renderer.materials = materials;
			}
		}
	}
}
