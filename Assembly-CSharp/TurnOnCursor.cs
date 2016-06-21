using System;
using TheForest.Utils;
using UnityEngine;

public class TurnOnCursor : MonoBehaviour
{
	private void Start()
	{
		TheForest.Utils.Input.UnLockMouse();
	}
}
