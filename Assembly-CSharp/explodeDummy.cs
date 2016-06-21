using System;
using UnityEngine;

public class explodeDummy : MonoBehaviour
{
	public GameObject explodedGo;

	public GameObject removeGo;

	private void Explosion(float dist)
	{
		UnityEngine.Object.Instantiate(this.explodedGo, base.transform.position, base.transform.rotation);
		UnityEngine.Object.Destroy(this.removeGo);
	}
}
