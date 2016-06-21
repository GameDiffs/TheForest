using System;
using UnityEngine;

public class clscannon : MonoBehaviour
{
	public Transform vargamactor;

	public Transform vargamactorspawnpoint;

	public Transform vargamcannonball;

	public Transform vargamcannonballspawn;

	public float vargammaxcharge = 100f;

	public float vargamchargespeed = 100f;

	public float vargamfirerate = 1f;

	private float varcannonballforce = 15000f;

	private float varlastfired;

	private float varcharge;

	private Rect varchargelabel = new Rect(0f, 200f, 300f, 50f);

	private Transform varcannonball;

	private bool varstarted;

	private void Start()
	{
		this.varchargelabel = new Rect(100f, (float)(Screen.height - 35), 200f, 40f);
	}

	private void OnMouseDrag()
	{
		this.varcharge = (this.varcharge + Time.deltaTime * this.vargamchargespeed) % this.vargammaxcharge;
	}

	private void OnMouseUp()
	{
		if (this.vargamcannonballspawn != null && this.vargamcannonball != null && this.vargamcannonballspawn != null && Time.timeSinceLevelLoad - this.varlastfired > this.vargamfirerate)
		{
			this.varcannonball = (UnityEngine.Object.Instantiate(this.vargamcannonball, this.vargamcannonballspawn.transform.position, this.vargamcannonballspawn.transform.rotation) as Transform);
			this.varcannonball.transform.parent = base.transform;
			this.varcannonball.GetComponent<Rigidbody>().isKinematic = false;
			if (this.varcannonball.GetComponent<clscannonball>() != null)
			{
				this.varcannonball.GetComponent<clscannonball>().varcannon = this;
			}
			this.varlastfired = Time.timeSinceLevelLoad;
			this.varcannonball.GetComponent<Rigidbody>().AddForce(this.vargamcannonballspawn.transform.forward * (this.varcannonballforce * (this.varcharge / this.vargammaxcharge)));
		}
		this.varcharge = 0f;
	}

	public void metresetactor()
	{
		if (this.vargamactor != null && this.vargamactorspawnpoint != null)
		{
			UnityEngine.Object.Instantiate(this.vargamactor, this.vargamactorspawnpoint.position, Quaternion.identity);
		}
	}

	private void OnGUI()
	{
		if (this.varstarted)
		{
			if (Time.timeSinceLevelLoad - this.varlastfired < this.vargamfirerate)
			{
				GUI.contentColor = Color.red;
			}
			else
			{
				GUI.contentColor = Color.green;
			}
			GUI.Label(this.varchargelabel, "Cannon charge: " + this.varcharge + "\n(click the cannon)");
		}
	}

	public void metactivate()
	{
		this.varstarted = true;
	}
}
