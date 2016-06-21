using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace Tasharen
{
	public class DataNode
	{
		public string name;

		public object value;

		public List<DataNode> children = new List<DataNode>();

		private static object[] mInvokeParams = new object[1];

		private static Dictionary<string, Type> mNameToType = new Dictionary<string, Type>();

		private static Dictionary<Type, string> mTypeToName = new Dictionary<Type, string>();

		public Type type
		{
			get
			{
				return (this.value == null) ? typeof(void) : this.value.GetType();
			}
		}

		public object Get(Type type)
		{
			return DataNode.ConvertValue(this.value, type);
		}

		public T Get<T>()
		{
			if (this.value is T)
			{
				return (T)((object)this.value);
			}
			object obj = this.Get(typeof(T));
			return (this.value == null) ? default(T) : ((T)((object)obj));
		}

		public DataNode AddChild()
		{
			DataNode dataNode = new DataNode();
			this.children.Add(dataNode);
			return dataNode;
		}

		public DataNode AddChild(string name)
		{
			DataNode dataNode = this.AddChild();
			dataNode.name = name;
			return dataNode;
		}

		public DataNode AddChild(string name, object value)
		{
			DataNode dataNode = this.AddChild();
			dataNode.name = name;
			dataNode.value = ((!(value is Enum)) ? value : value.ToString());
			return dataNode;
		}

		public DataNode SetChild(string name, object value)
		{
			DataNode dataNode = this.GetChild(name);
			if (dataNode == null)
			{
				dataNode = this.AddChild();
			}
			dataNode.name = name;
			dataNode.value = ((!(value is Enum)) ? value : value.ToString());
			return dataNode;
		}

		public DataNode GetChild(string name)
		{
			for (int i = 0; i < this.children.Count; i++)
			{
				if (this.children[i].name == name)
				{
					return this.children[i];
				}
			}
			return null;
		}

		public T GetChild<T>(string name)
		{
			DataNode child = this.GetChild(name);
			if (child == null)
			{
				return default(T);
			}
			return child.Get<T>();
		}

		public T GetChild<T>(string name, T defaultValue)
		{
			DataNode child = this.GetChild(name);
			if (child == null)
			{
				return defaultValue;
			}
			return child.Get<T>();
		}

		public void Write(StreamWriter writer)
		{
			this.Write(writer, 0);
		}

		public void Read(StreamReader reader)
		{
			string nextLine = DataNode.GetNextLine(reader);
			int num = DataNode.CalculateTabs(nextLine);
			this.Read(reader, nextLine, ref num);
		}

		public void Clear()
		{
			this.value = null;
			this.children.Clear();
		}

		private string GetValueDataString()
		{
			if (this.value is float)
			{
				return ((float)this.value).ToString(CultureInfo.InvariantCulture);
			}
			if (this.value is Vector2)
			{
				Vector2 vector = (Vector2)this.value;
				return vector.x.ToString(CultureInfo.InvariantCulture) + ", " + vector.y.ToString(CultureInfo.InvariantCulture);
			}
			if (this.value is Vector3)
			{
				Vector3 vector2 = (Vector3)this.value;
				return string.Concat(new string[]
				{
					vector2.x.ToString(CultureInfo.InvariantCulture),
					", ",
					vector2.y.ToString(CultureInfo.InvariantCulture),
					", ",
					vector2.z.ToString(CultureInfo.InvariantCulture)
				});
			}
			if (this.value is Vector4)
			{
				Vector4 vector3 = (Vector4)this.value;
				return string.Concat(new string[]
				{
					vector3.x.ToString(CultureInfo.InvariantCulture),
					", ",
					vector3.y.ToString(CultureInfo.InvariantCulture),
					", ",
					vector3.z.ToString(CultureInfo.InvariantCulture),
					", ",
					vector3.w.ToString(CultureInfo.InvariantCulture)
				});
			}
			if (this.value is Quaternion)
			{
				Vector3 eulerAngles = ((Quaternion)this.value).eulerAngles;
				return string.Concat(new string[]
				{
					eulerAngles.x.ToString(CultureInfo.InvariantCulture),
					", ",
					eulerAngles.y.ToString(CultureInfo.InvariantCulture),
					", ",
					eulerAngles.z.ToString(CultureInfo.InvariantCulture)
				});
			}
			if (this.value is Color)
			{
				Color color = (Color)this.value;
				return string.Concat(new string[]
				{
					color.r.ToString(CultureInfo.InvariantCulture),
					", ",
					color.g.ToString(CultureInfo.InvariantCulture),
					", ",
					color.b.ToString(CultureInfo.InvariantCulture),
					", ",
					color.a.ToString(CultureInfo.InvariantCulture)
				});
			}
			if (this.value is Color32)
			{
				Color color2 = (Color32)this.value;
				return string.Concat(new object[]
				{
					color2.r,
					", ",
					color2.g,
					", ",
					color2.b,
					", ",
					color2.a
				});
			}
			if (this.value is Rect)
			{
				Rect rect = (Rect)this.value;
				return string.Concat(new string[]
				{
					rect.x.ToString(CultureInfo.InvariantCulture),
					", ",
					rect.y.ToString(CultureInfo.InvariantCulture),
					", ",
					rect.width.ToString(CultureInfo.InvariantCulture),
					", ",
					rect.height.ToString(CultureInfo.InvariantCulture)
				});
			}
			if (this.value != null)
			{
				return this.value.ToString().Replace("\n", "\\n");
			}
			return string.Empty;
		}

		private string GetValueString()
		{
			if (this.type == typeof(string))
			{
				return "\"" + this.value + "\"";
			}
			if (this.type == typeof(Vector2) || this.type == typeof(Vector3) || this.type == typeof(Color))
			{
				return "(" + this.GetValueDataString() + ")";
			}
			return string.Format("{0}({1})", DataNode.TypeToName(this.type), this.GetValueDataString());
		}

		private bool SetValue(string text, Type type, string[] parts)
		{
			if (type == null || type == typeof(void))
			{
				this.value = null;
			}
			else if (type == typeof(string))
			{
				this.value = text;
			}
			else if (type == typeof(bool))
			{
				bool flag;
				if (bool.TryParse(text, out flag))
				{
					this.value = flag;
				}
			}
			else if (type == typeof(byte))
			{
				byte b;
				if (byte.TryParse(text, out b))
				{
					this.value = b;
				}
			}
			else if (type == typeof(short))
			{
				short num;
				if (short.TryParse(text, out num))
				{
					this.value = num;
				}
			}
			else if (type == typeof(ushort))
			{
				ushort num2;
				if (ushort.TryParse(text, out num2))
				{
					this.value = num2;
				}
			}
			else if (type == typeof(int))
			{
				int num3;
				if (int.TryParse(text, out num3))
				{
					this.value = num3;
				}
			}
			else if (type == typeof(uint))
			{
				uint num4;
				if (uint.TryParse(text, out num4))
				{
					this.value = num4;
				}
			}
			else if (type == typeof(float))
			{
				float num5;
				if (float.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, out num5))
				{
					this.value = num5;
				}
			}
			else if (type == typeof(double))
			{
				double num6;
				if (double.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, out num6))
				{
					this.value = num6;
				}
			}
			else if (type == typeof(Vector2))
			{
				if (parts == null)
				{
					parts = text.Split(new char[]
					{
						','
					});
				}
				Vector2 vector;
				if (parts.Length == 2 && float.TryParse(parts[0], NumberStyles.Float, CultureInfo.InvariantCulture, out vector.x) && float.TryParse(parts[1], NumberStyles.Float, CultureInfo.InvariantCulture, out vector.y))
				{
					this.value = vector;
				}
			}
			else if (type == typeof(Vector3))
			{
				if (parts == null)
				{
					parts = text.Split(new char[]
					{
						','
					});
				}
				Vector3 vector2;
				if (parts.Length == 3 && float.TryParse(parts[0], NumberStyles.Float, CultureInfo.InvariantCulture, out vector2.x) && float.TryParse(parts[1], NumberStyles.Float, CultureInfo.InvariantCulture, out vector2.y) && float.TryParse(parts[2], NumberStyles.Float, CultureInfo.InvariantCulture, out vector2.z))
				{
					this.value = vector2;
				}
			}
			else if (type == typeof(Vector4))
			{
				if (parts == null)
				{
					parts = text.Split(new char[]
					{
						','
					});
				}
				Vector4 vector3;
				if (parts.Length == 4 && float.TryParse(parts[0], NumberStyles.Float, CultureInfo.InvariantCulture, out vector3.x) && float.TryParse(parts[1], NumberStyles.Float, CultureInfo.InvariantCulture, out vector3.y) && float.TryParse(parts[2], NumberStyles.Float, CultureInfo.InvariantCulture, out vector3.z) && float.TryParse(parts[3], NumberStyles.Float, CultureInfo.InvariantCulture, out vector3.w))
				{
					this.value = vector3;
				}
			}
			else if (type == typeof(Quaternion))
			{
				if (parts == null)
				{
					parts = text.Split(new char[]
					{
						','
					});
				}
				Quaternion quaternion;
				if (parts.Length == 4 && float.TryParse(parts[0], NumberStyles.Float, CultureInfo.InvariantCulture, out quaternion.x) && float.TryParse(parts[1], NumberStyles.Float, CultureInfo.InvariantCulture, out quaternion.y) && float.TryParse(parts[2], NumberStyles.Float, CultureInfo.InvariantCulture, out quaternion.z) && float.TryParse(parts[3], NumberStyles.Float, CultureInfo.InvariantCulture, out quaternion.w))
				{
					this.value = quaternion;
				}
			}
			else if (type == typeof(Color32))
			{
				if (parts == null)
				{
					parts = text.Split(new char[]
					{
						','
					});
				}
				Color32 color;
				if (parts.Length == 4 && byte.TryParse(parts[0], out color.r) && byte.TryParse(parts[1], out color.g) && byte.TryParse(parts[2], out color.b) && byte.TryParse(parts[3], out color.a))
				{
					this.value = color;
				}
			}
			else if (type == typeof(Color))
			{
				if (parts == null)
				{
					parts = text.Split(new char[]
					{
						','
					});
				}
				Color color2;
				if (parts.Length == 4 && float.TryParse(parts[0], NumberStyles.Float, CultureInfo.InvariantCulture, out color2.r) && float.TryParse(parts[1], NumberStyles.Float, CultureInfo.InvariantCulture, out color2.g) && float.TryParse(parts[2], NumberStyles.Float, CultureInfo.InvariantCulture, out color2.b) && float.TryParse(parts[3], NumberStyles.Float, CultureInfo.InvariantCulture, out color2.a))
				{
					this.value = color2;
				}
			}
			else if (type == typeof(Rect))
			{
				if (parts == null)
				{
					parts = text.Split(new char[]
					{
						','
					});
				}
				Vector4 vector4;
				if (parts.Length == 4 && float.TryParse(parts[0], NumberStyles.Float, CultureInfo.InvariantCulture, out vector4.x) && float.TryParse(parts[1], NumberStyles.Float, CultureInfo.InvariantCulture, out vector4.y) && float.TryParse(parts[2], NumberStyles.Float, CultureInfo.InvariantCulture, out vector4.z) && float.TryParse(parts[3], NumberStyles.Float, CultureInfo.InvariantCulture, out vector4.w))
				{
					this.value = new Rect(vector4.x, vector4.y, vector4.z, vector4.w);
				}
			}
			else
			{
				if (!type.IsSubclassOf(typeof(Component)))
				{
					try
					{
						MethodInfo method = type.GetMethod("FromString", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
						if (method == null)
						{
							bool result = false;
							return result;
						}
						DataNode.mInvokeParams[0] = text.Replace("\\n", "\n");
						this.value = method.Invoke(null, DataNode.mInvokeParams);
					}
					catch (Exception ex)
					{
						Debug.LogWarning(ex.Message);
						bool result = false;
						return result;
					}
					return true;
				}
				return false;
			}
			return true;
		}

		public override string ToString()
		{
			string empty = string.Empty;
			this.Write(ref empty, 0);
			return empty;
		}

		private void Write(ref string data, int tab)
		{
			if (!string.IsNullOrEmpty(this.name))
			{
				for (int i = 0; i < tab; i++)
				{
					data += "\t";
				}
				data += DataNode.Escape(this.name);
				if (this.value != null)
				{
					data = data + " = " + this.GetValueString();
				}
				data += "\n";
				for (int j = 0; j < this.children.Count; j++)
				{
					this.children[j].Write(ref data, tab + 1);
				}
			}
		}

		private void Write(StreamWriter writer, int tab)
		{
			if (!string.IsNullOrEmpty(this.name))
			{
				for (int i = 0; i < tab; i++)
				{
					writer.Write("\t");
				}
				writer.Write(DataNode.Escape(this.name));
				if (this.value != null)
				{
					writer.Write(" = ");
					writer.Write(this.GetValueString());
				}
				writer.Write("\n");
				for (int j = 0; j < this.children.Count; j++)
				{
					this.children[j].Write(writer, tab + 1);
				}
			}
		}

		private string Read(StreamReader reader, string line, ref int offset)
		{
			if (line != null)
			{
				int num = offset;
				this.Set(line, num);
				line = DataNode.GetNextLine(reader);
				offset = DataNode.CalculateTabs(line);
				while (line != null)
				{
					if (offset != num + 1)
					{
						break;
					}
					line = this.AddChild().Read(reader, line, ref offset);
				}
			}
			return line;
		}

		private bool Set(string line, int offset)
		{
			int num = line.IndexOf("=", offset);
			if (num == -1)
			{
				this.name = DataNode.Unescape(line.Substring(offset)).Trim();
				return true;
			}
			this.name = DataNode.Unescape(line.Substring(offset, num - offset)).Trim();
			line = line.Substring(num + 1).Trim();
			if (line.Length < 3)
			{
				return false;
			}
			if (line[0] == '"' && line[line.Length - 1] == '"')
			{
				this.value = line.Substring(1, line.Length - 2);
				return true;
			}
			if (line[0] != '(' || line[line.Length - 1] != ')')
			{
				Type type = typeof(string);
				int num2 = line.IndexOf('(');
				if (num2 != -1)
				{
					int num3 = (line[line.Length - 1] != ')') ? line.LastIndexOf(')', num2) : (line.Length - 1);
					if (num3 != -1 && line.Length > 2)
					{
						string text = line.Substring(0, num2);
						type = DataNode.NameToType(text);
						line = line.Substring(num2 + 1, num3 - num2 - 1);
					}
				}
				return this.SetValue(line, type, null);
			}
			line = line.Substring(1, line.Length - 2);
			string[] array = line.Split(new char[]
			{
				','
			});
			if (array.Length == 1)
			{
				return this.SetValue(line, typeof(float), null);
			}
			if (array.Length == 2)
			{
				return this.SetValue(line, typeof(Vector2), array);
			}
			if (array.Length == 3)
			{
				return this.SetValue(line, typeof(Vector3), array);
			}
			if (array.Length == 4)
			{
				return this.SetValue(line, typeof(Color), array);
			}
			this.value = line;
			return true;
		}

		private static string GetNextLine(StreamReader reader)
		{
			string text = reader.ReadLine();
			while (text != null && text.Trim().StartsWith("//"))
			{
				text = reader.ReadLine();
				if (text == null)
				{
					return null;
				}
			}
			return text;
		}

		private static int CalculateTabs(string line)
		{
			if (line != null)
			{
				for (int i = 0; i < line.Length; i++)
				{
					if (line[i] != '\t')
					{
						return i;
					}
				}
			}
			return 0;
		}

		private static string Escape(string val)
		{
			if (!string.IsNullOrEmpty(val))
			{
				val = val.Replace("\n", "\\n");
				val = val.Replace("\t", "\\t");
			}
			return val;
		}

		private static string Unescape(string val)
		{
			if (!string.IsNullOrEmpty(val))
			{
				val = val.Replace("\\n", "\n");
				val = val.Replace("\\t", "\t");
			}
			return val;
		}

		private static Type NameToType(string name)
		{
			Type type;
			if (!DataNode.mNameToType.TryGetValue(name, out type))
			{
				type = Type.GetType(name);
				if (type == null)
				{
					if (name == "String")
					{
						type = typeof(string);
					}
					else if (name == "Vector2")
					{
						type = typeof(Vector2);
					}
					else if (name == "Vector3")
					{
						type = typeof(Vector3);
					}
					else if (name == "Vector4")
					{
						type = typeof(Vector4);
					}
					else if (name == "Quaternion")
					{
						type = typeof(Quaternion);
					}
					else if (name == "Color")
					{
						type = typeof(Color);
					}
					else if (name == "Rect")
					{
						type = typeof(Rect);
					}
					else if (name == "Color32")
					{
						type = typeof(Color32);
					}
				}
				DataNode.mNameToType[name] = type;
			}
			return type;
		}

		private static string TypeToName(Type type)
		{
			string text;
			if (!DataNode.mTypeToName.TryGetValue(type, out text))
			{
				text = type.ToString();
				if (text.StartsWith("System."))
				{
					text = text.Substring(7);
				}
				if (text.StartsWith("UnityEngine."))
				{
					text = text.Substring(12);
				}
				DataNode.mTypeToName[type] = text;
			}
			return text;
		}

		private static object ConvertValue(object value, Type type)
		{
			if (type.IsAssignableFrom(value.GetType()))
			{
				return value;
			}
			if (type.IsEnum)
			{
				if (value.GetType() == typeof(int))
				{
					return value;
				}
				if (value.GetType() == typeof(string))
				{
					string b = (string)value;
					if (!string.IsNullOrEmpty(b))
					{
						string[] names = Enum.GetNames(type);
						for (int i = 0; i < names.Length; i++)
						{
							if (names[i] == b)
							{
								return Enum.GetValues(type).GetValue(i);
							}
						}
					}
				}
			}
			return null;
		}
	}
}
