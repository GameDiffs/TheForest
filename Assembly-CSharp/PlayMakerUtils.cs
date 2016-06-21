using HutongGames.PlayMaker;
using System;
using System.Text.RegularExpressions;
using UnityEngine;

public class PlayMakerUtils
{
	public static void SendEventToGameObject(PlayMakerFSM fromFsm, GameObject target, string fsmEvent)
	{
		PlayMakerUtils.SendEventToGameObject(fromFsm, target, fsmEvent, null);
	}

	public static void SendEventToGameObject(PlayMakerFSM fromFsm, GameObject target, string fsmEvent, FsmEventData eventData)
	{
		if (eventData != null)
		{
			Fsm.EventData = eventData;
		}
		FsmEventTarget fsmEventTarget = new FsmEventTarget();
		fsmEventTarget.excludeSelf = false;
		fsmEventTarget.gameObject = new FsmOwnerDefault
		{
			OwnerOption = OwnerDefaultOption.SpecifyGameObject,
			GameObject = new FsmGameObject(),
			GameObject = 
			{
				Value = target
			}
		};
		fsmEventTarget.target = FsmEventTarget.EventTarget.GameObject;
		fsmEventTarget.sendToChildren = false;
		fromFsm.Fsm.Event(fsmEventTarget, fsmEvent);
	}

	public static void RefreshValueFromFsmVar(Fsm fromFsm, FsmVar fsmVar)
	{
		if (fromFsm == null)
		{
			return;
		}
		if (fsmVar == null)
		{
			return;
		}
		if (!fsmVar.useVariable)
		{
			return;
		}
		switch (fsmVar.Type)
		{
		case VariableType.Float:
			fsmVar.GetValueFrom(fromFsm.Variables.GetFsmFloat(fsmVar.variableName));
			break;
		case VariableType.Int:
			fsmVar.GetValueFrom(fromFsm.Variables.GetFsmInt(fsmVar.variableName));
			break;
		case VariableType.Bool:
			fsmVar.GetValueFrom(fromFsm.Variables.GetFsmBool(fsmVar.variableName));
			break;
		case VariableType.GameObject:
			fsmVar.GetValueFrom(fromFsm.Variables.GetFsmGameObject(fsmVar.variableName));
			break;
		case VariableType.String:
			fsmVar.GetValueFrom(fromFsm.Variables.GetFsmString(fsmVar.variableName));
			break;
		case VariableType.Vector2:
			fsmVar.GetValueFrom(fromFsm.Variables.GetFsmVector2(fsmVar.variableName));
			break;
		case VariableType.Vector3:
			fsmVar.GetValueFrom(fromFsm.Variables.GetFsmVector3(fsmVar.variableName));
			break;
		case VariableType.Color:
			fsmVar.GetValueFrom(fromFsm.Variables.GetFsmColor(fsmVar.variableName));
			break;
		case VariableType.Rect:
			fsmVar.GetValueFrom(fromFsm.Variables.GetFsmRect(fsmVar.variableName));
			break;
		case VariableType.Material:
			fsmVar.GetValueFrom(fromFsm.Variables.GetFsmMaterial(fsmVar.variableName));
			break;
		case VariableType.Texture:
			fsmVar.GetValueFrom(fromFsm.Variables.GetFsmVector3(fsmVar.variableName));
			break;
		case VariableType.Quaternion:
			fsmVar.GetValueFrom(fromFsm.Variables.GetFsmQuaternion(fsmVar.variableName));
			break;
		}
	}

