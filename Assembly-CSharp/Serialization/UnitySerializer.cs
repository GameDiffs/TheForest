using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Text;
using UniLinq;
using UnityEngine;

namespace Serialization
{
	public static class UnitySerializer
	{
		public struct FinalProcess
		{
			public List<Action> deferredActions;

			public List<UnitySerializer.DeferredSetter> deferredSetters;

			public List<IDeserialized> deserializedObjects;
		}

		public class Nuller
		{
		}

		private class TypePusher : IDisposable
		{
			public TypePusher(Type t)
			{
				UnitySerializer.currentTypes.Push(t);
			}

			public void Dispose()
			{
				UnitySerializer.currentTypes.Pop();
			}
		}

		public class DeferredSetter
		{
			public int priority;

			public readonly UnitySerializer.GetData deferredRetrievalFunction;

			public bool enabled = true;

			internal readonly Dictionary<string, object> parameters = new Dictionary<string, object>();

			internal Action _setAction;

			public DeferredSetter(UnitySerializer.GetData retrievalFunction)
			{
				this.deferredRetrievalFunction = retrievalFunction;
			}
		}

		private class EntryConfiguration
		{
			public GetSet Setter;

			public Type Type;
		}

		public class ForceJSON : IDisposable
		{
			public ForceJSON()
			{
				UnitySerializer._forceJSON++;
			}

			public void Dispose()
			{
				UnitySerializer._forceJSON--;
			}
		}

		private class KnownTypesStackEntry
		{
			public List<Type> knownTypesList;

			public Dictionary<Type, ushort> knownTypesLookup;
		}

		public class MissingConstructorException : Exception
		{
			public MissingConstructorException(string message) : base(message)
			{
			}
		}

		public class ObjectMappingEventArgs : EventArgs
		{
			public object Instance;

			public Type TypeToConstruct;
		}

		internal class PropertyNameStackEntry
		{
			public List<string> propertyList;

			public Dictionary<string, ushort> propertyLookup;
		}

		public class SerializationScope : IDisposable
		{
			private static Stack<bool> _primaryScopeStack = new Stack<bool>();

			private static bool _hasSetPrimaryScope;

			private static bool _primaryScope;

			internal static int _counter = 0;

			private readonly List<UnitySerializer.DeferredSetter> _fixupFunctions;

			private readonly List<Action> _finalDeserialization;

			public static bool IsInScope
			{
				get
				{
					return UnitySerializer.SerializationScope._counter != 0;
				}
			}

			public static bool IsPrimaryScope
			{
				get
				{
					return UnitySerializer.SerializationScope._primaryScope || true;
				}
			}

			public SerializationScope()
			{
				UnitySerializer.SerializationScope._primaryScopeStack.Push(UnitySerializer.SerializationScope._primaryScope);
				UnitySerializer.SerializationScope._primaryScope = false;
				if (UnitySerializer._seenObjects == null)
				{
					UnitySerializer._seenObjects = new Dictionary<object, int>();
				}
				if (UnitySerializer._loadedObjects == null)
				{
					UnitySerializer._loadedObjects = new Dictionary<int, object>();
				}
				if (UnitySerializer._seenTypes == null)
				{
					UnitySerializer._seenTypes = new Dictionary<Type, bool>();
				}
				if (UnitySerializer.SerializationScope._counter == 0)
				{
					this._fixupFunctions = UnitySerializer.FixupFunctions;
					UnitySerializer.FixupFunctions = new List<UnitySerializer.DeferredSetter>();
					this._finalDeserialization = UnitySerializer.FinalDeserialization;
					UnitySerializer.FinalDeserialization = new List<Action>();
					UnitySerializer._seenObjects.Clear();
					UnitySerializer._loadedObjects.Clear();
					UnitySerializer._seenTypes.Clear();
					UnitySerializer._nextId = 0;
				}
				UnitySerializer.SerializationScope._counter++;
			}

			public static void SetPrimaryScope()
			{
				if (UnitySerializer.SerializationScope._hasSetPrimaryScope)
				{
					return;
				}
				UnitySerializer.SerializationScope._primaryScope = true;
				UnitySerializer.SerializationScope._hasSetPrimaryScope = true;
			}

			public void Dispose()
			{
				UnitySerializer.SerializationScope._primaryScope = UnitySerializer.SerializationScope._primaryScopeStack.Pop();
				if (--UnitySerializer.SerializationScope._counter != 0)
				{
					return;
				}
				UnitySerializer.SerializationScope._hasSetPrimaryScope = false;
				UnitySerializer.RunDeferredActions(1, true);
				UnitySerializer.FinalDeserialization = this._finalDeserialization;
				UnitySerializer.FixupFunctions = this._fixupFunctions;
				UnitySerializer._nextId = 0;
				UnitySerializer._seenObjects.Clear();
				UnitySerializer._loadedObjects.Clear();
				UnitySerializer._seenTypes.Clear();
				if (UnitySerializer._knownTypesLookup != null)
				{
					UnitySerializer._knownTypesLookup.Clear();
				}
				if (UnitySerializer._knownTypesList != null)
				{
					UnitySerializer._knownTypesList.Clear();
				}
				if (UnitySerializer._propertyLookup != null)
				{
					UnitySerializer._propertyLookup.Clear();
				}
				if (UnitySerializer._propertyList != null)
				{
					UnitySerializer._propertyList.Clear();
				}
			}
		}

		public class SerializationSplitScope : IDisposable
		{
			private readonly List<UnitySerializer.DeferredSetter> _fixupFunctions;

			private readonly List<Action> _finalDeserialization;

			private int _previousCounter;

			public SerializationSplitScope()
			{
				this._previousCounter = UnitySerializer.SerializationScope._counter;
				UnitySerializer.CreateStacks();
				if (UnitySerializer._seenObjects == null)
				{
					UnitySerializer._seenObjects = new Dictionary<object, int>();
				}
				if (UnitySerializer._loadedObjects == null)
				{
					UnitySerializer._loadedObjects = new Dictionary<int, object>();
				}
				if (UnitySerializer._seenTypes == null)
				{
					UnitySerializer._seenTypes = new Dictionary<Type, bool>();
				}
				UnitySerializer._seenTypesStack.Push(UnitySerializer._seenTypes);
				UnitySerializer._storedObjectsStack.Push(UnitySerializer._seenObjects);
				UnitySerializer._loadedObjectStack.Push(UnitySerializer._loadedObjects);
				UnitySerializer._idStack.Push(UnitySerializer._nextId);
				UnitySerializer._nextId = 0;
				UnitySerializer.SerializationScope._counter = 0;
				UnitySerializer._seenObjects = new Dictionary<object, int>();
				UnitySerializer._loadedObjects = new Dictionary<int, object>();
				UnitySerializer._seenTypes = new Dictionary<Type, bool>();
				this._fixupFunctions = UnitySerializer.FixupFunctions;
				UnitySerializer.FixupFunctions = new List<UnitySerializer.DeferredSetter>();
				this._finalDeserialization = UnitySerializer.FinalDeserialization;
				UnitySerializer.FinalDeserialization = new List<Action>();
			}

			public void Dispose()
			{
				UnitySerializer._seenObjects = UnitySerializer._storedObjectsStack.Pop();
				UnitySerializer._loadedObjects = UnitySerializer._loadedObjectStack.Pop();
				UnitySerializer._seenTypes = UnitySerializer._seenTypesStack.Pop();
				UnitySerializer._nextId = UnitySerializer._idStack.Pop();
				UnitySerializer.SerializationScope._counter = this._previousCounter;
				UnitySerializer.RunDeferredActions(1, true);
				UnitySerializer.FinalDeserialization = this._finalDeserialization;
				UnitySerializer.FixupFunctions = this._fixupFunctions;
			}
		}

		public class TypeMappingEventArgs : EventArgs
		{
			public string TypeName = string.Empty;

			public Type UseType;
		}

		public delegate object GetData(Dictionary<string, object> parameters);

		public delegate object ReadAValue(BinaryReader reader);

		private delegate void WriteAValue(BinaryWriter writer, object value);

		internal delegate void ScanTypeFunction(Type type, Attribute attribute);

		public static Encoding TextEncoding;

		private static readonly string DataPath;

		internal static List<Action> FinalDeserialization;

		private static readonly Dictionary<Type, IEnumerable<FieldInfo>> FieldLists;

		private static readonly Dictionary<Type, IEnumerable<PropertyInfo>> PropertyLists;

		private static readonly Dictionary<Type, IEnumerable<PropertyInfo>> ChecksumLists;

		internal static List<Type> _knownTypesList;

		internal static Dictionary<Type, ushort> _knownTypesLookup;

		private static Dictionary<object, int> _seenObjects;

		private static Dictionary<Type, bool> _seenTypes;

		private static Dictionary<int, object> _loadedObjects;

		internal static List<string> _propertyList;

		internal static Dictionary<string, ushort> _propertyLookup;

		private static Stack<List<UnitySerializer.DeferredSetter>> _deferredStack;

		private static Stack<List<Action>> _finalActions;

		private static Stack<Dictionary<int, object>> _loadedObjectStack;

		private static Stack<Dictionary<Type, bool>> _seenTypesStack;

		private static Stack<Dictionary<object, int>> _storedObjectsStack;

		private static Stack<UnitySerializer.KnownTypesStackEntry> _knownTypesStack;

		private static Stack<UnitySerializer.PropertyNameStackEntry> _propertyNamesStack;

		private static Stack<int> _idStack;

		public static bool IgnoreIds;

		private static int _nextId;

		private static readonly Dictionary<Type, ISerializeObject> Serializers;

		internal static readonly Dictionary<Type, ISpecialist> Specialists;

		private static readonly Dictionary<Type, ISerializeObject> SubTypeSerializers;

		private static readonly Dictionary<Type, ICreateObject> Creators;

		private static readonly Dictionary<Type, IProvideAttributeList> AttributeLists;

		public static readonly Dictionary<Type, bool> DEFERRED;

		private static readonly Dictionary<Assembly, bool> Assemblies;

		public static bool Verbose;

		private static readonly Dictionary<Type, ushort> PrewarmedTypes;

		public static readonly List<Type> PrewarmLookup;

		private static readonly Dictionary<string, ushort> PrewarmedNames;

		private static readonly HashSet<Type> privateTypes;

		private static readonly Stack<Type> currentTypes;

		public static int currentVersion;

		private static int _forceJSON;

		private static readonly Dictionary<Type, Dictionary<string, UnitySerializer.EntryConfiguration>> StoredTypes;

		public static object currentlySerializingObject;

		private static GameObject _componentHelper;

		private static readonly Dictionary<Type, ISerializeObject> cachedSerializers;

		public static object DeserializingObject;

		private static readonly Stack<object> DeserializingStack;

		internal static List<UnitySerializer.DeferredSetter> FixupFunctions;

		internal static List<IDeserialized> DeserializedObject;

		private static readonly Dictionary<Type, UnitySerializer.WriteAValue> Writers;

		public static readonly Dictionary<Type, UnitySerializer.ReadAValue> Readers;

		private static readonly Dictionary<string, bool> componentNames;

		public static event Func<Type, bool> CanSerialize
		{
			[MethodImpl(MethodImplOptions.Synchronized)]
			add
			{
				UnitySerializer.CanSerialize = (Func<Type, bool>)Delegate.Combine(UnitySerializer.CanSerialize, value);
			}
			[MethodImpl(MethodImplOptions.Synchronized)]
			remove
			{
				UnitySerializer.CanSerialize = (Func<Type, bool>)Delegate.Remove(UnitySerializer.CanSerialize, value);
			}
		}

		public static event EventHandler<UnitySerializer.ObjectMappingEventArgs> CreateType
		{
			[MethodImpl(MethodImplOptions.Synchronized)]
			add
			{
				UnitySerializer.CreateType = (EventHandler<UnitySerializer.ObjectMappingEventArgs>)Delegate.Combine(UnitySerializer.CreateType, value);
			}
			[MethodImpl(MethodImplOptions.Synchronized)]
			remove
			{
				UnitySerializer.CreateType = (EventHandler<UnitySerializer.ObjectMappingEventArgs>)Delegate.Remove(UnitySerializer.CreateType, value);
			}
		}

