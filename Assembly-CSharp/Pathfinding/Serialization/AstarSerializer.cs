using Pathfinding.Ionic.Zip;
using Pathfinding.Serialization.JsonFx;
using Pathfinding.Util;
using System;
using System.IO;
using System.Text;
using UnityEngine;

namespace Pathfinding.Serialization
{
	public class AstarSerializer
	{
		private const string binaryExt = ".binary";

		private const string jsonExt = ".json";

		private AstarData data;

		public JsonWriterSettings writerSettings;

		public JsonReaderSettings readerSettings;

		private ZipFile zip;

		private MemoryStream str;

		private GraphMeta meta;

		private SerializeSettings settings;

		private NavGraph[] graphs;

		private int graphIndexOffset;

		private uint checksum = 4294967295u;

		private UTF8Encoding encoding = new UTF8Encoding();

		private static StringBuilder _stringBuilder = new StringBuilder();

		public AstarSerializer(AstarData data)
		{
			this.data = data;
			this.settings = SerializeSettings.Settings;
		}

		public AstarSerializer(AstarData data, SerializeSettings settings)
		{
			this.data = data;
			this.settings = settings;
		}

		private static StringBuilder GetStringBuilder()
		{
			AstarSerializer._stringBuilder.Length = 0;
			return AstarSerializer._stringBuilder;
		}

		public void SetGraphIndexOffset(int offset)
		{
			this.graphIndexOffset = offset;
		}

		private void AddChecksum(byte[] bytes)
		{
			this.checksum = Checksum.GetChecksum(bytes, this.checksum);
		}

		public uint GetChecksum()
		{
			return this.checksum;
		}

		public void OpenSerialize()
		{
			this.zip = new ZipFile();
			this.zip.AlternateEncoding = Encoding.UTF8;
			this.zip.AlternateEncodingUsage = ZipOption.Always;
			this.writerSettings = new JsonWriterSettings();
			this.writerSettings.AddTypeConverter(new VectorConverter());
			this.writerSettings.AddTypeConverter(new BoundsConverter());
			this.writerSettings.AddTypeConverter(new LayerMaskConverter());
			this.writerSettings.AddTypeConverter(new MatrixConverter());
			this.writerSettings.AddTypeConverter(new GuidConverter());
			this.writerSettings.AddTypeConverter(new UnityObjectConverter());
			this.writerSettings.PrettyPrint = this.settings.prettyPrint;
			this.meta = new GraphMeta();
		}

		public byte[] CloseSerialize()
		{
			byte[] array = this.SerializeMeta();
			this.AddChecksum(array);
			this.zip.AddEntry("meta.json", array);
			DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
			foreach (ZipEntry current in this.zip.Entries)
			{
				current.AccessedTime = dateTime;
				current.CreationTime = dateTime;
				current.LastModified = dateTime;
				current.ModifiedTime = dateTime;
			}
			MemoryStream memoryStream = new MemoryStream();
			this.zip.Save(memoryStream);
			array = memoryStream.ToArray();
			memoryStream.Dispose();
			this.zip.Dispose();
			this.zip = null;
			return array;
		}

		public void SerializeGraphs(NavGraph[] _graphs)
		{
			if (this.graphs != null)
			{
				throw new InvalidOperationException("Cannot serialize graphs multiple times.");
			}
			this.graphs = _graphs;
			if (this.zip == null)
			{
				throw new NullReferenceException("You must not call CloseSerialize before a call to this function");
			}
			if (this.graphs == null)
			{
				this.graphs = new NavGraph[0];
			}
			for (int i = 0; i < this.graphs.Length; i++)
			{
				if (this.graphs[i] != null)
				{
					byte[] array = this.Serialize(this.graphs[i]);
					this.AddChecksum(array);
					this.zip.AddEntry("graph" + i + ".json", array);
				}
			}
		}

		private byte[] SerializeMeta()
		{
			this.meta.version = AstarPath.Version;
			this.meta.graphs = this.data.graphs.Length;
			this.meta.guids = new string[this.data.graphs.Length];
			this.meta.typeNames = new string[this.data.graphs.Length];
			this.meta.nodeCounts = new int[this.data.graphs.Length];
			for (int i = 0; i < this.data.graphs.Length; i++)
			{
				if (this.data.graphs[i] != null)
				{
					this.meta.guids[i] = this.data.graphs[i].guid.ToString();
					this.meta.typeNames[i] = this.data.graphs[i].GetType().FullName;
				}
			}
			StringBuilder stringBuilder = AstarSerializer.GetStringBuilder();
			JsonWriter jsonWriter = new JsonWriter(stringBuilder, this.writerSettings);
			jsonWriter.Write(this.meta);
			return this.encoding.GetBytes(stringBuilder.ToString());
		}

