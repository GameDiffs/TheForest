using System;
using System.Collections.Generic;
using UnityEngine;

public static class CoopTreeGrid
{
	private struct Node
	{
		public int NewHasPlayer;

		public int OldHasPlayer;

		public List<BoltEntity> Trees;

		public List<CoopGridObject> Objects;
	}

	public const int WORLD_EXTENTS = 2048;

	public const int WORLD_SIZE = 4096;

	public const int NODE_SIZE = 64;

	public const int NODE_COUNT = 64;

	public const int NODES_PER_FRAME = 512;

	private static int r_debug = -2147483648;

	private static int c_debug = -2147483648;

	private static CoopTreeGrid.Node[] Nodes;

	private static Queue<BoltEntity> AttachQueue = new Queue<BoltEntity>();

	public static void DrawGrid()
	{
		if (CoopTreeGrid.Nodes != null)
		{
			int num = -2048;
			int num2 = 2048;
			if (CoopTreeGrid.r_debug != -2147483648 && CoopTreeGrid.c_debug != -2147483648)
			{
				Debug.DrawLine(new Vector3((float)(num + CoopTreeGrid.c_debug * 64), 0f, (float)num), new Vector3((float)(num + CoopTreeGrid.c_debug * 64), 0f, (float)num2), Color.magenta);
				Debug.DrawLine(new Vector3((float)num, 0f, (float)(num + CoopTreeGrid.r_debug * 64)), new Vector3((float)num2, 0f, (float)(num + CoopTreeGrid.r_debug * 64)), Color.magenta);
			}
			int num3 = CoopTreeGrid.r_debug * 64 + CoopTreeGrid.c_debug;
			if (CoopTreeGrid.Nodes[num3].Trees != null)
			{
				Vector3 start = new Vector3((float)(num + CoopTreeGrid.c_debug * 64 + 32), 0f, (float)(num + CoopTreeGrid.r_debug * 64 + 32));
				for (int i = 0; i < CoopTreeGrid.Nodes[num3].Trees.Count; i++)
				{
					Debug.DrawLine(start, CoopTreeGrid.Nodes[num3].Trees[i].transform.position, Color.red);
				}
			}
		}
	}

	public static void Clear()
	{
		CoopTreeGrid.Nodes = null;
		CoopTreeGrid.AttachQueue = null;
	}

	internal static void RegisterObject(CoopGridObject obj)
	{
		CoopTreeGrid.RegisterObject(obj, false);
	}

	internal static void RegisterObject(CoopGridObject obj, bool is_update)
	{
		int num = CoopTreeGrid.CalculateNode(obj.transform.position);
		if (CoopTreeGrid.Nodes[num].Objects == null)
		{
			CoopTreeGrid.Nodes[num].Objects = new List<CoopGridObject>();
		}
		CoopTreeGrid.Nodes[num].Objects.Add(obj);
		obj.CurrentNode = num;
		if (!is_update)
		{
			obj.entity.Freeze(false);
		}
	}

	internal static void RemoveObject(CoopGridObject obj)
	{
		if (obj.CurrentNode >= 0)
		{
			CoopTreeGrid.Nodes[obj.CurrentNode].Objects.Remove(obj);
		}
	}

	internal static void UpdateObject(CoopGridObject obj)
	{
		int num = CoopTreeGrid.CalculateNode(obj.transform.position);
		if (num != obj.CurrentNode)
		{
			CoopTreeGrid.Nodes[obj.CurrentNode].Objects.Remove(obj);
			if (CoopTreeGrid.Nodes[num].Objects == null)
			{
				CoopTreeGrid.Nodes[num].Objects = new List<CoopGridObject>();
			}
			CoopTreeGrid.Nodes[num].Objects.Add(obj);
			obj.CurrentNode = num;
		}
	}

	internal static int CalculateNode(Vector3 position)
	{
		Vector3 vector = position;
		int num = Mathf.Clamp(2048 + (int)vector.x, 0, 4096);
		int num2 = Mathf.Clamp(2048 + (int)vector.z, 0, 4096);
		int num3 = Mathf.Clamp(num / 64, 0, 63);
		int num4 = Mathf.Clamp(num2 / 64, 0, 63);
		return num4 * 64 + num3;
	}

