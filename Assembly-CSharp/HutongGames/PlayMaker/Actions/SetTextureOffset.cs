using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Material), HutongGames.PlayMaker.Tooltip("Sets the Offset of a named texture in a Game Object's Material. Useful for scrolling texture effects.")]
	public class SetTextureOffset : ComponentAction<Renderer>
	{
		[CheckForComponent(typeof(Renderer)), RequiredField]
		public FsmOwnerDefault gameObject;

		public FsmInt materialIndex;

		[RequiredField, UIHint(UIHint.NamedColor)]
		public FsmString namedTexture;

		[RequiredField]
		public FsmFloat offsetX;

		[RequiredField]
		public FsmFloat offsetY;

		public bool everyFrame;

		public override void Reset()
		{
			this.gameObject = null;
			this.materialIndex = 0;
			this.namedTexture = "_MainTex";
			this.offsetX = 0f;
			this.offsetY = 0f;
			this.everyFrame = false;
		}

		public override void OnEnter()
		{
			this.DoSetTextureOffset();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		public override void OnUpdate()
		{
			this.DoSetTextureOffset();
		}

		private void DoSetTextureOffset()
		{
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
				base.renderer.material.SetTextureOffset(this.namedTexture.Value, new Vector2(this.offsetX.Value, this.offsetY.Value));
			}
			else if (base.renderer.materials.Length > this.materialIndex.Value)
			{
				Material[] materials = base.renderer.materials;
				materials[this.materialIndex.Value].SetTextureOffset(this.namedTexture.Value, new Vector2(this.offsetX.Value, this.offsetY.Value));
				base.renderer.materials = materials;
			}
		}
	}
}
