using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using TheForest.Utils;
using UnityEngine;

public class lb_BirdController : MonoBehaviour
{
	public int idealNumberOfBirds;

	public int maximumNumberOfBirds;

	public Camera currentCamera;

	public float unspawnDistance = 10f;

	private sceneTracker sceneInfo;

	public float drag;

	public float landDrag;

	public float flySpeed;

	public float landSpeed;

	public float upSpeed;

	public LayerMask groundLayer;

	private Collider[] hits;

	public bool seagull = true;

	public bool blueBird = true;

	public bool redBird = true;

	public bool chickadee = true;

	public bool sparrow = true;

	public bool crow = true;

	private bool pause;

	private GameObject[] myBirds;

	private List<string> myBirdTypes = new List<string>();

	public List<GameObject> birdGroundTargets = new List<GameObject>();

	public List<GameObject> birdPerchTargets = new List<GameObject>();

	private int activeBirds;

	private int birdIndex;

	public int initIdealBirds;

	public int initMaxBirds;

	private int modIdealBirds;

	private int modMaxBirds;

	private int birdCount;

	private int repeatCount;

	private bool breakBool;

	private Vector2 tempPos;

	private Vector3 flyPos;

	private int layerMask;

	private List<GameObject> gtRemove = new List<GameObject>();

	private List<GameObject> ptRemove = new List<GameObject>();

	public void AllFlee()
	{
		for (int i = 0; i < this.myBirds.Length; i++)
		{
			if (this.myBirds[i].activeSelf)
			{
				this.myBirds[i].SendMessage("Flee");
			}
		}
	}

	public void Pause()
	{
		if (this.pause)
		{
			this.AllUnPause();
		}
		else
		{
			this.AllPause();
		}
	}

	public void AllPause()
	{
		this.pause = true;
		for (int i = 0; i < this.myBirds.Length; i++)
		{
			if (this.myBirds[i].activeSelf)
			{
				this.myBirds[i].SendMessage("PauseBird");
			}
		}
	}

	public void AllUnPause()
	{
		this.pause = false;
		for (int i = 0; i < this.myBirds.Length; i++)
		{
			if (this.myBirds[i].activeSelf)
			{
				this.myBirds[i].SendMessage("UnPauseBird");
			}
		}
	}

	public void SpawnAmount(int amt)
	{
		for (int i = 0; i <= amt; i++)
		{
			this.SpawnBird();
		}
	}

	public void ChangeCamera(Camera cam)
	{
		this.currentCamera = cam;
	}

	private void checkHandConditions()
	{
		if (LocalPlayer.GameObject && this.currentCamera)
		{
			this.birdCount = 0;
			if (LocalPlayer.Ridigbody.velocity.magnitude < 0.2f && !this.breakBool && LocalPlayer.TargetFunctions.allAttackers == 0 && !LocalPlayer.Animator.GetBool("bookHeld"))
			{
				for (int i = 0; i < this.myBirds.Length; i++)
				{
					if (this.myBirds[i])
					{
						lb_Bird component = this.myBirds[i].GetComponent<lb_Bird>();
						if (component && component.birdVisible)
						{
							this.birdCount++;
						}
					}
				}
				if (this.birdCount > 4)
				{
					this.repeatCount++;
				}
				if (this.repeatCount > 3 && UnityEngine.Random.value < 0.5f)
				{
					LocalPlayer.GameObject.SendMessage("doBirdOnHand");
					this.repeatCount = 0;
					this.breakBool = true;
					base.Invoke("resetBreakBool", 100f);
				}
			}
			else
			{
				this.repeatCount = 0;
				this.birdCount = 0;
			}
		}
	}

	private void resetBreakBool()
	{
		this.breakBool = false;
	}

