using System;
using UnityEngine;

public class spawnFeathers : MonoBehaviour
{
	public GameObject[] feather;

	private int counter;

	private int amount;

	public int minFeathers;

	public int maxFeathers;

	public int maxSpawnEvents;

	public float spawnChance = 0.1f;

	public bool spawnWithPopFoce;

	public float popForceMult = 1f;

	public bool dontUseHitForce;

	private Rigidbody rb;

	private int maxSpawn;

	public float hitForce = -20000f;

	private bool startUp;

	private void Start()
	{
		this.rb = base.GetComponent<Rigidbody>();
		base.Invoke("setStart", 1f);
	}

	private void setStart()
	{
		this.startUp = true;
	}

	private void OnTriggerEnter(Collider other)
	{
		if (!this.startUp)
		{
			return;
		}
		if (other.gameObject.CompareTag("Weapon"))
		{
			if (UnityEngine.Random.value < this.spawnChance && this.maxSpawn < this.maxSpawnEvents)
			{
				this.amount = 0;
				this.counter = UnityEngine.Random.Range(this.minFeathers, this.maxFeathers);
				while (this.amount < this.counter)
				{
					GameObject gameObject = UnityEngine.Object.Instantiate(this.feather[UnityEngine.Random.Range(0, this.feather.Length)], base.transform.position, Quaternion.identity) as GameObject;
					if (this.spawnWithPopFoce)
					{
						gameObject.GetComponent<Rigidbody>().AddForce(UnityEngine.Random.Range(-200f * this.popForceMult, 200f * this.popForceMult), UnityEngine.Random.Range(500f * this.popForceMult, 800f * this.popForceMult), UnityEngine.Random.Range(-200f * this.popForceMult, 200f * this.popForceMult));
						gameObject.GetComponent<Rigidbody>().AddTorque(1000f, (float)UnityEngine.Random.Range(200, 1200), (float)UnityEngine.Random.Range(200, 1200));
					}
					this.amount++;
				}
				this.maxSpawn++;
			}
			if (!this.dontUseHitForce)
			{
				this.rb.AddForce(0f, this.hitForce, 0f);
			}
		}
	}
}
