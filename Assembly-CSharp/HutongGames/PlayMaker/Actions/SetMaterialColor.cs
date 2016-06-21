using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Material), HutongGames.PlayMaker.Tooltip("Sets a named color value in a game object's material.")]
	public class SetMaterialColor : ComponentAction<Renderer>
	{
		[CheckForComponent(typeof(Renderer)), HutongGames.PlayMaker.Tooltip("The GameObject that the material is applied to.")]
		public FsmOwnerDefault gameObject;

		[HutongGames.PlayMaker.Tooltip("GameObjects can have multiple materials. Specify an index to target a specific material.")]
		public FsmInt materialIndex;

		[HutongGames.PlayMaker.Tooltip("Alternatively specify a Material instead of a GameObject and Index.")]
		public FsmMaterial material;

		[HutongGames.PlayMaker.Tooltip("A named color parameter in the shader."), UIHint(UIHint.NamedColor)]
		public FsmString namedColor;

		[RequiredField, HutongGames.PlayMaker.Tooltip("Set the parameter value.")]
		public FsmColor color;

		[HutongGames.PlayMaker.Tooltip("Repeat every frame. Useful if the value is animated.")]
		public bool everyFrame;

		public override void Reset()
		{
			this.gameObject = null;
			this.materialIndex = 0;
			this.material = null;
			this.namedColor = "_Color";
			this.color = Color.black;
			this.everyFrame = false;
		}

		public override void OnEnter()
		{
			this.DoSetMaterialColor();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		public override void OnUpdate()
		{
			this.DoSetMaterialColor();
		}

		private void DoSetMaterialColor()
		{
			if (this.color.IsNone)
			{
				return;
			}
			string text = this.namedColor.Value;
			if (text == string.Empty)
			{
				text = "_Color";
			}
			if (this.material.Value != null)
			{
				this.material.Value.SetColor(text, this.color.Value);
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
				base.renderer.material.SetColor(text, this.color.Value);
			}
			else if (base.renderer.materials.Length > this.materialIndex.Value)
			{
				Material[] materials = base.renderer.materials;
				materials[this.materialIndex.Value].SetColor(text, this.color.Value);
				base.renderer.materials = materials;
			}
		}
	}
}