		public static event EventHandler<UnitySerializer.TypeMappingEventArgs> MapMissingType
		{
			[MethodImpl(MethodImplOptions.Synchronized)]
			add
			{
				UnitySerializer.MapMissingType = (EventHandler<UnitySerializer.TypeMappingEventArgs>)Delegate.Combine(UnitySerializer.MapMissingType, value);
			}
			[MethodImpl(MethodImplOptions.Synchronized)]
			remove
			{
				UnitySerializer.MapMissingType = (EventHandler<UnitySerializer.TypeMappingEventArgs>)Delegate.Remove(UnitySerializer.MapMissingType, value);
			}
		}

		public static bool IsChecksum
		{
			get;
			private set;
		}

		static UnitySerializer()
		{
			UnitySerializer.TextEncoding = Encoding.Default;
			UnitySerializer.FinalDeserialization = new List<Action>();
			UnitySerializer.FieldLists = new Dictionary<Type, IEnumerable<FieldInfo>>();
			UnitySerializer.PropertyLists = new Dictionary<Type, IEnumerable<PropertyInfo>>();
			UnitySerializer.ChecksumLists = new Dictionary<Type, IEnumerable<PropertyInfo>>();
			UnitySerializer.Serializers = new Dictionary<Type, ISerializeObject>();
			UnitySerializer.Specialists = new Dictionary<Type, ISpecialist>();
			UnitySerializer.SubTypeSerializers = new Dictionary<Type, ISerializeObject>();
			UnitySerializer.Creators = new Dictionary<Type, ICreateObject>();
			UnitySerializer.AttributeLists = new Dictionary<Type, IProvideAttributeList>();
			UnitySerializer.DEFERRED = new Dictionary<Type, bool>();
			UnitySerializer.Assemblies = new Dictionary<Assembly, bool>();
			UnitySerializer.PrewarmedTypes = new Dictionary<Type, ushort>();
			UnitySerializer.PrewarmLookup = new List<Type>();
			UnitySerializer.PrewarmedNames = new Dictionary<string, ushort>();
			UnitySerializer.privateTypes = new HashSet<Type>();
			UnitySerializer.currentTypes = new Stack<Type>();
			UnitySerializer.StoredTypes = new Dictionary<Type, Dictionary<string, UnitySerializer.EntryConfiguration>>();
			UnitySerializer.cachedSerializers = new Dictionary<Type, ISerializeObject>();
			UnitySerializer.DeserializingStack = new Stack<object>();
			UnitySerializer.FixupFunctions = new List<UnitySerializer.DeferredSetter>();
			UnitySerializer.DeserializedObject = new List<IDeserialized>();
			UnitySerializer.Writers = new Dictionary<Type, UnitySerializer.WriteAValue>();
			UnitySerializer.Readers = new Dictionary<Type, UnitySerializer.ReadAValue>();
			UnitySerializer.componentNames = new Dictionary<string, bool>();
			UnitySerializer.componentNames = (from m in typeof(Component).GetFields().Cast<MemberInfo>().Concat(typeof(Component).GetProperties().Cast<MemberInfo>())
			select m.Name).ToDictionary((string m) => m, (string m) => true);
			ushort num = 60000;
			string[] prewarmTypes = PreWarm.PrewarmTypes;
			for (int i = 0; i < prewarmTypes.Length; i++)
			{
				string typeName = prewarmTypes[i];
				Type type = Type.GetType(typeName);
				if (type == null)
				{
					num += 1;
					UnitySerializer.PrewarmLookup.Add(null);
				}
				else
				{
					Dictionary<Type, ushort> arg_1D5_0 = UnitySerializer.PrewarmedTypes;
					Type arg_1D5_1 = type;
					ushort expr_1D0 = num;
					num = expr_1D0 + 1;
					arg_1D5_0[arg_1D5_1] = expr_1D0;
					UnitySerializer.PrewarmLookup.Add(type);
				}
			}
			num = 50000;
			string[] prewarmNames = PreWarm.PrewarmNames;
			for (int j = 0; j < prewarmNames.Length; j++)
			{
				string text = prewarmNames[j];
				Dictionary<string, ushort> arg_21C_0 = UnitySerializer.PrewarmedNames;
				string arg_21C_1 = text;
				ushort expr_217 = num;
				num = expr_217 + 1;
				arg_21C_0[arg_21C_1] = expr_217;
			}
			UnitySerializer.DataPath = Application.persistentDataPath;
			UnitySerializer.RegisterSerializationAssembly();
			Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
			for (int k = 0; k < assemblies.Length; k++)
			{
				Assembly assembly = assemblies[k];
				UnitySerializer.RegisterSerializationAssembly(assembly);
			}
			UnitySerializer.Writers[typeof(string)] = new UnitySerializer.WriteAValue(UnitySerializer.StringWriter);
			UnitySerializer.Writers[typeof(decimal)] = new UnitySerializer.WriteAValue(UnitySerializer.DecimalWriter);
			UnitySerializer.Writers[typeof(float)] = new UnitySerializer.WriteAValue(UnitySerializer.FloatWriter);
			UnitySerializer.Writers[typeof(byte[])] = new UnitySerializer.WriteAValue(UnitySerializer.ByteArrayWriter);
			UnitySerializer.Writers[typeof(bool)] = new UnitySerializer.WriteAValue(UnitySerializer.BoolWriter);
			UnitySerializer.Writers[typeof(Guid)] = new UnitySerializer.WriteAValue(UnitySerializer.GuidWriter);
			UnitySerializer.Writers[typeof(DateTime)] = new UnitySerializer.WriteAValue(UnitySerializer.DateTimeWriter);
			UnitySerializer.Writers[typeof(TimeSpan)] = new UnitySerializer.WriteAValue(UnitySerializer.TimeSpanWriter);
			UnitySerializer.Writers[typeof(char)] = new UnitySerializer.WriteAValue(UnitySerializer.CharWriter);
			UnitySerializer.Writers[typeof(ushort)] = new UnitySerializer.WriteAValue(UnitySerializer.UShortWriter);
			UnitySerializer.Writers[typeof(double)] = new UnitySerializer.WriteAValue(UnitySerializer.DoubleWriter);
			UnitySerializer.Writers[typeof(ulong)] = new UnitySerializer.WriteAValue(UnitySerializer.ULongWriter);
			UnitySerializer.Writers[typeof(int)] = new UnitySerializer.WriteAValue(UnitySerializer.IntWriter);
			UnitySerializer.Writers[typeof(uint)] = new UnitySerializer.WriteAValue(UnitySerializer.UIntWriter);
			UnitySerializer.Writers[typeof(byte)] = new UnitySerializer.WriteAValue(UnitySerializer.ByteWriter);
			UnitySerializer.Writers[typeof(long)] = new UnitySerializer.WriteAValue(UnitySerializer.LongWriter);
			UnitySerializer.Writers[typeof(short)] = new UnitySerializer.WriteAValue(UnitySerializer.ShortWriter);
			UnitySerializer.Writers[typeof(sbyte)] = new UnitySerializer.WriteAValue(UnitySerializer.SByteWriter);
			UnitySerializer.Readers[typeof(string)] = new UnitySerializer.ReadAValue(UnitySerializer.AStringReader);
			UnitySerializer.Readers[typeof(decimal)] = new UnitySerializer.ReadAValue(UnitySerializer.DecimalReader);
			UnitySerializer.Readers[typeof(float)] = new UnitySerializer.ReadAValue(UnitySerializer.FloatReader);
			UnitySerializer.Readers[typeof(byte[])] = new UnitySerializer.ReadAValue(UnitySerializer.ByteArrayReader);
			UnitySerializer.Readers[typeof(bool)] = new UnitySerializer.ReadAValue(UnitySerializer.BoolReader);
			UnitySerializer.Readers[typeof(Guid)] = new UnitySerializer.ReadAValue(UnitySerializer.GuidReader);
			UnitySerializer.Readers[typeof(DateTime)] = new UnitySerializer.ReadAValue(UnitySerializer.DateTimeReader);
			UnitySerializer.Readers[typeof(TimeSpan)] = new UnitySerializer.ReadAValue(UnitySerializer.TimeSpanReader);
			UnitySerializer.Readers[typeof(char)] = new UnitySerializer.ReadAValue(UnitySerializer.CharReader);
			UnitySerializer.Readers[typeof(ushort)] = new UnitySerializer.ReadAValue(UnitySerializer.UShortReader);
			UnitySerializer.Readers[typeof(double)] = new UnitySerializer.ReadAValue(UnitySerializer.DoubleReader);
			UnitySerializer.Readers[typeof(ulong)] = new UnitySerializer.ReadAValue(UnitySerializer.ULongReader);
			UnitySerializer.Readers[typeof(int)] = new UnitySerializer.ReadAValue(UnitySerializer.IntReader);
			UnitySerializer.Readers[typeof(uint)] = new UnitySerializer.ReadAValue(UnitySerializer.UIntReader);
			UnitySerializer.Readers[typeof(byte)] = new UnitySerializer.ReadAValue(UnitySerializer.ByteReader);
			UnitySerializer.Readers[typeof(long)] = new UnitySerializer.ReadAValue(UnitySerializer.LongReader);
			UnitySerializer.Readers[typeof(short)] = new UnitySerializer.ReadAValue(UnitySerializer.ShortReader);
			UnitySerializer.Readers[typeof(sbyte)] = new UnitySerializer.ReadAValue(UnitySerializer.SByteReader);
		}

		public static void ForceJSONSerialization()
		{
			UnitySerializer._forceJSON++;
		}

		public static void UnforceJSONSerialization()
		{
			UnitySerializer._forceJSON--;
		}

		public static T Copy<T>(T original) where T : class
		{
			return UnitySerializer.Deserialize<T>(UnitySerializer.Serialize(original));
		}

		public static Type GetTypeEx(object fullTypeName)
		{
			string typeName = fullTypeName as string;
			if (typeName != null)
			{
				while (typeName.Contains("Version"))
				{
					typeName = typeName.Substring(0, typeName.IndexOf("Version")) + typeName.Substring(typeName.IndexOf(",", typeName.IndexOf("Version")) + 2);
				}
				Type type = Type.GetType(typeName);
				if (type != null)
				{
					return type;
				}
				Assembly assembly = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault((Assembly a) => a.GetType(typeName) != null);
				return (assembly == null) ? null : assembly.GetType(typeName);
			}
			else
			{
				if (!(fullTypeName is ushort))
				{
					return null;
				}
				if ((ushort)fullTypeName >= 60000)
				{
					return UnitySerializer.PrewarmLookup[(int)((ushort)fullTypeName - 60000)];
				}
				return UnitySerializer._knownTypesList[(int)((ushort)fullTypeName)];
			}
		}

		public static void SerializeToFile(object obj, string fileName)
		{
			using (FileStream fileStream = File.Create(UnitySerializer.DataPath + "/" + fileName))
			{
				UnitySerializer.Serialize(obj, fileStream, false);
			}
		}

		public static T DeserializeFromFile<T>(string fileName) where T : class
		{
			if (!File.Exists(UnitySerializer.DataPath + "/" + fileName))
			{
				return (T)((object)null);
			}
			T result;
			using (FileStream fileStream = File.Open(UnitySerializer.DataPath + "/" + fileName, FileMode.Open))
			{
				result = (UnitySerializer.Deserialize(fileStream) as T);
			}
			return result;
		}

		public static void InformDeserializedObjects(UnitySerializer.FinalProcess process)
		{
			foreach (IDeserialized current in process.deserializedObjects)
			{
				current.Deserialized();
			}
		}

		public static void InformDeserializedObjects()
		{
			foreach (IDeserialized current in UnitySerializer.DeserializedObject)
			{
				try
				{
					current.Deserialized();
				}
				catch
				{
				}
			}
			UnitySerializer.DeserializedObject.Clear();
		}

		public static void AddFinalAction(Action a)
		{
			UnitySerializer.FinalDeserialization.Add(a);
		}

