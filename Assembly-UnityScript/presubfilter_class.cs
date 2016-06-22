using System;
using System.Collections.Generic;

[Serializable]
public class presubfilter_class
{
	public string subfilter_text;

	public bool foldout;

	public bool subfilters_active;

	public bool subfilters_foldout;

	public List<int> subfilter_index;

	public presubfilter_class()
	{
		this.subfilter_text = "Masks (0)";
		this.foldout = true;
		this.subfilters_active = true;
		this.subfilters_foldout = true;
		this.subfilter_index = new List<int>();
	}

	public override void set_subfilter_text(int length)
	{
		if (length > 1)
		{
			this.subfilter_text = "Masks (" + length + ")";
		}
		else
		{
			this.subfilter_text = "Mask (" + length + ")";
		}
	}
}
