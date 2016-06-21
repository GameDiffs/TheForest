using System;
using System.Collections.Generic;
using UniLinq;
using UnityEngine;

public class Octree
{
	private struct Collidable
	{
		public int InstanceId;

		public Transform Transform;
	}

	private class Node
	{
		public readonly Octree Tree;

		public readonly Octree.Node Parent;

		public readonly Vector3 Center;

		public readonly Vector3 Extents;

		public readonly int Level;

		public Octree.Node[] Children;

		public Dictionary<int, Transform> Transforms;

		public Node(Octree tree, Octree.Node parent, Vector3 center, Vector3 extents, int level)
		{
			this.Tree = tree;
			this.Level = level;
			this.Parent = parent;
			this.Center = center;
			this.Extents = extents;
			this.Transforms = new Dictionary<int, Transform>();
		}
	}

	private const int MAX_DEPTH = 7;

	private const int MAX_OBJECTS = 8;

	private const int XN_YN_ZN = 0;

	private const int XP_YN_ZN = 1;

	private const int XN_YP_ZN = 2;

	private const int XP_YP_ZN = 3;

	private const int XN_YN_ZP = 4;

	private const int XP_YN_ZP = 5;

	private const int XN_YP_ZP = 6;

	private const int XP_YP_ZP = 7;

	private Octree.Node root;

	private Dictionary<int, Octree.Node> nodes;

	private static Octree _instance;

	private List<int> clearList = new List<int>();

	public static Octree Instance
	{
		get
		{
			if (Octree._instance == null)
			{
				Octree._instance = new Octree(Vector3.zero, new Vector3(4096f, 4096f, 4096f));
			}
			return Octree._instance;
		}
	}

	public Octree(Vector3 center, Vector3 size)
	{
		this.root = new Octree.Node(this, null, center, size * 0.5f, 1);
		this.Divide(this.root);
		this.nodes = new Dictionary<int, Octree.Node>();
	}

	public void Update(Transform t)
	{
		int instanceID = t.GetInstanceID();
		Octree.Node node = null;
		if (this.nodes.TryGetValue(instanceID, out node))
		{
			node.Transforms.Remove(instanceID);
			this.nodes.Remove(instanceID);
		}
		this.Insert(this.root, t, instanceID, t.position);
	}

	public void Purge()
	{
		this.Clear(this.root);
		this.Purge(this.root);
	}

	private void Clear(Octree.Node n)
	{
		if (n == null)
		{
			return;
		}
		foreach (KeyValuePair<int, Transform> current in n.Transforms)
		{
			if (!current.Value)
			{
				this.clearList.Add(current.Key);
			}
		}
		for (int i = 0; i < this.clearList.Count; i++)
		{
			n.Transforms.Remove(this.clearList[i]);
		}
		this.clearList.Clear();
		if (n.Children != null)
		{
			for (int j = 0; j < n.Children.Length; j++)
			{
				this.Clear(n.Children[j]);
			}
		}
	}

	private void Purge(Octree.Node n)
	{
		if (n == null)
		{
			return;
		}
		if (n.Children != null)
		{
			int num = 0;
			int num2 = 0;
			for (int i = 0; i < n.Children.Length; i++)
			{
				this.Purge(n.Children[i]);
				if (n.Children[i].Children == null)
				{
					num++;
					num2 += n.Children[i].Transforms.Count;
				}
			}
			if (n.Level > 2 && num2 <= 8 && num == 8)
			{
				IEnumerable<Transform> enumerable = n.Children.SelectMany((Octree.Node x) => x.Transforms.Values);
				n.Children = null;
				foreach (Transform current in enumerable)
				{
					this.Insert(n, current, current.GetInstanceID(), current.position);
				}
			}
		}
	}

	private void Insert(Octree.Node n, Transform t, int id, Vector3 p)
	{
		if (n.Children == null)
		{
			if (n.Transforms.Count < 8 || n.Level == 7)
			{
				this.nodes[id] = n;
				n.Transforms[id] = t;
				return;
			}
			this.Divide(n);
			foreach (Transform current in n.Transforms.Values)
			{
				this.Insert(n, current, current.GetInstanceID(), current.position);
			}
			n.Transforms.Clear();
		}
		int num = 0;
		if (p.x - n.Center.x >= 0f)
		{
			num |= 1;
		}
		if (p.y - n.Center.y >= 0f)
		{
			num |= 2;
		}
		if (p.z - n.Center.z >= 0f)
		{
			num |= 4;
		}
		this.Insert(n.Children[num], t, id, p);
	}

	private void Divide(Octree.Node n)
	{
		float num = n.Extents.x * 0.5f;
		float num2 = n.Extents.y * 0.5f;
		float num3 = n.Extents.z * 0.5f;
		int level = n.Level + 1;
		Vector3 extents = new Vector3(num, num2, num3);
		float x = n.Center.x - num;
		float x2 = n.Center.x + num;
		float y = n.Center.y - num2;
		float y2 = n.Center.y + num2;
		float z = n.Center.z - num3;
		float z2 = n.Center.z + num3;
		n.Children = new Octree.Node[8];
		n.Children[0] = new Octree.Node(this, n, new Vector3(x, y, z), extents, level);
		n.Children[4] = new Octree.Node(this, n, new Vector3(x, y, z2), extents, level);
		n.Children[2] = new Octree.Node(this, n, new Vector3(x, y2, z), extents, level);
		n.Children[6] = new Octree.Node(this, n, new Vector3(x, y2, z2), extents, level);
		n.Children[1] = new Octree.Node(this, n, new Vector3(x2, y, z), extents, level);
		n.Children[5] = new Octree.Node(this, n, new Vector3(x2, y, z2), extents, level);
		n.Children[3] = new Octree.Node(this, n, new Vector3(x2, y2, z), extents, level);
		n.Children[7] = new Octree.Node(this, n, new Vector3(x2, y2, z2), extents, level);
	}

	public void Draw()
	{
		this.Draw(this.root, true);
		this.Draw(this.root, false);
	}

	private void Draw(Octree.Node n, bool empty)
	{
	}
}
