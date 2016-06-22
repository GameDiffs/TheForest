using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class save_trees : MonoBehaviour
{
	public List<tree_save_class> tree_save;

	public int treeTypes;

	public save_trees()
	{
		this.tree_save = new List<tree_save_class>();
	}

	public override void Main()
	{
	}
}
