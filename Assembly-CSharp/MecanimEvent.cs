using System;
using System.Collections.Generic;

[Serializable]
public class MecanimEvent
{
	public string functionName;

	public float normalizedTime;

	public MecanimEventParamTypes paramType;

	public int intParam;

	public float floatParam;

	public string stringParam;

	public bool boolParam;

	public EventCondition condition;

	public bool critical;

	public bool isEnable = true;

	private EventContext context;

	public static EventContext Context
	{
		get;
		protected set;
	}

	public object parameter
	{
		get
		{
			switch (this.paramType)
			{
			case MecanimEventParamTypes.Int32:
				return this.intParam;
			case MecanimEventParamTypes.Float:
				return this.floatParam;
			case MecanimEventParamTypes.String:
				return this.stringParam;
			case MecanimEventParamTypes.Boolean:
				return this.boolParam;
			default:
				return null;
			}
		}
	}

	public MecanimEvent()
	{
		this.condition = new EventCondition();
	}

	public MecanimEvent(MecanimEvent other)
	{
		this.Copy(other);
	}

	public void Copy(MecanimEvent other)
	{
		this.normalizedTime = other.normalizedTime;
		this.functionName = other.functionName;
		this.paramType = other.paramType;
		switch (this.paramType)
		{
		case MecanimEventParamTypes.Int32:
			this.intParam = other.intParam;
			break;
		case MecanimEventParamTypes.Float:
			this.floatParam = other.floatParam;
			break;
		case MecanimEventParamTypes.String:
			this.stringParam = other.stringParam;
			break;
		case MecanimEventParamTypes.Boolean:
			this.boolParam = other.boolParam;
			break;
		}
		this.condition = new EventCondition();
		this.condition.conditions = new List<EventConditionEntry>(other.condition.conditions);
		this.critical = other.critical;
		this.isEnable = other.isEnable;
	}

	public void Reset()
	{
		this.normalizedTime = 0f;
		this.functionName = null;
		this.paramType = MecanimEventParamTypes.None;
		this.intParam = 0;
		this.floatParam = 0f;
		this.stringParam = null;
		this.boolParam = false;
		this.condition.Reset();
		this.critical = false;
		this.isEnable = false;
	}

	public EventContext GetContext()
	{
		return this.context;
	}

	public void SetContext(EventContext context)
	{
		this.context = context;
		this.context.current = this;
	}

	public static void SetCurrentContext(MecanimEvent e)
	{
		MecanimEvent.Context = e.context;
	}
}
