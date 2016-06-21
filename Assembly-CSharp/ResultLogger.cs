using System;
using System.Collections;
using System.Text;
using UnityEngine;

public class ResultLogger : UnityEngine.Object
{
	public static void logObject(object result)
	{
		if (result.GetType() == typeof(ArrayList))
		{
			ResultLogger.logArraylist((ArrayList)result);
		}
		else if (result.GetType() == typeof(Hashtable))
		{
			ResultLogger.logHashtable((Hashtable)result);
		}
		else
		{
			Debug.Log("result is not a hashtable or arraylist");
		}
	}

	public static void logArraylist(ArrayList result)
	{
		StringBuilder stringBuilder = new StringBuilder();
		foreach (Hashtable item in result)
		{
			ResultLogger.addHashtableToString(stringBuilder, item);
			stringBuilder.Append("\n--------------------\n");
		}
		Debug.Log(stringBuilder.ToString());
	}

	public static void logHashtable(Hashtable result)
	{
		StringBuilder stringBuilder = new StringBuilder();
		ResultLogger.addHashtableToString(stringBuilder, result);
		Debug.Log(stringBuilder.ToString());
	}

	public static void addHashtableToString(StringBuilder builder, Hashtable item)
	{
		foreach (DictionaryEntry dictionaryEntry in item)
		{
			if (dictionaryEntry.Value is Hashtable)
			{
				builder.AppendFormat("{0}: ", dictionaryEntry.Key);
				ResultLogger.addHashtableToString(builder, (Hashtable)dictionaryEntry.Value);
			}
			else if (dictionaryEntry.Value is ArrayList)
			{
				builder.AppendFormat("{0}: ", dictionaryEntry.Key);
				ResultLogger.addArraylistToString(builder, (ArrayList)dictionaryEntry.Value);
			}
			else
			{
				builder.AppendFormat("{0}: {1}\n", dictionaryEntry.Key, dictionaryEntry.Value);
			}
		}
	}

	public static void addArraylistToString(StringBuilder builder, ArrayList result)
	{
		foreach (object current in result)
		{
			if (current is Hashtable)
			{
				ResultLogger.addHashtableToString(builder, (Hashtable)current);
			}
			else if (current is ArrayList)
			{
				ResultLogger.addArraylistToString(builder, (ArrayList)current);
			}
			builder.Append("\n--------------------\n");
		}
		Debug.Log(builder.ToString());
	}
}