		public byte[] Serialize(NavGraph graph)
		{
			StringBuilder stringBuilder = AstarSerializer.GetStringBuilder();
			JsonWriter jsonWriter = new JsonWriter(stringBuilder, this.writerSettings);
			jsonWriter.Write(graph);
			return this.encoding.GetBytes(stringBuilder.ToString());
		}

		public void SerializeNodes()
		{
			if (!this.settings.nodes)
			{
				return;
			}
			if (this.graphs == null)
			{
				throw new InvalidOperationException("Cannot serialize nodes with no serialized graphs (call SerializeGraphs first)");
			}
			for (int i = 0; i < this.graphs.Length; i++)
			{
				byte[] array = this.SerializeNodes(i);
				this.AddChecksum(array);
				this.zip.AddEntry("graph" + i + "_nodes.binary", array);
			}
			for (int j = 0; j < this.graphs.Length; j++)
			{
				byte[] array2 = this.SerializeNodeConnections(j);
				this.AddChecksum(array2);
				this.zip.AddEntry("graph" + j + "_conns.binary", array2);
			}
		}

		private byte[] SerializeNodes(int index)
		{
			return new byte[0];
		}

		public void SerializeExtraInfo()
		{
			if (!this.settings.nodes)
			{
				return;
			}
			int totCount = 0;
			for (int i = 0; i < this.graphs.Length; i++)
			{
				if (this.graphs[i] != null)
				{
					this.graphs[i].GetNodes(delegate(GraphNode node)
					{
						totCount = Math.Max(node.NodeIndex, totCount);
						if (node.NodeIndex == -1)
						{
							Debug.LogError("Graph contains destroyed nodes. This is a bug.");
						}
						return true;
					});
				}
			}
			MemoryStream memoryStream = new MemoryStream();
			BinaryWriter wr = new BinaryWriter(memoryStream);
			wr.Write(totCount);
			int c = 0;
			for (int j = 0; j < this.graphs.Length; j++)
			{
				if (this.graphs[j] != null)
				{
					this.graphs[j].GetNodes(delegate(GraphNode node)
					{
						c = Math.Max(node.NodeIndex, c);
						wr.Write(node.NodeIndex);
						return true;
					});
				}
			}
			if (c != totCount)
			{
				throw new Exception("Some graphs are not consistent in their GetNodes calls, sequential calls give different results.");
			}
			byte[] array = memoryStream.ToArray();
			wr.Close();
			this.AddChecksum(array);
			this.zip.AddEntry("graph_references.binary", array);
			for (int k = 0; k < this.graphs.Length; k++)
			{
				if (this.graphs[k] != null)
				{
					MemoryStream memoryStream2 = new MemoryStream();
					BinaryWriter binaryWriter = new BinaryWriter(memoryStream2);
					GraphSerializationContext ctx = new GraphSerializationContext(binaryWriter);
					this.graphs[k].SerializeExtraInfo(ctx);
					byte[] array2 = memoryStream2.ToArray();
					binaryWriter.Close();
					this.AddChecksum(array2);
					this.zip.AddEntry("graph" + k + "_extra.binary", array2);
					memoryStream2 = new MemoryStream();
					binaryWriter = new BinaryWriter(memoryStream2);
					ctx = new GraphSerializationContext(binaryWriter);
					this.graphs[k].GetNodes(delegate(GraphNode node)
					{
						node.SerializeReferences(ctx);
						return true;
					});
					binaryWriter.Close();
					array2 = memoryStream2.ToArray();
					this.AddChecksum(array2);
					this.zip.AddEntry("graph" + k + "_references.binary", array2);
				}
			}
		}

		private byte[] SerializeNodeConnections(int index)
		{
			return new byte[0];
		}

		public void SerializeEditorSettings(GraphEditorBase[] editors)
		{
			if (editors == null || !this.settings.editorSettings)
			{
				return;
			}
			for (int i = 0; i < editors.Length; i++)
			{
				if (editors[i] == null)
				{
					return;
				}
				StringBuilder stringBuilder = AstarSerializer.GetStringBuilder();
				JsonWriter jsonWriter = new JsonWriter(stringBuilder, this.writerSettings);
				jsonWriter.Write(editors[i]);
				byte[] bytes = this.encoding.GetBytes(stringBuilder.ToString());
				if (bytes.Length > 2)
				{
					this.AddChecksum(bytes);
					this.zip.AddEntry("graph" + i + "_editor.json", bytes);
				}
			}
		}

