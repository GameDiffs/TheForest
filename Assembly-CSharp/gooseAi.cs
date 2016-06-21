using System;
using UnityEngine;

public class gooseAi : MonoBehaviour
{
	private Animator anim;

	private Vector3 lookAtPos;

	private Vector3 flightVector;

	private Vector3 initPos;

	private Transform thisTr;

	private Transform rootTr;

	private Quaternion lastRotation;

	private Quaternion desiredRotation;

	private void Start()
	{
		this.anim = base.transform.GetComponentInChildren<Animator>();
		this.thisTr = base.transform.GetChild(0).transform;
		this.rootTr = base.transform.root;
		this.anim.speed = UnityEngine.Random.Range(1.6f, 1.85f);
		this.lookAtPos = this.thisTr.forward * 100f;
		this.flightVector = this.thisTr.right * 1000f;
		base.Invoke("getPosition", UnityEngine.Random.Range(1f, 3f));
		this.initPos = this.thisTr.position;
	}

	private void Fly()
	{
		base.CancelInvoke("getPosition");
		this.lookAtPos = this.flightVector;
		base.Invoke("doTakeOff", UnityEngine.Random.Range(2f, 4.2f));
		base.Invoke("disableGoose", 30f);
	}

	private void disableGoose()
	{
		this.thisTr.position = this.initPos;
		base.gameObject.SetActive(false);
		base.Invoke("getPosition", UnityEngine.Random.Range(1f, 3f));
	}

	private void LateUpdate()
	{
		this.DoSmoothLookAt();
	}

	private void DoSmoothLookAt()
	{
		Vector3 vector = this.lookAtPos - this.thisTr.position;
		if (vector != Vector3.zero && vector.sqrMagnitude > 0f)
		{
			this.desiredRotation = Quaternion.LookRotation(vector, Vector3.up);
		}
		this.lastRotation = Quaternion.Slerp(this.lastRotation, this.desiredRotation, 0.5f * Time.deltaTime);
		this.thisTr.rotation = this.lastRotation;
	}

	private void getPosition()
	{
		if (Vector3.Distance(this.thisTr.position, this.rootTr.position) > 10f)
		{
			this.lookAtPos = this.rootTr.position;
			this.lookAtPos.y = this.thisTr.position.y;
		}
		else
		{
			this.lookAtPos = gooseAi.Circle(200f);
			this.lookAtPos += this.thisTr.position;
			this.lookAtPos.y = this.thisTr.position.y;
		}
		base.Invoke("getPosition", (float)UnityEngine.Random.Range(5, 10));
	}

	private void doTakeOff()
	{
		this.anim.SetTrigger("takeOff");
	}

	public static Vector2 Circle(float radius)
	{
		Vector2 insideUnitCircle = UnityEngine.Random.insideUnitCircle;
		insideUnitCircle.Normalize();
		return insideUnitCircle * radius;
	}
}
