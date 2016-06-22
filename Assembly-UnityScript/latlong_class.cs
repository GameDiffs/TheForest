using System;

[Serializable]
public class latlong_class
{
	public double latitude;

	public double longitude;

	public latlong_class()
	{
	}

	public latlong_class(double latitude1, double longitude1)
	{
		this.latitude = latitude1;
		this.longitude = longitude1;
	}

	public override void reset()
	{
		this.latitude = (double)0;
		this.longitude = (double)0;
	}
}
