using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

public class CoopLogStopper : MonoBehaviour
{
	[SerializeField]
	private Rigidbody rb;

	[SerializeField]
	private Collider rbCollider;

	private bool stopping;

	private void OnEnable()
	{
		if (BoltNetwork.isClient)
		{
			UnityEngine.Object.Destroy(this.rb);
			UnityEngine.Object.Destroy(this.rbCollider);
		}
		else
		{
			this.rb.isKinematic = false;
			this.stopping = false;
		}
	}

	private void OnCollisionEnter(Collision collision)
	{
		if (BoltNetwork.isServer && !this.stopping && collision.transform.GetComponentInChildren<Terrain>())
		{
			base.StartCoroutine(this.Stop(3));
		}
	}

	private void OnCollisionExit(Collision collision)
	{
		if (BoltNetwork.isServer && !this.stopping && collision.transform.GetComponentInChildren<Terrain>())
		{
			Vector3 velocity = this.rb.velocity;
			if (velocity.y > 0f)
			{
				velocity.y = 0f;
			}
			this.rb.velocity = velocity;
		}
	}

	[DebuggerHidden]
	private IEnumerator Stop(int time)
	{
		CoopLogStopper.<Stop>c__Iterator1A <Stop>c__Iterator1A = new CoopLogStopper.<Stop>c__Iterator1A();
		<Stop>c__Iterator1A.time = time;
		<Stop>c__Iterator1A.<$>time = time;
		<Stop>c__Iterator1A.<>f__this = this;
		return <Stop>c__Iterator1A;
	}
}