		public bool OpenDeserialize(byte[] bytes)
		{
			this.readerSettings = new JsonReaderSettings();
			this.readerSettings.AddTypeConverter(new VectorConverter());
			this.readerSettings.AddTypeConverter(new BoundsConverter());
			this.readerSettings.AddTypeConverter(new LayerMaskConverter());
			this.readerSettings.AddTypeConverter(new MatrixConverter());
			this.readerSettings.AddTypeConverter(new GuidConverter());
			this.readerSettings.AddTypeConverter(new UnityObjectConverter());
			this.str = new MemoryStream();
			this.str.Write(bytes, 0, bytes.Length);
			this.str.Position = 0L;
			try
			{
				this.zip = ZipFile.Read(this.str);
			}
			catch (Exception arg)
			{
				Debug.LogWarning("Caught exception when loading from zip\n" + arg);
				this.str.Dispose();
				return false;
			}
			this.meta = this.DeserializeMeta(this.zip["meta.json"]);
			if (AstarSerializer.FullyDefinedVersion(this.meta.version) > AstarSerializer.FullyDefinedVersion(AstarPath.Version))
			{
				Debug.LogWarning(string.Concat(new object[]
				{
					"Trying to load data from a newer version of the A* Pathfinding Project\nCurrent version: ",
					AstarPath.Version,
					" Data version: ",
					this.meta.version,
					"\nThis is usually fine as the stored data is usually backwards and forwards compatible.\nHowever node data (not settings) can get corrupted between versions, so it is recommended to recalculate any caches (those for faster startup) and resave any files. Even if it seems to load fine, it might cause subtle bugs.\n"
				}));
			}
			else if (AstarSerializer.FullyDefinedVersion(this.meta.version) < AstarSerializer.FullyDefinedVersion(AstarPath.Version))
			{
				Debug.LogWarning(string.Concat(new object[]
				{
					"Trying to load data from an older version of the A* Pathfinding Project\nCurrent version: ",
					AstarPath.Version,
					" Data version: ",
					this.meta.version,
					"\nThis is usually fine, it just means you have upgraded to a new version.\nHowever node data (not settings) can get corrupted between versions, so it is recommended to recalculate any caches (those for faster startup) and resave any files. Even if it seems to load fine, it might cause subtle bugs.\n"
				}));
			}
			return true;
		}

		private static Version FullyDefinedVersion(Version v)
		{
			return new Version(Mathf.Max(v.Major, 0), Mathf.Max(v.Minor, 0), Mathf.Max(v.Build, 0), Mathf.Max(v.Revision, 0));
		}

		public void CloseDeserialize()
		{
			this.str.Dispose();
			this.zip.Dispose();
			this.zip = null;
			this.str = null;
		}

		public NavGraph[] DeserializeGraphs()
		{
			this.graphs = new NavGraph[this.meta.graphs];
			int num = 0;
			for (int i = 0; i < this.meta.graphs; i++)
			{
				Type graphType = this.meta.GetGraphType(i);
				if (!object.Equals(graphType, null))
				{
					num++;
					ZipEntry zipEntry = this.zip["graph" + i + ".json"];
					if (zipEntry == null)
					{
						throw new FileNotFoundException(string.Concat(new object[]
						{
							"Could not find data for graph ",
							i,
							" in zip. Entry 'graph+",
							i,
							".json' does not exist"
						}));
					}
					NavGraph navGraph = this.data.CreateGraph(graphType);
					navGraph.graphIndex = (uint)(i + this.graphIndexOffset);
					string @string = this.GetString(zipEntry);
					JsonReader jsonReader = new JsonReader(@string, this.readerSettings);
					jsonReader.PopulateObject<NavGraph>(ref navGraph);
					this.graphs[i] = navGraph;
					if (this.graphs[i].guid.ToString() != this.meta.guids[i])
					{
						throw new Exception(string.Concat(new object[]
						{
							"Guid in graph file not equal to guid defined in meta file. Have you edited the data manually?\n",
							this.graphs[i].guid,
							" != ",
							this.meta.guids[i]
						}));
					}
				}
			}
			NavGraph[] array = new NavGraph[num];
			num = 0;
			for (int j = 0; j < this.graphs.Length; j++)
			{
				if (this.graphs[j] != null)
				{
					array[num] = this.graphs[j];
					num++;
				}
			}
			this.graphs = array;
			return this.graphs;
		}

