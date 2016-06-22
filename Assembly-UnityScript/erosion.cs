using System;
using UnityEngine;

[Serializable]
public class erosion : MonoBehaviour
{
	public Vector3 scale;

	public float health;

	public float mass;

	public Terrain terrain;

	public Vector3 terrain_size;

	public Vector3 terrain_position;

	public Vector3 rotation1;

	public Quaternion rotation;

	public Vector3 speed_world;

	public Vector3 speed_local;

	public float air_density;

	public float deltaTime;

	public float air_resistance_front;

	public float air_resistance_back;

	public float air_resistance_side;

	public float air_resistance_top;

	public float height;

	public terrain_class preterrain;

	public int heightmap_y;

	public int heightmap_x;

	public Transform myTransform;

	public bool out_of_range;

	public int count_wide;

	public int count_length;

	public Vector3 position;

	public float height_old;

	public bool height_set;

	public Vector3 position_start;

	public Vector3 scale_start;

	public float multiplier;

	public int type;

	public float point1;

	public Vector3 point2;

	public float point3;

	public float heading_old;

	public bool speed_x;

	public float loop_time;

	public float x_def;

	public float shave;

	public float release;

	public float eroded;

	public float erose;

	public erosion()
	{
		this.air_density = (float)1;
		this.deltaTime = 0.01f;
		this.multiplier = (float)1;
	}

	public override void Start()
	{
		this.scale = this.transform.localScale;
		this.scale_start = this.scale;
		this.terrain_size = this.terrain.terrainData.size;
		this.terrain_position = this.terrain.transform.position;
		this.myTransform = this.transform;
		this.position_start = this.myTransform.position;
		this.type = UnityEngine.Random.Range(0, 10);
		this.air_resistance_front = (float)0;
		this.air_resistance_back = 0.2f;
		this.air_resistance_side = (float)5;
		this.air_resistance_top = 0.001f;
		this.health = (float)UnityEngine.Random.Range(10, 200);
		this.mass = this.health;
		this.speed_local.z = (float)UnityEngine.Random.Range(0, 1);
		this.speed_world = this.myTransform.TransformDirection(this.speed_local);
		this.deltaTime = 0.01f;
		this.x_def = (float)UnityEngine.Random.Range(-10, 10);
	}

	public override void erosion()
	{
		if (this.type < 5)
		{
			this.erosion1();
		}
		else if (this.type < 10)
		{
			this.erosion1();
		}
	}

	public override Vector3 random_position(Vector3 pos1)
	{
		pos1.x = UnityEngine.Random.Range(pos1.x - (float)50, pos1.x + (float)50);
		pos1.y = UnityEngine.Random.Range(pos1.y - (float)50, pos1.y + (float)50);
		pos1.z = UnityEngine.Random.Range(pos1.z - (float)50, pos1.z + (float)50);
		this.x_def = (float)UnityEngine.Random.Range(-7, 7);
		this.eroded = (float)0;
		this.loop_time = (float)0;
		return pos1;
	}