		public static UnitySerializer.FinalProcess TakeOwnershipOfFinalization()
		{
			UnitySerializer.FinalProcess result = default(UnitySerializer.FinalProcess);
			List<UnitySerializer.DeferredSetter> fixupFunctions = UnitySerializer.FixupFunctions;
			lock (fixupFunctions)
			{
				result.deserializedObjects = UnitySerializer.DeserializedObject;
				result.deferredActions = UnitySerializer.FinalDeserialization;
				result.deferredSetters = UnitySerializer.FixupFunctions;
				UnitySerializer.FinalDeserialization = new List<Action>();
				UnitySerializer.FixupFunctions = new List<UnitySerializer.DeferredSetter>();
				UnitySerializer.DeserializedObject = new List<IDeserialized>();
			}
			return result;
		}

		public static void RunDeferredActions(int count = 1, bool clear = true)
		{
			List<UnitySerializer.DeferredSetter> fixupFunctions = UnitySerializer.FixupFunctions;
			lock (fixupFunctions)
			{
				for (int i = 0; i < count; i++)
				{
					foreach (UnitySerializer.DeferredSetter current in from f in UnitySerializer.FixupFunctions
					where f.enabled
					select f)
					{
						try
						{
							current.deferredRetrievalFunction(current.parameters);
							current._setAction();
						}
						catch (Exception ex)
						{
							Radical.LogError(string.Concat(new string[]
							{
								"Failed deferred deserialization with error ",
								ex.GetType().FullName,
								"'",
								ex.Message,
								"' @ ",
								ex.StackTrace
							}));
						}
					}
					foreach (Action current2 in UnitySerializer.FinalDeserialization)
					{
						current2();
					}
				}
				if (clear)
				{
					UnitySerializer.FixupFunctions.Clear();
					UnitySerializer.FinalDeserialization.Clear();
				}
			}
		}

		public static void RunDeferredActions(UnitySerializer.FinalProcess process, int count = 1, bool clear = true)
		{
			List<UnitySerializer.DeferredSetter> fixupFunctions = UnitySerializer.FixupFunctions;
			lock (fixupFunctions)
			{
				List<UnitySerializer.DeferredSetter> fixupFunctions2 = UnitySerializer.FixupFunctions;
				List<Action> finalDeserialization = UnitySerializer.FinalDeserialization;
				UnitySerializer.FixupFunctions = process.deferredSetters;
				UnitySerializer.FinalDeserialization = process.deferredActions;
				for (int i = 0; i < count; i++)
				{
					foreach (UnitySerializer.DeferredSetter current in from f in process.deferredSetters
					where f.enabled
					select f)
					{
						try
						{
							current.deferredRetrievalFunction(current.parameters);
							current._setAction();
						}
						catch (Exception ex)
						{
							Radical.LogError(string.Concat(new string[]
							{
								"Failed deferred deserialization with error ",
								ex.GetType().FullName,
								"'",
								ex.Message,
								"' @ ",
								ex.StackTrace
							}));
						}
					}
					foreach (Action current2 in process.deferredActions)
					{
						current2();
					}
				}
				if (clear)
				{
					process.deferredActions.Clear();
					process.deferredSetters.Clear();
				}
				UnitySerializer.FixupFunctions = fixupFunctions2;
				UnitySerializer.FinalDeserialization = finalDeserialization;
			}
		}

		public static void AddFixup(UnitySerializer.DeferredSetter setter)
		{
			List<UnitySerializer.DeferredSetter> fixupFunctions = UnitySerializer.FixupFunctions;
			lock (fixupFunctions)
			{
				UnitySerializer.FixupFunctions.Add(setter);
			}
		}

		internal static bool CanSerializeType(Type tp)
		{
			return UnitySerializer.CanSerialize == null || UnitySerializer.CanSerialize(tp);
		}

		internal static void PushPropertyNames(bool clear)
		{
			if (UnitySerializer.SerializationScope.IsPrimaryScope)
			{
				UnitySerializer._propertyNamesStack.Push(new UnitySerializer.PropertyNameStackEntry
				{
					propertyList = UnitySerializer._propertyList,
					propertyLookup = UnitySerializer._propertyLookup
				});
				if (clear)
				{
					UnitySerializer._propertyList = new List<string>();
					UnitySerializer._propertyLookup = new Dictionary<string, ushort>();
				}
			}
			else
			{
				UnitySerializer._propertyList = (UnitySerializer._propertyList ?? new List<string>());
				UnitySerializer._propertyLookup = (UnitySerializer._propertyLookup ?? new Dictionary<string, ushort>());
			}
		}

		internal static void PushPropertyNames()
		{
			UnitySerializer.PushPropertyNames(true);
		}

		internal static void PopPropertyNames()
		{
			if (UnitySerializer.SerializationScope.IsPrimaryScope)
			{
				UnitySerializer.PropertyNameStackEntry propertyNameStackEntry = UnitySerializer._propertyNamesStack.Pop();
				UnitySerializer._propertyList = propertyNameStackEntry.propertyList;
				UnitySerializer._propertyLookup = propertyNameStackEntry.propertyLookup;
			}
		}

		private static void InvokeCreateType(UnitySerializer.ObjectMappingEventArgs e)
		{
			EventHandler<UnitySerializer.ObjectMappingEventArgs> createType = UnitySerializer.CreateType;
			if (createType != null)
			{
				createType(null, e);
			}
		}

		internal static void InvokeMapMissingType(UnitySerializer.TypeMappingEventArgs e)
		{
			EventHandler<UnitySerializer.TypeMappingEventArgs> mapMissingType = UnitySerializer.MapMissingType;
			if (mapMissingType != null)
			{
				mapMissingType(null, e);
			}
		}

		public static T Deserialize<T>(byte[] array) where T : class
		{
			if (UnitySerializer._forceJSON > 0)
			{
				return UnitySerializer.JSONDeserialize<T>(UnitySerializer.ConvertBytesToJSON(array));
			}
			return UnitySerializer.Deserialize(array) as T;
		}

		public static void WriteToFile(this byte[] data, string filename = null)
		{
			FileStream fileStream = File.Create(filename ?? "test_output.data");
			BinaryWriter binaryWriter = new BinaryWriter(fileStream);
			binaryWriter.Write(data);
			binaryWriter.Flush();
			fileStream.Close();
		}

		public static void WriteToFile(this string str, string filename = null)
		{
			FileStream fileStream = File.Create(filename ?? "test_output.txt");
			StreamWriter streamWriter = new StreamWriter(fileStream);
			streamWriter.Write(str);
			streamWriter.Flush();
			fileStream.Close();
		}

		public static T JSONDeserialize<T>(string data) where T : class
		{
			return UnitySerializer.JSONDeserialize(data) as T;
		}

		public static T JSONDeserialize<T>(Stream stream) where T : class
		{
			return UnitySerializer.JSONDeserialize(stream) as T;
		}

		public static T Deserialize<T>(Stream stream) where T : class
		{
			return UnitySerializer.Deserialize(stream) as T;
		}

		public static string GetChecksum(object item)
		{
			if (item == null)
			{
				return string.Empty;
			}
			byte[] array = new byte[17];
			array.Initialize();
			bool isChecksum = UnitySerializer.IsChecksum;
			UnitySerializer.IsChecksum = true;
			byte[] array2 = UnitySerializer.Serialize(item);
			UnitySerializer.IsChecksum = isChecksum;
			for (int i = 0; i < array2.Length; i++)
			{
				byte[] expr_44_cp_0 = array;
				int expr_44_cp_1 = i & 15;
				expr_44_cp_0[expr_44_cp_1] ^= array2[i];
			}
			return string.Concat(new object[]
			{
				item.GetType().Name,
				"-",
				array2.Count<byte>(),
				"-",
				UnitySerializer.Encode(array)
			});
		}

		private static string Encode(byte[] checksum)
		{
			string source = Convert.ToBase64String(checksum);
			return source.Aggregate(string.Empty, (string current, char c) => current + ((!char.IsLetterOrDigit(c)) ? char.GetNumericValue(c) : ((double)c)));
		}

		public static void RegisterSerializationAssembly()
		{
			UnitySerializer.RegisterSerializationAssembly(null);
		}

		public static void RegisterSerializationAssembly(Assembly assembly)
		{
			if (assembly == null)
			{
				assembly = Assembly.GetCallingAssembly();
			}
			if (UnitySerializer.Assemblies.ContainsKey(assembly))
			{
				return;
			}
			UnitySerializer.Assemblies[assembly] = true;
			UnitySerializer.ScanAllTypesForAttribute(delegate(Type tp, Attribute attr)
			{
				UnitySerializer.Serializers[((SerializerAttribute)attr).SerializesType] = (Activator.CreateInstance(tp) as ISerializeObject);
			}, assembly, typeof(SerializerAttribute));
			UnitySerializer.ScanAllTypesForAttribute(delegate(Type tp, Attribute attr)
			{
				UnitySerializer.AttributeLists[((AttributeListProvider)attr).AttributeListType] = (Activator.CreateInstance(tp) as IProvideAttributeList);
			}, assembly, typeof(AttributeListProvider));
			UnitySerializer.ScanAllTypesForAttribute(delegate(Type tp, Attribute attr)
			{
				UnitySerializer.SubTypeSerializers[((SubTypeSerializerAttribute)attr).SerializesType] = (Activator.CreateInstance(tp) as ISerializeObject);
			}, assembly, typeof(SubTypeSerializerAttribute));
			UnitySerializer.ScanAllTypesForAttribute(delegate(Type tp, Attribute attr)
			{
				UnitySerializer.Specialists[tp] = (Activator.CreateInstance(tp) as ISpecialist);
			}, assembly, typeof(SpecialistProvider));
			UnitySerializer.ScanAllTypesForAttribute(delegate(Type tp, Attribute attr)
			{
				UnitySerializer.Creators[((CreatorFor)attr).CreatesType] = (Activator.CreateInstance(tp) as ICreateObject);
			}, assembly, typeof(CreatorFor));
		}

		internal static void ScanAllTypesForAttribute(UnitySerializer.ScanTypeFunction function, Assembly assembly)
		{
			UnitySerializer.ScanAllTypesForAttribute(function, assembly, null);
		}

		internal static void ScanAllTypesForAttribute(UnitySerializer.ScanTypeFunction function, Assembly assembly, Type attribute)
		{
			try
			{
				Type[] types = assembly.GetTypes();
				for (int i = 0; i < types.Length; i++)
				{
					Type type = types[i];
					if (attribute != null)
					{
						Attribute[] customAttributes = Attribute.GetCustomAttributes(type, attribute, false);
						Attribute[] array = customAttributes;
						for (int j = 0; j < array.Length; j++)
						{
							Attribute attribute2 = array[j];
							function(type, attribute2);
						}
					}
					else
					{
						function(type, null);
					}
				}
			}
			catch
			{
			}
		}

