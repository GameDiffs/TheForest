using System;
using UnityEngine;

public class rockSound : MonoBehaviour
{
	public float maximumSpeed;

	public float speedThreshold = 2.5f;

	public string rockOnTree;

	public string rockBush;

	public string rockWater;

	public string rockOnGround;

	public string stickHitTree;

	public GameObject soundDetect;

	public float soundRange;

	private GameObject soundGo;

	private Transform spawnedSound;

	private int waterLayer;

	private bool soundDelay;

	private bool effectDelay;

	public Rigidbody rigidBody;

	private int inWaterCount;

	private bool InWater
	{
		get
		{
			return this.inWaterCount > 0;
		}
	}

	private void Awake()
	{
		this.soundDelay = false;
		base.Invoke("disableSoundDelay", 2f);
		this.waterLayer = LayerMask.NameToLayer("Water");
		this.rigidBody = base.GetComponent<Rigidbody>();
	}

	private void Start()
	{
		FMOD_StudioSystem.PreloadEvent(this.rockOnTree);
		FMOD_StudioSystem.PreloadEvent(this.rockBush);
		FMOD_StudioSystem.PreloadEvent(this.rockWater);
		FMOD_StudioSystem.PreloadEvent(this.rockOnGround);
	}

	private void disableSoundDelay()
	{
		this.soundDelay = false;
	}

	private void PlayEvent(string path)
	{
		if (!this.rigidBody)
		{
			return;
		}
		if (path.Length > 0)
		{
			if (this.maximumSpeed > 0f)
			{
				FMODCommon.PlayOneshot(path, base.transform.position, FMODCommon.NetworkRole.Any, new object[]
				{
					"speed",
					this.rigidBody.velocity.magnitude / this.maximumSpeed
				});
			}
			else
			{
				FMODCommon.PlayOneshot(path, base.transform.position, FMODCommon.NetworkRole.Any, new object[0]);
			}
		}
		this.effectDelay = true;
		base.Invoke("disableEffectDelay", 1f);
	}

	public void OnCollisionEnter(Collision collision)
	{
		if (!this.rigidBody)
		{
			return;
		}
		if ((collision.gameObject.CompareTag("Tree") || collision.gameObject.CompareTag("Stick") || collision.gameObject.layer == 17 || collision.gameObject.layer == 20 || collision.gameObject.layer == 25 || UnderfootSurfaceDetector.GetSurfaceType(collision.collider) != UnderfootSurfaceDetector.SurfaceType.None) && this.rigidBody.velocity.magnitude > this.speedThreshold)
		{
			if (!this.InWater && !this.effectDelay)
			{
				this.PlayEvent(this.rockOnTree);
			}
			if (!this.soundDelay)
			{
				this.enableSound();
				this.soundDelay = true;
			}
		}
		if ((collision.gameObject.CompareTag("TerrainMain") || collision.gameObject.layer == 26) && this.rigidBody.velocity.magnitude > this.speedThreshold)
		{
			if (!this.InWater && !this.effectDelay)
			{
				if (this.rockOnGround.Length > 0)
				{
					this.PlayEvent(this.rockOnGround);
				}
				else
				{
					this.PlayEvent(this.rockOnTree);
				}
			}
			if (!this.soundDelay)
			{
				this.enableSound();
				this.soundDelay = true;
			}
		}
		if (collision.gameObject.CompareTag("Float") && this.rigidBody.velocity.magnitude > this.speedThreshold)
		{
			if (!this.InWater && !this.effectDelay)
			{
				if (this.rockOnGround.Length > 0)
				{
					this.PlayEvent(this.stickHitTree);
				}
				else
				{
					this.PlayEvent(this.rockOnTree);
				}
			}
			if (!this.soundDelay)
			{
				this.enableSound();
				this.soundDelay = true;
			}
		}
	}

	public void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.CompareTag("SmallTree"))
		{
		}
		if (!this.rigidBody)
		{
			return;
		}
		if (other.gameObject.layer == this.waterLayer)
		{
			if (!this.InWater && !this.effectDelay && this.rigidBody.velocity.magnitude > this.speedThreshold)
			{
				this.PlayEvent(this.rockWater);
			}
			this.inWaterCount++;
		}
	}

	public void OnTriggerExit(Collider other)
	{
		if (other.gameObject.layer == this.waterLayer)
		{
			this.inWaterCount--;
		}
	}

	private void enableSound()
	{
		this.spawnedSound = (UnityEngine.Object.Instantiate(this.soundDetect.transform, base.transform.position, base.transform.rotation) as Transform);
		this.spawnedSound.SendMessage("setRange", this.soundRange, SendMessageOptions.DontRequireReceiver);
		base.Invoke("disableSound", 0.5f);
	}

	private void disableSound()
	{
		if (this.spawnedSound)
		{
			UnityEngine.Object.Destroy(this.spawnedSound.gameObject);
		}
		this.soundDelay = false;
	}

	private void disableEffectDelay()
	{
		this.effectDelay = false;
	}

	private void OnDisable()
	{
		this.disableSound();
	}

	private void OnDestroy()
	{
		this.disableSound();
	}
}
