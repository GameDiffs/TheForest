using Boo.Lang.Runtime;
using System;
using System.Collections;
using UnityEngine;
using UnityScript.Lang;

[Serializable]
public class MasterFire : MonoBehaviour
{
	public float fireSpread;

	public Transform fireParticle;

	public float spreadTime;

	public float burnTime;

	public bool burnOnStart;

	private bool fireStarted;

	private float fTime;

	private Transform burnP;

	private bool fireSp;

	public MasterFire()
	{
		this.fireSpread = 3f;
		this.spreadTime = 3f;
		this.burnTime = 6f;
	}

	public override void Update()
	{
		this.spreadFire();
	}

	public override void startFire()
	{
		if (!this.fireStarted)
		{
			this.fireStarted = true;
			this.burnP = (Transform)UnityEngine.Object.Instantiate(this.fireParticle, this.transform.position, Quaternion.identity);
			this.burnP.parent = this.transform;
			this.fTime = Time.time;
		}
	}

	public override void endFire()
	{
		this.fireStarted = false;
		if (this.burnP.GetComponent<ParticleEmitter>())
		{
			this.burnP.GetComponent<ParticleEmitter>().emit = false;
		}
		IEnumerator enumerator = UnityRuntimeServices.GetEnumerator(this.burnP);
		while (enumerator.MoveNext())
		{
			object arg_5E_0;
			object expr_44 = arg_5E_0 = enumerator.Current;
			if (!(expr_44 is Transform))
			{
				arg_5E_0 = RuntimeServices.Coerce(expr_44, typeof(Transform));
			}
			Transform transform = (Transform)arg_5E_0;
			if (transform.GetComponent<ParticleEmitter>())
			{
				transform.GetComponent<ParticleEmitter>().emit = false;
				UnityRuntimeServices.Update(enumerator, transform);
			}
		}
		UnityEngine.Object.Destroy(this);
	}

	public override void spreadFire()
	{
		if (Physics.OverlapSphere(this.transform.position, this.fireSpread) != null)
		{
			Collider[] array = Physics.OverlapSphere(this.transform.position, this.fireSpread);
			int i = 0;
			Collider[] array2 = array;
			int length = array2.Length;
			while (i < length)
			{
				if (array2[i].CompareTag("fire"))
				{
					array2[i].SendMessage("startFire");
				}
				array2[i].SendMessage("Burn", SendMessageOptions.DontRequireReceiver);
				i++;
			}
		}
	}

	public override void Start()
	{
		if (this.burnOnStart)
		{
			this.startFire();
		}
	}

	public override void Main()
	{
	}
}