		internal static IEnumerable<PropertyInfo> GetPropertyInfo(Type itm)
		{
			Dictionary<Type, IEnumerable<PropertyInfo>> propertyLists = UnitySerializer.PropertyLists;
			IEnumerable<PropertyInfo> result;
			lock (propertyLists)
			{
				IEnumerable<PropertyInfo> enumerable;
				if (!UnitySerializer.IsChecksum)
				{
					if (!UnitySerializer.PropertyLists.TryGetValue(itm, out enumerable))
					{
						Type tp = itm;
						bool allowSimple = true;
						string[] validNames = (from p in UnitySerializer.AttributeLists
						where p.Key.IsAssignableFrom(tp)
						select p).SelectMany(delegate(KeyValuePair<Type, IProvideAttributeList> p)
						{
							allowSimple = (allowSimple && p.Value.AllowAllSimple(tp));
							return p.Value.GetAttributeList(tp);
						}).ToArray<string>();
						if (validNames.FirstOrDefault<string>() == null)
						{
							validNames = null;
						}
						Type containingType = itm;
						enumerable = (from p in containingType.GetProperties(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public)
						where !typeof(Component).IsAssignableFrom(tp) || tp == typeof(Component) || !UnitySerializer.componentNames.ContainsKey(p.Name)
						where p.PropertyType.GetCustomAttributes(typeof(DoNotSerialize), true).Count<object>() == 0 && p.GetGetMethod() != null && (!containingType.IsDefined(typeof(DoNotSerializePublic), true) || p.IsDefined(typeof(SerializeThis), true)) && !p.GetCustomAttributes(typeof(DoNotSerialize), true).Any<object>() && !p.GetCustomAttributes(typeof(ObsoleteAttribute), true).Any<object>() && !p.GetIndexParameters().Any<ParameterInfo>() && p.GetSetMethod() != null && UnitySerializer.CanSerializeType(p.PropertyType) && ((p.PropertyType.IsValueType && allowSimple) || validNames == null || validNames.Any((string n) => n == p.Name))
						select p).ToArray<PropertyInfo>();
						UnitySerializer.PropertyLists[itm] = enumerable;
					}
				}
				else if (!UnitySerializer.ChecksumLists.TryGetValue(itm, out enumerable))
				{
					Type tp = itm;
					bool allowSimple = true;
					string[] validNames = (from p in UnitySerializer.AttributeLists
					where p.Key.IsAssignableFrom(tp)
					select p).SelectMany(delegate(KeyValuePair<Type, IProvideAttributeList> p)
					{
						allowSimple &= p.Value.AllowAllSimple(tp);
						return p.Value.GetAttributeList(tp);
					}).ToArray<string>();
					string[] availableNames = validNames;
					if (availableNames.FirstOrDefault<string>() == null)
					{
						validNames = null;
					}
					enumerable = (from p in tp.GetProperties(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public)
					where !typeof(Component).IsAssignableFrom(tp) || tp == typeof(Component) || !UnitySerializer.componentNames.ContainsKey(p.Name)
					where !p.PropertyType.GetCustomAttributes(typeof(DoNotSerialize), true).Any<object>() && p.GetGetMethod() != null && (!tp.IsDefined(typeof(DoNotSerializePublic), true) || p.IsDefined(typeof(SerializeThis), true)) && !p.GetCustomAttributes(typeof(DoNotSerialize), true).Any<object>() && !p.GetCustomAttributes(typeof(DoNotChecksum), true).Any<object>() && !p.GetCustomAttributes(typeof(ObsoleteAttribute), true).Any<object>() && !p.GetIndexParameters().Any<ParameterInfo>() && p.GetSetMethod() != null && UnitySerializer.CanSerializeType(p.PropertyType) && ((allowSimple && p.PropertyType.IsValueType) || validNames == null || availableNames.Any((string n) => n == p.Name))
					select p).ToArray<PropertyInfo>();
					UnitySerializer.ChecksumLists[itm] = enumerable;
				}
				PropertyInfo[] array = (enumerable as PropertyInfo[]) ?? enumerable.ToArray<PropertyInfo>();
				IEnumerable<PropertyInfo> arg_208_0;
				if (UnitySerializer.IgnoreIds && enumerable != null)
				{
					IEnumerable<PropertyInfo> enumerable2 = from p in array
					where !p.GetCustomAttributes(typeof(SerializerId), true).Any<object>()
					select p;
					arg_208_0 = enumerable2;
				}
				else
				{
					arg_208_0 = array;
				}
				result = arg_208_0;
			}
			return result;
		}

		public static IEnumerable<PropertyInfo> GetProperties(Type item)
		{
			bool isChecksum = UnitySerializer.IsChecksum;
			bool ignoreIds = UnitySerializer.IgnoreIds;
			UnitySerializer.IsChecksum = false;
			UnitySerializer.IgnoreIds = false;
			IEnumerable<PropertyInfo> propertyInfo = UnitySerializer.GetPropertyInfo(item);
			UnitySerializer.IsChecksum = isChecksum;
			UnitySerializer.IgnoreIds = ignoreIds;
			return propertyInfo;
		}

		public static IEnumerable<FieldInfo> GetFields(Type item)
		{
			bool isChecksum = UnitySerializer.IsChecksum;
			bool ignoreIds = UnitySerializer.IgnoreIds;
			UnitySerializer.IsChecksum = false;
			UnitySerializer.IgnoreIds = false;
			IEnumerable<FieldInfo> fieldInfo = UnitySerializer.GetFieldInfo(item);
			UnitySerializer.IsChecksum = isChecksum;
			UnitySerializer.IgnoreIds = ignoreIds;
			return fieldInfo;
		}

		public static void AddPrivateType(Type tp)
		{
			UnitySerializer.privateTypes.Add(tp);
		}

		internal static IEnumerable<FieldInfo> GetFieldInfo(Type itm)
		{
			Dictionary<Type, IEnumerable<FieldInfo>> fieldLists = UnitySerializer.FieldLists;
			IEnumerable<FieldInfo> result;
			lock (fieldLists)
			{
				IEnumerable<FieldInfo> enumerable;
				if (UnitySerializer.FieldLists.ContainsKey(itm))
				{
					enumerable = UnitySerializer.FieldLists[itm];
				}
				else
				{
					Type tp = itm;
					bool allowSimple = true;
					List<string> validNames = (from p in UnitySerializer.AttributeLists
					where p.Key.IsAssignableFrom(tp)
					select p).SelectMany(delegate(KeyValuePair<Type, IProvideAttributeList> p)
					{
						allowSimple = (allowSimple && p.Value.AllowAllSimple(tp));
						return p.Value.GetAttributeList(tp);
					}).ToList<string>();
					if (validNames.FirstOrDefault<string>() == null)
					{
						validNames = null;
					}
					List<Type> list = new List<Type>();
					List<Type> list2 = new List<Type>();
					Type type2 = tp;
					bool flag = false;
					do
					{
						if (UnitySerializer.privateTypes.Any(new Func<Type, bool>(UnitySerializer.currentTypes.Contains)) || flag || type2.IsDefined(typeof(SerializeAll), false) || type2.GetInterface("IEnumerator") != null)
						{
							if (type2.GetInterface("IEnumerator") != null && !flag)
							{
								flag = true;
								UnitySerializer.privateTypes.Add(type2);
							}
							list.Add(type2);
						}
						list2.Add(type2);
						type2 = type2.BaseType;
					}
					while (type2 != null);
					IEnumerable<FieldInfo> enumerable2 = (from p in (from p in tp.GetFields(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.SetField)
					where !tp.IsDefined(typeof(DoNotSerializePublic), true) || p.IsDefined(typeof(SerializeThis), true)
					select p).Concat(list2.SelectMany((Type type) => from f in type.GetFields(BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.SetField)
					where f.IsDefined(typeof(SerializeThis), true)
					select f)).Concat(list.SelectMany((Type p) => p.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.SetField))).Concat(SerializePrivateFieldOfType.GetFields(tp))
					where !p.IsLiteral && !p.FieldType.IsDefined(typeof(DoNotSerialize), false) && !p.IsDefined(typeof(DoNotSerialize), false) && UnitySerializer.CanSerializeType(p.FieldType) && ((p.FieldType.IsValueType && allowSimple) || validNames == null || validNames.Any((string n) => n == p.Name))
					where !typeof(Component).IsAssignableFrom(tp) || tp == typeof(Component) || !UnitySerializer.componentNames.ContainsKey(p.Name)
					select p).ToArray<FieldInfo>();
					UnitySerializer.FieldLists[itm] = enumerable2;
					enumerable = enumerable2;
				}
				IEnumerable<FieldInfo> arg_236_0;
				if (UnitySerializer.IsChecksum)
				{
					IEnumerable<FieldInfo> enumerable2 = from p in enumerable
					where !p.GetCustomAttributes(typeof(DoNotChecksum), true).Any<object>()
					select p;
					arg_236_0 = enumerable2;
				}
				else
				{
					arg_236_0 = enumerable;
				}
				result = arg_236_0;
			}
			return result;
		}

		internal static ushort GetPropertyDefinitionId(string name)
		{
			ushort num;
			if (!UnitySerializer.PrewarmedNames.TryGetValue(name, out num) && !UnitySerializer._propertyLookup.TryGetValue(name, out num))
			{
				num = (ushort)UnitySerializer._propertyLookup.Count;
				UnitySerializer._propertyLookup[name] = num;
			}
			return num;
		}

		public static object Deserialize(IStorage storage)
		{
			object result;
			using (new UnitySerializer.SerializationScope())
			{
				bool verbose = UnitySerializer.Verbose;
				UnitySerializer.Verbose = false;
				UnitySerializer.CreateStacks();
				try
				{
					UnitySerializer.PushKnownTypes(false);
					UnitySerializer.PushPropertyNames(false);
					storage.StartDeserializing();
					object obj = UnitySerializer.DeserializeObject(new Entry
					{
						Name = "root"
					}, storage);
					storage.FinishedDeserializing();
					result = obj;
				}
				finally
				{
					UnitySerializer.PopKnownTypes();
					UnitySerializer.PopPropertyNames();
					UnitySerializer.Verbose = verbose;
				}
			}
			return result;
		}

		public static object Deserialize(Stream inputStream)
		{
			return UnitySerializer.Deserialize(inputStream, null);
		}

		public static object JSONDeserialize(Stream inputStream)
		{
			return UnitySerializer.JSONDeserialize(inputStream, null);
		}

		public static object JSONDeserialize(Stream inputStream, object instance)
		{
			object result;
			using (new UnitySerializer.SerializationScope())
			{
				bool verbose = UnitySerializer.Verbose;
				UnitySerializer.CreateStacks();
				try
				{
					UnitySerializer.PushKnownTypes();
					UnitySerializer.PushPropertyNames();
					StreamReader streamReader = new StreamReader(inputStream);
					string json = streamReader.ReadToEnd();
					JSONSerializer jSONSerializer = new JSONSerializer(json);
					jSONSerializer.StartDeserializing();
					object obj = UnitySerializer.DeserializeObject(new Entry
					{
						Name = "root",
						Value = instance
					}, jSONSerializer);
					jSONSerializer.FinishedDeserializing();
					result = obj;
				}
				catch (Exception ex)
				{
					Radical.LogError("Serialization error: " + ex.ToString());
					result = null;
				}
				finally
				{
					UnitySerializer.PopKnownTypes();
					UnitySerializer.PopPropertyNames();
					UnitySerializer.Verbose = verbose;
				}
			}
			return result;
		}

		public static object Deserialize(Stream inputStream, object instance)
		{
			object result;
			using (new UnitySerializer.SerializationScope())
			{
				bool verbose = UnitySerializer.Verbose;
				UnitySerializer.CreateStacks();
				try
				{
					UnitySerializer.PushKnownTypes();
					UnitySerializer.PushPropertyNames();
					BinaryReader binaryReader = new BinaryReader(inputStream);
					string text = binaryReader.ReadString();
					UnitySerializer.currentVersion = int.Parse(text.Substring(4));
					if (UnitySerializer.currentVersion >= 5)
					{
						inputStream.Position = 0L;
						BinarySerializer binarySerializer = new BinarySerializer(binaryReader.ReadBytes((int)inputStream.Length));
						binarySerializer.StartDeserializing();
						object obj = UnitySerializer.DeserializeObject(new Entry
						{
							Name = "root",
							Value = instance
						}, binarySerializer);
						binarySerializer.FinishedDeserializing();
						result = obj;
					}
					else
					{
						result = null;
					}
				}
				catch (Exception ex)
				{
					Radical.LogError("Serialization error: " + ex.ToString());
					result = null;
				}
				finally
				{
					UnitySerializer.PopKnownTypes();
					UnitySerializer.PopPropertyNames();
					UnitySerializer.Verbose = verbose;
				}
			}
			return result;
		}

		public static string Escape(string input)
		{
			return input.Replace("\\", "\\\\").Replace("\"", "\\\"");
		}

		public static string UnEscape(string input)
		{
			return (!input.Contains("\"")) ? input.Replace("\\\"", "\"").Replace("\\\\", "\\") : input;
		}

		private static string ConvertBytesToJSON(byte[] data)
		{
			return UnitySerializer.TextEncoding.GetString(data);
		}

		private static byte[] ConvertJSONToBytes(string data)
		{
			return UnitySerializer.TextEncoding.GetBytes(data);
		}

		internal static void PopKnownTypes()
		{
			if (UnitySerializer.SerializationScope.IsPrimaryScope)
			{
				UnitySerializer.KnownTypesStackEntry knownTypesStackEntry = UnitySerializer._knownTypesStack.Pop();
				UnitySerializer._knownTypesList = knownTypesStackEntry.knownTypesList;
				UnitySerializer._knownTypesLookup = knownTypesStackEntry.knownTypesLookup;
			}
		}

