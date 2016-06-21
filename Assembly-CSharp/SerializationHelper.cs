using System;
using UnityEngine;

public static class SerializationHelper
{
	public static bool IsDeserializing(this GameObject go)
	{
		UniqueIdentifier component = go.GetComponent<UniqueIdentifier>();
		return component != null && component.IsDeserializing;
	}
}
