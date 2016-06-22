using System;
using UnityEngine;

public class CreateCraterOnImpact : MonoBehaviour
{
	public float Radius = 15f;

	public float Depth = 10f;

	public float Noise = 0.5f;

	public GameObject Explosion;

	private void OnCollisionEnter(Collision collision)
	{
		if (this.Explosion)
		{
			UnityEngine.Object.Instantiate(this.Explosion, collision.contacts[0].point, Quaternion.identity);
		}
		CraterMaker component = collision.gameObject.GetComponent<CraterMaker>();
		if (component)
		{
			component.Create(collision.contacts[0].point, this.Radius, this.Depth, this.Noise);
		}
		UnityEngine.Object.Destroy(base.gameObject);
	}
}
