using System;
using System.Collections.Generic;

[Serializable]
public class grass_save_class
{
	public List<detail_save_class> details;

	public int resolution;

	public grass_save_class()
	{
		this.details = new List<detail_save_class>();
	}
}
