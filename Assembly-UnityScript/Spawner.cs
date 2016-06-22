using System;
using UnityEngine;

[Serializable]
public class Spawner : MonoBehaviour
{
	public GameObject prefab;

	public Transform target;

	public GameObject other;

	[NonSerialized]
	public static int numberSpawned;

	public Mesh createMesh;

	public string[] testArrayItems;

	public Spawner()
	{
		this.testArrayItems = new string[10];
	}

	public override void Start()
	{
	}

	public override void OnGUI()
	{
		GUILayout.BeginArea(new Rect((float)0, (float)(Screen.height - 60), (float)100, (float)100));
		GUILayout.Label(Spawner.numberSpawned.ToString(), new GUILayoutOption[0]);
		GUILayout.EndArea();
	}

	public override void Main()
	{
	}
}
