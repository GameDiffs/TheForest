using System;
using UnityEngine;

[AddComponentMenu("AFS/Touch Bending/CollisionGS")]
public class touchBendingCollisionGSMultiple : MonoBehaviour
{
	public float stiffness = 10f;

	public float disturbance = 0.3f;

	public float duration = 5f;

	public bool localSpace;

	private Transform myTransform;

	private Renderer myRenderer;

	private MaterialPropertyBlock TouchMaterialBlock;

	private Matrix4x4 myMatrix;

	private Vector3 axis;

	private Vector3 axis1;

	public bool touched;

	private bool doubletouched;

	private bool left;

	private bool finished = true;

	private bool left1;

	private bool finished1 = true;

	private float intialTouchForce;

	private float touchBending;

	private float targetTouchBending;

	private float easingControl;

	private float intialTouchForce1;

	private float touchBending1;

	private float targetTouchBending1;

	private float easingControl1;

	private int Player_ID;

	private touchBendingPlayerListener PlayerVars;

	private Vector3 Player_Direction;

	private float Player_Speed;

	private int Player1_ID;

	private touchBendingPlayerListener PlayerVars1;

	private Vector3 Player_Direction1;

	private float Player_Speed1;

	private float timer;

	private float timer1;

	private float lerptime;

	private float scale;

	private float scale1;

	public Renderer[] addRenderer;

	private void Awake()
	{
		this.myTransform = base.transform;
		this.myRenderer = base.GetComponent<Renderer>();
	}

	private void OnEnable()
	{
		this.TouchMaterialBlock = new MaterialPropertyBlock();
		this.TouchMaterialBlock.Clear();
		this.TouchMaterialBlock.SetVector("_TouchBendingForce", Vector4.zero);
		this.myRenderer.SetPropertyBlock(this.TouchMaterialBlock);
	}

	private void OnSpawned()
	{
		this.TouchMaterialBlock = new MaterialPropertyBlock();
		this.TouchMaterialBlock.Clear();
		this.TouchMaterialBlock.SetVector("_TouchBendingForce", Vector4.zero);
		this.myRenderer.SetPropertyBlock(this.TouchMaterialBlock);
		this.touched = false;
		this.doubletouched = false;
	}

	private void OnDespawned()
	{
		this.TouchMaterialBlock.Clear();
		this.myRenderer.SetPropertyBlock(this.TouchMaterialBlock);
		this.TouchMaterialBlock = null;
	}