	public static object GetValueFromFsmVar(Fsm fromFsm, FsmVar fsmVar)
	{
		if (fromFsm == null)
		{
			return null;
		}
		if (fsmVar == null)
		{
			return null;
		}
		if (fsmVar.useVariable)
		{
			string variableName = fsmVar.variableName;
			switch (fsmVar.Type)
			{
			case VariableType.Float:
				return fromFsm.Variables.GetFsmFloat(variableName).Value;
			case VariableType.Int:
				return fromFsm.Variables.GetFsmInt(variableName).Value;
			case VariableType.Bool:
				return fromFsm.Variables.GetFsmBool(variableName).Value;
			case VariableType.GameObject:
				return fromFsm.Variables.GetFsmGameObject(variableName).Value;
			case VariableType.String:
				return fromFsm.Variables.GetFsmString(variableName).Value;
			case VariableType.Vector2:
				return fromFsm.Variables.GetFsmVector2(variableName).Value;
			case VariableType.Vector3:
				return fromFsm.Variables.GetFsmVector3(variableName).Value;
			case VariableType.Color:
				return fromFsm.Variables.GetFsmColor(variableName).Value;
			case VariableType.Rect:
				return fromFsm.Variables.GetFsmRect(variableName).Value;
			case VariableType.Material:
				return fromFsm.Variables.GetFsmMaterial(variableName).Value;
			case VariableType.Texture:
				return fromFsm.Variables.GetFsmTexture(variableName).Value;
			case VariableType.Quaternion:
				return fromFsm.Variables.GetFsmQuaternion(variableName).Value;
			case VariableType.Object:
				return fromFsm.Variables.GetFsmObject(variableName).Value;
			}
		}
		else
		{
			switch (fsmVar.Type)
			{
			case VariableType.Float:
				return fsmVar.floatValue;
			case VariableType.Int:
				return fsmVar.intValue;
			case VariableType.Bool:
				return fsmVar.boolValue;
			case VariableType.GameObject:
				return fsmVar.gameObjectValue;
			case VariableType.String:
				return fsmVar.stringValue;
			case VariableType.Vector2:
				return fsmVar.vector2Value;
			case VariableType.Vector3:
				return fsmVar.vector3Value;
			case VariableType.Color:
				return fsmVar.colorValue;
			case VariableType.Rect:
				return fsmVar.rectValue;
			case VariableType.Material:
				return fsmVar.materialValue;
			case VariableType.Texture:
				return fsmVar.textureValue;
			case VariableType.Quaternion:
				return fsmVar.quaternionValue;
			case VariableType.Object:
				return fsmVar.objectReference;
			}
		}
		return null;
	}

