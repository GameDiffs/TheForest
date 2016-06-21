using HutongGames.PlayMaker;
using System;
using TheForest.Utils;
using TheForest.World;
using UnityEngine;

public class enemyWeaponMelee : MonoBehaviour
{
	public bool netPrefab;

	public bool mainTrigger;

	private mutantScriptSetup setup;

	private mutantAI ai;

	private mutantAI_net ai_net;

	private enemyAnimEvents events;

	private Animator animator;

	private Transform rootTr;

	public GameObject bodyCollider;

	private int layerMask;

	private int blockHash;

	public int attackerType;

	public bool behindWallCheck;

	[Header("FMOD")]
	public string weaponHitEvent;

	public string parryEvent;

	public string blockEvent;

	public string shellBlockEvent;

	private FsmBool fsmAttackStructure;

	private bool hasPreloaded;

	public bool leader;

	public bool maleSkinny;

	public bool femaleSkinny;

	public bool male;

	public bool female;

	public bool creepy;

	public bool creepy_male;

	public bool creepy_baby;

	public bool creepy_fat;

	public bool firemanMain;

	public bool pale;

	private int enemyHitMask;

	private void OnDeserialized()
	{
		this.doAwake();
	}

	private void Awake()
	{
		this.doAwake();
	}

	private void doAwake()
	{
		this.enemyHitMask = 36841472;
		this.rootTr = base.transform.root;
		if (!this.netPrefab)
		{
			this.setup = base.transform.root.GetComponentInChildren<mutantScriptSetup>();
			this.ai = base.transform.root.GetComponentInChildren<mutantAI>();
		}
		if (this.netPrefab)
		{
			this.ai_net = base.transform.root.GetComponentInChildren<mutantAI_net>();
		}
		this.animator = base.transform.root.GetComponentInChildren<Animator>();
		this.events = base.transform.root.GetComponentInChildren<enemyAnimEvents>();
		this.blockHash = Animator.StringToHash("block");
		FMODCommon.PreloadEvents(new string[]
		{
			this.weaponHitEvent,
			this.parryEvent,
			this.blockEvent,
			this.shellBlockEvent
		});
		this.hasPreloaded = true;
	}

	private void OnDisable()
	{
		if (this.hasPreloaded)
		{
			FMODCommon.UnloadEvents(new string[]
			{
				this.weaponHitEvent,
				this.parryEvent,
				this.blockEvent,
				this.shellBlockEvent
			});
			this.hasPreloaded = false;
		}
	}

	private void Start()
	{
		if (!this.netPrefab && this.mainTrigger)
		{
			this.fsmAttackStructure = this.setup.pmCombat.FsmVariables.GetFsmBool("attackStructure");
		}
		base.Invoke("setupAttackerType", 1f);
	}

