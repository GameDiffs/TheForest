using Boo.Lang;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

[Serializable]
public class RuntimeTerrains : MonoBehaviour
{
	[CompilerGenerated]
	[Serializable]
	internal sealed class $GenerateUpdate$748 : GenericGenerator<object>
	{
		internal RuntimeTerrains $self_$750;

		public $GenerateUpdate$748(RuntimeTerrains self_)
		{
			this.$self_$750 = self_;
		}

		public override IEnumerator<object> GetEnumerator()
		{
			return new RuntimeTerrains.$GenerateUpdate$748.$(this.$self_$750);
		}
	}

	[CompilerGenerated]
	[Serializable]
	internal sealed class $GenerateStart$751 : GenericGenerator<object>
	{
		internal RuntimeTerrains $self_$753;

		public $GenerateStart$751(RuntimeTerrains self_)
		{
			this.$self_$753 = self_;
		}

		public override IEnumerator<object> GetEnumerator()
		{
			return new RuntimeTerrains.$GenerateStart$751.$(this.$self_$753);
		}
	}

	private GameObject TerrainComposer;

	private terraincomposer_save tc_script;

	private GameObject TerrainComposerClone;

	private terraincomposer_save tc_script2;

	private float frames;

	private int currentOutput;

	public bool generateOnStart;

	public bool createTerrainsOnTheFly;

	public bool autoSpeed;

	public int targetFrames;

	public int generateSpeed;

	public bool heightmapOutput;

	public bool splatOutput;

	public bool treeOutput;

	public bool grassOutput;

	public bool objectOutput;

	public int seed;

	public bool randomSeed;

	public bool randomizeHeightmapOutput;

	public Vector2 randomizeHeightmapRange;

	public bool randomizeTreeOutput;

	public Vector2 randomizeTreeRange;

	public bool randomizeGrassOutput;

	public Vector2 randomizeGrassRange;

	public bool randomizeObjectOutput;

	public Vector2 randomizeObjectRange;

	private GUIStyle myStyle;

	public RuntimeTerrains()
	{
		this.generateOnStart = true;
		this.autoSpeed = true;
		this.targetFrames = 90;
		this.generateSpeed = 500;
		this.seed = 10;
		this.randomizeHeightmapRange = new Vector2((float)0, (float)1000);
		this.randomizeTreeRange = new Vector2((float)0, (float)1000);
		this.randomizeGrassRange = new Vector2((float)0, (float)1000);
		this.randomizeObjectRange = new Vector2((float)0, (float)1000);
	}

	public override void Start()
	{
		this.TerrainComposer = GameObject.Find("TerrainComposer_Save");
		this.tc_script = (terraincomposer_save)this.TerrainComposer.GetComponent(typeof(terraincomposer_save));
		this.tc_script.heightmap_output = false;
		this.tc_script.splat_output = false;
		this.myStyle = new GUIStyle();
		if (this.generateOnStart)
		{
			this.StartCoroutine_Auto(this.GenerateStart());
		}
	}

	public override IEnumerator GenerateUpdate()
	{
		return new RuntimeTerrains.$GenerateUpdate$748(this).GetEnumerator();
	}

	public override IEnumerator GenerateStart()
	{
		return new RuntimeTerrains.$GenerateStart$751(this).GetEnumerator();
	}

	public override void GenerateStop()
	{
		UnityEngine.Object.Destroy(this.TerrainComposerClone);
	}

	public override bool SelectCloneOutput()
	{
		this.tc_script.disable_outputs();
		if (this.randomSeed)
		{
			this.seed = (int)(Time.realtimeSinceStartup * (float)2000);
		}
		int arg_15A_0;
		if (this.currentOutput == 0)
		{
			this.currentOutput = 1;
			if (this.heightmapOutput)
			{
				this.tc_script.heightmap_output = true;
				this.CreateClone();
				if (this.randomizeHeightmapOutput)
				{
					this.tc_script2.randomize_layer_offset(layer_output_enum.heightmap, this.randomizeHeightmapRange, this.seed);
				}
				arg_15A_0 = 1;
				return arg_15A_0 != 0;
			}
		}
		if (this.currentOutput == 1)
		{
			this.currentOutput = 2;
			if (this.splatOutput)
			{
				this.tc_script.splat_output = true;
				this.CreateClone();
				arg_15A_0 = 1;
				return arg_15A_0 != 0;
			}
		}
		if (this.currentOutput == 2)
		{
			this.currentOutput = 3;
			if (this.treeOutput)
			{
				this.tc_script.tree_output = true;
				this.CreateClone();
				arg_15A_0 = 1;
				return arg_15A_0 != 0;
			}
		}
		if (this.currentOutput == 3)
		{
			this.currentOutput = 4;
			if (this.grassOutput)
			{
				this.tc_script.grass_output = true;
				this.CreateClone();
				arg_15A_0 = 1;
				return arg_15A_0 != 0;
			}
		}
		if (this.currentOutput == 4)
		{
			this.currentOutput = 5;
			if (this.objectOutput)
			{
				this.tc_script.object_output = true;
				this.CreateClone();
				arg_15A_0 = 1;
				return arg_15A_0 != 0;
			}
		}
		arg_15A_0 = 0;
		return arg_15A_0 != 0;
	}

	public override void CreateClone()
	{
		if (this.tc_script2)
		{
			this.GenerateStop();
		}
		this.TerrainComposerClone = UnityEngine.Object.Instantiate<GameObject>(this.TerrainComposer);
		this.TerrainComposerClone.name = "<Generating>";
		this.tc_script2 = (terraincomposer_save)this.TerrainComposerClone.GetComponent(typeof(terraincomposer_save));
		this.tc_script2.script_base = this.tc_script;
		this.tc_script2.auto_speed = this.autoSpeed;
		this.tc_script2.generate_speed = this.generateSpeed;
		this.tc_script2.target_frame = this.targetFrames;
		this.tc_script2.runtime = true;
		this.tc_script2.generate_begin();
	}

	public override void TerrainsFlush()
	{
		for (int i = 0; i < this.tc_script.terrains.Count; i++)
		{
			this.tc_script.terrains[i].terrain.Flush();
		}
		Debug.Log("Flushed!");
	}

	public override void ParentTerrains()
	{
		GameObject gameObject = new GameObject();
		gameObject.name = "_Terrains";
		for (int i = 0; i < this.tc_script.terrains.Count; i++)
		{
			this.tc_script.terrains[i].terrain.transform.parent = gameObject.transform;
		}
	}

	public override void Main()
	{
	}
}
