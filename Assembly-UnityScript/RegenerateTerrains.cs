using System;
using UnityEngine;

[Serializable]
public class RegenerateTerrains : MonoBehaviour
{
	public RuntimeTerrains script;

	public override void Start()
	{
	}

	public override void Update()
	{
		if (Input.GetKeyDown("g"))
		{
			this.script.createTerrainsOnTheFly = false;
			this.StartCoroutine_Auto(this.script.GenerateStart());
		}
	}

	public override void Main()
	{
	}
}
