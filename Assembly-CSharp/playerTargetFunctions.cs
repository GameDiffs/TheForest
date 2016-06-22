using Bolt;
using HutongGames.PlayMaker;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using TheForest.Utils;
using UnityEngine;

public class playerTargetFunctions : MonoBehaviour
{
	private PlayMakerFSM pmControl;

	private PlayMakerFSM pmRotate;

	private FsmVector3 fsmSmashPosLook;

	private FsmVector3 fsmTreePos;

	public playerScriptSetup setup;

	private Animator animator;

	private Transform playerTr;

	private Transform playerRoot;

	public int maxAttackers;

	public int allEnemies;

	public float playerDist;

	private float tempVal;

	public GameObject[] allEnemyGo;

	public int allAttackers;

	public bool coveredInMud;

	public List<GameObject> visibleEnemies = new List<GameObject>();

	public float closeVisRange;

	public float longVisRange;

	public float visionRange;

	public float lighterRange;

	public float crouchRange;

	public float bushRange;

	private float mudRange;

	private float setCloseVisRange;

	private float setLongVisRange;

	private void Start()
	{
		this.setup = base.GetComponent<playerScriptSetup>();
		this.animator = base.GetComponent<Animator>();
		this.playerTr = base.transform;
		this.playerRoot = base.transform.root;
		this.pmControl = this.setup.pmControl;
		this.pmRotate = this.setup.pmRotate;
		this.fsmSmashPosLook = this.pmControl.FsmVariables.GetFsmVector3("smashPosLook");
		this.fsmTreePos = this.pmRotate.FsmVariables.GetFsmVector3("treeStandPos");
		if (!base.IsInvoking("sendStealthValues") && this.setup.sceneInfo)
		{
			base.InvokeRepeating("sendStealthValues", 1f, 1f);
		}
		if (BoltNetwork.isRunning)
		{
			this.maxAttackers = 12;
		}
	}

	private void Awake()
	{
	}

	private void Update()
	{
		this.allAttackers = this.setup.proxyAttackers.arrayList.Count;
		if (BoltNetwork.isRunning)
		{
			this.maxAttackers = 12;
		}
		this.bushRange = 0f;
	}

	public void sendPlayerAttacking()
	{
		foreach (GameObject current in this.setup.sceneInfo.visibleEnemies)
		{
			if (current)
			{
				current.SendMessage("setPlayerAttacking", SendMessageOptions.DontRequireReceiver);
			}
		}
		if (BoltNetwork.isRunning && BoltNetwork.isClient)
		{
			playerSwingWeapon playerSwingWeapon = playerSwingWeapon.Create(GlobalTargets.OnlyServer);
			playerSwingWeapon.Send();
		}
	}

	private void sendStealthValues()
	{
		if (this.animator.GetBool("lighterHeld"))
		{
			if (Clock.Dark && !Clock.InCave)
			{
				this.sendLighterRange(60f);
			}
			else
			{
				this.sendLighterRange(35f);
			}
		}
		else
		{
			this.sendLighterRange(0f);
		}
		foreach (GameObject current in this.setup.sceneInfo.closeEnemies)
		{
			if (current)
			{
				current.SendMessage("setBushRange", this.bushRange, SendMessageOptions.DontRequireReceiver);
			}
		}
		float num = (!this.coveredInMud) ? LocalPlayer.Stats.Stealth : Mathf.Max(10f, LocalPlayer.Stats.Stealth);
		this.sendMudRange(-num);
	}

	public void addAttacker(int hash)
	{
		if (this.setup.proxyAttackers.arrayList.Count < this.maxAttackers && !this.setup.proxyAttackers.arrayList.Contains(hash))
		{
			this.setup.proxyAttackers.arrayList.Add(hash);
		}
	}

	public void removeAttacker(int hash)
	{
		if (this.setup.proxyAttackers.arrayList.Contains(hash))
		{
			this.setup.proxyAttackers.arrayList.Remove(hash);
		}
	}

	private void findAllEnemies()
	{
		this.allEnemyGo = GameObject.FindGameObjectsWithTag("enemyRoot");
	}

	public void sendVisionRange(float range)
	{
		float num;
		if (Clock.InCave)
		{
			num = range * 0.75f;
		}
		else
		{
			num = range;
		}
		this.visionRange = range;
		foreach (GameObject current in this.setup.sceneInfo.closeEnemies)
		{
			if (current)
			{
				current.SendMessage("setVisionRange", num, SendMessageOptions.DontRequireReceiver);
			}
		}
	}

