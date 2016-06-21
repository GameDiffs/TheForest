using System;
using UnityEngine;

public class enableChild : MonoBehaviour
{
	public GameObject child1;

	private void Start()
	{
		base.Invoke("enableChildGo", 0.5f);
	}

	private void enableChildGo()
	{
		this.child1.SetActive(true);
	}
}