	public override void erosion1()
	{
		if (!this.out_of_range)
		{
			this.rotation1 = this.terrain.terrainData.GetInterpolatedNormal((this.myTransform.position.x - this.terrain_position.x) / this.terrain_size.x, (this.myTransform.position.z - this.terrain_position.z) / this.terrain_size.z);
			this.rotation1.x = this.rotation1.x / (float)3 * (float)2;
			this.rotation1.z = this.rotation1.z / (float)3 * (float)2;
			this.rotation = Quaternion.LookRotation(this.rotation1);
			float x = this.rotation.eulerAngles.x + (float)90;
			Vector3 eulerAngles = this.rotation.eulerAngles;
			float num = eulerAngles.x = x;
			Vector3 vector = this.rotation.eulerAngles = eulerAngles;
			this.rotation1 = this.rotation.eulerAngles;
			if (Mathf.Abs(Mathf.DeltaAngle(this.rotation1.y, this.heading_old)) > (float)90 && this.height_set)
			{
				this.health = (float)-1;
			}
			this.shave = this.rotation1.x;
			if (this.shave < (float)45)
			{
				this.release = (float)45 - this.shave;
			}
			else
			{
				this.release = (float)0;
			}
			if (this.shave > (float)45)
			{
				this.shave = (float)90 - this.shave;
			}
			this.myTransform.localEulerAngles = this.rotation1;
			this.heading_old = this.rotation1.y;
		}
		this.multiplier = (float)1;
		if (!this.speed_x)
		{
			this.speed_local.z = this.preterrain.heightmap_conversion.x;
		}
		else
		{
			this.speed_local.x = -this.preterrain.heightmap_conversion.x;
		}
		this.scale.x = (float)1500 / (this.height + (float)40) + this.scale_start.x;
		if (this.health < (float)0 || this.out_of_range || this.speed_local.z < (float)0)
		{
			this.health = (float)UnityEngine.Random.Range(10, 200);
			this.myTransform.position = this.random_position(this.position_start);
			this.height_set = false;
			this.speed_local = new Vector3((float)0, (float)0, (float)0);
			this.out_of_range = false;
		}
		else
		{
			this.position = this.myTransform.position;
			this.speed_local.x = this.release / (float)45 * this.x_def;
			this.loop_time += this.release / (float)45;
			this.heightmap_x = (int)Mathf.Round((this.position.x - this.terrain_position.x) / this.preterrain.heightmap_conversion.x);
			this.heightmap_y = (int)Mathf.Round((this.position.z - this.terrain_position.z) / this.preterrain.heightmap_conversion.y);
			this.out_of_range = false;
			if ((float)this.heightmap_y > this.preterrain.heightmap_resolution - (float)1)
			{
				this.out_of_range = true;
			}
			if ((float)this.heightmap_x > this.preterrain.heightmap_resolution - (float)1)
			{
				this.out_of_range = true;
			}
			if (this.heightmap_x < 0)
			{
				this.out_of_range = true;
			}
			if (this.heightmap_y < 0)
			{
				this.out_of_range = true;
			}
			if (!this.out_of_range)
			{
				if (this.height_set)
				{
					this.erose = this.scale.y * (this.shave / (float)5) / (float)10 * this.multiplier;
					if (this.eroded > this.scale.y * (this.release / (float)25) / (float)10 * this.multiplier)
					{
						this.erose -= this.scale.y * (this.release / (float)25) / (float)10 * this.multiplier;
					}
					this.eroded += this.erose;
				}
				this.height_set = true;
				if ((float)this.heightmap_y > this.preterrain.heightmap_resolution - (float)2)
				{
					this.heightmap_y = (int)(this.preterrain.heightmap_resolution - (float)2);
				}
				if ((float)this.heightmap_x > this.preterrain.heightmap_resolution - (float)2)
				{
					this.heightmap_x = (int)(this.preterrain.heightmap_resolution - (float)2);
				}
				if (this.heightmap_x < 0)
				{
					this.heightmap_x = 0;
				}
				if (this.heightmap_y < 0)
				{
					this.heightmap_y = 0;
				}
				if (this.preterrain.map[this.heightmap_y, this.heightmap_x, 2] < (float)1)
				{
					this.preterrain.map[this.heightmap_y, this.heightmap_x, 2] = this.preterrain.map[this.heightmap_y, this.heightmap_x, 2] + this.scale.y * this.speed_local.z * (this.height / (float)500) / (float)800;
					this.preterrain.map[this.heightmap_y, this.heightmap_x, 0] = this.preterrain.map[this.heightmap_y, this.heightmap_x, 0] - this.scale.y * this.speed_local.z * (this.height / (float)500) / (float)800 / (float)4;
					this.preterrain.map[this.heightmap_y, this.heightmap_x, 1] = this.preterrain.map[this.heightmap_y, this.heightmap_x, 1] - this.scale.y * this.speed_local.z * (this.height / (float)500) / (float)800 / (float)4;
					this.preterrain.map[this.heightmap_y, this.heightmap_x, 3] = this.preterrain.map[this.heightmap_y, this.heightmap_x, 3] - this.scale.y * this.speed_local.z * (this.height / (float)500) / (float)800 / (float)4;
					this.preterrain.map[this.heightmap_y, this.heightmap_x, 4] = this.preterrain.map[this.heightmap_y, this.heightmap_x, 4] - this.scale.y * this.speed_local.z * (this.height / (float)500) / (float)800 / (float)4;
				}
			}
			this.myTransform.Translate(this.speed_local.x, this.speed_local.y, this.speed_local.z);
			this.height = this.terrain.SampleHeight(this.myTransform.position);
			float y = this.height;
			Vector3 vector2 = this.myTransform.position;
			float num2 = vector2.y = y;
			Vector3 vector3 = this.myTransform.position = vector2;
		}
	}

