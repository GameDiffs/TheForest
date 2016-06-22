using System;
using System.Collections;
using System.Diagnostics;
using TheForest.Utils;
using UnityEngine;

namespace TheForest.SerializableTaskSystem
{
	[Serializable]
	public class ProximityCondition : ACondition
	{
		[Header("Proximity")]
		public string _objectTag;

		public bool _isTag;

		public float _distance;

		public bool _use2dDistance;

		public bool _inCaveOnly;

		private Transform _targetObject;

		private Coroutine _routine;

		public override void Init()
		{
			GameObject gameObject = (!this._isTag) ? GameObject.Find(this._objectTag) : GameObject.FindWithTag(this._objectTag);
			if (gameObject)
			{
				this._targetObject = gameObject.transform;
				this._routine = Scene.ActiveMB.StartCoroutine(this.CheckProximityRoutine());
			}
			else
			{
				UnityEngine.Debug.LogError("ProximityCondition didn't find object with " + ((!this._isTag) ? "name" : "tag") + ": " + this._objectTag);
			}
		}

		public override void Clear()
		{
			if (this._routine != null)
			{
				Scene.ActiveMB.StopCoroutine(this._routine);
			}
			base.Clear();
		}

		[DebuggerHidden]
		public IEnumerator CheckProximityRoutine()
		{
			ProximityCondition.<CheckProximityRoutine>c__Iterator189 <CheckProximityRoutine>c__Iterator = new ProximityCondition.<CheckProximityRoutine>c__Iterator189();
			<CheckProximityRoutine>c__Iterator.<>f__this = this;
			return <CheckProximityRoutine>c__Iterator;
		}

		private bool IsWithinRangeOfTarget()
		{
			if (this._inCaveOnly && !Scene.Atmosphere.InACave)
			{
				return false;
			}
			if (this._use2dDistance)
			{
				return Vector2.Distance(new Vector2(this._targetObject.position.x, this._targetObject.position.z), new Vector2(LocalPlayer.Transform.position.x, LocalPlayer.Transform.position.z)) < this._distance;
			}
			return Vector3.Distance(this._targetObject.position, LocalPlayer.Transform.position) < this._distance;
		}
	}
}