	private void OnTriggerEnter(Collider other)
	{
		touchBendingPlayerListener component = other.GetComponent<touchBendingPlayerListener>();
		if (component != null && component.enabled)
		{
			if (!this.touched)
			{
				this.Player_ID = other.GetInstanceID();
				this.PlayerVars = component;
				this.Player_Direction = this.PlayerVars.Player_Direction;
				this.Player_Speed = this.PlayerVars.Player_Speed;
				this.intialTouchForce = this.Player_Speed;
				this.axis = this.PlayerVars.Player_Direction;
				this.axis = Quaternion.Euler(0f, 90f, 0f) * this.axis;
				this.timer = 0f;
				this.touched = true;
				this.left = false;
				this.targetTouchBending = 1f;
				this.touchBending = this.targetTouchBending;
				this.finished = false;
			}
			else
			{
				if (this.doubletouched)
				{
					this.SwapTouchBending();
				}
				this.Player1_ID = other.GetInstanceID();
				this.PlayerVars1 = component;
				this.Player_Direction1 = this.PlayerVars1.Player_Direction;
				this.Player_Speed1 = this.PlayerVars1.Player_Speed;
				this.intialTouchForce1 = this.Player_Speed1;
				this.axis1 = this.Player_Direction1;
				this.axis1 = Quaternion.Euler(0f, 90f, 0f) * this.axis1;
				this.timer1 = 0f;
				this.left1 = false;
				this.targetTouchBending1 = 1f;
				this.touchBending1 = this.targetTouchBending1;
				this.finished1 = false;
				this.lerptime = this.duration - this.timer;
				this.doubletouched = true;
			}
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (this.Player_ID != this.Player1_ID)
		{
			if (other.GetInstanceID() == this.Player_ID)
			{
				this.left = true;
				this.targetTouchBending = 0f;
			}
			else
			{
				this.left1 = true;
				this.targetTouchBending1 = 0f;
			}
		}
		else
		{
			this.left = true;
			this.targetTouchBending = 0f;
			this.left1 = true;
			this.targetTouchBending1 = 0f;
		}
	}

	private void Update()
	{
		if (this.touched)
		{
			this.Player_Speed = this.PlayerVars.Player_Speed;
			this.touchBending = Mathf.Lerp(this.touchBending, this.targetTouchBending, this.timer / this.duration);
			this.easingControl = this.Bounce(this.timer);
			if (!this.doubletouched)
			{
				if (this.finished && this.targetTouchBending == 0f)
				{
					this.ResetTouchBending();
				}
				else
				{
					Quaternion q = Quaternion.Euler(this.axis * (this.intialTouchForce * this.stiffness) * this.easingControl);
					this.myMatrix.SetTRS(Vector3.zero, q, new Vector3(1f, 1f, 1f));
					if (this.localSpace)
					{
						this.myMatrix *= base.transform.localToWorldMatrix;
					}
					this.TouchMaterialBlock.SetMatrix("_RotMatrix", this.myMatrix);
					this.scale = this.Player_Speed * this.easingControl * this.disturbance;
					this.TouchMaterialBlock.SetVector("_TouchBendingForce", new Vector4(this.Player_Direction.x * this.scale, this.Player_Direction.y * this.scale, this.Player_Direction.z * this.scale, 1f));
					if (this.left)
					{
						this.timer += Time.deltaTime;
					}
					else
					{
						this.timer += Time.deltaTime * this.Player_Speed;
					}
				}
			}
			else if (this.finished && this.targetTouchBending == 0f)
			{
				this.SwapTouchBending();
				this.doubletouched = false;
				this.Player_Speed = this.PlayerVars.Player_Speed;
				this.touchBending = Mathf.Lerp(this.touchBending, this.targetTouchBending, this.timer / this.duration);
				this.easingControl = this.Bounce(this.timer);
				if (this.finished && this.targetTouchBending == 0f)
				{
					this.ResetTouchBending();
				}
				else
				{
					Quaternion q2 = Quaternion.Euler(this.axis * (this.intialTouchForce * this.stiffness) * this.easingControl);
					this.myMatrix.SetTRS(Vector3.zero, q2, new Vector3(1f, 1f, 1f));
					if (this.localSpace)
					{
						this.myMatrix *= base.transform.localToWorldMatrix;
					}
					this.TouchMaterialBlock.SetMatrix("_RotMatrix", this.myMatrix);
					this.scale = this.Player_Speed * this.easingControl * this.disturbance;
					this.TouchMaterialBlock.SetVector("_TouchBendingForce", new Vector4(this.Player_Direction.x * this.scale, this.Player_Direction.y * this.scale, this.Player_Direction.z * this.scale, 1f));
					if (this.left)
					{
						this.timer += Time.deltaTime;
					}
					else
					{
						this.timer += Time.deltaTime * this.Player_Speed;
					}
				}
			}
			else
			{
				this.Player_Speed1 = this.PlayerVars1.Player_Speed;
				this.touchBending1 = Mathf.Lerp(this.touchBending1, this.targetTouchBending1, this.timer1 / this.duration);
				this.easingControl1 = this.Bounce1(this.timer1);
				if (this.finished1 && this.targetTouchBending1 == 0f)
				{
					this.doubletouched = false;
				}
				else
				{
					Quaternion quaternion = Quaternion.Euler(this.axis * (this.intialTouchForce * this.stiffness) * this.easingControl);
					Quaternion rhs = Quaternion.Euler(this.axis1 * (this.intialTouchForce1 * this.stiffness) * this.easingControl1);
					quaternion *= rhs;
					this.myMatrix.SetTRS(Vector3.zero, quaternion, new Vector3(1f, 1f, 1f));
					if (this.localSpace)
					{
						this.myMatrix *= base.transform.localToWorldMatrix;
					}
					this.TouchMaterialBlock.SetMatrix("_RotMatrix", this.myMatrix);
					this.scale = this.Player_Speed * this.easingControl * this.disturbance;
					this.scale1 = this.Player_Speed1 * this.easingControl1 * this.disturbance;
					this.TouchMaterialBlock.SetVector("_TouchBendingForce", Vector4.Lerp(new Vector4(this.Player_Direction.x * this.scale, this.Player_Direction.y * this.scale, this.Player_Direction.z * this.scale, 1f), new Vector4(this.Player_Direction1.x * this.scale1, this.Player_Direction1.y * this.scale1, this.Player_Direction1.z * this.scale1, 1f), this.timer1 / (this.lerptime + 0.0001f) * 8f));
					if (this.left)
					{
						this.timer += Time.deltaTime;
					}
					else
					{
						this.timer += Time.deltaTime * this.Player_Speed;
					}
					if (this.left1)
					{
						this.timer1 += Time.deltaTime;
					}
					else
					{
						this.timer1 += Time.deltaTime * this.Player_Speed1;
					}
				}
			}
		}
		this.myRenderer.SetPropertyBlock(this.TouchMaterialBlock);
		for (int i = 0; i < this.addRenderer.Length; i++)
		{
			if (this.addRenderer[i] != null)
			{
				this.addRenderer[i].SetPropertyBlock(this.TouchMaterialBlock);
			}
		}
	}

	public float Bounce(float x)
	{
		if (x / this.duration >= 1f)
		{
			if (this.easingControl == 0f && this.left)
			{
				this.finished = true;
			}
			return this.targetTouchBending;
		}
		return Mathf.Lerp(Mathf.Sin(x * 10f / this.duration) / (x + 1.25f) * 8f, this.touchBending, Mathf.Sqrt(x / this.duration));
	}

	public float Bounce1(float x)
	{
		if (x / this.duration >= 1f)
		{
			if (this.easingControl1 == 0f && this.left1)
			{
				this.finished1 = true;
			}
			return this.targetTouchBending1;
		}
		return Mathf.Lerp(Mathf.Sin(x * 10f / this.duration) / (x + 1.25f) * 8f, this.touchBending1, Mathf.Sqrt(x / this.duration));
	}

	public void SwapTouchBending()
	{
		this.Player_ID = this.Player1_ID;
		this.PlayerVars = this.PlayerVars1;
		this.Player_Direction = this.Player_Direction1;
		this.Player_Speed = this.Player_Speed1;
		this.intialTouchForce = this.intialTouchForce1;
		this.touchBending = this.touchBending1;
		this.targetTouchBending = this.targetTouchBending1;
		this.easingControl = this.easingControl1;
		this.left = this.left1;
		this.finished = this.finished1;
		this.axis = this.axis1;
		this.timer = this.timer1;
	}

	public void ResetTouchBending()
	{
		this.TouchMaterialBlock.Clear();
		this.TouchMaterialBlock.SetVector("_TouchBendingForce", Vector4.zero);
		this.touched = false;
		this.doubletouched = false;
	}
}
