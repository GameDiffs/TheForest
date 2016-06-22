using System;
using UnityEngine;

[Serializable]
public class DestroyFast : MonoBehaviour
{
	public GameObject Group;

	public override void Awake()
	{
		this.Invoke("DestroyMe", (float)30);
	}

	public override void DestroyMe()
	{
		UnityEngine.Object.Destroy(this.gameObject);
	}

	public override void Main()
	{
	}
}