		private static void PushKnownTypes(bool clear)
		{
			if (UnitySerializer.SerializationScope.IsPrimaryScope)
			{
				UnitySerializer._knownTypesStack.Push(new UnitySerializer.KnownTypesStackEntry
				{
					knownTypesList = UnitySerializer._knownTypesList,
					knownTypesLookup = UnitySerializer._knownTypesLookup
				});
				if (!clear)
				{
					return;
				}
				UnitySerializer._knownTypesList = new List<Type>();
				UnitySerializer._knownTypesLookup = new Dictionary<Type, ushort>();
			}
			else
			{
				UnitySerializer._knownTypesList = (UnitySerializer._knownTypesList ?? new List<Type>());
				UnitySerializer._knownTypesLookup = (UnitySerializer._knownTypesLookup ?? new Dictionary<Type, ushort>());
			}
		}

		internal static void PushKnownTypes()
		{
			UnitySerializer.PushKnownTypes(true);
		}

		public static object Deserialize(byte[] bytes)
		{
			if (UnitySerializer._forceJSON > 0)
			{
				return UnitySerializer.JSONDeserialize(UnitySerializer.ConvertBytesToJSON(bytes));
			}
			object result;
			using (new UnitySerializer.SerializationScope())
			{
				using (MemoryStream memoryStream = new MemoryStream(bytes))
				{
					result = UnitySerializer.Deserialize(memoryStream);
				}
			}
			return result;
		}

		public static object JSONDeserialize(string json)
		{
			object result;
			using (new UnitySerializer.SerializationScope())
			{
				using (MemoryStream memoryStream = new MemoryStream(UnitySerializer.TextEncoding.GetBytes(UnitySerializer.UnEscape(json))))
				{
					result = UnitySerializer.JSONDeserialize(memoryStream);
				}
			}
			return result;
		}

		public static void DeserializeInto(byte[] bytes, object instance)
		{
			using (new UnitySerializer.SerializationScope())
			{
				using (MemoryStream memoryStream = new MemoryStream(bytes))
				{
					UnitySerializer.Deserialize(memoryStream, instance);
				}
			}
		}

		public static void JSONDeserializeInto(string json, object instance)
		{
			using (new UnitySerializer.SerializationScope())
			{
				using (MemoryStream memoryStream = new MemoryStream(UnitySerializer.TextEncoding.GetBytes(UnitySerializer.UnEscape(json))))
				{
					UnitySerializer.JSONDeserialize(memoryStream, instance);
				}
			}
		}

		private static void CreateStacks()
		{
			if (UnitySerializer._propertyNamesStack == null)
			{
				UnitySerializer._propertyNamesStack = new Stack<UnitySerializer.PropertyNameStackEntry>();
			}
			if (UnitySerializer._knownTypesStack == null)
			{
				UnitySerializer._knownTypesStack = new Stack<UnitySerializer.KnownTypesStackEntry>();
			}
			if (UnitySerializer._loadedObjectStack == null)
			{
				UnitySerializer._loadedObjectStack = new Stack<Dictionary<int, object>>();
			}
			if (UnitySerializer._storedObjectsStack == null)
			{
				UnitySerializer._storedObjectsStack = new Stack<Dictionary<object, int>>();
			}
			if (UnitySerializer._seenTypesStack == null)
			{
				UnitySerializer._seenTypesStack = new Stack<Dictionary<Type, bool>>();
			}
			if (UnitySerializer._deferredStack == null)
			{
				UnitySerializer._deferredStack = new Stack<List<UnitySerializer.DeferredSetter>>();
			}
			if (UnitySerializer._finalActions == null)
			{
				UnitySerializer._finalActions = new Stack<List<Action>>();
			}
			if (UnitySerializer._idStack == null)
			{
				UnitySerializer._idStack = new Stack<int>();
			}
		}

		public static void JSONSerialize(object item, IStorage storage)
		{
			UnitySerializer.JSONSerialize(item, storage, false);
		}

		public static void Serialize(object item, IStorage storage)
		{
			UnitySerializer.Serialize(item, storage, false);
		}

		public static void Serialize(object item, IStorage storage, bool forDeserializeInto)
		{
			bool verbose = UnitySerializer.Verbose;
			UnitySerializer.Verbose = false;
			UnitySerializer.CreateStacks();
			using (new UnitySerializer.SerializationScope())
			{
				UnitySerializer.SerializationScope.SetPrimaryScope();
				try
				{
					storage.StartSerializing();
					UnitySerializer.SerializeObject(new Entry
					{
						Name = "root",
						Value = item
					}, storage, forDeserializeInto);
					storage.FinishedSerializing();
				}
				finally
				{
					UnitySerializer.Verbose = verbose;
				}
			}
		}

		public static void JSONSerialize(object item, IStorage storage, bool forDeserializeInto)
		{
			bool verbose = UnitySerializer.Verbose;
			UnitySerializer.Verbose = false;
			UnitySerializer.CreateStacks();
			using (new UnitySerializer.SerializationScope())
			{
				UnitySerializer.SerializationScope.SetPrimaryScope();
				try
				{
					storage.StartSerializing();
					UnitySerializer.SerializeObject(new Entry
					{
						Name = "root",
						Value = item
					}, storage, forDeserializeInto);
					storage.FinishedSerializing();
				}
				finally
				{
					UnitySerializer.Verbose = verbose;
				}
			}
		}

		public static void JSONSerialize(object item, Stream outputStream)
		{
			UnitySerializer.JSONSerialize(item, outputStream, false);
		}

		public static void Serialize(object item, Stream outputStream)
		{
			UnitySerializer.Serialize(item, outputStream, false);
		}

		public static void JSONSerialize(object item, Stream outputStream, bool forDeserializeInto)
		{
			UnitySerializer.CreateStacks();
			using (new UnitySerializer.SerializationScope())
			{
				UnitySerializer.SerializationScope.SetPrimaryScope();
				JSONSerializer jSONSerializer = new JSONSerializer();
				jSONSerializer.StartSerializing();
				UnitySerializer.SerializeObject(new Entry
				{
					Name = "root",
					Value = item
				}, jSONSerializer, forDeserializeInto);
				jSONSerializer.FinishedSerializing();
				StreamWriter streamWriter = new StreamWriter(outputStream);
				streamWriter.Write(jSONSerializer.Data);
				streamWriter.Flush();
				outputStream.Flush();
			}
		}

		public static void Serialize(object item, Stream outputStream, bool forDeserializeInto)
		{
			UnitySerializer.CreateStacks();
			using (new UnitySerializer.SerializationScope())
			{
				UnitySerializer.SerializationScope.SetPrimaryScope();
				BinarySerializer binarySerializer = new BinarySerializer();
				binarySerializer.StartSerializing();
				UnitySerializer.SerializeObject(new Entry
				{
					Name = "root",
					Value = item
				}, binarySerializer, forDeserializeInto);
				binarySerializer.FinishedSerializing();
				BinaryWriter binaryWriter = new BinaryWriter(outputStream);
				binaryWriter.Write(binarySerializer.Data);
				binaryWriter.Flush();
			}
		}

		public static byte[] Serialize(object item)
		{
			if (UnitySerializer._forceJSON > 0)
			{
				return UnitySerializer.ConvertJSONToBytes(UnitySerializer.JSONSerialize(item));
			}
			byte[] result;
			using (new UnitySerializer.SerializationScope())
			{
				using (MemoryStream memoryStream = new MemoryStream())
				{
					UnitySerializer.Serialize(item, memoryStream);
					result = memoryStream.ToArray();
				}
			}
			return result;
		}

		public static string JSONSerialize(object item)
		{
			string @string;
			using (new UnitySerializer.SerializationScope())
			{
				using (MemoryStream memoryStream = new MemoryStream())
				{
					UnitySerializer.JSONSerialize(item, memoryStream);
					@string = UnitySerializer.TextEncoding.GetString(memoryStream.ToArray());
				}
			}
			return @string;
		}

		public static string JSONSerializeForDeserializeInto(object item)
		{
			string @string;
			using (new UnitySerializer.SerializationScope())
			{
				using (MemoryStream memoryStream = new MemoryStream())
				{
					UnitySerializer.JSONSerialize(item, memoryStream, true);
					@string = UnitySerializer.TextEncoding.GetString(memoryStream.ToArray());
				}
			}
			return @string;
		}

		public static byte[] SerializeForDeserializeInto(object item)
		{
			byte[] result;
			using (new UnitySerializer.SerializationScope())
			{
				using (MemoryStream memoryStream = new MemoryStream())
				{
					UnitySerializer.Serialize(item, memoryStream, true);
					result = memoryStream.ToArray();
				}
			}
			return result;
		}

		public static string JSONSerialize(object item, bool makeVerbose)
		{
			string @string;
			using (new UnitySerializer.SerializationScope())
			{
				using (MemoryStream memoryStream = new MemoryStream())
				{
					bool verbose = UnitySerializer.Verbose;
					UnitySerializer.Verbose = makeVerbose;
					UnitySerializer.JSONSerialize(item, memoryStream);
					UnitySerializer.Verbose = verbose;
					@string = UnitySerializer.TextEncoding.GetString(memoryStream.ToArray());
				}
			}
			return @string;
		}

		public static byte[] Serialize(object item, bool makeVerbose)
		{
			if (UnitySerializer._forceJSON > 0)
			{
				return UnitySerializer.ConvertJSONToBytes(UnitySerializer.JSONSerialize(item, makeVerbose));
			}
			byte[] result;
			using (new UnitySerializer.SerializationScope())
			{
				using (MemoryStream memoryStream = new MemoryStream())
				{
					bool verbose = UnitySerializer.Verbose;
					UnitySerializer.Verbose = makeVerbose;
					UnitySerializer.Serialize(item, memoryStream);
					UnitySerializer.Verbose = verbose;
					result = memoryStream.ToArray();
				}
			}
			return result;
		}

		private static bool IsSimpleType(Type tp)
		{
			return tp.IsPrimitive || tp == typeof(string) || tp.IsEnum || tp == typeof(DateTime) || tp == typeof(TimeSpan) || tp == typeof(Guid) || tp == typeof(decimal);
		}

		private static void SerializeObjectAndProperties(object item, Type itemType, IStorage storage)
		{
			object obj = UnitySerializer.currentlySerializingObject;
			UnitySerializer.currentlySerializingObject = item;
			UnitySerializer.WriteFields(itemType, item, storage);
			UnitySerializer.WriteProperties(itemType, item, storage);
			UnitySerializer.currentlySerializingObject = obj;
		}

