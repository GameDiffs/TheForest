using System;

[Serializable]
public class neighbor_class
{
	public int left;

	public int right;

	public int top;

	public int bottom;

	public int top_left;

	public int top_right;

	public int bottom_left;

	public int bottom_right;

	public int self;

	public neighbor_class()
	{
		this.left = -1;
		this.right = -1;
		this.top = -1;
		this.bottom = -1;
		this.top_left = -1;
		this.top_right = -1;
		this.bottom_left = -1;
		this.bottom_right = -1;
	}
}