	public void sendLighterRange(float range)
	{
		float num;
		if (Clock.InCave)
		{
			num = range * 0.75f;
		}
		else
		{
			num = range;
		}
		this.lighterRange = range;
		foreach (GameObject current in this.setup.sceneInfo.closeEnemies)
		{
			if (current)
			{
				current.SendMessage("setLighterRange", num, SendMessageOptions.DontRequireReceiver);
			}
		}
	}

	public void sendMudRange(float range)
	{
		float num;
		if (Clock.InCave)
		{
			num = range * 0.75f;
		}
		else
		{
			num = range;
		}
		this.mudRange = range;
		foreach (GameObject current in this.setup.sceneInfo.closeEnemies)
		{
			if (current)
			{
				current.SendMessage("setMudRange", num, SendMessageOptions.DontRequireReceiver);
			}
		}
	}

	public void enableCrouchLayers()
	{
		this.crouchRange = -12f;
		foreach (GameObject current in this.setup.sceneInfo.closeEnemies)
		{
			if (current)
			{
				current.SendMessage("setcrouchRange", this.crouchRange, SendMessageOptions.DontRequireReceiver);
				current.SendMessage("setVisionLayersOn", SendMessageOptions.DontRequireReceiver);
			}
		}
	}

	public void disableCrouchLayers()
	{
		this.crouchRange = 0f;
		foreach (GameObject current in this.setup.sceneInfo.closeEnemies)
		{
			if (current)
			{
				current.SendMessage("setcrouchRange", this.crouchRange, SendMessageOptions.DontRequireReceiver);
			}
			if (current)
			{
				current.SendMessage("setVisionLayersOff", SendMessageOptions.DontRequireReceiver);
			}
		}
	}

	[DebuggerHidden]
	private IEnumerator enableAimAtTarget()
	{
		playerTargetFunctions.<enableAimAtTarget>c__IteratorE5 <enableAimAtTarget>c__IteratorE = new playerTargetFunctions.<enableAimAtTarget>c__IteratorE5();
		<enableAimAtTarget>c__IteratorE.<>f__this = this;
		return <enableAimAtTarget>c__IteratorE;
	}

	[DebuggerHidden]
	private IEnumerator disableAimAtTarget()
	{
		playerTargetFunctions.<disableAimAtTarget>c__IteratorE6 <disableAimAtTarget>c__IteratorE = new playerTargetFunctions.<disableAimAtTarget>c__IteratorE6();
		<disableAimAtTarget>c__IteratorE.<>f__this = this;
		return <disableAimAtTarget>c__IteratorE;
	}

	[DebuggerHidden]
	private IEnumerator setSmashPosition(Vector3 pos)
	{
		playerTargetFunctions.<setSmashPosition>c__IteratorE7 <setSmashPosition>c__IteratorE = new playerTargetFunctions.<setSmashPosition>c__IteratorE7();
		<setSmashPosition>c__IteratorE.pos = pos;
		<setSmashPosition>c__IteratorE.<$>pos = pos;
		<setSmashPosition>c__IteratorE.<>f__this = this;
		return <setSmashPosition>c__IteratorE;
	}

	[DebuggerHidden]
	private IEnumerator setTreePosition(Vector3 pos)
	{
		playerTargetFunctions.<setTreePosition>c__IteratorE8 <setTreePosition>c__IteratorE = new playerTargetFunctions.<setTreePosition>c__IteratorE8();
		<setTreePosition>c__IteratorE.pos = pos;
		<setTreePosition>c__IteratorE.<$>pos = pos;
		<setTreePosition>c__IteratorE.<>f__this = this;
		return <setTreePosition>c__IteratorE;
	}

	[DebuggerHidden]
	private IEnumerator moveToTreeTarget(Vector3 pos)
	{
		playerTargetFunctions.<moveToTreeTarget>c__IteratorE9 <moveToTreeTarget>c__IteratorE = new playerTargetFunctions.<moveToTreeTarget>c__IteratorE9();
		<moveToTreeTarget>c__IteratorE.pos = pos;
		<moveToTreeTarget>c__IteratorE.<$>pos = pos;
		<moveToTreeTarget>c__IteratorE.<>f__this = this;
		return <moveToTreeTarget>c__IteratorE;
	}
}
