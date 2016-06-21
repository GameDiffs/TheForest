using Serialization;
using System;
using System.Collections.Generic;

public class ProvideAttributes : IProvideAttributeList
{
	private string[] _attributes;

	protected bool AllSimple = true;

	public ProvideAttributes(string[] attributes) : this(attributes, true)
	{
	}

	public ProvideAttributes(string[] attributes, bool allSimple)
	{
		this._attributes = attributes;
		this.AllSimple = allSimple;
	}

	public IEnumerable<string> GetAttributeList(Type tp)
	{
		return this._attributes;
	}

	public virtual bool AllowAllSimple(Type tp)
	{
		return this.AllSimple;
	}
}