	private void setupAttackerType()
	{
		if (this.netPrefab)
		{
			this.leader = this.ai_net.leader;
			this.maleSkinny = this.ai_net.maleSkinny;
			this.femaleSkinny = this.ai_net.femaleSkinny;
			this.male = this.ai_net.male;
			this.female = this.ai_net.female;
			this.creepy = this.ai_net.creepy;
			this.creepy_male = this.ai_net.creepy_male;
			this.creepy_baby = this.ai_net.creepy_baby;
			this.creepy_fat = this.ai_net.creepy_fat;
			this.firemanMain = this.ai_net.fireman;
			this.pale = this.ai_net.pale;
		}
		else
		{
			this.leader = this.ai.leader;
			this.maleSkinny = this.ai.maleSkinny;
			this.femaleSkinny = this.ai.femaleSkinny;
			this.male = this.ai.male;
			this.female = this.ai.female;
			this.creepy = this.ai.creepy;
			this.creepy_male = this.ai.creepy_male;
			this.creepy_baby = this.ai.creepy_baby;
			this.creepy_fat = this.ai.creepy_fat;
			this.firemanMain = this.ai.fireman;
			this.pale = this.ai.pale;
			this.bodyCollider = this.setup.bodyCollider;
		}
		if (this.maleSkinny || this.femaleSkinny)
		{
			this.attackerType = 1;
		}
		else if (this.pale)
		{
			this.attackerType = 2;
		}
		else if (this.male || this.female)
		{
			this.attackerType = 0;
		}
		if (this.creepy || this.creepy_male)
		{
			this.attackerType = 3;
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.CompareTag("trapTrigger"))
		{
			other.gameObject.SendMessage("CutRope", SendMessageOptions.DontRequireReceiver);
		}
		if (!this.netPrefab && LocalPlayer.Transform && LocalPlayer.Animator.GetBool("deathBool"))
		{
			return;
		}
		if (other.gameObject.CompareTag("playerHitDetect") && this.mainTrigger)
		{
			if (!Scene.SceneTracker.hasAttackedPlayer)
			{
				Scene.SceneTracker.hasAttackedPlayer = true;
				Scene.SceneTracker.Invoke("resetHasAttackedPlayer", (float)UnityEngine.Random.Range(60, 180));
			}
			Animator componentInParent = other.gameObject.GetComponentInParent<Animator>();
			Vector3 position = this.rootTr.position;
			position.y += 3.3f;
			Vector3 direction = other.transform.position - position;
			RaycastHit[] array = Physics.RaycastAll(position, direction, direction.magnitude, this.enemyHitMask);
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i].transform.gameObject.layer == 11 || array[i].transform.gameObject.layer == 13 || array[i].transform.gameObject.layer == 17 || array[i].transform.gameObject.layer == 20 || array[i].transform.gameObject.layer == 21 || array[i].transform.gameObject.layer == 25)
				{
					return;
				}
			}
			if (!this.creepy_male && !this.creepy && !this.creepy_baby && !this.creepy_fat && this.events && componentInParent && this.events.parryBool && componentInParent.GetNextAnimatorStateInfo(1).tagHash == this.blockHash)
			{
				int parryDir = this.events.parryDir;
				this.animator.SetIntegerReflected("parryDirInt", parryDir);
				this.animator.SetTriggerReflected("parryTrigger");
				this.events.StartCoroutine("disableAllWeapons");
				playerHitReactions componentInParent2 = other.gameObject.GetComponentInParent<playerHitReactions>();
				if (componentInParent2 != null)
				{
					componentInParent2.enableParryState();
				}
				FMODCommon.PlayOneshotNetworked(this.parryEvent, base.transform, FMODCommon.NetworkRole.Server);
				return;
			}
			other.transform.root.SendMessage("getHitDirection", this.rootTr.position, SendMessageOptions.DontRequireReceiver);
			int num = 0;
			if (this.maleSkinny || this.femaleSkinny)
			{
				if (this.pale)
				{
					num = 10;
				}
				else
				{
					num = 13;
				}
			}
			else if (this.male && this.pale)
			{
				num = 22;
			}
			else if (this.male && !this.firemanMain)
			{
				num = 20;
			}
			else if (this.female)
			{
				num = 17;
			}
			else if (this.creepy)
			{
				if (this.pale)
				{
					num = 35;
				}
				else
				{
					num = 28;
				}
			}
			else if (this.creepy_male)
			{
				if (this.pale)
				{
					num = 40;
				}
				else
				{
					num = 30;
				}
			}
			else if (this.creepy_baby)
			{
				num = 26;
			}
			else if (this.firemanMain)
			{
				num = 8;
				if (!this.events.noFireAttack)
				{
					if (BoltNetwork.isRunning && this.netPrefab)
					{
						other.gameObject.SendMessageUpwards("Burn", SendMessageOptions.DontRequireReceiver);
					}
					else
					{
						other.gameObject.SendMessageUpwards("Burn", SendMessageOptions.DontRequireReceiver);
					}
				}
			}
			PlayerStats component = other.transform.root.GetComponent<PlayerStats>();
			if (this.male || this.female || this.creepy_male || this.creepy_fat || this.creepy || this.creepy_baby)
			{
				netId component2 = other.transform.GetComponent<netId>();
				if (BoltNetwork.isServer && component2)
				{
					other.transform.root.SendMessage("StartPrediction", SendMessageOptions.DontRequireReceiver);
					return;
				}
				if (BoltNetwork.isClient && this.netPrefab && !component2)
				{
					other.transform.root.SendMessage("setCurrentAttacker", this, SendMessageOptions.DontRequireReceiver);
					other.transform.root.SendMessage("hitFromEnemy", num, SendMessageOptions.DontRequireReceiver);
					other.transform.root.SendMessage("StartPrediction", SendMessageOptions.DontRequireReceiver);
				}
				else if (BoltNetwork.isServer)
				{
					if (!component2)
					{
						other.transform.root.SendMessage("setCurrentAttacker", this, SendMessageOptions.DontRequireReceiver);
						other.transform.root.SendMessage("hitFromEnemy", num, SendMessageOptions.DontRequireReceiver);
					}
				}
				else if (!BoltNetwork.isRunning && component)
				{
					component.setCurrentAttacker(this);
					component.hitFromEnemy(num);
				}
			}
			else if (!this.netPrefab && component)
			{
				component.setCurrentAttacker(this);
				component.hitFromEnemy(num);
			}
		}
		if (other.gameObject.CompareTag("enemyCollide") && this.mainTrigger && this.bodyCollider)
		{
			this.setupAttackerType();
			if (other.gameObject != this.bodyCollider)
			{
				other.transform.SendMessageUpwards("getAttackDirection", UnityEngine.Random.Range(0, 2), SendMessageOptions.DontRequireReceiver);
				other.transform.SendMessageUpwards("getCombo", UnityEngine.Random.Range(1, 4), SendMessageOptions.DontRequireReceiver);
				other.transform.SendMessage("getAttackerType", this.attackerType, SendMessageOptions.DontRequireReceiver);
				other.transform.SendMessage("getAttacker", this.rootTr.gameObject, SendMessageOptions.DontRequireReceiver);
				other.transform.SendMessageUpwards("Hit", 6, SendMessageOptions.DontRequireReceiver);
				FMODCommon.PlayOneshotNetworked(this.weaponHitEvent, base.transform, FMODCommon.NetworkRole.Server);
			}
		}
		if (other.gameObject.CompareTag("BreakableWood") || (other.gameObject.CompareTag("BreakableRock") && this.mainTrigger))
		{
			other.transform.SendMessage("Hit", 50, SendMessageOptions.DontRequireReceiver);
			other.SendMessage("LocalizedHit", new LocalizedHitData(base.transform.position, 50f), SendMessageOptions.DontRequireReceiver);
			FMODCommon.PlayOneshotNetworked(this.weaponHitEvent, base.transform, FMODCommon.NetworkRole.Server);
		}
		if (other.gameObject.CompareTag("SmallTree") && !this.mainTrigger)
		{
			other.SendMessage("Hit", 2, SendMessageOptions.DontRequireReceiver);
		}
		if (other.gameObject.CompareTag("Fire") && this.mainTrigger && this.firemanMain && !this.events.noFireAttack)
		{
			other.SendMessage("Burn", SendMessageOptions.DontRequireReceiver);
		}
		if (other.gameObject.CompareTag("Tree") && this.mainTrigger && this.creepy_male)
		{
			other.SendMessage("Explosion", 5f, SendMessageOptions.DontRequireReceiver);
			FMODCommon.PlayOneshotNetworked(this.weaponHitEvent, base.transform, FMODCommon.NetworkRole.Server);
		}
		if ((other.gameObject.CompareTag("structure") || other.gameObject.CompareTag("SLTier1") || other.gameObject.CompareTag("SLTier2") || other.gameObject.CompareTag("SLTier3") || other.gameObject.CompareTag("jumpObject")) && this.mainTrigger)
		{
			getStructureStrength component3 = other.gameObject.GetComponent<getStructureStrength>();
			if (component3 == null)
			{
				return;
			}
			int num2;
			if (this.creepy_male || this.creepy)
			{
				if (this.creepy)
				{
					if (component3._strength == getStructureStrength.strength.veryStrong)
					{
						num2 = 15;
					}
					else
					{
						num2 = 30;
					}
				}
				else
				{
					num2 = 40;
				}
			}
			else if (this.maleSkinny || this.femaleSkinny)
			{
				if (component3._strength == getStructureStrength.strength.weak)
				{
					num2 = 7;
				}
				else
				{
					num2 = 0;
				}
			}
			else if (this.pale)
			{
				if (component3._strength == getStructureStrength.strength.veryStrong)
				{
					num2 = 0;
				}
				else
				{
					num2 = 6;
				}
			}
			else if (component3._strength == getStructureStrength.strength.strong || component3._strength == getStructureStrength.strength.veryStrong)
			{
				num2 = 0;
			}
			else
			{
				num2 = 7;
			}
			other.SendMessage("Hit", num2, SendMessageOptions.DontRequireReceiver);
			other.SendMessage("LocalizedHit", new LocalizedHitData(base.transform.position, (float)num2), SendMessageOptions.DontRequireReceiver);
			FMODCommon.PlayOneshotNetworked(this.weaponHitEvent, base.transform, FMODCommon.NetworkRole.Server);
		}
	}
}