	private void Start()
	{
		this.layerMask = 513;
		base.InvokeRepeating("checkHandConditions", 1f, 8f);
		base.InvokeRepeating("updateBirdAmounts", 1f, 120f);
		this.initIdealBirds = this.idealNumberOfBirds;
		this.initMaxBirds = this.maximumNumberOfBirds;
		this.modIdealBirds = this.idealNumberOfBirds;
		this.modMaxBirds = this.maximumNumberOfBirds;
		this.sceneInfo = Scene.SceneTracker;
		if (this.idealNumberOfBirds >= this.maximumNumberOfBirds)
		{
			this.idealNumberOfBirds = this.maximumNumberOfBirds - 1;
		}
		if (this.seagull)
		{
			this.myBirdTypes.Add("lb_seagull");
		}
		if (this.blueBird)
		{
			this.myBirdTypes.Add("lb_blueBird");
		}
		if (this.redBird)
		{
			this.myBirdTypes.Add("lb_redBird");
		}
		if (this.chickadee)
		{
			this.myBirdTypes.Add("lb_chickadee");
		}
		if (this.sparrow)
		{
			this.myBirdTypes.Add("lb_sparrow");
		}
		if (this.crow)
		{
			this.myBirdTypes.Add("lb_crow");
		}
		this.myBirds = new GameObject[this.maximumNumberOfBirds];
		for (int i = 0; i < this.myBirds.Length; i++)
		{
			GameObject original = Resources.Load(this.myBirdTypes[UnityEngine.Random.Range(0, this.myBirdTypes.Count)], typeof(GameObject)) as GameObject;
			this.myBirds[i] = (UnityEngine.Object.Instantiate(original, Vector3.zero, Quaternion.identity) as GameObject);
			this.myBirds[i].SendMessage("SetController", this);
			this.myBirds[i].SetActive(false);
		}
		GameObject[] array = GameObject.FindGameObjectsWithTag("lb_groundTarget");
		GameObject[] array2 = GameObject.FindGameObjectsWithTag("lb_perchTarget");
		if (this.currentCamera)
		{
			for (int j = 0; j < array.Length; j++)
			{
				if (Vector3.Distance(array[j].transform.position, this.currentCamera.transform.position) < this.unspawnDistance)
				{
					this.birdGroundTargets.Add(array[j]);
				}
			}
			for (int k = 0; k < array2.Length; k++)
			{
				if (Vector3.Distance(array2[k].transform.position, this.currentCamera.transform.position) < this.unspawnDistance)
				{
					this.birdPerchTargets.Add(array2[k]);
				}
			}
		}
		if (!base.IsInvoking("UpdateBirds"))
		{
			base.InvokeRepeating("UpdateBirds", 30f, 2f);
		}
		base.StopCoroutine("UpdateTargets");
		this.birdPerchTargets.Clear();
		this.birdGroundTargets.Clear();
		base.StartCoroutine("UpdateTargets");
	}

	private void Update()
	{
		if (this.currentCamera == null && Camera.main)
		{
			this.currentCamera = Camera.main;
		}
	}

	private void OnDeserialized()
	{
		base.StopCoroutine("UpdateTargets");
	}

	private void OnEnable()
	{
		base.StopCoroutine("UpdateTargets");
		base.StartCoroutine("UpdateTargets");
		if (!base.IsInvoking("UpdateBirds"))
		{
			base.InvokeRepeating("UpdateBirds", 30f, 2f);
		}
	}

	private Vector3 FindPointInGroundTarget(GameObject target)
	{
		Vector3 vector;
		vector.x = UnityEngine.Random.Range(target.GetComponent<Collider>().bounds.max.x, target.GetComponent<Collider>().bounds.min.x);
		vector.y = target.GetComponent<Collider>().bounds.max.y;
		vector.z = UnityEngine.Random.Range(target.GetComponent<Collider>().bounds.max.z, target.GetComponent<Collider>().bounds.min.z);
		float y = target.GetComponent<Collider>().bounds.size.y;
		RaycastHit raycastHit;
		if (y > 0f && Physics.Raycast(vector, -Vector3.up, out raycastHit, y, this.groundLayer))
		{
			return raycastHit.point;
		}
		return vector;
	}