	public override void erosion2()
	{
		if (!this.out_of_range)
		{
			this.rotation1 = this.terrain.terrainData.GetInterpolatedNormal((this.myTransform.position.x - this.terrain_position.x) / this.terrain_size.x, (this.myTransform.position.z - this.terrain_position.z) / this.terrain_size.z);
			this.height = this.terrain.SampleHeight(this.myTransform.position);
			this.rotation1.x = this.rotation1.x / (float)3 * (float)2;
			this.rotation1.z = this.rotation1.z / (float)3 * (float)2;
			this.rotation = Quaternion.LookRotation(this.rotation1);
			float x = this.rotation.eulerAngles.x + (float)90;
			Vector3 eulerAngles = this.rotation.eulerAngles;
			float num = eulerAngles.x = x;
			Vector3 vector = this.rotation.eulerAngles = eulerAngles;
			float y = this.rotation.eulerAngles.y + (float)90;
			Vector3 eulerAngles2 = this.rotation.eulerAngles;
			float num2 = eulerAngles2.y = y;
			Vector3 vector2 = this.rotation.eulerAngles = eulerAngles2;
			this.rotation = Quaternion.RotateTowards(this.myTransform.rotation, this.rotation, (float)3);
			this.rotation1 = this.rotation.eulerAngles;
		}
		this.speed_local = this.myTransform.InverseTransformDirection(this.speed_world);
		if (this.speed_local.z < (float)0)
		{
			this.multiplier = this.speed_local.z * (float)-1;
		}
		else
		{
			this.speed_local.z = this.speed_local.z + (this.rotation1.x + (float)30) * this.deltaTime * 0.03f * this.scale_start.z;
			this.multiplier = (float)1;
		}
		this.scale.x = (float)1500 / (this.height + (float)40) + this.scale_start.x;
		this.scale.z = this.speed_local.z / (float)2;
		float x2 = this.scale.x;
		Vector3 localScale = this.myTransform.localScale;
		float num3 = localScale.x = x2;
		Vector3 vector3 = this.myTransform.localScale = localScale;
		float z = this.scale.z;
		Vector3 localScale2 = this.myTransform.localScale;
		float num4 = localScale2.z = z;
		Vector3 vector4 = this.myTransform.localScale = localScale2;
		if (this.speed_local.z > (float)0)
		{
			this.speed_local.z = this.speed_local.z - this.deltaTime * this.air_resistance_front * this.air_density * (this.speed_local.z * this.speed_local.z);
			if (this.speed_local.z < (float)0)
			{
				this.speed_local.z = (float)0;
			}
		}
		if (this.speed_local.z < (float)0)
		{
			this.speed_local.z = this.speed_local.z + this.deltaTime * this.air_resistance_back * this.air_density * (this.speed_local.z * this.speed_local.z);
			if (this.speed_local.z > (float)0)
			{
				this.speed_local.z = (float)0;
			}
		}
		if (this.speed_local.x > (float)0)
		{
			this.speed_local.x = this.speed_local.x - this.deltaTime * this.air_resistance_side * this.air_density * (this.speed_local.x * this.speed_local.x);
			if (this.speed_local.x < (float)0)
			{
				this.speed_local.x = (float)0;
			}
		}
		if (this.speed_local.x < (float)0)
		{
			this.speed_local.x = this.speed_local.x + this.deltaTime * this.air_resistance_side * this.air_density * (this.speed_local.x * this.speed_local.x);
			if (this.speed_local.x > (float)0)
			{
				this.speed_local.x = (float)0;
			}
		}
		if (this.speed_local.y > (float)0)
		{
			this.speed_local.y = this.speed_local.y - this.deltaTime * this.air_resistance_top * this.air_density * (this.speed_local.y * this.speed_local.y);
			if (this.speed_local.y < (float)0)
			{
				this.speed_local.y = (float)0;
			}
		}
		if (this.speed_local.y < (float)0)
		{
			this.speed_local.y = this.speed_local.y + this.deltaTime * this.air_resistance_top * this.air_density * (this.speed_local.y * this.speed_local.y);
			if (this.speed_local.y > (float)0)
			{
				this.speed_local.y = (float)0;
			}
		}
		this.health -= (float)1;
		if (this.health < (float)0 || this.out_of_range || this.speed_local.z < (float)0)
		{
			this.health = (float)UnityEngine.Random.Range(15, 555);
			this.myTransform.position = this.position_start;
			this.height_set = false;
			this.speed_local.z = (float)0;
		}
		this.position = this.myTransform.position;
		this.heightmap_x = (int)Mathf.Round((this.position.x - this.terrain_position.x) / this.preterrain.heightmap_conversion.x);
		this.heightmap_y = (int)Mathf.Round((this.position.z - this.terrain_position.z) / this.preterrain.heightmap_conversion.y);
		this.out_of_range = false;
		if ((float)this.heightmap_y > this.preterrain.heightmap_resolution - (float)1)
		{
			this.out_of_range = true;
		}
		if ((float)this.heightmap_x > this.preterrain.heightmap_resolution - (float)1)
		{
			this.out_of_range = true;
		}
		if (this.heightmap_x < 0)
		{
			this.out_of_range = true;
		}
		if (this.heightmap_y < 0)
		{
			this.out_of_range = true;
		}
		if (!this.out_of_range)
		{
			if (this.height_set)
			{
			}
			this.height_set = true;
			if ((float)this.heightmap_y > this.preterrain.heightmap_resolution - (float)2)
			{
				this.heightmap_y = (int)(this.preterrain.heightmap_resolution - (float)2);
			}
			if ((float)this.heightmap_x > this.preterrain.heightmap_resolution - (float)2)
			{
				this.heightmap_x = (int)(this.preterrain.heightmap_resolution - (float)2);
			}
			if (this.heightmap_x < 0)
			{
				this.heightmap_x = 0;
			}
			if (this.heightmap_y < 0)
			{
				this.heightmap_y = 0;
			}
			if (this.preterrain.map[this.heightmap_y, this.heightmap_x, 2] < (float)1)
			{
			}
		}
		this.speed_world = this.myTransform.TransformDirection(this.speed_local);
		this.myTransform.rotation = this.rotation;
		this.myTransform.Translate(this.speed_world.x, this.speed_world.y, this.speed_world.z, Space.World);
		this.height = this.terrain.SampleHeight(this.myTransform.position);
		if (this.myTransform.position.y < this.height)
		{
			float y2 = this.height;
			Vector3 vector5 = this.myTransform.position;
			float num5 = vector5.y = y2;
			Vector3 vector6 = this.myTransform.position = vector5;
		}
	}

