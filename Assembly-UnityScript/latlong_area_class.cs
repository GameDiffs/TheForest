using System;

[Serializable]
public class latlong_area_class
{
	public latlong_class latlong1;

	public latlong_class latlong2;

	public latlong_area_class()
	{
		this.latlong1 = new latlong_class();
		this.latlong2 = new latlong_class();
	}
}
