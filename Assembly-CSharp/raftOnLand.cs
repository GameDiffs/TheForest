using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

public class raftOnLand : MonoBehaviour
{
	public float massOnLand;

	public float massOnWater;

	public float maxWobble;

	public float wobbleSpeed;

	public float noiseX;

	public float noiseY;

	public float repeat = 1f;

	public GameObject wobbleBox;

	public ForceMode _forceMode;

	private RaftPush _push;

	private Rigidbody _rb;

	private float setWobble;

	private float slope = 0.5f;

	private float interval = 1f;

	private float smooth = 0.5f;

	private float tNext;

	private float dir = 1f;

	private IRaftState raftState;

	private void Start()
	{
		this._rb = base.transform.GetComponent<Rigidbody>();
		this._push = base.transform.GetComponentInChildren<RaftPush>();
		if (BoltNetwork.isServer)
		{
			base.StartCoroutine(this.WaitAttachMP());
		}
	}

	[DebuggerHidden]
	private IEnumerator WaitAttachMP()
	{
		raftOnLand.<WaitAttachMP>c__IteratorEC <WaitAttachMP>c__IteratorEC = new raftOnLand.<WaitAttachMP>c__IteratorEC();
		<WaitAttachMP>c__IteratorEC.<>f__this = this;
		return <WaitAttachMP>c__IteratorEC;
	}

	private void Update()
	{
		if (this.raftState != null && this.raftState.InWater != this._push._buoyancy.InWater)
		{
			this.raftState.InWater = this._push._buoyancy.InWater;
		}
	}

	private void OnCollisionStay(Collision col)
	{
		if (col == null)
		{
			return;
		}
		if (col.gameObject == null)
		{
			return;
		}
		if (this._rb == null)
		{
			return;
		}
		if (col.gameObject.CompareTag("TerrainMain") && (!this._push || this._push._state != RaftPush.States.DriverLocked))
		{
			this._rb.mass = Mathf.Lerp(this._rb.mass, this.massOnLand, 0.1f);
			this._rb.angularDrag = 0.05f;
			this._rb.drag = 0.05f;
		}
	}

	private void OnCollisionExit(Collision col)
	{
		if (col == null)
		{
			return;
		}
		if (col.gameObject == null)
		{
			return;
		}
		if (this._rb == null)
		{
			return;
		}
		if (col.gameObject.CompareTag("TerrainMain"))
		{
			this._rb.mass = this.massOnWater;
			this._rb.angularDrag = 2f;
			this._rb.drag = 0.4f;
		}
	}
}
