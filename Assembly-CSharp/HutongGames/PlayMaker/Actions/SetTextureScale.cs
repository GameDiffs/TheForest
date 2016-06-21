using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Material), HutongGames.PlayMaker.Tooltip("Sets the Scale of a named texture in a Game Object's Material. Useful for special effects.")]
	public class SetTextureScale : ComponentAction<Renderer>
	{
		[CheckForComponent(typeof(Renderer)), RequiredField]
		public FsmOwnerDefault gameObject;

		public FsmInt materialIndex;

		[UIHint(UIHint.NamedColor)]
		public FsmString namedTexture;

		[RequiredField]
		public FsmFloat scaleX;

		[RequiredField]
		public FsmFloat scaleY;

		public bool everyFrame;

		public override void Reset()
		{
			this.gameObject = null;
			this.materialIndex = 0;
			this.namedTexture = "_MainTex";
			this.scaleX = 1f;
			this.scaleY = 1f;
			this.everyFrame = false;
		}

		public override void OnEnter()
		{
			this.DoSetTextureScale();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		public override void OnUpdate()
		{
			this.DoSetTextureScale();
		}

		private void DoSetTextureScale()
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
				base.renderer.material.SetTextureScale(this.namedTexture.Value, new Vector2(this.scaleX.Value, this.scaleY.Value));
			}
			else if (base.renderer.materials.Length > this.materialIndex.Value)
			{
				Material[] materials = base.renderer.materials;
				materials[this.materialIndex.Value].SetTextureScale(this.namedTexture.Value, new Vector2(this.scaleX.Value, this.scaleY.Value));
				base.renderer.materials = materials;
			}
		}
	}
}
