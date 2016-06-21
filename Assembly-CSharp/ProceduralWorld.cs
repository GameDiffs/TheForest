using Pathfinding;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class ProceduralWorld : MonoBehaviour
{
	[Serializable]
	public class ProceduralPrefab
	{
		public GameObject prefab;

		public float density;

		public float perlin;

		public float perlinPower = 1f;

		public Vector2 perlinOffset = Vector2.zero;

		public float perlinScale = 1f;

		public float random = 1f;

		public bool singleFixed;
	}

	private class ProceduralTile
	{
		private int x;

		private int z;

		private System.Random rnd;

		private bool staticBatching;

		private ProceduralWorld world;

		private Transform root;

		private IEnumerator ie;

		public bool destroyed
		{
			get;
			private set;
		}

		public ProceduralTile(ProceduralWorld world, int x, int z)
		{
			this.x = x;
			this.z = z;
			this.world = world;
			this.rnd = new System.Random(x * 10007 ^ z * 36007);
		}

		[DebuggerHidden]
		public IEnumerator Generate()
		{
			ProceduralWorld.ProceduralTile.<Generate>c__IteratorD <Generate>c__IteratorD = new ProceduralWorld.ProceduralTile.<Generate>c__IteratorD();
			<Generate>c__IteratorD.<>f__this = this;
			return <Generate>c__IteratorD;
		}

		public void ForceFinish()
		{
			while (this.ie != null && this.root != null && this.ie.MoveNext())
			{
			}
			this.ie = null;
		}

		private Vector3 RandomInside()
		{
			return new Vector3
			{
				x = ((float)this.x + (float)this.rnd.NextDouble()) * this.world.tileSize,
				z = ((float)this.z + (float)this.rnd.NextDouble()) * this.world.tileSize
			};
		}

		private Vector3 RandomInside(float px, float pz)
		{
			return new Vector3
			{
				x = (px + (float)this.rnd.NextDouble() / (float)this.world.subTiles) * this.world.tileSize,
				z = (pz + (float)this.rnd.NextDouble() / (float)this.world.subTiles) * this.world.tileSize
			};
		}

		private Quaternion RandomYRot()
		{
			return Quaternion.Euler(360f * (float)this.rnd.NextDouble(), 0f, 360f * (float)this.rnd.NextDouble());
		}

		[DebuggerHidden]
		private IEnumerator InternalGenerate()
		{
			ProceduralWorld.ProceduralTile.<InternalGenerate>c__IteratorE <InternalGenerate>c__IteratorE = new ProceduralWorld.ProceduralTile.<InternalGenerate>c__IteratorE();
			<InternalGenerate>c__IteratorE.<>f__this = this;
			return <InternalGenerate>c__IteratorE;
		}

		public void Destroy()
		{
			if (this.root != null)
			{
				UnityEngine.Debug.Log(string.Concat(new object[]
				{
					"Destroying tile ",
					this.x,
					", ",
					this.z
				}));
				UnityEngine.Object.Destroy(this.root.gameObject);
				this.root = null;
			}
			this.ie = null;
		}
	}

	public Transform target;

	public ProceduralWorld.ProceduralPrefab[] prefabs;

	public int range = 1;

	public float tileSize = 100f;

	public int subTiles = 20;

	public bool staticBatching;

	private Queue<IEnumerator> tileGenerationQueue = new Queue<IEnumerator>();

	private Dictionary<Int2, ProceduralWorld.ProceduralTile> tiles = new Dictionary<Int2, ProceduralWorld.ProceduralTile>();

	private void Start()
	{
		this.Update();
		AstarPath.active.Scan();
		base.StartCoroutine(this.GenerateTiles());
	}

	private void Update()
	{
		Int2 @int = new Int2(Mathf.RoundToInt((this.target.position.x - this.tileSize * 0.5f) / this.tileSize), Mathf.RoundToInt((this.target.position.z - this.tileSize * 0.5f) / this.tileSize));
		this.range = ((this.range >= 1) ? this.range : 1);
		bool flag = true;
		while (flag)
		{
			flag = false;
			foreach (KeyValuePair<Int2, ProceduralWorld.ProceduralTile> current in this.tiles)
			{
				if (Mathf.Abs(current.Key.x - @int.x) > this.range || Mathf.Abs(current.Key.y - @int.y) > this.range)
				{
					current.Value.Destroy();
					this.tiles.Remove(current.Key);
					flag = true;
					break;
				}
			}
		}
		for (int i = @int.x - this.range; i <= @int.x + this.range; i++)
		{
			for (int j = @int.y - this.range; j <= @int.y + this.range; j++)
			{
				if (!this.tiles.ContainsKey(new Int2(i, j)))
				{
					ProceduralWorld.ProceduralTile proceduralTile = new ProceduralWorld.ProceduralTile(this, i, j);
					IEnumerator enumerator2 = proceduralTile.Generate();
					enumerator2.MoveNext();
					this.tileGenerationQueue.Enqueue(enumerator2);
					this.tiles.Add(new Int2(i, j), proceduralTile);
				}
			}
		}
		for (int k = @int.x - 1; k <= @int.x + 1; k++)
		{
			for (int l = @int.y - 1; l <= @int.y + 1; l++)
			{
				this.tiles[new Int2(k, l)].ForceFinish();
			}
		}
	}

	[DebuggerHidden]
	private IEnumerator GenerateTiles()
	{
		ProceduralWorld.<GenerateTiles>c__IteratorC <GenerateTiles>c__IteratorC = new ProceduralWorld.<GenerateTiles>c__IteratorC();
		<GenerateTiles>c__IteratorC.<>f__this = this;
		return <GenerateTiles>c__IteratorC;
	}
}