	public static bool ApplyValueToFsmVar(Fsm fromFsm, FsmVar fsmVar, object value)
	{
		if (fromFsm == null)
		{
			return false;
		}
		if (fsmVar == null)
		{
			return false;
		}
		if (value == null)
		{
			if (fsmVar.Type == VariableType.Bool)
			{
				FsmBool fsmBool = fromFsm.Variables.GetFsmBool(fsmVar.variableName);
				fsmBool.Value = false;
			}
			else if (fsmVar.Type == VariableType.Color)
			{
				FsmColor fsmColor = fromFsm.Variables.GetFsmColor(fsmVar.variableName);
				fsmColor.Value = Color.black;
			}
			else if (fsmVar.Type == VariableType.Int)
			{
				FsmInt fsmInt = fromFsm.Variables.GetFsmInt(fsmVar.variableName);
				fsmInt.Value = 0;
			}
			else if (fsmVar.Type == VariableType.Float)
			{
				FsmFloat fsmFloat = fromFsm.Variables.GetFsmFloat(fsmVar.variableName);
				fsmFloat.Value = 0f;
			}
			else if (fsmVar.Type == VariableType.GameObject)
			{
				FsmGameObject fsmGameObject = fromFsm.Variables.GetFsmGameObject(fsmVar.variableName);
				fsmGameObject.Value = null;
			}
			else if (fsmVar.Type == VariableType.Material)
			{
				FsmMaterial fsmMaterial = fromFsm.Variables.GetFsmMaterial(fsmVar.variableName);
				fsmMaterial.Value = null;
			}
			else if (fsmVar.Type == VariableType.Object)
			{
				FsmObject fsmObject = fromFsm.Variables.GetFsmObject(fsmVar.variableName);
				fsmObject.Value = null;
			}
			else if (fsmVar.Type == VariableType.Quaternion)
			{
				FsmQuaternion fsmQuaternion = fromFsm.Variables.GetFsmQuaternion(fsmVar.variableName);
				fsmQuaternion.Value = Quaternion.identity;
			}
			else if (fsmVar.Type == VariableType.Rect)
			{
				FsmRect fsmRect = fromFsm.Variables.GetFsmRect(fsmVar.variableName);
				fsmRect.Value = new Rect(0f, 0f, 0f, 0f);
			}
			else if (fsmVar.Type == VariableType.String)
			{
				FsmString fsmString = fromFsm.Variables.GetFsmString(fsmVar.variableName);
				fsmString.Value = string.Empty;
			}
			else if (fsmVar.Type == VariableType.String)
			{
				FsmTexture fsmTexture = fromFsm.Variables.GetFsmTexture(fsmVar.variableName);
				fsmTexture.Value = null;
			}
			else if (fsmVar.Type == VariableType.Vector2)
			{
				FsmVector2 fsmVector = fromFsm.Variables.GetFsmVector2(fsmVar.variableName);
				fsmVector.Value = Vector2.zero;
			}
			else if (fsmVar.Type == VariableType.Vector3)
			{
				FsmVector3 fsmVector2 = fromFsm.Variables.GetFsmVector3(fsmVar.variableName);
				fsmVector2.Value = Vector3.zero;
			}
			return true;
		}
		Type type = value.GetType();
		Type type2 = null;
		switch (fsmVar.Type)
		{
		case VariableType.Float:
			type2 = typeof(float);
			break;
		case VariableType.Int:
			type2 = typeof(int);
			break;
		case VariableType.Bool:
			type2 = typeof(bool);
			break;
		case VariableType.GameObject:
			type2 = typeof(GameObject);
			break;
		case VariableType.String:
			type2 = typeof(string);
			break;
		case VariableType.Vector2:
			type2 = typeof(Vector2);
			break;
		case VariableType.Vector3:
			type2 = typeof(Vector3);
			break;
		case VariableType.Color:
			type2 = typeof(Color);
			break;
		case VariableType.Rect:
			type2 = typeof(Rect);
			break;
		case VariableType.Material:
			type2 = typeof(Material);
			break;
		case VariableType.Texture:
			type2 = typeof(Texture2D);
			break;
		case VariableType.Quaternion:
			type2 = typeof(Quaternion);
			break;
		case VariableType.Object:
			type2 = typeof(UnityEngine.Object);
			break;
		}
		if (!type2.Equals(type))
		{
			if (!type.Equals(typeof(ProceduralMaterial)))
			{
				Debug.LogError(string.Concat(new object[]
				{
					"The fsmVar value <",
					type2,
					"> doesn't match the value <",
					type,
					">"
				}));
				return false;
			}
		}
		if (type == typeof(bool))
		{
			FsmBool fsmBool2 = fromFsm.Variables.GetFsmBool(fsmVar.variableName);
			fsmBool2.Value = (bool)value;
		}
		else if (type == typeof(Color))
		{
			FsmColor fsmColor2 = fromFsm.Variables.GetFsmColor(fsmVar.variableName);
			fsmColor2.Value = (Color)value;
		}
		else if (type == typeof(int))
		{
			FsmInt fsmInt2 = fromFsm.Variables.GetFsmInt(fsmVar.variableName);
			fsmInt2.Value = (int)value;
		}
		else if (type == typeof(float))
		{
			FsmFloat fsmFloat2 = fromFsm.Variables.GetFsmFloat(fsmVar.variableName);
			fsmFloat2.Value = (float)value;
		}
		else if (type == typeof(GameObject))
		{
			FsmGameObject fsmGameObject2 = fromFsm.Variables.GetFsmGameObject(fsmVar.variableName);
			fsmGameObject2.Value = (GameObject)value;
		}
		else if (type == typeof(Material))
		{
			FsmMaterial fsmMaterial2 = fromFsm.Variables.GetFsmMaterial(fsmVar.variableName);
			fsmMaterial2.Value = (Material)value;
		}
		else if (type == typeof(ProceduralMaterial))
		{
			FsmMaterial fsmMaterial3 = fromFsm.Variables.GetFsmMaterial(fsmVar.variableName);
			fsmMaterial3.Value = (ProceduralMaterial)value;
		}
		else if (type == typeof(UnityEngine.Object))
		{
			FsmObject fsmObject2 = fromFsm.Variables.GetFsmObject(fsmVar.variableName);
			fsmObject2.Value = (UnityEngine.Object)value;
		}
		else if (type == typeof(Quaternion))
		{
			FsmQuaternion fsmQuaternion2 = fromFsm.Variables.GetFsmQuaternion(fsmVar.variableName);
			fsmQuaternion2.Value = (Quaternion)value;
		}
		else if (type == typeof(Rect))
		{
			FsmRect fsmRect2 = fromFsm.Variables.GetFsmRect(fsmVar.variableName);
			fsmRect2.Value = (Rect)value;
		}
		else if (type == typeof(string))
		{
			FsmString fsmString2 = fromFsm.Variables.GetFsmString(fsmVar.variableName);
			fsmString2.Value = (string)value;
		}
		else if (type == typeof(Texture2D))
		{
			FsmTexture fsmTexture2 = fromFsm.Variables.GetFsmTexture(fsmVar.variableName);
			fsmTexture2.Value = (Texture2D)value;
		}
		else if (type == typeof(Vector2))
		{
			FsmVector2 fsmVector3 = fromFsm.Variables.GetFsmVector2(fsmVar.variableName);
			fsmVector3.Value = (Vector2)value;
		}
		else if (type == typeof(Vector3))
		{
			FsmVector3 fsmVector4 = fromFsm.Variables.GetFsmVector3(fsmVar.variableName);
			fsmVector4.Value = (Vector3)value;
		}
		else
		{
			Debug.LogWarning("?!?!" + type);
		}
		return true;
	}

