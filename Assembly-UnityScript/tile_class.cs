using System;

[Serializable]
public class tile_class
{
	public int x;

	public int y;

	private bool $initialized__tile_class$;

	public tile_class()
	{
		if (!this.$initialized__tile_class$)
		{
			this.$initialized__tile_class$ = true;
		}
	}

	public tile_class(int x1, int y2)
	{
		if (!this.$initialized__tile_class$)
		{
			this.$initialized__tile_class$ = true;
		}
		this.x = x1;
		this.y = y2;
	}

	public override void reset()
	{
		this.x = 0;
		this.y = 0;
	}
}