	public override void erosion5()
	{
		if (!this.out_of_range)
		{
			this.rotation1 = this.terrain.terrainData.GetInterpolatedNormal((this.myTransform.position.x - this.terrain_position.x) / this.terrain_size.x, (this.myTransform.position.z - this.terrain_position.z) / this.terrain_size.z);
			this.height = this.terrain.SampleHeight(this.myTransform.position);
			this.rotation1.x = this.rotation1.x / (float)3 * (float)2;
			this.rotation1.z = this.rotation1.z / (float)3 * (float)2;
			this.rotation = Quaternion.LookRotation(this.rotation1);
			float x = this.rotation.eulerAngles.x + (float)90;
			Vector3 eulerAngles = this.rotation.eulerAngles;
			float num = eulerAngles.x = x;
			Vector3 vector = this.rotation.eulerAngles = eulerAngles;
			float y = this.rotation.eulerAngles.y + (float)180;
			Vector3 eulerAngles2 = this.rotation.eulerAngles;
			float num2 = eulerAngles2.y = y;
			Vector3 vector2 = this.rotation.eulerAngles = eulerAngles2;
			this.rotation1 = this.rotation.eulerAngles;
		}
		this.speed_local = this.myTransform.InverseTransformDirection(this.speed_world);
		this.speed_local.z = this.preterrain.heightmap_conversion.x;
		this.multiplier = (float)1;
		this.scale.x = (float)1500 / (this.height + (float)40) + this.scale_start.x;
		if (this.speed_local.z > (float)0)
		{
			this.speed_local.z = this.speed_local.z - this.deltaTime * this.air_resistance_front * this.air_density * (this.speed_local.z * this.speed_local.z);
			if (this.speed_local.z < (float)0)
			{
				this.speed_local.z = (float)0;
			}
		}
		if (this.speed_local.z < (float)0)
		{
			this.speed_local.z = this.speed_local.z + this.deltaTime * this.air_resistance_back * this.air_density * (this.speed_local.z * this.speed_local.z);
			if (this.speed_local.z > (float)0)
			{
				this.speed_local.z = (float)0;
			}
		}
		if (this.speed_local.x > (float)0)
		{
			this.speed_local.x = this.speed_local.x - this.deltaTime * this.air_resistance_side * this.air_density * (this.speed_local.x * this.speed_local.x);
			if (this.speed_local.x < (float)0)
			{
				this.speed_local.x = (float)0;
			}
		}
		if (this.speed_local.x < (float)0)
		{
			this.speed_local.x = this.speed_local.x + this.deltaTime * this.air_resistance_side * this.air_density * (this.speed_local.x * this.speed_local.x);
			if (this.speed_local.x > (float)0)
			{
				this.speed_local.x = (float)0;
			}
		}
		if (this.speed_local.y > (float)0)
		{
			this.speed_local.y = this.speed_local.y - this.deltaTime * this.air_resistance_top * this.air_density * (this.speed_local.y * this.speed_local.y);
			if (this.speed_local.y < (float)0)
			{
				this.speed_local.y = (float)0;
			}
		}
		if (this.speed_local.y < (float)0)
		{
			this.speed_local.y = this.speed_local.y + this.deltaTime * this.air_resistance_top * this.air_density * (this.speed_local.y * this.speed_local.y);
			if (this.speed_local.y > (float)0)
			{
				this.speed_local.y = (float)0;
			}
		}
		this.health -= (float)1;
		if (this.health < (float)0 || this.out_of_range || this.speed_local.z < (float)0)
		{
			this.health = (float)UnityEngine.Random.Range(15, 555);
			this.myTransform.position = this.position_start;
			this.height_set = false;
		}
		this.position = this.myTransform.position;
		this.heightmap_x = (int)Mathf.Round((this.position.x - this.terrain_position.x) / this.preterrain.heightmap_conversion.x);
		this.heightmap_y = (int)Mathf.Round((this.position.z - this.terrain_position.z) / this.preterrain.heightmap_conversion.y);
		this.out_of_range = false;
		if ((float)this.heightmap_y > this.preterrain.heightmap_resolution - (float)1)
		{
			this.out_of_range = true;
		}
		if ((float)this.heightmap_x > this.preterrain.heightmap_resolution - (float)1)
		{
			this.out_of_range = true;
		}
		if (this.heightmap_x < 0)
		{
			this.out_of_range = true;
		}
		if (this.heightmap_y < 0)
		{
			this.out_of_range = true;
		}
		if (!this.out_of_range)
		{
			if (this.height_set)
			{
			}
			this.height_set = true;
			if ((float)this.heightmap_y > this.preterrain.heightmap_resolution - (float)2)
			{
				this.heightmap_y = (int)(this.preterrain.heightmap_resolution - (float)2);
			}
			if ((float)this.heightmap_x > this.preterrain.heightmap_resolution - (float)2)
			{
				this.heightmap_x = (int)(this.preterrain.heightmap_resolution - (float)2);
			}
			if (this.heightmap_x < 0)
			{
				this.heightmap_x = 0;
			}
			if (this.heightmap_y < 0)
			{
				this.heightmap_y = 0;
			}
			if (this.preterrain.map[this.heightmap_y, this.heightmap_x, 2] < (float)1)
			{
			}
		}
		this.speed_world = this.myTransform.TransformDirection(this.speed_local);
		this.myTransform.rotation = this.rotation;
		this.myTransform.Translate(this.speed_world.x, this.speed_world.y, this.speed_world.z, Space.World);
		if (this.myTransform.position.y < this.height)
		{
			float y2 = this.height;
			Vector3 vector3 = this.myTransform.position;
			float num3 = vector3.y = y2;
			Vector3 vector4 = this.myTransform.position = vector3;
		}
	}

