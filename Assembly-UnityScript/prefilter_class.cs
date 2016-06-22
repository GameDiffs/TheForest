using System;
using System.Collections.Generic;

[Serializable]
public class prefilter_class
{
	public string filter_text;

	public bool foldout;

	public List<int> filter_index;

	public bool filters_active;

	public bool filters_foldout;

	public prefilter_class()
	{
		this.filter_text = "Filter (1)";
		this.foldout = true;
		this.filter_index = new List<int>();
		this.filters_active = true;
		this.filters_foldout = true;
	}

	public override void set_filter_text()
	{
		if (this.filter_index.Count < 2)
		{
			this.filter_text = "Filter (" + this.filter_index.Count + ")";
		}
		else
		{
			this.filter_text = "Filters (" + this.filter_index.Count + ")";
		}
	}
}
