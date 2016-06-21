using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("ArrayMaker/ArrayList"), HutongGames.PlayMaker.Tooltip("Return the closest GameObject within an arrayList from a transform or position.")]
	public class ArrayListGetClosestGameObject : ArrayListActions
	{
		[ActionSection("Set up"), CheckForComponent(typeof(PlayMakerArrayListProxy)), RequiredField, HutongGames.PlayMaker.Tooltip("The gameObject with the PlayMaker ArrayList Proxy component")]
		public FsmOwnerDefault gameObject;

		[HutongGames.PlayMaker.Tooltip("Author defined Reference of the PlayMaker ArrayList Proxy component ( necessary if several component coexists on the same GameObject")]
		public FsmString reference;

		[HutongGames.PlayMaker.Tooltip("Compare the distance of the items in the list to the position of this gameObject")]
		public FsmGameObject distanceFrom;

		[HutongGames.PlayMaker.Tooltip("If DistanceFrom declared, use OrDistanceFromVector3 as an offset")]
		public FsmVector3 orDistanceFromVector3;

		public bool everyframe;

		[ActionSection("Result"), UIHint(UIHint.Variable)]
		public FsmGameObject closestGameObject;

		[UIHint(UIHint.Variable)]
		public FsmInt closestIndex;

		public override void Reset()
		{
			this.gameObject = null;
			this.reference = null;
			this.distanceFrom = null;
			this.orDistanceFromVector3 = null;
			this.closestGameObject = null;
			this.closestIndex = null;
			this.everyframe = true;
		}

		public override void OnEnter()
		{
			if (!base.SetUpArrayListProxyPointer(base.Fsm.GetOwnerDefaultTarget(this.gameObject), this.reference.Value))
			{
				base.Finish();
			}
			this.DoFindClosestGo();
			if (!this.everyframe)
			{
				base.Finish();
			}
		}

		public override void OnUpdate()
		{
			this.DoFindClosestGo();
		}

		private void DoFindClosestGo()
		{
			if (!base.isProxyValid())
			{
				return;
			}
			Vector3 vector = this.orDistanceFromVector3.Value;
			GameObject value = this.distanceFrom.Value;
			if (value != null)
			{
				vector += value.transform.position;
			}
			float num = float.PositiveInfinity;
			int num2 = 0;
			foreach (GameObject gameObject in this.proxy.arrayList)
			{
				if (gameObject != null)
				{
					float sqrMagnitude = (gameObject.transform.position - vector).sqrMagnitude;
					if (sqrMagnitude <= num)
					{
						num = sqrMagnitude;
						this.closestGameObject.Value = gameObject;
						this.closestIndex.Value = num2;
					}
				}
				num2++;
			}
		}
	}
}
