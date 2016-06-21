using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Material), HutongGames.PlayMaker.Tooltip("Get a texture from a material on a GameObject")]
	public class GetMaterialTexture : ComponentAction<Renderer>
	{
		[CheckForComponent(typeof(Renderer)), RequiredField, HutongGames.PlayMaker.Tooltip("The GameObject the Material is applied to.")]
		public FsmOwnerDefault gameObject;

		[HutongGames.PlayMaker.Tooltip("The index of the Material in the Materials array.")]
		public FsmInt materialIndex;

		[HutongGames.PlayMaker.Tooltip("The texture to get. See Unity Shader docs for names."), UIHint(UIHint.NamedTexture)]
		public FsmString namedTexture;

		[RequiredField, Title("StoreTexture"), HutongGames.PlayMaker.Tooltip("Store the texture in a variable."), UIHint(UIHint.Variable)]
		public FsmTexture storedTexture;

		[HutongGames.PlayMaker.Tooltip("Get the shared version of the texture.")]
		public bool getFromSharedMaterial;

		public override void Reset()
		{
			this.gameObject = null;
			this.materialIndex = 0;
			this.namedTexture = "_MainTex";
			this.storedTexture = null;
			this.getFromSharedMaterial = false;
		}

		public override void OnEnter()
		{
			this.DoGetMaterialTexture();
			base.Finish();
		}

		private void DoGetMaterialTexture()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (!base.UpdateCache(ownerDefaultTarget))
			{
				return;
			}
			string text = this.namedTexture.Value;
			if (text == string.Empty)
			{
				text = "_MainTex";
			}
			if (this.materialIndex.Value == 0 && !this.getFromSharedMaterial)
			{
				this.storedTexture.Value = base.renderer.material.GetTexture(text);
			}
			else if (this.materialIndex.Value == 0 && this.getFromSharedMaterial)
			{
				this.storedTexture.Value = base.renderer.sharedMaterial.GetTexture(text);
			}
			else if (base.renderer.materials.Length > this.materialIndex.Value && !this.getFromSharedMaterial)
			{
				Material[] materials = base.renderer.materials;
				this.storedTexture.Value = base.renderer.materials[this.materialIndex.Value].GetTexture(text);
				base.renderer.materials = materials;
			}
			else if (base.renderer.materials.Length > this.materialIndex.Value && this.getFromSharedMaterial)
			{
				Material[] sharedMaterials = base.renderer.sharedMaterials;
				this.storedTexture.Value = base.renderer.sharedMaterials[this.materialIndex.Value].GetTexture(text);
				base.renderer.materials = sharedMaterials;
			}
		}
	}
}
