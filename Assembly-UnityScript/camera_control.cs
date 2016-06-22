using System;
using UnityEngine;

[Serializable]
public class camera_control : MonoBehaviour
{
	public float speed_forward;

	public float speed_forward_max;

	public float speed_side;

	public float speed_side_max;

	public float speed_up;

	public float speed_up_max;

	public float speed_y;

	public float speed_y_max;

	public float sp_up;

	public float sp_forward;

	public float sp_side;

	public float sp_y;

	public float r;

	public float spf_t;

	public float sps_t;

	public float spu_t;

	public float spy_t;

	public bool input;

	public int turbo;

	public float speed_turbo;

	public int Gear;

	public int TurboMax;

	public float y_axis;

	public float z_axis;

	public float x_axis;

	public float z_axis_max;

	public float x_axis_max;

	public Vector3 positionOld;

	public float speed1;

	public camera_control()
	{
		this.speed_forward_max = (float)40;
		this.speed_side = (float)40;
		this.speed_side_max = (float)40;
		this.speed_up_max = (float)30;
		this.speed_y_max = (float)12;
		this.sp_up = 10f;
		this.sp_forward = 1.5f;
		this.sp_side = 1.5f;
		this.r = 0.25f;
		this.Gear = 1;
		this.TurboMax = 100;
		this.z_axis_max = (float)20;
		this.x_axis_max = (float)20;
	}

	public override void Awake()
	{
		this.spy_t = (float)0;
	}

	public override void Start()
	{
		this.speed_forward = (float)0;
		this.speed_side = (float)0;
		this.InvokeRepeating("speed", (float)0, 0.1f);
	}

	public override void Update()
	{
		this.r = Time.deltaTime * 5.5f / Time.timeScale;
		this.sp_forward = Time.deltaTime * (float)25 / Time.timeScale;
		this.sp_side = Time.deltaTime * (float)25 / Time.timeScale;
		this.sp_up = Time.deltaTime * (float)25 / Time.timeScale;
		this.sp_y = Time.deltaTime * (float)4 / Time.timeScale;
		this.z_axis = Mathf.DeltaAngle(this.transform.rotation.eulerAngles.z, (float)0);
		this.x_axis = Mathf.DeltaAngle((float)0, this.transform.rotation.eulerAngles.x);
		this.y_axis = Mathf.DeltaAngle((float)0, this.transform.rotation.eulerAngles.y);
		if (Input.GetKey("up"))
		{
			this.input = true;
			if (this.speed_forward < this.speed_forward_max)
			{
				this.speed_forward += this.sp_forward;
			}
			this.spf_t = Time.time;
			this.sps_t = Time.time;
		}
		if (Input.GetKey("down"))
		{
			this.input = true;
			if (this.speed_forward > -this.speed_forward_max)
			{
				this.speed_forward -= this.sp_forward;
				this.spf_t = Time.time;
				this.sps_t = Time.time;
			}
		}
		if (Input.GetKey("right"))
		{
			this.input = true;
			if (this.speed_side < this.speed_side_max)
			{
				this.speed_side += this.sp_side;
			}
			this.sps_t = Time.time;
			this.spf_t = Time.time;
		}
		if (Input.GetKey("left"))
		{
			this.input = true;
			if (this.speed_side > -this.speed_side_max)
			{
				this.speed_side -= this.sp_side;
			}
			this.sps_t = Time.time;
			this.spf_t = Time.time;
		}
		if (Input.GetKey("q"))
		{
			this.input = true;
			if (this.speed_up < this.speed_up_max)
			{
				this.speed_up += this.sp_up;
			}
			this.spu_t = Time.time;
		}
		if (Input.GetKey("z"))
		{
			if (this.speed_up > -this.speed_up_max)
			{
				this.speed_up -= this.sp_up;
			}
			this.spu_t = Time.time;
		}
		if (Input.GetKey("w"))
		{
			this.input = true;
			if (this.speed_y < this.speed_y_max)
			{
				this.speed_y += this.sp_y;
			}
			this.spy_t = Time.time;
		}
		if (Input.GetKey("q"))
		{
			this.input = true;
			if (this.speed_y > -this.speed_y_max)
			{
				this.speed_y -= this.sp_y;
			}
			this.spy_t = Time.time;
		}
		if ((this.speed_forward > (float)0 && Time.time > this.spf_t) || this.turbo == 2)
		{
			this.speed_forward -= this.sp_forward / (float)2;
			if (this.speed_forward < (float)0)
			{
				this.speed_forward = (float)0;
			}
		}
		if ((this.speed_forward < (float)0 && Time.time > this.spf_t) || this.turbo == 2)
		{
			this.speed_forward += this.sp_forward / (float)2;
			if (this.speed_forward > (float)0)
			{
				this.speed_forward = (float)0;
			}
		}
		if (this.speed_side > (float)0 && Time.time > this.sps_t)
		{
			this.speed_side -= this.sp_side / (float)2;
			if (this.speed_side < (float)0)
			{
				this.speed_side = (float)0;
			}
		}
		if (this.speed_side < (float)0 && Time.time > this.sps_t)
		{
			this.speed_side += this.sp_side / (float)2;
			if (this.speed_side > (float)0)
			{
				this.speed_side = (float)0;
			}
		}
		if (this.speed_up > (float)0 && Time.time > this.spu_t)
		{
			this.speed_up -= this.sp_up / (float)2;
			if (this.speed_up < (float)0)
			{
				this.speed_up = (float)0;
			}
		}
		if (this.speed_up < (float)0 && Time.time > this.spu_t)
		{
			this.speed_up += this.sp_up / (float)2;
			if (this.speed_up > (float)0)
			{
				this.speed_up = (float)0;
			}
		}
		if (this.speed_y > (float)0 && Time.time > this.spy_t)
		{
			this.speed_y -= this.sp_y;
			if (this.speed_y < (float)0)
			{
				this.speed_y = (float)0;
			}
		}
		if (this.speed_y < (float)0 && Time.time > this.spy_t)
		{
			this.speed_y += this.sp_y;
			if (this.speed_y > (float)0)
			{
				this.speed_y = (float)0;
			}
		}
		if (this.speed_y > -0.2f && this.speed_y < 0.2f && Time.time > this.spy_t + 0.2f)
		{
			this.speed_y = (float)0;
		}
		float num = Mathf.Round(this.speed_side / (float)25 * (float)100) / (float)100;
		float num2 = Mathf.Round(this.speed_up / (float)25 * (float)100) / (float)100;
		float num3 = Mathf.Round(this.speed_forward / (float)25 * (float)100) / (float)100;
		if (num != (float)0 || num2 != (float)0 || num3 != (float)0)
		{
			this.transform.Translate(num * Time.deltaTime * (float)60, num2 * Time.deltaTime * (float)60, num3 * Time.deltaTime * (float)60);
		}
		this.input = false;
	}

	public override void OnGUI()
	{
	}

	public override void speed()
	{
		this.speed1 = Mathf.Round((this.transform.position - this.positionOld).magnitude) * (float)10 * 3.6f;
		this.positionOld = this.transform.position;
	}

	public override void Main()
	{
		this.spf_t = Time.time;
		this.sps_t = Time.time;
		this.spu_t = Time.time;
	}
}
