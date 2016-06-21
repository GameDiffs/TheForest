using System;
using TheForest.Utils;
using UnityEngine;

public class groupEncounterSetup : MonoBehaviour
{
	private sceneTracker info;

	private Animator animator;

	private dummyAnimatorControl dummyControl;

	public GameObject lookatGo;

	public Transform leaderPos;

	public Transform followerPos1;

	public Transform followerPos2;

	public Transform followerPos3;

	public Transform followerPos4;

	public bool leaderPosFull;

	public bool followerPosFull1;

	public bool followerPosFull2;

	public bool followerPosFull3;

	public bool followerPosFull4;

	public bool typeRitual1;

	public bool typeRitual2;

	public bool typeFeeding1;

	public bool typeMourning1;

	public bool occupied;

	public bool encounterCoolDown;

	public GameObject trigger;

	public GameObject dummyGo;

	public GameObject dummyPrefab;

	private void SpawnMP()
	{
	}

	private void InitAnimator()
	{
		GameObject gameObject;
		if (this.dummyGo)
		{
			gameObject = this.dummyGo;
		}
		else if (this.lookatGo)
		{
			gameObject = this.lookatGo;
		}
		else
		{
			gameObject = null;
		}
		if (gameObject)
		{
			this.animator = (gameObject.GetComponent<Animator>() ?? gameObject.GetComponentInChildren<Animator>());
			this.dummyControl = (gameObject.GetComponent<dummyAnimatorControl>() ?? gameObject.GetComponentInChildren<dummyAnimatorControl>());
		}
	}

	private void Start()
	{
		this.InitAnimator();
		this.info = Scene.SceneTracker;
		this.setupEncounter();
		base.Invoke("forceEnable", 0.5f);
	}

	private void OnDeserialized()
	{
		this.setupEncounter();
	}

	private void OnEnable()
	{
		this.setupEncounter();
		if (this.dummyControl)
		{
			this.dummyControl.enabled = true;
		}
	}

	private void setupEncounter()
	{
		if (this.typeMourning1)
		{
			dummyTypeSetup component = base.transform.GetComponent<dummyTypeSetup>();
			if (component && (component._type == EnemyType.regularMale || component._type == EnemyType.regularMaleLeader || component._type == EnemyType.regularFemale || component._type == EnemyType.regularMaleFireman) && this.info && base.gameObject)
			{
				this.info.addToEncounters(base.gameObject);
			}
		}
		else if (this.info && base.gameObject)
		{
			this.info.addToEncounters(base.gameObject);
		}
		if (!base.IsInvoking("checkEncounterConditions"))
		{
			base.InvokeRepeating("checkEncounterConditions", 1f, 20f);
		}
		this.encounterCoolDown = false;
	}

	private void checkEncounterConditions()
	{
		float num = Vector3.Angle(Vector3.up, base.transform.up);
		if (num > 15f)
		{
			this.disableEncounter();
		}
		int layerMask = 36708352;
		Collider[] array = Physics.OverlapSphere(base.transform.position, 7f, layerMask);
		if (array.Length == 0)
		{
			return;
		}
		if (array[0])
		{
			this.disableEncounter();
		}
	}

	private void forceEnable()
	{
		base.transform.gameObject.SetActive(true);
	}

	private void OnDisable()
	{
		this.disableEncounter();
	}

	private void OnDestroy()
	{
		this.disableEncounter();
	}

	private void OnSpawned()
	{
		this.SpawnMP();
		this.InitAnimator();
	}

	private void removeDummyGeo()
	{
		UnityEngine.Object.Destroy(this.dummyGo);
		UnityEngine.Object.Destroy(this.trigger);
	}

	private void dropFromCarry()
	{
	}

	public void forceEnableAnimator()
	{
		this.animator.enabled = true;
	}

	public void disableEncounter()
	{
		if (base.gameObject && this.info)
		{
			this.info.removeFromEncounters(base.gameObject);
		}
		base.CancelInvoke("checkEncounterConditions");
		this.occupied = false;
		this.encounterCoolDown = false;
	}

	public void enableEncounterCoolDown()
	{
		this.encounterCoolDown = true;
		base.Invoke("resetEncounterCoolDown", 300f);
	}

	private void resetEncounterCoolDown()
	{
		this.encounterCoolDown = false;
	}
}
