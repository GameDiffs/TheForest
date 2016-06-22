using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class save_grass : MonoBehaviour
{
	public List<grass_save_class> grass_save;

	public save_grass()
	{
		this.grass_save = new List<grass_save_class>();
	}

	public override void Main()
	{
	}
}