	public static string ParseValueToString(object item, bool useBytes)
	{
		return string.Empty;
	}

	public static string ParseValueToString(object item)
	{
		if (item.GetType() == typeof(string))
		{
			return "string(" + item.ToString() + ")";
		}
		if (item.GetType() == typeof(bool))
		{
			int num = (!(bool)item) ? 0 : 1;
			return "bool(" + num + ")";
		}
		if (item.GetType() == typeof(float))
		{
			float num2 = float.Parse(item.ToString());
			return "float(" + num2 + ")";
		}
		if (item.GetType() == typeof(int))
		{
			int num3 = int.Parse(item.ToString());
			return "int(" + num3 + ")";
		}
		if (item.GetType() == typeof(Vector2))
		{
			Vector2 vector = (Vector2)item;
			return string.Concat(new object[]
			{
				"vector2(",
				vector.x,
				",",
				vector.y,
				")"
			});
		}
		if (item.GetType() == typeof(Vector3))
		{
			Vector3 vector2 = (Vector3)item;
			return string.Concat(new object[]
			{
				"vector3(",
				vector2.x,
				",",
				vector2.y,
				",",
				vector2.z,
				")"
			});
		}
		if (item.GetType() == typeof(Vector4))
		{
			Vector4 vector3 = (Vector4)item;
			return string.Concat(new object[]
			{
				"vector4(",
				vector3.x,
				",",
				vector3.y,
				",",
				vector3.z,
				",",
				vector3.w,
				")"
			});
		}
		if (item.GetType() == typeof(Quaternion))
		{
			Quaternion quaternion = (Quaternion)item;
			return string.Concat(new object[]
			{
				"quaternion(",
				quaternion.x,
				",",
				quaternion.y,
				",",
				quaternion.z,
				",",
				quaternion.w,
				")"
			});
		}
		if (item.GetType() == typeof(Rect))
		{
			Rect rect = (Rect)item;
			return string.Concat(new object[]
			{
				"quaternion(",
				rect.x,
				",",
				rect.y,
				",",
				rect.width,
				",",
				rect.height,
				")"
			});
		}
		if (item.GetType() == typeof(Color))
		{
			Color color = (Color)item;
			return string.Concat(new object[]
			{
				"color(",
				color.r,
				",",
				color.g,
				",",
				color.b,
				",",
				color.a,
				")"
			});
		}
		if (item.GetType() == typeof(GameObject))
		{
			GameObject gameObject = (GameObject)item;
			return "gameObject(" + gameObject.name + ")";
		}
		Debug.LogWarning("ParseValueToString type not supported " + item.GetType());
		return "<" + item.GetType() + "> not supported";
	}

