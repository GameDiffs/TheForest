using System;
using System.Collections.Generic;

[Serializable]
public class tree_save_class
{
	public List<treeInstance_class> treeInstances;

	public tree_save_class()
	{
		this.treeInstances = new List<treeInstance_class>();
	}
}
