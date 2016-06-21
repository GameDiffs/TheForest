using System;
using UnityEngine;

[AddComponentMenu("Storage/Tests/Simple Caller")]
public class SimpleCaller : MonoBehaviour
{
	public NetworkView otherView;

	private void Start()
	{
		Network.InitializeServer(200, 8081, true);
	}

	private void Update()
	{
	}

	private void OnGUI()
	{
		if (GUILayout.Button("Call Print", new GUILayoutOption[0]))
		{
			this.otherView.RPCEx("PrintThis", RPCMode.All, new object[]
			{
				"Hello World"
			});
		}
	}
}