	public static object ParseValueFromString(string source, bool useBytes)
	{
		return null;
	}

	public static object ParseValueFromString(string source)
	{
		if (source == null)
		{
			return null;
		}
		if (source.StartsWith("string("))
		{
			source = source.Substring(7, source.Length - 8);
			return source;
		}
		if (source.StartsWith("bool("))
		{
			source = source.Substring(5, source.Length - 6);
			bool flag = int.Parse(source) == 1;
			return flag;
		}
		if (source.StartsWith("int("))
		{
			source = source.Substring(4, source.Length - 5);
			int num = int.Parse(source);
			return num;
		}
		if (source.StartsWith("float("))
		{
			source = source.Substring(6, source.Length - 7);
			float num2 = float.Parse(source);
			return num2;
		}
		if (source.StartsWith("vector2("))
		{
			string text = "vector2\\([x],[y]\\)";
			string str = "[-+]?[0-9]*\\.?[0-9]+([eE][-+]?[0-9]+)?";
			text = text.Replace("[x]", "(?<x>" + str + ")");
			text = text.Replace("[y]", "(?<y>" + str + ")");
			text = "^\\s*" + text;
			Regex regex = new Regex(text);
			Match match = regex.Match(source);
			if (match.Groups["x"].Value != string.Empty && match.Groups["y"].Value != string.Empty)
			{
				return new Vector2(float.Parse(match.Groups["x"].Value), float.Parse(match.Groups["y"].Value));
			}
			return Vector2.zero;
		}
		else if (source.StartsWith("vector3("))
		{
			string text2 = "vector3\\([x],[y],[z]\\)";
			string str2 = "[-+]?[0-9]*\\.?[0-9]+([eE][-+]?[0-9]+)?";
			text2 = text2.Replace("[x]", "(?<x>" + str2 + ")");
			text2 = text2.Replace("[y]", "(?<y>" + str2 + ")");
			text2 = text2.Replace("[z]", "(?<z>" + str2 + ")");
			text2 = "^\\s*" + text2;
			Regex regex2 = new Regex(text2);
			Match match2 = regex2.Match(source);
			if (match2.Groups["x"].Value != string.Empty && match2.Groups["y"].Value != string.Empty && match2.Groups["z"].Value != string.Empty)
			{
				return new Vector3(float.Parse(match2.Groups["x"].Value), float.Parse(match2.Groups["y"].Value), float.Parse(match2.Groups["z"].Value));
			}
			return Vector3.zero;
		}
		else if (source.StartsWith("vector4("))
		{
			string text3 = "vector4\\([x],[y],[z],[w]\\)";
			string str3 = "[-+]?[0-9]*\\.?[0-9]+([eE][-+]?[0-9]+)?";
			text3 = text3.Replace("[x]", "(?<x>" + str3 + ")");
			text3 = text3.Replace("[y]", "(?<y>" + str3 + ")");
			text3 = text3.Replace("[z]", "(?<z>" + str3 + ")");
			text3 = text3.Replace("[w]", "(?<w>" + str3 + ")");
			text3 = "^\\s*" + text3;
			Regex regex3 = new Regex(text3);
			Match match3 = regex3.Match(source);
			if (match3.Groups["x"].Value != string.Empty && match3.Groups["y"].Value != string.Empty && match3.Groups["z"].Value != string.Empty && match3.Groups["z"].Value != string.Empty)
			{
				return new Vector4(float.Parse(match3.Groups["x"].Value), float.Parse(match3.Groups["y"].Value), float.Parse(match3.Groups["z"].Value), float.Parse(match3.Groups["w"].Value));
			}
			return Vector4.zero;
		}
		else if (source.StartsWith("rect("))
		{
			string text4 = "rect\\([x],[y],[w],[h]\\)";
			string str4 = "[-+]?[0-9]*\\.?[0-9]+([eE][-+]?[0-9]+)?";
			text4 = text4.Replace("[x]", "(?<x>" + str4 + ")");
			text4 = text4.Replace("[y]", "(?<y>" + str4 + ")");
			text4 = text4.Replace("[w]", "(?<w>" + str4 + ")");
			text4 = text4.Replace("[h]", "(?<h>" + str4 + ")");
			text4 = "^\\s*" + text4;
			Regex regex4 = new Regex(text4);
			Match match4 = regex4.Match(source);
			if (match4.Groups["x"].Value != string.Empty && match4.Groups["y"].Value != string.Empty && match4.Groups["w"].Value != string.Empty && match4.Groups["h"].Value != string.Empty)
			{
				return new Rect(float.Parse(match4.Groups["x"].Value), float.Parse(match4.Groups["y"].Value), float.Parse(match4.Groups["w"].Value), float.Parse(match4.Groups["h"].Value));
			}
			return new Rect(0f, 0f, 0f, 0f);
		}
		else if (source.StartsWith("quaternion("))
		{
			string text5 = "quaternion\\([x],[y],[z],[w]\\)";
			string str5 = "[-+]?[0-9]*\\.?[0-9]+([eE][-+]?[0-9]+)?";
			text5 = text5.Replace("[x]", "(?<x>" + str5 + ")");
			text5 = text5.Replace("[y]", "(?<y>" + str5 + ")");
			text5 = text5.Replace("[z]", "(?<z>" + str5 + ")");
			text5 = text5.Replace("[w]", "(?<w>" + str5 + ")");
			text5 = "^\\s*" + text5;
			Regex regex5 = new Regex(text5);
			Match match5 = regex5.Match(source);
			if (match5.Groups["x"].Value != string.Empty && match5.Groups["y"].Value != string.Empty && match5.Groups["z"].Value != string.Empty && match5.Groups["z"].Value != string.Empty)
			{
				return new Quaternion(float.Parse(match5.Groups["x"].Value), float.Parse(match5.Groups["y"].Value), float.Parse(match5.Groups["z"].Value), float.Parse(match5.Groups["w"].Value));
			}
			return Quaternion.identity;
		}
		else if (source.StartsWith("color("))
		{
			string text6 = "color\\([r],[g],[b],[a]\\)";
			string str6 = "[-+]?[0-9]*\\.?[0-9]+([eE][-+]?[0-9]+)?";
			text6 = text6.Replace("[r]", "(?<r>" + str6 + ")");
			text6 = text6.Replace("[g]", "(?<g>" + str6 + ")");
			text6 = text6.Replace("[b]", "(?<b>" + str6 + ")");
			text6 = text6.Replace("[a]", "(?<a>" + str6 + ")");
			text6 = "^\\s*" + text6;
			Regex regex6 = new Regex(text6);
			Match match6 = regex6.Match(source);
			if (match6.Groups["r"].Value != string.Empty && match6.Groups["g"].Value != string.Empty && match6.Groups["b"].Value != string.Empty && match6.Groups["a"].Value != string.Empty)
			{
				return new Color(float.Parse(match6.Groups["r"].Value), float.Parse(match6.Groups["g"].Value), float.Parse(match6.Groups["b"].Value), float.Parse(match6.Groups["a"].Value));
			}
			return Color.black;
		}
		else
		{
			if (source.StartsWith("gameObject("))
			{
				source = source.Substring(11, source.Length - 12);
				return GameObject.Find(source);
			}
			Debug.LogWarning("ParseValueFromString failed for " + source);
			return null;
		}
	}
}