	private void updateBirdAmounts()
	{
		if (this.initIdealBirds < this.modIdealBirds)
		{
			this.initIdealBirds++;
		}
		if (this.initMaxBirds < this.modMaxBirds)
		{
			this.initMaxBirds++;
		}
	}

	private void UpdateBirds()
	{
		if (Clock.Dark)
		{
			this.idealNumberOfBirds = this.initIdealBirds / 3;
			this.maximumNumberOfBirds = this.initMaxBirds / 3;
		}
		else
		{
			this.idealNumberOfBirds = this.initIdealBirds;
			this.maximumNumberOfBirds = this.initMaxBirds;
		}
		if (this.activeBirds < this.idealNumberOfBirds && this.AreThereActiveTargets() && !Clock.planecrash && !Clock.InCave)
		{
			this.SpawnBird();
		}
		else if (this.activeBirds < this.maximumNumberOfBirds && (double)UnityEngine.Random.value < 0.05 && this.AreThereActiveTargets() && !Clock.planecrash && !Clock.InCave)
		{
			this.SpawnBird();
		}
		if (this.currentCamera)
		{
			if (Clock.InCave)
			{
				GameObject[] array = this.myBirds;
				for (int i = 0; i < array.Length; i++)
				{
					GameObject bird = array[i];
					this.Unspawn(bird);
				}
				this.birdIndex = ((this.birdIndex != this.myBirds.Length - 1) ? (this.birdIndex + 1) : 0);
			}
			else if (this.myBirds[this.birdIndex].activeSelf && Vector3.Distance(this.myBirds[this.birdIndex].transform.position, this.currentCamera.transform.position) > this.unspawnDistance)
			{
				this.Unspawn(this.myBirds[this.birdIndex]);
			}
		}
		this.birdIndex = ((this.birdIndex != this.myBirds.Length - 1) ? (this.birdIndex + 1) : 0);
	}

	[DebuggerHidden]
	private IEnumerator UpdateTargets()
	{
		lb_BirdController.<UpdateTargets>c__Iterator213 <UpdateTargets>c__Iterator = new lb_BirdController.<UpdateTargets>c__Iterator213();
		<UpdateTargets>c__Iterator.<>f__this = this;
		return <UpdateTargets>c__Iterator;
	}

	private bool BirdOffCamera(Vector3 birdPos)
	{
		if (!this.currentCamera)
		{
			return false;
		}
		Vector3 vector = this.currentCamera.WorldToViewportPoint(birdPos);
		return vector.x < 0f || vector.x > 1f || vector.y < 0f || vector.y > 1f;
	}

	public void Unspawn(GameObject bird)
	{
		bird.transform.position = Vector3.zero;
		bird.SetActive(false);
		this.activeBirds--;
	}

	public void despawnAll()
	{
		base.CancelInvoke("UpdateBirds");
		base.StopAllCoroutines();
		GameObject[] array = this.myBirds;
		for (int i = 0; i < array.Length; i++)
		{
			GameObject gameObject = array[i];
			if (gameObject)
			{
				this.Unspawn(gameObject);
			}
		}
	}

	private void SpawnBird()
	{
		if (!this.pause)
		{
			GameObject gameObject = null;
			int num = Mathf.FloorToInt((float)UnityEngine.Random.Range(0, this.myBirds.Length));
			int num2 = 0;
			while (gameObject == null)
			{
				if (!this.myBirds[num].activeSelf)
				{
					gameObject = this.myBirds[num];
				}
				num = ((num + 1 < this.myBirds.Length) ? (num + 1) : 0);
				num2++;
				if (num2 >= this.myBirds.Length)
				{
					return;
				}
			}
			lb_Bird component = gameObject.GetComponent<lb_Bird>();
			if (!this.currentCamera)
			{
				return;
			}
			if (this.sceneInfo.beachMarkers.Count > 0)
			{
				int num3 = 0;
				foreach (GameObject current in this.sceneInfo.beachMarkers)
				{
					if (Vector3.Distance(current.transform.position, this.currentCamera.transform.position) < 120f)
					{
						num3++;
					}
				}
				if (num3 == 0 && component.typeSeagull)
				{
					num3 = 0;
					return;
				}
				if (num3 > 0 && (component.typeSparrow || component.typeCrow || component.typeBlueBird || component.typeRedBird))
				{
					num3 = 0;
					return;
				}
			}
			gameObject.transform.position = this.FindPositionOffCamera();
			if (gameObject.transform.position == Vector3.zero)
			{
				return;
			}
			gameObject.SetActive(true);
			this.activeBirds++;
			this.BirdFindTarget(gameObject);
		}
	}

