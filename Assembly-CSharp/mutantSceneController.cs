using System;
using UnityEngine;

public class mutantSceneController : MonoBehaviour
{
	public GameObject mutant;

	private void startAnimation()
	{
		if (this.mutant)
		{
			base.Invoke("enableMutant", 0.5f);
		}
		base.transform.GetComponent<Animator>().SetBool("begin", true);
		base.Invoke("cleanUp", 14f);
	}

	private void enableMutant()
	{
		this.mutant.SetActive(true);
	}

	private void cleanUp()
	{
		UnityEngine.Object.Destroy(base.gameObject);
	}
}
