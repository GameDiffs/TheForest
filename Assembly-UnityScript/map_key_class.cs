using System;

[Serializable]
public class map_key_class
{
	public int pulls_startDay;

	public int pulls_startHour;

	public int pulls_startMinute;

	public int pulls;

	public string key;

	public override void reset()
	{
		this.pulls = 0;
		this.pulls_startDay = DateTime.Now.Day;
		this.pulls_startHour = DateTime.Now.Hour;
		this.pulls_startMinute = DateTime.Now.Minute;
	}
}
