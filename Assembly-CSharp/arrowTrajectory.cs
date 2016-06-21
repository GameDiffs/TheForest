using System;
using UnityEngine;

public class arrowTrajectory : MonoBehaviour
{
	public GameObject _pickup;

	private Rigidbody rb;

	private Quaternion rot;

	private bool forceDisable;

	private void Awake()
	{
		this.rb = base.GetComponent<Rigidbody>();
	}

	private void Start()
	{
		this.forceDisable = false;
	}

	private void Update()
	{
		if (this.forceDisable)
		{
			if (this.rb)
			{
				this.rb.AddTorque((float)UnityEngine.Random.Range(-100, 100), (float)UnityEngine.Random.Range(-100, 100), (float)UnityEngine.Random.Range(-100, 100));
			}
			base.enabled = false;
			return;
		}
		if (this.rb && !this.rb.isKinematic)
		{
			if (this.rb.velocity.sqrMagnitude > 1f)
			{
				base.transform.LookAt(base.transform.position + this.rb.velocity);
				base.transform.rotation *= Quaternion.Euler(90f, 0f, 0f);
			}
			else
			{
				this.ActivatePickup();
				base.enabled = false;
			}
		}
		else
		{
			base.Invoke("ActivatePickup", 0.75f);
			base.enabled = false;
		}
	}

	private void OnCollisionEnter(Collision other)
	{
		this.forceDisable = true;
	}

	private void ActivatePickup()
	{
		this._pickup.SetActive(true);
	}
}
