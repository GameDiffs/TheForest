using System;

[Serializable]
public class tree_parameter_class
{
	public int prototype;

	public float density;

	public float scale;

	public tree_parameter_class()
	{
		this.density = (float)1;
		this.scale = (float)1;
	}
}
