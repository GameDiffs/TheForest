using System;
using UnityEngine;

public class SomeBaseClass : MonoBehaviour
{
	[RPC]
	protected void PrintThis(string text)
	{
		Debug.Log(text);
	}
}