		internal static object CreateObject(Type itemType)
		{
			object result;
			try
			{
				if (UnitySerializer.Creators.ContainsKey(itemType))
				{
					result = UnitySerializer.Creators[itemType].Create(itemType);
				}
				else
				{
					if (typeof(Component).IsAssignableFrom(itemType))
					{
						if (typeof(Transform).IsAssignableFrom(itemType))
						{
							result = null;
							return result;
						}
						if (UnitySerializer._componentHelper == null)
						{
							UnitySerializer._componentHelper = new GameObject("Component Helper")
							{
								hideFlags = HideFlags.HideAndDontSave,
								active = false
							};
						}
						try
						{
							Component component = UnitySerializer._componentHelper.GetComponent(itemType);
							if (component == null)
							{
								component = UnitySerializer._componentHelper.AddComponent(itemType);
							}
							result = component;
							return result;
						}
						catch
						{
							result = null;
							return result;
						}
					}
					if (itemType.IsSubclassOf(typeof(ScriptableObject)))
					{
						result = ScriptableObject.CreateInstance(itemType);
					}
					else
					{
						result = ((!itemType.IsDefined(typeof(CreateUsingEvent), false)) ? Activator.CreateInstance(itemType) : UnitySerializer.CreateInstance(itemType));
					}
				}
			}
			catch (Exception)
			{
				try
				{
					ConstructorInfo constructor = itemType.GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, new Type[0], null);
					result = ((constructor == null) ? UnitySerializer.CreateInstance(itemType) : constructor.Invoke(new object[0]));
				}
				catch
				{
					result = UnitySerializer.CreateInstance(itemType);
				}
			}
			return result;
		}

		public static void DummyAction()
		{
		}

		private static object CreateInstance(Type itemType)
		{
			UnitySerializer.ObjectMappingEventArgs objectMappingEventArgs = new UnitySerializer.ObjectMappingEventArgs
			{
				TypeToConstruct = itemType
			};
			if (typeof(MulticastDelegate).IsAssignableFrom(itemType))
			{
				return Delegate.CreateDelegate(typeof(Action), typeof(UnitySerializer).GetMethod("DummyAction", BindingFlags.Static | BindingFlags.Public));
			}
			UnitySerializer.InvokeCreateType(objectMappingEventArgs);
			if (objectMappingEventArgs.Instance != null && (objectMappingEventArgs.Instance.GetType() == itemType || objectMappingEventArgs.Instance.GetType().IsSubclassOf(itemType)))
			{
				return objectMappingEventArgs.Instance;
			}
			string message = string.Format("Could not construct an object of type '{0}', it must be creatable in this scope and have a default parameterless constructor or you should handle the CreateType event on UnitySerializer to construct the object", itemType.FullName);
			throw new UnitySerializer.MissingConstructorException(message);
		}

		internal static ushort GetTypeId(Type tp)
		{
			ushort num;
			if (!UnitySerializer.PrewarmedTypes.TryGetValue(tp, out num) && !UnitySerializer._knownTypesLookup.TryGetValue(tp, out num))
			{
				num = (ushort)UnitySerializer._knownTypesLookup.Count;
				UnitySerializer._knownTypesLookup[tp] = num;
			}
			return num;
		}

		private static void UpdateEntryWithName(Entry entry)
		{
			if (entry.Name == null)
			{
				Radical.Log("Invalid Entry", new object[0]);
			}
			Dictionary<string, UnitySerializer.EntryConfiguration> dictionary;
			if (!UnitySerializer.StoredTypes.TryGetValue(entry.OwningType, out dictionary))
			{
				dictionary = new Dictionary<string, UnitySerializer.EntryConfiguration>();
				UnitySerializer.StoredTypes[entry.OwningType] = dictionary;
			}
			UnitySerializer.EntryConfiguration entryConfiguration;
			if (!dictionary.TryGetValue(entry.Name, out entryConfiguration))
			{
				entryConfiguration = new UnitySerializer.EntryConfiguration();
				PropertyInfo property = entry.OwningType.GetProperty(entry.Name, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public);
				if (property != null)
				{
					entryConfiguration.Type = property.PropertyType;
					entryConfiguration.Setter = new GetSetGeneric(property);
				}
				else
				{
					FieldInfo field = UnitySerializer.GetField(entry.OwningType, entry.Name);
					if (field != null)
					{
						entryConfiguration.Type = field.FieldType;
						entryConfiguration.Setter = new GetSetGeneric(field);
					}
				}
				dictionary[entry.Name] = entryConfiguration;
			}
			entry.StoredType = entryConfiguration.Type;
			entry.Setter = entryConfiguration.Setter;
		}

		private static FieldInfo GetField(Type tp, string name)
		{
			FieldInfo result = null;
			while (tp != null && (result = tp.GetField(name, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)) == null)
			{
				tp = tp.BaseType;
			}
			return result;
		}

		private static void SerializeObject(Entry entry, IStorage storage)
		{
			UnitySerializer.SerializeObject(entry, storage, false);
		}

		private static bool CompareToNull(object o)
		{
			return (!(o is UnityEngine.Object)) ? (o == null) : (!(UnityEngine.Object)o);
		}

		private static void SerializeObject(Entry entry, IStorage storage, bool first)
		{
			object value = entry.Value;
			int num = UnitySerializer._nextId++;
			if (UnitySerializer.CompareToNull(value))
			{
				entry.Value = new UnitySerializer.Nuller();
				value = entry.Value;
			}
			if (storage.StartSerializing(entry, num))
			{
				UnitySerializer._seenObjects[value] = num;
				return;
			}
			Type type = value.GetType();
			using (new UnitySerializer.TypePusher(type))
			{
				if (UnitySerializer.IsSimpleType(type))
				{
					storage.WriteSimpleValue((!type.IsEnum) ? value : Convert.ChangeType(value, Enum.GetUnderlyingType(type), CultureInfo.InvariantCulture));
				}
				else if (!type.IsValueType && UnitySerializer._seenObjects.ContainsKey(value) && !first)
				{
					storage.BeginWriteObject(UnitySerializer._seenObjects[value], value.GetType(), true);
					storage.EndWriteObject();
				}
				else
				{
					bool flag = false;
					if (!first)
					{
						if (UnitySerializer.Serializers.ContainsKey(type))
						{
							storage.BeginWriteObject(num, type, false);
							storage.BeginWriteProperty("data", typeof(object[]));
							ISerializeObject serializeObject = UnitySerializer.Serializers[type];
							object[] value2 = serializeObject.Serialize(value);
							using (new UnitySerializer.SerializationSplitScope())
							{
								UnitySerializer.SerializeObject(new Entry
								{
									Name = "data",
									Value = value2,
									StoredType = typeof(object[])
								}, storage);
							}
							storage.EndWriteProperty();
							storage.EndWriteObject();
							UnitySerializer._seenObjects[value] = num;
							return;
						}
						ISerializeObject value3;
						if (!UnitySerializer.cachedSerializers.TryGetValue(type, out value3))
						{
							foreach (KeyValuePair<Type, ISerializeObject> current in UnitySerializer.SubTypeSerializers)
							{
								if (current.Key.IsAssignableFrom(type) && (!current.Value.GetType().IsDefined(typeof(OnlyInterfaces), false) || type.IsInterface))
								{
									value3 = current.Value;
									break;
								}
							}
							UnitySerializer.cachedSerializers[type] = value3;
						}
						if (value3 != null)
						{
							if (!(value3 is ISerializeObjectEx) || (value3 as ISerializeObjectEx).CanSerialize(type, entry.Value))
							{
								storage.BeginWriteObject(num, type, false);
								storage.BeginWriteProperty("data", typeof(object[]));
								object[] value4 = value3.Serialize(value);
								using (new UnitySerializer.SerializationSplitScope())
								{
									UnitySerializer.SerializeObject(new Entry
									{
										Name = "data",
										Value = value4,
										StoredType = typeof(object[])
									}, storage);
								}
								storage.EndWriteProperty();
								storage.EndWriteObject();
								UnitySerializer._seenObjects[value] = num;
							}
							return;
						}
					}
					else
					{
						flag = true;
					}
					if (!flag && !type.IsValueType)
					{
						UnitySerializer._seenObjects[value] = num;
					}
					storage.BeginWriteObject(num, type, false);
					if (value is Array)
					{
						if (((Array)value).Rank == 1)
						{
							UnitySerializer.SerializeArray(value as Array, type, storage);
						}
						else
						{
							UnitySerializer.SerializeMultiDimensionArray(value as Array, type, storage);
						}
						storage.EndWriteObject();
					}
					else
					{
						if (value is IDictionary)
						{
							UnitySerializer.SerializeDictionary(value as IDictionary, type, storage);
						}
						else if (value is IList)
						{
							UnitySerializer.SerializeList(value as IList, type, storage);
						}
						UnitySerializer.SerializeObjectAndProperties(value, type, storage);
						storage.EndWriteObject();
					}
				}
			}
		}

		private static void SerializeList(ICollection item, Type tp, IStorage storage)
		{
			Type storedType = null;
			if (tp.IsGenericType)
			{
				Type[] genericArguments = tp.GetGenericArguments();
				storedType = genericArguments[0];
			}
			storage.BeginWriteList(item.Count, item.GetType());
			Entry entry = new Entry();
			int num = 0;
			foreach (object current in item)
			{
				entry.Value = current;
				entry.StoredType = storedType;
				if (!storage.BeginWriteListItem(num++, current))
				{
					UnitySerializer.SerializeObject(entry, storage);
				}
				storage.EndWriteListItem();
			}
			storage.EndWriteList();
		}

		private static void SerializeDictionary(IDictionary item, Type tp, IStorage storage)
		{
			Type storedType = null;
			Type storedType2 = null;
			if (tp.IsGenericType)
			{
				Type[] genericArguments = tp.GetGenericArguments();
				storedType = genericArguments[0];
				storedType2 = genericArguments[1];
			}
			storage.BeginWriteDictionary(item.Count, item.GetType());
			storage.BeginWriteDictionaryKeys();
			int num = 0;
			foreach (object current in item.Keys)
			{
				if (!storage.BeginWriteDictionaryKey(num++, current))
				{
					UnitySerializer.SerializeObject(new Entry
					{
						StoredType = storedType,
						Value = current
					}, storage);
				}
				storage.EndWriteDictionaryKey();
			}
			storage.EndWriteDictionaryKeys();
			storage.BeginWriteDictionaryValues();
			num = 0;
			foreach (object current2 in item.Values)
			{
				if (!storage.BeginWriteDictionaryValue(num++, current2))
				{
					UnitySerializer.SerializeObject(new Entry
					{
						StoredType = storedType2,
						Value = current2
					}, storage);
				}
				storage.EndWriteDictionaryValue();
			}
			storage.EndWriteDictionaryValues();
			storage.EndWriteDictionary();
		}

		private static void SerializeArray(Array item, Type tp, IStorage storage)
		{
			Type elementType = tp.GetElementType();
			if (UnitySerializer.IsSimpleType(elementType))
			{
				storage.WriteSimpleArray(item.Length, item);
			}
			else
			{
				int length = item.Length;
				storage.BeginWriteObjectArray(length, item.GetType());
				for (int i = 0; i < length; i++)
				{
					object value = item.GetValue(i);
					if (!storage.BeginWriteObjectArrayItem(i, value))
					{
						UnitySerializer.SerializeObject(new Entry
						{
							Value = item.GetValue(i),
							StoredType = elementType
						}, storage);
					}
					storage.EndWriteObjectArrayItem();
				}
				storage.EndWriteObjectArray();
			}
		}

		private static void SerializeMultiDimensionArray(Array item, Type tp, IStorage storage)
		{
			int rank = item.Rank;
			int length = item.GetLength(0);
			storage.BeginMultiDimensionArray(item.GetType(), rank, length);
			int[] array = new int[rank];
			for (int i = 0; i < rank; i++)
			{
				array[i] = 0;
				storage.WriteArrayDimension(i, item.GetLength(i));
			}
			UnitySerializer.SerializeArrayPart(item, 0, array, storage);
			storage.EndMultiDimensionArray();
		}

		private static void SerializeArrayPart(Array item, int i, int[] indices, IStorage storage)
		{
			int length = item.GetLength(i);
			for (int j = 0; j < length; j++)
			{
				indices[i] = j;
				if (i != item.Rank - 2)
				{
					UnitySerializer.SerializeArrayPart(item, i + 1, indices, storage);
				}
				else
				{
					Type elementType = item.GetType().GetElementType();
					int length2 = item.GetLength(i + 1);
					Array array = Array.CreateInstance(elementType, length2);
					for (int k = 0; k < length2; k++)
					{
						indices[i + 1] = k;
						array.SetValue(item.GetValue(indices), k);
					}
					UnitySerializer.SerializeArray(array, array.GetType(), storage);
				}
			}
		}

		private static void WriteProperties(Type itemType, object item, IStorage storage)
		{
			bool seen = UnitySerializer._seenTypes.ContainsKey(itemType) && UnitySerializer._seenTypes[itemType];
			UnitySerializer._seenTypes[itemType] = true;
			Entry[] properties = GetWritableAttributes.GetProperties(item, seen);
			storage.BeginWriteProperties(properties.Length);
			Entry[] array = properties;
			for (int i = 0; i < array.Length; i++)
			{
				Entry entry = array[i];
				storage.BeginWriteProperty(entry.Name, entry.PropertyInfo.PropertyType);
				UnitySerializer.SerializeObject(entry, storage, false);
				storage.EndWriteProperty();
			}
			storage.EndWriteProperties();
		}

		private static void WriteFields(Type itemType, object item, IStorage storage)
		{
			bool flag = UnitySerializer._seenTypes.ContainsKey(itemType);
			if (!flag)
			{
				UnitySerializer._seenTypes[itemType] = false;
			}
			Entry[] fields = GetWritableAttributes.GetFields(item, flag);
			storage.BeginWriteFields(fields.Length);
			Entry[] array = fields;
			for (int i = 0; i < array.Length; i++)
			{
				Entry entry = array[i];
				storage.BeginWriteField(entry.Name, entry.FieldInfo.FieldType);
				UnitySerializer.SerializeObject(entry, storage, false);
				storage.EndWriteField();
			}
			storage.EndWriteFields();
		}

		internal static object DeserializeObject(Entry entry, IStorage storage)
		{
			object result;
			try
			{
				int num = UnitySerializer._nextId++;
				storage.DeserializeGetName(entry);
				if (entry.MustHaveName)
				{
					UnitySerializer.UpdateEntryWithName(entry);
				}
				object obj = storage.StartDeserializing(entry);
				if (obj != null)
				{
					storage.FinishDeserializing(entry);
					result = obj;
				}
				else
				{
					Type storedType = entry.StoredType;
					if (storedType == null)
					{
						result = null;
					}
					else if (UnitySerializer.IsSimpleType(storedType))
					{
						if (storedType.IsEnum)
						{
							result = Enum.Parse(storedType, storage.ReadSimpleValue(Enum.GetUnderlyingType(storedType)).ToString(), true);
						}
						else
						{
							result = storage.ReadSimpleValue(storedType);
						}
					}
					else
					{
						bool flag;
						int num2 = storage.BeginReadObject(out flag);
						if (num2 != -1 && entry.Value == null && flag)
						{
							try
							{
								object obj2 = UnitySerializer._loadedObjects[num2];
								storage.EndReadObject();
								result = obj2;
								return result;
							}
							catch
							{
								throw new SerializationException("Error when trying to link to a previously seen object. The stream gave an object id of " + num2 + " but that was not found.  It is possible that anerror has caused the data stream to become corrupt and that this id is wildly out of range.  Ids should be sequential numbers starting at 1 for the first object or value seen and then incrementing thereafter.");
							}
						}
						if (entry.Value != null)
						{
							UnitySerializer._loadedObjects[num] = entry.Value;
						}
						if (entry.Value == null)
						{
							if (UnitySerializer.Serializers.ContainsKey(storedType))
							{
								ISerializeObject serializeObject = UnitySerializer.Serializers[storedType];
								Entry entry2 = new Entry
								{
									Name = "data",
									StoredType = typeof(object[])
								};
								storage.BeginReadProperty(entry2);
								object[] data = null;
								using (new UnitySerializer.SerializationSplitScope())
								{
									data = (object[])UnitySerializer.DeserializeObject(entry2, storage);
								}
								object obj3 = serializeObject.Deserialize(data, entry.Value);
								storage.EndReadProperty();
								storage.EndReadObject();
								UnitySerializer._loadedObjects[num] = obj3;
								storage.FinishDeserializing(entry);
								result = obj3;
								return result;
							}
							ISerializeObject serializeObject2;
							if (!UnitySerializer.cachedSerializers.TryGetValue(storedType, out serializeObject2))
							{
								serializeObject2 = null;
								foreach (KeyValuePair<Type, ISerializeObject> current in UnitySerializer.SubTypeSerializers)
								{
									if (current.Key.IsAssignableFrom(storedType) && (!current.Value.GetType().IsDefined(typeof(OnlyInterfaces), false) || storedType.IsInterface))
									{
										serializeObject2 = current.Value;
										break;
									}
								}
								UnitySerializer.cachedSerializers[storedType] = serializeObject2;
							}
							if (serializeObject2 != null && (!(serializeObject2 is ISerializeObjectEx) || (serializeObject2 as ISerializeObjectEx).CanSerialize(storedType, entry.Value)))
							{
								Entry entry3 = new Entry
								{
									Name = "data",
									StoredType = typeof(object[])
								};
								storage.BeginReadProperty(entry3);
								object[] data2;
								using (new UnitySerializer.SerializationSplitScope())
								{
									data2 = (object[])UnitySerializer.DeserializeObject(entry3, storage);
								}
								object obj4 = serializeObject2.Deserialize(data2, entry.Value);
								storage.EndReadProperty();
								storage.EndReadObject();
								UnitySerializer._loadedObjects[num] = obj4;
								storage.FinishDeserializing(entry);
								result = obj4;
								return result;
							}
						}
						if (storedType.IsArray)
						{
							int count;
							bool flag2 = storage.IsMultiDimensionalArray(out count);
							if (flag2)
							{
								object obj5 = UnitySerializer.DeserializeMultiDimensionArray(storedType, storage, num);
								storage.EndReadObject();
								UnitySerializer._loadedObjects[num] = obj5;
								storage.FinishDeserializing(entry);
								result = obj5;
							}
							else
							{
								object obj6 = UnitySerializer.DeserializeArray(storedType, storage, count, num);
								storage.EndReadObject();
								UnitySerializer._loadedObjects[num] = obj6;
								storage.FinishDeserializing(entry);
								result = obj6;
							}
						}
						else
						{
							object obj7 = entry.Value ?? UnitySerializer.CreateObject(storedType);
							if (storedType.IsValueType)
							{
								obj7 = RuntimeHelpers.GetObjectValue(obj7);
							}
							UnitySerializer._loadedObjects[num] = obj7;
							if (obj7 is IDictionary)
							{
								UnitySerializer.DeserializeDictionary(obj7 as IDictionary, storedType, storage);
							}
							if (obj7 is IList)
							{
								UnitySerializer.DeserializeList(obj7 as IList, storedType, storage);
							}
							object obj8 = UnitySerializer.DeserializeObjectAndProperties(obj7, storedType, storage);
							storage.EndReadObject();
							if (obj8 is IDeserialized)
							{
								UnitySerializer.DeserializedObject.Add(obj8 as IDeserialized);
							}
							if (obj7 is UnitySerializer.Nuller)
							{
								result = null;
							}
							else
							{
								result = obj8;
							}
						}
					}
				}
			}
			finally
			{
			}
			return result;
		}

		private static object DeserializeArray(Type itemType, IStorage storage, int count, int objectID)
		{
			Type elementType = itemType.GetElementType();
			Array result = null;
			if (UnitySerializer.IsSimpleType(elementType))
			{
				result = storage.ReadSimpleArray(elementType, count);
				UnitySerializer._loadedObjects[objectID] = result;
			}
			else
			{
				if (count == -1)
				{
					count = storage.BeginReadObjectArray(itemType);
				}
				result = Array.CreateInstance(elementType, count);
				UnitySerializer._loadedObjects[objectID] = result;
				int num = 0;
				while ((count != -1) ? (num < count) : storage.HasMore())
				{
					Entry entry = new Entry
					{
						StoredType = elementType
					};
					object obj = storage.BeginReadObjectArrayItem(num, entry);
					obj = (obj ?? UnitySerializer.DeserializeObject(entry, storage));
					if (obj != null && obj.GetType().IsDefined(typeof(DeferredAttribute), true))
					{
						object toSet = obj;
						obj = new UnitySerializer.DeferredSetter((Dictionary<string, object> d) => toSet);
					}
					if (obj is UnitySerializer.DeferredSetter)
					{
						UnitySerializer.DeferredSetter st = obj as UnitySerializer.DeferredSetter;
						int pos = num;
						UnitySerializer.DeferredSetter nd = new UnitySerializer.DeferredSetter(st.deferredRetrievalFunction)
						{
							enabled = st.enabled
						};
						nd._setAction = delegate
						{
							if (result != null)
							{
								result.SetValue(nd.deferredRetrievalFunction(st.parameters), pos);
							}
						};
						UnitySerializer.AddFixup(nd);
					}
					else
					{
						result.SetValue(obj, num);
					}
					storage.EndReadObjectArrayItem();
					num++;
				}
				if (count != -1)
				{
					storage.EndReadObjectArray();
				}
			}
			return result;
		}

		private static object DeserializeMultiDimensionArray(Type itemType, IStorage storage, int objectID)
		{
			int num;
			int num2;
			storage.BeginReadMultiDimensionalArray(out num, out num2);
			int[] array = new int[num];
			int[] array2 = new int[num];
			for (int i = 0; i < num; i++)
			{
				array[i] = storage.ReadArrayDimension(i);
				array2[i] = 0;
			}
			Type elementType = itemType.GetElementType();
			Array array3 = Array.CreateInstance(elementType, array);
			UnitySerializer.DeserializeArrayPart(array3, 0, array2, itemType, storage, objectID);
			return array3;
		}

		private static void DeserializeArrayPart(Array sourceArrays, int i, int[] indices, Type itemType, IStorage storage, int objectID)
		{
			int length = sourceArrays.GetLength(i);
			for (int j = 0; j < length; j++)
			{
				indices[i] = j;
				if (i != sourceArrays.Rank - 2)
				{
					UnitySerializer.DeserializeArrayPart(sourceArrays, i + 1, indices, itemType, storage, objectID);
				}
				else
				{
					Array array = (Array)UnitySerializer.DeserializeArray(itemType, storage, -1, objectID);
					int length2 = sourceArrays.GetLength(i + 1);
					for (int k = 0; k < length2; k++)
					{
						indices[i + 1] = k;
						sourceArrays.SetValue(array.GetValue(k), indices);
					}
				}
			}
		}

		private static object DeserializeDictionary(IDictionary o, Type itemType, IStorage storage)
		{
			Type type = null;
			Type type2 = null;
			if (itemType.IsGenericType)
			{
				Type[] genericArguments = itemType.GetGenericArguments();
				type = genericArguments[0];
				type2 = genericArguments[1];
			}
			int num = storage.BeginReadDictionary(type, type2);
			storage.BeginReadDictionaryKeys();
			List<object> list = new List<object>();
			int num2 = 0;
			while ((num != -1) ? (num2 < num) : storage.HasMore())
			{
				Entry entry = new Entry
				{
					StoredType = type
				};
				object obj = storage.BeginReadDictionaryKeyItem(num2, entry) ?? UnitySerializer.DeserializeObject(entry, storage);
				if (obj.GetType().IsDefined(typeof(DeferredAttribute), true))
				{
					object toSet = obj;
					obj = new UnitySerializer.DeferredSetter((Dictionary<string, object> d) => toSet);
				}
				if (obj is UnitySerializer.DeferredSetter)
				{
					UnitySerializer.DeferredSetter st = obj as UnitySerializer.DeferredSetter;
					UnitySerializer.DeferredSetter nd = new UnitySerializer.DeferredSetter(st.deferredRetrievalFunction)
					{
						enabled = st.enabled
					};
					list.Add(null);
					int c = list.Count - 1;
					nd._setAction = delegate
					{
						if (list.Count > c)
						{
							list[c] = nd.deferredRetrievalFunction(st.parameters);
						}
					};
					UnitySerializer.AddFixup(nd);
				}
				else
				{
					list.Add(obj);
				}
				storage.EndReadDictionaryKeyItem();
				num2++;
			}
			storage.EndReadDictionaryKeys();
			storage.BeginReadDictionaryValues();
			int num3 = 0;
			while ((num != -1) ? (num3 < num) : storage.HasMore())
			{
				Entry entry2 = new Entry
				{
					StoredType = type2
				};
				object obj2 = storage.BeginReadDictionaryValueItem(num3, entry2) ?? UnitySerializer.DeserializeObject(entry2, storage);
				if ((obj2 != null && obj2.GetType().IsDefined(typeof(DeferredAttribute), true)) || list[num3] == null)
				{
					object toSet = obj2;
					obj2 = new UnitySerializer.DeferredSetter((Dictionary<string, object> d) => toSet);
				}
				if (obj2 is UnitySerializer.DeferredSetter)
				{
					UnitySerializer.DeferredSetter st = obj2 as UnitySerializer.DeferredSetter;
					UnitySerializer.DeferredSetter nd = new UnitySerializer.DeferredSetter(st.deferredRetrievalFunction)
					{
						enabled = st.enabled
					};
					int index = num3;
					nd._setAction = delegate
					{
						if (o != null && list != null)
						{
							o[list[index]] = nd.deferredRetrievalFunction(st.parameters);
						}
					};
					UnitySerializer.AddFixup(nd);
				}
				else
				{
					o[list[num3]] = obj2;
				}
				storage.EndReadDictionaryValueItem();
				num3++;
			}
			storage.EndReadDictionaryValues();
			storage.EndReadDictionary();
			if (UnitySerializer.currentVersion >= 7 && UnitySerializer.currentVersion < 9)
			{
				UnitySerializer.DeserializeObjectAndProperties(o, itemType, storage);
			}
			return o;
		}

		private static object DeserializeList(IList o, Type itemType, IStorage storage)
		{
			Type type = null;
			if (itemType.IsGenericType)
			{
				Type[] genericArguments = itemType.GetGenericArguments();
				type = genericArguments[0];
			}
			int num = storage.BeginReadList(type);
			int num2 = 0;
			while ((num != -1) ? (num2 < num) : storage.HasMore())
			{
				Entry entry = new Entry
				{
					StoredType = type
				};
				object obj = storage.BeginReadListItem(num2, entry) ?? UnitySerializer.DeserializeObject(entry, storage);
				if (obj != null && obj.GetType().IsDefined(typeof(DeferredAttribute), true))
				{
					object toSet = obj;
					obj = new UnitySerializer.DeferredSetter((Dictionary<string, object> d) => toSet);
				}
				if (obj is UnitySerializer.DeferredSetter)
				{
					UnitySerializer.DeferredSetter st = obj as UnitySerializer.DeferredSetter;
					UnitySerializer.DeferredSetter nd = new UnitySerializer.DeferredSetter(st.deferredRetrievalFunction)
					{
						enabled = st.enabled
					};
					nd._setAction = delegate
					{
						if (o != null)
						{
							o.Add(nd.deferredRetrievalFunction(st.parameters));
						}
					};
					UnitySerializer.AddFixup(nd);
				}
				else
				{
					o.Add(obj);
				}
				storage.EndReadListItem();
				num2++;
			}
			if (UnitySerializer.currentVersion >= 7 && UnitySerializer.currentVersion < 9)
			{
				UnitySerializer.DeserializeObjectAndProperties(o, itemType, storage);
			}
			storage.EndReadList();
			return o;
		}

		private static object DeserializeObjectAndProperties(object o, Type itemType, IStorage storage)
		{
			UnitySerializer.DeserializingStack.Push(UnitySerializer.DeserializingObject);
			try
			{
				object obj = UnitySerializer.currentlySerializingObject;
				UnitySerializer.currentlySerializingObject = o;
				UnitySerializer.DeserializingObject = o;
				UnitySerializer.DeserializeFields(storage, itemType, o);
				UnitySerializer.DeserializeProperties(storage, itemType, o);
				UnitySerializer.currentlySerializingObject = obj;
			}
			finally
			{
				UnitySerializer.DeserializingObject = UnitySerializer.DeserializingStack.Pop();
			}
			return o;
		}

		private static void DeserializeProperties(IStorage storage, Type itemType, object o)
		{
			int num = storage.BeginReadProperties();
			int num2 = 0;
			while ((num == -1) ? storage.HasMore() : (num2 < num))
			{
				Entry entry = storage.BeginReadProperty(new Entry
				{
					OwningType = itemType,
					MustHaveName = true
				});
				object obj = UnitySerializer.DeserializeObject(entry, storage);
				if (entry.Setter != null && obj != null)
				{
					try
					{
						if (obj.GetType().IsDefined(typeof(DeferredAttribute), true))
						{
							object toSet = obj;
							obj = new UnitySerializer.DeferredSetter((Dictionary<string, object> d) => toSet);
						}
						if (obj is UnitySerializer.DeferredSetter)
						{
							UnitySerializer.DeferredSetter setter = obj as UnitySerializer.DeferredSetter;
							UnitySerializer.DeferredSetter deferredSetter = new UnitySerializer.DeferredSetter(setter.deferredRetrievalFunction)
							{
								enabled = setter.enabled
							};
							deferredSetter._setAction = delegate
							{
								entry.Setter.Set(o, setter.deferredRetrievalFunction(setter.parameters));
							};
							if (entry.OwningType.IsValueType)
							{
								deferredSetter._setAction();
							}
							else
							{
								UnitySerializer.AddFixup(deferredSetter);
							}
						}
						else
						{
							entry.Setter.Set(o, obj);
						}
					}
					catch (ArgumentException)
					{
						try
						{
							Type underlyingType = Nullable.GetUnderlyingType(entry.Setter.Info.PropertyType);
							if (underlyingType != null && underlyingType.IsEnum)
							{
								entry.Setter.Info.SetValue(o, Enum.Parse(underlyingType, obj.ToString(), true), null);
							}
							else
							{
								entry.Setter.Info.SetValue(o, Convert.ChangeType(obj, entry.Setter.Info.PropertyType, null), null);
							}
						}
						catch (Exception ex)
						{
							Radical.LogError("Serialization error: " + ex.ToString());
						}
					}
					catch (Exception ex2)
					{
						Radical.LogError("Serialization error: " + ex2.ToString());
					}
				}
				storage.EndReadProperty();
				num2++;
			}
			storage.EndReadProperties();
		}

		private static void DeserializeFields(IStorage storage, Type itemType, object o)
		{
			int num = storage.BeginReadFields();
			int num2 = 0;
			while ((num != -1) ? (num2 < num) : storage.HasMore())
			{
				Entry entry = storage.BeginReadField(new Entry
				{
					OwningType = itemType,
					MustHaveName = true
				});
				object obj = UnitySerializer.DeserializeObject(entry, storage);
				if (entry.Setter != null && obj != null)
				{
					try
					{
						if (obj.GetType().IsDefined(typeof(DeferredAttribute), true))
						{
							object toSet = obj;
							obj = new UnitySerializer.DeferredSetter((Dictionary<string, object> d) => toSet);
						}
						if (obj is UnitySerializer.DeferredSetter)
						{
							UnitySerializer.DeferredSetter setter = obj as UnitySerializer.DeferredSetter;
							UnitySerializer.DeferredSetter deferredSetter = new UnitySerializer.DeferredSetter(setter.deferredRetrievalFunction)
							{
								enabled = setter.enabled,
								_setAction = delegate
								{
									if (entry.Setter != null)
									{
										entry.Setter.Set(o, setter.deferredRetrievalFunction(setter.parameters));
									}
								}
							};
							if (entry.OwningType.IsValueType)
							{
								deferredSetter._setAction();
							}
							else
							{
								UnitySerializer.AddFixup(deferredSetter);
							}
						}
						else
						{
							entry.Setter.Set(o, obj);
						}
					}
					catch (ArgumentException)
					{
						try
						{
							Type underlyingType = Nullable.GetUnderlyingType(entry.Setter.FieldInfo.FieldType);
							if (underlyingType != null && underlyingType.IsEnum)
							{
								entry.Setter.FieldInfo.SetValue(o, Enum.Parse(underlyingType, obj.ToString(), true));
							}
							else
							{
								entry.Setter.FieldInfo.SetValue(o, Convert.ChangeType(obj, entry.Setter.FieldInfo.FieldType, null));
							}
						}
						catch (Exception ex)
						{
							Radical.LogError("Serialization error: " + ex.ToString());
						}
					}
					catch (Exception ex2)
					{
						Radical.LogError("Serialization error: " + ex2.ToString());
					}
				}
				storage.EndReadField();
				num2++;
			}
			storage.EndReadFields();
		}

		private static object ShortReader(BinaryReader reader)
		{
			return reader.ReadInt16();
		}

		private static object LongReader(BinaryReader reader)
		{
			return reader.ReadInt64();
		}

		private static object GuidReader(BinaryReader reader)
		{
			if (UnitySerializer.currentVersion >= 10)
			{
				return new Guid(reader.ReadBytes(16));
			}
			return new Guid(reader.ReadString());
		}

		private static object SByteReader(BinaryReader reader)
		{
			return reader.ReadSByte();
		}

		private static object ByteReader(BinaryReader reader)
		{
			return reader.ReadByte();
		}

		private static object UIntReader(BinaryReader reader)
		{
			return reader.ReadUInt32();
		}

		private static object IntReader(BinaryReader reader)
		{
			return reader.ReadInt32();
		}

		private static object ULongReader(BinaryReader reader)
		{
			return reader.ReadUInt64();
		}

		private static object DoubleReader(BinaryReader reader)
		{
			return reader.ReadDouble();
		}

		private static object UShortReader(BinaryReader reader)
		{
			return reader.ReadUInt16();
		}

		private static object CharReader(BinaryReader reader)
		{
			return reader.ReadChar();
		}

		private static object FloatReader(BinaryReader reader)
		{
			return reader.ReadSingle();
		}

		private static object TimeSpanReader(BinaryReader reader)
		{
			return new TimeSpan(reader.ReadInt64());
		}

		private static object DateTimeReader(BinaryReader reader)
		{
			return new DateTime(reader.ReadInt64());
		}

		private static object ByteArrayReader(BinaryReader reader)
		{
			int count = reader.ReadInt32();
			return reader.ReadBytes(count);
		}

		private static object DecimalReader(BinaryReader reader)
		{
			return new decimal(new int[]
			{
				reader.ReadInt32(),
				reader.ReadInt32(),
				reader.ReadInt32(),
				reader.ReadInt32()
			});
		}

		private static object BoolReader(BinaryReader reader)
		{
			return reader.ReadChar() == 'Y';
		}

		private static object AStringReader(BinaryReader reader)
		{
			string text = reader.ReadString();
			return (!(text == "~~NULL~~")) ? text : null;
		}

		private static void SByteWriter(BinaryWriter writer, object value)
		{
			writer.Write((sbyte)value);
		}

		private static void ShortWriter(BinaryWriter writer, object value)
		{
			writer.Write((short)value);
		}

		private static void LongWriter(BinaryWriter writer, object value)
		{
			writer.Write((long)value);
		}

		private static void ByteWriter(BinaryWriter writer, object value)
		{
			writer.Write((byte)value);
		}

		private static void UIntWriter(BinaryWriter writer, object value)
		{
			writer.Write((uint)value);
		}

		private static void IntWriter(BinaryWriter writer, object value)
		{
			writer.Write((int)value);
		}

		private static void ULongWriter(BinaryWriter writer, object value)
		{
			writer.Write((ulong)value);
		}

		private static void DoubleWriter(BinaryWriter writer, object value)
		{
			writer.Write((double)value);
		}

		private static void UShortWriter(BinaryWriter writer, object value)
		{
			writer.Write((ushort)value);
		}

		private static void CharWriter(BinaryWriter writer, object value)
		{
			writer.Write((char)value);
		}

		private static void TimeSpanWriter(BinaryWriter writer, object value)
		{
			writer.Write(((TimeSpan)value).Ticks);
		}

		private static void DateTimeWriter(BinaryWriter writer, object value)
		{
			writer.Write(((DateTime)value).Ticks);
		}

		private static void GuidWriter(BinaryWriter writer, object value)
		{
			writer.Write(((Guid)value).ToByteArray());
		}

		private static void BoolWriter(BinaryWriter writer, object value)
		{
			writer.Write((!(bool)value) ? 'N' : 'Y');
		}

		private static void ByteArrayWriter(BinaryWriter writer, object value)
		{
			byte[] array = value as byte[];
			writer.Write(array.Length);
			writer.Write(array);
		}

		private static void FloatWriter(BinaryWriter writer, object value)
		{
			writer.Write((float)value);
		}

		private static void DecimalWriter(BinaryWriter writer, object value)
		{
			int[] bits = decimal.GetBits((decimal)value);
			writer.Write(bits[0]);
			writer.Write(bits[1]);
			writer.Write(bits[2]);
			writer.Write(bits[3]);
		}

		private static void StringWriter(BinaryWriter writer, object value)
		{
			writer.Write((string)value);
		}

		internal static void WriteValue(BinaryWriter writer, object value)
		{
			UnitySerializer.WriteAValue writeAValue;
			if (!UnitySerializer.Writers.TryGetValue(value.GetType(), out writeAValue))
			{
				writer.Write((int)value);
				return;
			}
			writeAValue(writer, value);
		}

		internal static object ReadValue(BinaryReader reader, Type tp)
		{
			UnitySerializer.ReadAValue readAValue;
			if (!UnitySerializer.Readers.TryGetValue(tp, out readAValue))
			{
				return reader.ReadInt32();
			}
			return readAValue(reader);
		}
	}
}