	private bool AreThereActiveTargets()
	{
		return this.birdGroundTargets.Count > 0 || this.birdPerchTargets.Count > 0;
	}

	private Vector3 FindPositionOffCamera()
	{
		Vector3 vector = -LocalPlayer.Transform.forward * (float)UnityEngine.Random.Range(20, 40);
		vector = Quaternion.Euler(0f, UnityEngine.Random.Range(-1f, 1f) * 100f, 0f) * vector;
		vector = LocalPlayer.Transform.position + vector;
		vector.y += 30f;
		UnityEngine.Debug.DrawLine(LocalPlayer.Transform.position, vector, Color.green, 4f);
		return vector;
	}

	private Vector2 Circle2(float radius)
	{
		Vector2 insideUnitCircle = UnityEngine.Random.insideUnitCircle;
		insideUnitCircle.Normalize();
		return insideUnitCircle * radius;
	}

	public void BirdFindTarget(GameObject bird)
	{
		this.birdPerchTargets.RemoveAll((GameObject item) => item == null);
		this.birdPerchTargets.RemoveAll((GameObject item) => item != item.GetComponent<SphereCollider>().enabled);
		this.birdGroundTargets.RemoveAll((GameObject item) => item == null);
		this.birdGroundTargets.TrimExcess();
		if (this.birdGroundTargets.Count > 0 || this.birdPerchTargets.Count > 0)
		{
			float num = 0f;
			float num2 = (float)this.birdPerchTargets.Count * 1f;
			for (int i = 0; i < this.birdGroundTargets.Count; i++)
			{
				num += this.birdGroundTargets[i].GetComponent<Collider>().bounds.size.x * this.birdGroundTargets[i].GetComponent<Collider>().bounds.size.z;
			}
			if (num2 == 0f || UnityEngine.Random.value < num / (num + num2))
			{
				GameObject gameObject = this.birdGroundTargets[Mathf.FloorToInt((float)UnityEngine.Random.Range(0, this.birdGroundTargets.Count))];
				if (bird)
				{
					bird.SendMessage("setNewFlyTarget", this.FindPointInGroundTarget(gameObject), SendMessageOptions.DontRequireReceiver);
					bird.SendMessage("setFlyingToPerch", SendMessageOptions.DontRequireReceiver);
					bird.SendMessage("clearPerchTarget", SendMessageOptions.DontRequireReceiver);
				}
			}
			else
			{
				GameObject gameObject = this.birdPerchTargets[Mathf.FloorToInt((float)UnityEngine.Random.Range(0, this.birdPerchTargets.Count))];
				if (bird)
				{
					bird.SendMessage("setNewFlyTarget", gameObject.transform.position, SendMessageOptions.DontRequireReceiver);
					bird.SendMessage("setFlyingToPerch", SendMessageOptions.DontRequireReceiver);
					bird.SendMessage("setPerchTarget", gameObject, SendMessageOptions.DontRequireReceiver);
				}
			}
		}
		else if (this.currentCamera)
		{
			this.tempPos = this.Circle2(70f);
			this.flyPos = new Vector3(this.currentCamera.transform.position.x + this.tempPos.x, this.currentCamera.transform.position.y + (float)UnityEngine.Random.Range(10, 25), this.currentCamera.transform.position.z + this.tempPos.y);
			if (bird)
			{
				bird.SendMessage("flyToRandomTarget", this.flyPos, SendMessageOptions.DontRequireReceiver);
			}
		}
	}
}