	public override void erosion3()
	{
		if (!this.out_of_range)
		{
			this.rotation1 = this.terrain.terrainData.GetInterpolatedNormal((this.myTransform.position.x - this.terrain_position.x) / this.terrain_size.x, (this.myTransform.position.z - this.terrain_position.z) / this.terrain_size.z);
			this.rotation1.x = this.rotation1.x / (float)3 * (float)2;
			this.rotation1.z = this.rotation1.z / (float)3 * (float)2;
			this.rotation = Quaternion.LookRotation(this.rotation1);
			float x = this.rotation.eulerAngles.x + (float)90;
			Vector3 eulerAngles = this.rotation.eulerAngles;
			float num = eulerAngles.x = x;
			Vector3 vector = this.rotation.eulerAngles = eulerAngles;
			this.rotation = Quaternion.RotateTowards(this.myTransform.rotation, this.rotation, (float)3);
			this.rotation1 = this.rotation.eulerAngles;
		}
		this.speed_local = this.myTransform.InverseTransformDirection(this.speed_world);
		this.speed_local.z = this.speed_local.z + (this.rotation1.x + (float)30) * this.deltaTime * 0.03f * this.scale_start.z;
		this.multiplier = (float)1;
		this.scale.x = (float)1500 / (this.height + (float)40) + this.scale_start.x;
		if (this.speed_local.z > (float)0)
		{
			this.speed_local.z = this.speed_local.z - this.deltaTime * this.air_resistance_front * this.air_density * (this.speed_local.z * this.speed_local.z);
			if (this.speed_local.z < (float)0)
			{
				this.speed_local.z = (float)0;
			}
		}
		if (this.speed_local.z < (float)0)
		{
			this.speed_local.z = this.speed_local.z + this.deltaTime * this.air_resistance_back * this.air_density * (this.speed_local.z * this.speed_local.z);
			if (this.speed_local.z > (float)0)
			{
				this.speed_local.z = (float)0;
			}
		}
		if (this.speed_local.x > (float)0)
		{
			this.speed_local.x = this.speed_local.x - this.deltaTime * this.air_resistance_side * this.air_density * (this.speed_local.x * this.speed_local.x);
			if (this.speed_local.x < (float)0)
			{
				this.speed_local.x = (float)0;
			}
		}
		if (this.speed_local.x < (float)0)
		{
			this.speed_local.x = this.speed_local.x + this.deltaTime * this.air_resistance_side * this.air_density * (this.speed_local.x * this.speed_local.x);
			if (this.speed_local.x > (float)0)
			{
				this.speed_local.x = (float)0;
			}
		}
		if (this.speed_local.y > (float)0)
		{
			this.speed_local.y = this.speed_local.y - this.deltaTime * this.air_resistance_top * this.air_density * (this.speed_local.y * this.speed_local.y);
			if (this.speed_local.y < (float)0)
			{
				this.speed_local.y = (float)0;
			}
		}
		if (this.speed_local.y < (float)0)
		{
			this.speed_local.y = this.speed_local.y + this.deltaTime * this.air_resistance_top * this.air_density * (this.speed_local.y * this.speed_local.y);
			if (this.speed_local.y > (float)0)
			{
				this.speed_local.y = (float)0;
			}
		}
		this.health -= (float)1;
		if (this.health < (float)0 || this.out_of_range || this.speed_local.z < (float)0)
		{
			this.health = (float)UnityEngine.Random.Range(15, 555);
			this.myTransform.position = this.position_start;
			this.height_set = false;
			this.speed_local.z = (float)0;
		}
		this.position = this.myTransform.position - this.myTransform.forward * this.preterrain.heightmap_conversion.x;
		this.heightmap_x = (int)((this.position.x - this.terrain_position.x) / this.preterrain.heightmap_conversion.x);
		this.heightmap_y = (int)((this.position.z - this.terrain_position.z) / this.preterrain.heightmap_conversion.y);
		this.out_of_range = false;
		if ((float)this.heightmap_y > this.preterrain.heightmap_resolution - (float)1)
		{
			this.out_of_range = true;
		}
		if ((float)this.heightmap_x > this.preterrain.heightmap_resolution - (float)1)
		{
			this.out_of_range = true;
		}
		if (this.heightmap_x < 0)
		{
			this.out_of_range = true;
		}
		if (this.heightmap_y < 0)
		{
			this.out_of_range = true;
		}
		if (!this.out_of_range)
		{
			this.position = this.myTransform.position + this.myTransform.forward * this.preterrain.heightmap_conversion.x;
			this.heightmap_x = (int)((this.position.x - this.terrain_position.x) / this.preterrain.heightmap_conversion.x);
			this.heightmap_y = (int)((this.position.z - this.terrain_position.z) / this.preterrain.heightmap_conversion.y);
			this.out_of_range = false;
			if ((float)this.heightmap_y > this.preterrain.heightmap_resolution - (float)1)
			{
				this.out_of_range = true;
			}
			if ((float)this.heightmap_x > this.preterrain.heightmap_resolution - (float)1)
			{
				this.out_of_range = true;
			}
			if (this.heightmap_x < 0)
			{
				this.out_of_range = true;
			}
			if (this.heightmap_y < 0)
			{
				this.out_of_range = true;
			}
			if (!this.out_of_range)
			{
				this.position = this.myTransform.position;
				this.heightmap_x = (int)((this.position.x - this.terrain_position.x) / this.preterrain.heightmap_conversion.x);
				this.heightmap_y = (int)((this.position.z - this.terrain_position.z) / this.preterrain.heightmap_conversion.y);
				this.out_of_range = false;
				if ((float)this.heightmap_y > this.preterrain.heightmap_resolution - (float)1)
				{
					this.out_of_range = true;
				}
				if ((float)this.heightmap_x > this.preterrain.heightmap_resolution - (float)1)
				{
					this.out_of_range = true;
				}
				if (this.heightmap_x < 0)
				{
					this.out_of_range = true;
				}
				if (this.heightmap_y < 0)
				{
					this.out_of_range = true;
				}
			}
		}
		this.speed_world = this.myTransform.TransformDirection(this.speed_local);
		this.myTransform.rotation = this.rotation;
		this.myTransform.Translate(this.speed_world.x, this.speed_world.y, this.speed_world.z, Space.World);
		this.height = this.terrain.SampleHeight(this.myTransform.position);
		if (this.myTransform.position.y < this.height)
		{
			float y = this.height;
			Vector3 vector2 = this.myTransform.position;
			float num2 = vector2.y = y;
			Vector3 vector3 = this.myTransform.position = vector2;
		}
	}

