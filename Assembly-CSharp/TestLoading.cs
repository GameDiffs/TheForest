using System;
using UnityEngine;

[AddComponentMenu("Storage/Tests/Test Loading")]
public class TestLoading : MonoBehaviour
{
	private void Start()
	{
		UnityEngine.Object.DontDestroyOnLoad(this);
		base.Invoke("LoadLevel", 0.3f);
	}

	private void LoadLevel()
	{
		Application.LoadLevel("Example");
	}
}
