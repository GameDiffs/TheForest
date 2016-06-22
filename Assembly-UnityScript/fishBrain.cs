using System;
using UnityEngine;

[Serializable]
public class fishBrain : MonoBehaviour
{
	public int Speed;

	private Transform Middle;

	private Vector3 randomDirection;

	private float distance;

	private bool Dead;

	public GameObject MyTrigger;

	private bool Flee;

	private Vector3 PlayerPos;

	public fishBrain()
	{
		this.Speed = 1;
	}

	public override void Start()
	{
		this.Middle = this.transform.parent;
		this.InvokeRepeating("SwitchDirection", (float)0, (float)UnityEngine.Random.Range(2, 6));
		this.GetComponent<Animation>().Play("Swim");
	}

	public override void Update()
	{
		if (!this.Dead && !this.Flee)
		{
			this.transform.position = this.transform.position + this.transform.forward * (float)this.Speed * Time.deltaTime;
			if (this.distance > (float)1)
			{
				this.transform.Rotate(this.Middle.position * 0.1f * Time.deltaTime);
			}
			this.transform.Rotate(this.randomDirection * 0.1f * Time.deltaTime);
		}
		if (!this.Dead && this.Flee)
		{
			this.transform.Rotate(-this.PlayerPos * 0.1f * Time.deltaTime);
		}
	}

	public override void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.CompareTag("Player"))
		{
			this.PlayerPos = new Vector3((float)0, other.transform.position.y, (float)0);
			this.Retreat();
		}
	}

	public override void SwitchDirection()
	{
		this.randomDirection = new Vector3((float)0, (float)UnityEngine.Random.Range(-359, 359), (float)0);
	}

	public override void Retreat()
	{
		this.Flee = true;
		this.Speed += 2;
		this.Invoke("Calm", (float)UnityEngine.Random.Range(1, 5));
	}

	public override void Calm()
	{
		this.Speed -= 2;
		this.Flee = false;
	}

	public override void CheckDist()
	{
		this.distance = Vector3.Distance(this.transform.position, this.Middle.position);
	}

	public override void Drop()
	{
	}

	public override void Die()
	{
		this.Dead = true;
		this.GetComponent<Animation>().Stop();
		this.MyTrigger.SetActive(true);
	}

	public override void Main()
	{
	}
}
