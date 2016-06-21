using System;
using UnityEngine;

[AddComponentMenu("Storage/Rooms/Examples/Player Locator")]
public class PlayerLocator : MonoBehaviour
{
	public static PlayerLocator Current;

	public static GameObject PlayerGameObject;

	private void Awake()
	{
		UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
		PlayerLocator.Current = this;
		PlayerLocator.PlayerGameObject = base.gameObject;
	}
}
