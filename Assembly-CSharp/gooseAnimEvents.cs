using System;
using UnityEngine;

public class gooseAnimEvents : MonoBehaviour
{
	public Transform splashParticle;

	public Transform leftFoot;

	public Transform rightFoot;

	private void splashLeft()
	{
		Transform transform = UnityEngine.Object.Instantiate(this.splashParticle, this.leftFoot.position, base.transform.rotation) as Transform;
		float num = UnityEngine.Random.Range(1f, 2f);
		transform.localScale = new Vector3(num, num, num);
	}

	private void splashRight()
	{
		Transform transform = UnityEngine.Object.Instantiate(this.splashParticle, this.rightFoot.position, base.transform.rotation) as Transform;
		float num = UnityEngine.Random.Range(1f, 2f);
		transform.localScale = new Vector3(num, num, num);
	}
}
