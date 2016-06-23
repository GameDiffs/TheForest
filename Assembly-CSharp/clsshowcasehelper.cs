using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using U_r_g_utils;
using UnityEngine;

public class clsshowcasehelper : MonoBehaviour
{
	public int vargamaction;

	public GameObject vargamactor;

	private bool varself;

	private bool varactivated;

	private GameObject vartarget;

	private void metactivate()
	{
		this.varactivated = true;
		switch (this.vargamaction)
		{
		case 1:
		{
			this.varself = true;
			this.metspawn();
			base.Invoke("metemit", 0f);
			Transform transform = this.vartarget.transform;
			transform.GetComponent<Rigidbody>().AddForce(new Vector3(0f, 2f, 0f), ForceMode.VelocityChange);
			transform.GetComponent<Rigidbody>().AddTorque((float)UnityEngine.Random.Range(5, 20), 0f, (float)UnityEngine.Random.Range(5, 20), ForceMode.VelocityChange);
			break;
		}
		case 2:
			this.varself = false;
			base.Invoke("metspawnexplosion", 2f);
			break;
		case 3:
			this.varself = true;
			this.metemit();
			break;
		case 4:
			base.GetComponent<Animation>().Stop();
			clsurgutils.metgodriven(base.transform, false);
			break;
		case 5:
			base.StartCoroutine("metDriveRagdoll", 3);
			break;
		case 6:
			base.GetComponent<Animation>()["Balance"].wrapMode = WrapMode.Loop;
			base.InvokeRepeating("metAsm", 0f, 8f);
			break;
		case 7:
			break;
		case 8:
		{
			clscannon component = base.GetComponent<clscannon>();
			if (component != null)
			{
				component.metactivate();
			}
			break;
		}
		default:
			UnityEngine.Debug.LogError("Unmanaged action [" + this.vargamaction + "]");
			break;
		}
	}

	private string metgetbonepath(Transform varpbone, string varpstopby = "")
	{
		string text = string.Empty;
		Transform transform = varpbone;
		if (varpbone == null)
		{
			return text;
		}
		text = varpbone.name;
		while (transform.parent != null && varpbone.name != varpstopby)
		{
			transform = transform.parent;
			text = transform.name + "/" + text;
			if (transform.name == varpstopby)
			{
				break;
			}
		}
		return text;
	}

	[DebuggerHidden]
	private IEnumerator metAsmRoutine()
	{
		clsshowcasehelper.<metAsmRoutine>c__Iterator1E4 <metAsmRoutine>c__Iterator1E = new clsshowcasehelper.<metAsmRoutine>c__Iterator1E4();
		<metAsmRoutine>c__Iterator1E.<>f__this = this;
		return <metAsmRoutine>c__Iterator1E;
	}

	private void metAsm()
	{
		base.StartCoroutine("metAsmRoutine");
	}

	[DebuggerHidden]
	private IEnumerator metDriveRagdoll()
	{
		clsshowcasehelper.<metDriveRagdoll>c__Iterator1E5 <metDriveRagdoll>c__Iterator1E = new clsshowcasehelper.<metDriveRagdoll>c__Iterator1E5();
		<metDriveRagdoll>c__Iterator1E.<>f__this = this;
		return <metDriveRagdoll>c__Iterator1E;
	}

	private void metspawnexplosion()
	{
		this.metspawn();
		this.metdismember();
		UnityEngine.Object.Destroy(base.gameObject);
	}

	private void metdismember()
	{
		GameObject gameObject = GameObject.Find("/Zombie_D");
		Rigidbody[] componentsInChildren = gameObject.GetComponentsInChildren<Rigidbody>();
		clsdismemberator componentInChildren = gameObject.GetComponentInChildren<clsdismemberator>();
		int num = componentsInChildren.Length;
		num = UnityEngine.Random.Range(num / 3, num / 2);
		List<Transform> list = new List<Transform>();
		bool varplastcut = false;
		for (int i = 0; i < num; i++)
		{
			if (i == num - 1)
			{
				varplastcut = true;
			}
			Transform transform = clsurgutils.metdismemberpart(componentsInChildren[UnityEngine.Random.Range(2, componentsInChildren.Length)].transform, componentInChildren.vargamstumpmaterial, componentInChildren, null, null, true, varplastcut);
			if (transform != null)
			{
				list.Add(transform);
			}
		}
		foreach (Transform current in list)
		{
			Rigidbody[] componentsInChildren2 = current.GetComponentsInChildren<Rigidbody>();
			Vector3 vector = current.position - base.transform.position;
			UnityEngine.Debug.DrawRay(base.transform.position, vector, Color.yellow);
			for (int j = 0; j < componentsInChildren2.Length; j++)
			{
				componentsInChildren2[j].AddForce(vector * 3.5f, ForceMode.VelocityChange);
				float num2 = UnityEngine.Random.Range(1f, 10f);
				componentsInChildren2[j].AddTorque(new Vector3(num2, num2, num2), ForceMode.VelocityChange);
			}
		}
	}

	public void metspawn()
	{
		this.vartarget = (UnityEngine.Object.Instantiate(this.vargamactor, base.transform.position, Quaternion.identity) as GameObject);
		this.vartarget.SendMessage("metactivate");
	}

	public void metemit()
	{
		if (!this.varactivated)
		{
			UnityEngine.Debug.LogError("Need to be activated.");
			return;
		}
		ParticleEmitter componentInChildren;
		if (this.varself)
		{
			componentInChildren = base.GetComponentInChildren<ParticleEmitter>();
		}
		else
		{
			if (this.vargamactor == null)
			{
				UnityEngine.Debug.LogError("No actor", base.gameObject);
				return;
			}
			componentInChildren = this.vargamactor.GetComponentInChildren<ParticleEmitter>();
		}
		if (componentInChildren == null)
		{
			UnityEngine.Debug.LogError("No emitter", base.gameObject);
			return;
		}
		componentInChildren.emit = true;
	}
}