	internal static void Update(IEnumerable<BoltEntity> trees)
	{
		CoopTreeGrid.Nodes = new CoopTreeGrid.Node[4096];
		CoopTreeGrid.AttachQueue = new Queue<BoltEntity>();
		foreach (BoltEntity current in trees)
		{
			int num = CoopTreeGrid.CalculateNode(current.transform.position);
			if (CoopTreeGrid.Nodes[num].Trees == null)
			{
				CoopTreeGrid.Nodes[num].Trees = new List<BoltEntity>();
			}
			CoopTreeGrid.Nodes[num].Trees.Add(current);
		}
		CoopTreeGrid.Node[] nodes = CoopTreeGrid.Nodes;
		for (int i = 0; i < nodes.Length; i++)
		{
			CoopTreeGrid.Node node = nodes[i];
			if (node.Trees != null)
			{
				foreach (BoltEntity current2 in node.Trees)
				{
					CoopTreeGrid.AttachQueue.Enqueue(current2);
				}
			}
		}
	}

	internal static void AttachTrees()
	{
		if (BoltNetwork.isServer && CoopTreeGrid.AttachQueue != null)
		{
			int count = CoopTreeGrid.AttachQueue.Count;
			for (int i = 0; i < 10; i++)
			{
				if (CoopTreeGrid.AttachQueue.Count <= 0)
				{
					break;
				}
				BoltEntity boltEntity = CoopTreeGrid.AttachQueue.Dequeue();
				if (!boltEntity.IsAttached())
				{
					BoltNetwork.Attach(boltEntity.gameObject);
					boltEntity.Freeze(true);
				}
				else
				{
					i--;
				}
			}
			if (count > 0 && CoopTreeGrid.AttachQueue.Count == 0)
			{
				BoltEntity[] array = UnityEngine.Object.FindObjectsOfType<BoltEntity>();
				for (int j = 0; j < array.Length; j++)
				{
					BoltEntity entity = array[j];
					if (!entity.IsAttached())
					{
						BoltNetwork.Attach(entity);
					}
				}
			}
		}
	}

	internal static void AttachAdjacent(List<GameObject> positions)
	{
		if (CoopTreeGrid.Nodes == null)
		{
			return;
		}
		for (int i = 0; i < CoopTreeGrid.Nodes.Length; i++)
		{
			CoopTreeGrid.Nodes[i].NewHasPlayer = 0;
		}
		for (int j = 0; j < positions.Count; j++)
		{
			Vector3 position = positions[j].transform.position;
			int num = Mathf.Clamp(2048 + (int)position.x, 0, 4096);
			int num2 = Mathf.Clamp(2048 + (int)position.z, 0, 4096);
			int num3 = Mathf.Clamp(num / 64, 0, 63);
			int num4 = Mathf.Clamp(num2 / 64, 0, 63);
			for (int k = -2; k < 3; k++)
			{
				for (int l = -2; l < 3; l++)
				{
					int num5 = num4 + k;
					int num6 = num3 + l;
					if (num5 >= 0 && num5 < 64 && num6 >= 0 && num6 < 64)
					{
						CoopTreeGrid.Node[] expr_F3_cp_0 = CoopTreeGrid.Nodes;
						int expr_F3_cp_1 = num5 * 64 + num6;
						expr_F3_cp_0[expr_F3_cp_1].NewHasPlayer = expr_F3_cp_0[expr_F3_cp_1].NewHasPlayer + 1;
					}
				}
			}
		}
		for (int m = 0; m < CoopTreeGrid.Nodes.Length; m++)
		{
			CoopTreeGrid.Node node = CoopTreeGrid.Nodes[m];
			if (node.NewHasPlayer > node.OldHasPlayer || (node.NewHasPlayer == 0 && node.NewHasPlayer != node.OldHasPlayer))
			{
				if (node.Trees != null)
				{
					for (int n = 0; n < node.Trees.Count; n++)
					{
						BoltEntity boltEntity = node.Trees[n];
						if (boltEntity)
						{
							if (boltEntity.isAttached)
							{
								boltEntity.Freeze(node.NewHasPlayer == 0);
							}
						}
						else
						{
							node.Trees.RemoveAt(n);
							n--;
						}
					}
				}
				if (node.Objects != null)
				{
					for (int num7 = 0; num7 < node.Objects.Count; num7++)
					{
						CoopGridObject coopGridObject = node.Objects[num7];
						if (coopGridObject)
						{
							if (coopGridObject.entity && coopGridObject.entity.isAttached)
							{
								coopGridObject.entity.Freeze(node.NewHasPlayer == 0);
							}
						}
						else
						{
							node.Objects.RemoveAt(num7);
							num7--;
						}
					}
				}
			}
			CoopTreeGrid.Nodes[m].OldHasPlayer = CoopTreeGrid.Nodes[m].NewHasPlayer;
		}
	}
}
