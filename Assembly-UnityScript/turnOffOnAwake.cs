using System;
using UnityEngine;

[Serializable]
public class turnOffOnAwake : MonoBehaviour
{
	public override void Awake()
	{
		this.gameObject.SetActive(false);
	}

	public override void Main()
	{
	}
}
