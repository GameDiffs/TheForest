using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

namespace TheForest.Utils
{
	public class FakeParent : MonoBehaviour
	{
		public bool disableOnAwake;

		private Transform target;

		private float timer = 2f;

		private void Awake()
		{
			this.target = base.transform.parent;
			if (this.disableOnAwake && Scene.ActiveMB)
			{
				Scene.ActiveMB.StartCoroutine(this.DelayedAwake());
			}
			if (!base.gameObject.GetComponent<fixWeaponPosition>())
			{
				base.gameObject.AddComponent<fixWeaponPosition>();
			}
		}

		[DebuggerHidden]
		private IEnumerator DelayedAwake()
		{
			FakeParent.<DelayedAwake>c__Iterator1BE <DelayedAwake>c__Iterator1BE = new FakeParent.<DelayedAwake>c__Iterator1BE();
			<DelayedAwake>c__Iterator1BE.<>f__this = this;
			return <DelayedAwake>c__Iterator1BE;
		}

		private void OnEnable()
		{
			if (!base.transform.parent)
			{
				this.ReParent();
			}
		}

		private void OnDisable()
		{
			if (!base.gameObject.activeSelf && base.transform.parent)
			{
				Scene.ActiveMB.StartCoroutine(this.UnParent());
			}
		}

		[DebuggerHidden]
		private IEnumerator UnParent()
		{
			FakeParent.<UnParent>c__Iterator1BF <UnParent>c__Iterator1BF = new FakeParent.<UnParent>c__Iterator1BF();
			<UnParent>c__Iterator1BF.<>f__this = this;
			return <UnParent>c__Iterator1BF;
		}

		private void ReParent()
		{
			base.transform.parent = this.target;
			base.transform.localPosition = base.transform.position;
			base.transform.localRotation = base.transform.rotation;
		}
	}
}
