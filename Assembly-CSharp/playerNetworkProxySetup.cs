using System;
using System.Collections.Generic;
using UnityEngine;

public class playerNetworkProxySetup : MonoBehaviour
{
	public GameObject[] deleteGoList;

	public GameObject[] disableGoList;

	private HashSet<Type> allowedTypes;

	private void Awake()
	{
	}

	public void doNetworkSetup()
	{
	}
}