		public void DeserializeExtraInfo()
		{
			bool flag = false;
			for (int i = 0; i < this.graphs.Length; i++)
			{
				ZipEntry zipEntry = this.zip["graph" + i + "_extra.binary"];
				if (zipEntry != null)
				{
					flag = true;
					MemoryStream memoryStream = new MemoryStream();
					zipEntry.Extract(memoryStream);
					memoryStream.Seek(0L, SeekOrigin.Begin);
					BinaryReader reader2 = new BinaryReader(memoryStream);
					GraphSerializationContext ctx2 = new GraphSerializationContext(reader2, null, i + this.graphIndexOffset);
					this.graphs[i].DeserializeExtraInfo(ctx2);
				}
			}
			if (!flag)
			{
				return;
			}
			int totCount = 0;
			for (int j = 0; j < this.graphs.Length; j++)
			{
				if (this.graphs[j] != null)
				{
					this.graphs[j].GetNodes(delegate(GraphNode node)
					{
						totCount = Math.Max(node.NodeIndex, totCount);
						if (node.NodeIndex == -1)
						{
							Debug.LogError("Graph contains destroyed nodes. This is a bug.");
						}
						return true;
					});
				}
			}
			ZipEntry zipEntry2 = this.zip["graph_references.binary"];
			if (zipEntry2 == null)
			{
				throw new Exception("Node references not found in the data. Was this loaded from an older version of the A* Pathfinding Project?");
			}
			MemoryStream memoryStream2 = new MemoryStream();
			zipEntry2.Extract(memoryStream2);
			memoryStream2.Seek(0L, SeekOrigin.Begin);
			BinaryReader reader = new BinaryReader(memoryStream2);
			int num = reader.ReadInt32();
			GraphNode[] int2Node = new GraphNode[num + 1];
			try
			{
				for (int k = 0; k < this.graphs.Length; k++)
				{
					if (this.graphs[k] != null)
					{
						this.graphs[k].GetNodes(delegate(GraphNode node)
						{
							int2Node[reader.ReadInt32()] = node;
							return true;
						});
					}
				}
			}
			catch (Exception innerException)
			{
				throw new Exception("Some graph(s) has thrown an exception during GetNodes, or some graph(s) have deserialized more or fewer nodes than were serialized", innerException);
			}
			reader.Close();
			for (int l = 0; l < this.graphs.Length; l++)
			{
				if (this.graphs[l] != null)
				{
					zipEntry2 = this.zip["graph" + l + "_references.binary"];
					if (zipEntry2 == null)
					{
						throw new Exception("Node references for graph " + l + " not found in the data. Was this loaded from an older version of the A* Pathfinding Project?");
					}
					memoryStream2 = new MemoryStream();
					zipEntry2.Extract(memoryStream2);
					memoryStream2.Seek(0L, SeekOrigin.Begin);
					reader = new BinaryReader(memoryStream2);
					GraphSerializationContext ctx = new GraphSerializationContext(reader, int2Node, l + this.graphIndexOffset);
					this.graphs[l].GetNodes(delegate(GraphNode node)
					{
						node.DeserializeReferences(ctx);
						return true;
					});
				}
			}
		}

		public void PostDeserialization()
		{
			for (int i = 0; i < this.graphs.Length; i++)
			{
				if (this.graphs[i] != null)
				{
					this.graphs[i].PostDeserialization();
				}
			}
		}

		public void DeserializeEditorSettings(GraphEditorBase[] graphEditors)
		{
			if (graphEditors == null)
			{
				return;
			}
			for (int i = 0; i < graphEditors.Length; i++)
			{
				if (graphEditors[i] != null)
				{
					for (int j = 0; j < this.graphs.Length; j++)
					{
						if (this.graphs[j] != null && graphEditors[i].target == this.graphs[j])
						{
							ZipEntry zipEntry = this.zip["graph" + j + "_editor.json"];
							if (zipEntry != null)
							{
								string @string = this.GetString(zipEntry);
								JsonReader jsonReader = new JsonReader(@string, this.readerSettings);
								GraphEditorBase graphEditorBase = graphEditors[i];
								jsonReader.PopulateObject<GraphEditorBase>(ref graphEditorBase);
								graphEditors[i] = graphEditorBase;
								break;
							}
						}
					}
				}
			}
		}

		private string GetString(ZipEntry entry)
		{
			MemoryStream memoryStream = new MemoryStream();
			entry.Extract(memoryStream);
			memoryStream.Position = 0L;
			StreamReader streamReader = new StreamReader(memoryStream);
			string result = streamReader.ReadToEnd();
			memoryStream.Position = 0L;
			streamReader.Dispose();
			return result;
		}

		private GraphMeta DeserializeMeta(ZipEntry entry)
		{
			if (entry == null)
			{
				throw new Exception("No metadata found in serialized data.");
			}
			string @string = this.GetString(entry);
			JsonReader jsonReader = new JsonReader(@string, this.readerSettings);
			return (GraphMeta)jsonReader.Deserialize(typeof(GraphMeta));
		}

		public static void SaveToFile(string path, byte[] data)
		{
			using (FileStream fileStream = new FileStream(path, FileMode.Create))
			{
				fileStream.Write(data, 0, data.Length);
			}
		}

		public static byte[] LoadFromFile(string path)
		{
			byte[] result;
			using (FileStream fileStream = new FileStream(path, FileMode.Open))
			{
				byte[] array = new byte[(int)fileStream.Length];
				fileStream.Read(array, 0, (int)fileStream.Length);
				result = array;
			}
			return result;
		}
	}
}
