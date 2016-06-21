using System;
using UnityEngine;

public class AddTreeComponent : MonoBehaviour
{
	private void Start()
	{
		base.gameObject.AddComponent<Tree>();
	}
}
