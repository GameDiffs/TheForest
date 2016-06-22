using Bolt;
using System;
using UnityEngine;
using UnityScript.Lang;

[Serializable]
public class SuitCase : MonoBehaviour
{
	public GameObject Lid;

	public Transform InsidePos;

	public GameObject[] WorldItem;

	public GameObject Interior;

	public string OpenEvent;

	private int health;

	private bool preloadComplete;

	[NonSerialized]
	public static int GlobalAmount;

	public SuitCase()
	{
		this.health = 1;
	}

	public override void Start()
	{
		this.TryPreload();
	}

	public override void TryPreload()
	{
		if (!this.preloadComplete)
		{
			this.preloadComplete = FMOD_StudioSystem.PreloadEvent(this.OpenEvent);
		}
	}

	public override void Hit(int damage)
	{
		if (this.health > 0)
		{
			this.health -= damage;
			if (this.health <= 0)
			{
				this.transform.parent.rotation = Quaternion.Euler((float)0, this.transform.parent.rotation.y, (float)-90);
				this.Open();
			}
		}
	}

	public override void Open_Perform()
	{
		if (!this.Interior.activeSelf)
		{
			this.Interior.SetActive(true);
			this.Lid.GetComponent<Animation>().Play();
			if (FMOD_StudioSystem.instance)
			{
				FMOD_StudioSystem.instance.PlayOneShot(this.OpenEvent, this.transform.position, null);
			}
			if (!BoltNetwork.isClient)
			{
				GameObject gameObject;
				if (Extensions.get_length(this.WorldItem) > 0)
				{
					gameObject = (GameObject)UnityEngine.Object.Instantiate(this.WorldItem[UnityEngine.Random.Range(0, Extensions.get_length(this.WorldItem))], this.InsidePos.position, this.InsidePos.rotation);
				}
				if (BoltNetwork.isServer && Extensions.get_length(this.WorldItem) > 0)
				{
					BoltNetwork.Attach(gameObject as GameObject);
				}
				if (Extensions.get_length(this.WorldItem) > 0)
				{
					gameObject.transform.parent = this.InsidePos.transform;
				}
			}
			this.gameObject.SetActive(false);
		}
	}

	public override void Open()
	{
		if (BoltNetwork.isRunning)
		{
			OpenSuitcase2 openSuitcase = OpenSuitcase2.Create(GlobalTargets.OnlyServer);
			openSuitcase.Suitcase = this.GetComponentInParent<BoltEntity>();
			openSuitcase.Send();
		}
		else
		{
			this.Invoke("Open_Perform", (float)1);
		}
	}

	public override void OnEnable()
	{
		this.health = 1;
		SuitCase.GlobalAmount++;
		this.TryPreload();
	}

	public override void OnDisable()
	{
		SuitCase.GlobalAmount--;
		FMOD_StudioSystem.UnPreloadEvent(this.OpenEvent);
		this.preloadComplete = false;
	}

	public override void Main()
	{
	}
}
