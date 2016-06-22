using System;
using System.Collections;
using System.Diagnostics;
using TheForest.World;
using UnityEngine;

public class CoopLogStopper : MonoBehaviour
{
	[SerializeField]
	private Rigidbody rb;

	[SerializeField]
	private CapsuleCollider rbCollider;

	private bool stopping;

	private void OnEnable()
	{
		if (BoltNetwork.isClient)
		{
			this.rb.isKinematic = true;
			this.rb.useGravity = false;
			this.rbCollider.radius = 0.35f;
			this.rbCollider.height = 4.5f;
			UnityEngine.Object.Destroy(base.GetComponent<Buoyancy>());
			UnityEngine.Object.Destroy(base.GetComponent<EnableInWaterProxy>());
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