	public override void erosion4()
	{
		if (!this.out_of_range)
		{
			this.rotation1 = this.terrain.terrainData.GetInterpolatedNormal((this.myTransform.position.x - this.terrain_position.x) / this.terrain_size.x, (this.myTransform.position.z - this.terrain_position.z) / this.terrain_size.z);
			this.height = this.terrain.SampleHeight(this.myTransform.position);
			this.rotation1.x = this.rotation1.x / (float)3 * (float)2;
			this.rotation1.z = this.rotation1.z / (float)3 * (float)2;
			this.rotation = Quaternion.LookRotation(this.rotation1);
			float x = this.rotation.eulerAngles.x + (float)90;
			Vector3 eulerAngles = this.rotation.eulerAngles;
			float num = eulerAngles.x = x;
			Vector3 vector = this.rotation.eulerAngles = eulerAngles;
			this.rotation = Quaternion.RotateTowards(this.myTransform.rotation, this.rotation, (float)15);
			this.rotation1 = this.rotation.eulerAngles;
		}
		this.speed_local = this.myTransform.InverseTransformDirection(this.speed_world);
		if (this.speed_local.z < (float)0)
		{
			this.multiplier = this.speed_local.z * (float)-1;
		}
		else
		{
			this.speed_local.z = this.speed_local.z + (this.rotation1.x + (float)30) * this.deltaTime * 0.03f * this.scale_start.z;
			this.multiplier = (float)1;
		}
		this.scale.x = (float)1500 / (this.height + (float)40) + this.scale_start.x;
		this.scale.z = this.speed_local.z / (float)2;
		float x2 = this.scale.x;
		Vector3 localScale = this.myTransform.localScale;
		float num2 = localScale.x = x2;
		Vector3 vector2 = this.myTransform.localScale = localScale;
		float z = this.scale.z;
		Vector3 localScale2 = this.myTransform.localScale;
		float num3 = localScale2.z = z;
		Vector3 vector3 = this.myTransform.localScale = localScale2;
		if (this.speed_local.z > (float)0)
		{
			this.speed_local.z = this.speed_local.z - this.deltaTime * this.air_resistance_front * this.air_density * (this.speed_local.z * this.speed_local.z);
			if (this.speed_local.z < (float)0)
			{
				this.speed_local.z = (float)0;
			}
		}
		if (this.speed_local.z < (float)0)
		{
			this.speed_local.z = this.speed_local.z + this.deltaTime * this.air_resistance_back * this.air_density * (this.speed_local.z * this.speed_local.z);
			if (this.speed_local.z > (float)0)
			{
				this.speed_local.z = (float)0;
			}
		}
		if (this.speed_local.x > (float)0)
		{
			this.speed_local.x = this.speed_local.x - this.deltaTime * this.air_resistance_side * this.air_density * (this.speed_local.x * this.speed_local.x);
			if (this.speed_local.x < (float)0)
			{
				this.speed_local.x = (float)0;
			}
		}
		if (this.speed_local.x < (float)0)
		{
			this.speed_local.x = this.speed_local.x + this.deltaTime * this.air_resistance_side * this.air_density * (this.speed_local.x * this.speed_local.x);
			if (this.speed_local.x > (float)0)
			{
				this.speed_local.x = (float)0;
			}
		}
		if (this.speed_local.y > (float)0)
		{
			this.speed_local.y = this.speed_local.y - this.deltaTime * this.air_resistance_top * this.air_density * (this.speed_local.y * this.speed_local.y);
			if (this.speed_local.y < (float)0)
			{
				this.speed_local.y = (float)0;
			}
		}
		if (this.speed_local.y < (float)0)
		{
			this.speed_local.y = this.speed_local.y + this.deltaTime * this.air_resistance_top * this.air_density * (this.speed_local.y * this.speed_local.y);
			if (this.speed_local.y > (float)0)
			{
				this.speed_local.y = (float)0;
			}
		}
		this.health -= (float)1;
		if (this.health < (float)0 || this.out_of_range || this.speed_local.z < (float)0)
		{
			this.health = (float)UnityEngine.Random.Range(15, 555);
			this.myTransform.position = this.position_start;
			this.height_set = false;
			this.speed_local.z = (float)0;
		}
		this.position = this.myTransform.position - this.myTransform.forward * this.preterrain.heightmap_conversion.x;
		this.heightmap_x = (int)Mathf.Round((this.position.x - this.terrain_position.x) / this.preterrain.heightmap_conversion.x);
		this.heightmap_y = (int)Mathf.Round((this.position.z - this.terrain_position.z) / this.preterrain.heightmap_conversion.y);
		this.out_of_range = false;
		if ((float)this.heightmap_y > this.preterrain.heightmap_resolution - (float)1)
		{
			this.out_of_range = true;
		}
		if ((float)this.heightmap_x > this.preterrain.heightmap_resolution - (float)1)
		{
			this.out_of_range = true;
		}
		if (this.heightmap_x < 0)
		{
			this.out_of_range = true;
		}
		if (this.heightmap_y < 0)
		{
			this.out_of_range = true;
		}
		if (!this.out_of_range)
		{
			this.position = this.myTransform.position + this.myTransform.forward * this.preterrain.heightmap_conversion.x;
			this.heightmap_x = (int)Mathf.Round((this.position.x - this.terrain_position.x) / this.preterrain.heightmap_conversion.x);
			this.heightmap_y = (int)Mathf.Round((this.position.z - this.terrain_position.z) / this.preterrain.heightmap_conversion.y);
			this.out_of_range = false;
			if ((float)this.heightmap_y > this.preterrain.heightmap_resolution - (float)1)
			{
				this.out_of_range = true;
			}
			if ((float)this.heightmap_x > this.preterrain.heightmap_resolution - (float)1)
			{
				this.out_of_range = true;
			}
			if (this.heightmap_x < 0)
			{
				this.out_of_range = true;
			}
			if (this.heightmap_y < 0)
			{
				this.out_of_range = true;
			}
			if (!this.out_of_range)
			{
				this.position = this.myTransform.position;
				this.heightmap_x = (int)Mathf.Round((this.position.x - this.terrain_position.x) / this.preterrain.heightmap_conversion.x);
				this.heightmap_y = (int)Mathf.Round((this.position.z - this.terrain_position.z) / this.preterrain.heightmap_conversion.y);
				this.out_of_range = false;
				if ((float)this.heightmap_y > this.preterrain.heightmap_resolution - (float)1)
				{
					this.out_of_range = true;
				}
				if ((float)this.heightmap_x > this.preterrain.heightmap_resolution - (float)1)
				{
					this.out_of_range = true;
				}
				if (this.heightmap_x < 0)
				{
					this.out_of_range = true;
				}
				if (this.heightmap_y < 0)
				{
					this.out_of_range = true;
				}
			}
		}
		this.speed_world = this.myTransform.TransformDirection(this.speed_local);
		this.myTransform.rotation = this.rotation;
		this.myTransform.Translate(this.speed_world.x, this.speed_world.y, this.speed_world.z, Space.World);
		if (this.myTransform.position.y < this.height)
		{
			float y = this.height;
			Vector3 vector4 = this.myTransform.position;
			float num4 = vector4.y = y;
			Vector3 vector5 = this.myTransform.position = vector4;
		}
	}

	public override void Main()
	{
	}
}
