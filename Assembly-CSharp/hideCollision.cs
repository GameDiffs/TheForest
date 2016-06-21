using System;
using UnityEngine;

public class hideCollision : MonoBehaviour
{
	public bool destroy;

	private void Start()
	{
		base.Invoke("hideGameObject", 1f);
	}

	private void hideGameObject()
	{
		if (this.destroy)
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
		else
		{
			base.gameObject.SetActive(false);
		}
	}
}
