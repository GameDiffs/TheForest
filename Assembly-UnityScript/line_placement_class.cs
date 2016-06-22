using System;
using System.Collections.Generic;

[Serializable]
public class line_placement_class
{
	public bool foldout;

	public image_class preimage;

	public List<line_list_class> line_list;

	public bool line_list_foldout;

	public line_placement_class()
	{
		this.preimage = new image_class();
		this.line_list = new List<line_list_class>();
	}
}
