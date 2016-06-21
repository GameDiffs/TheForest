using System;

[Serializable]
public class EventConditionEntry
{
	public string conditionParam;

	public EventConditionParamTypes conditionParamType;

	public EventConditionModes conditionMode;

	public float floatValue;

	public int intValue;

	public bool boolValue;

	public void Reset()
	{
		this.conditionParam = null;
		this.conditionParamType = EventConditionParamTypes.Int;
		this.conditionMode = EventConditionModes.Equal;
		this.floatValue = 0f;
		this.intValue = 0;
		this.boolValue = false;
	}
}
