using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.GameObject), HutongGames.PlayMaker.Tooltip("Gets a Random Game Object from the scene.\nOptionally filter by Tag.")]
	public class GetRandomObject : FsmStateAction
	{
		[UIHint(UIHint.Tag)]
		public FsmString withTag;

		[RequiredField, UIHint(UIHint.Variable)]
		public FsmGameObject storeResult;

		[HutongGames.PlayMaker.Tooltip("Repeat every frame.")]
		public bool everyFrame;

		public override void Reset()
		{
			this.withTag = "Untagged";
			this.storeResult = null;
			this.everyFrame = false;
		}

		public override void OnEnter()
		{
			this.DoGetRandomObject();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		public override void OnUpdate()
		{
			this.DoGetRandomObject();
		}

		private void DoGetRandomObject()
		{
			GameObject[] array;
			if (this.withTag.Value != "Untagged")
			{
				array = GameObject.FindGameObjectsWithTag(this.withTag.Value);
			}
			else
			{
				array = (GameObject[])UnityEngine.Object.FindObjectsOfType(typeof(GameObject));
			}
			if (array.Length > 0)
			{
				this.storeResult.Value = array[UnityEngine.Random.Range(0, array.Length)];
				return;
			}
			this.storeResult.Value = null;
		}
	}
}
