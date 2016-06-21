using System;
using U_r_g_utils;
using UnityEngine;

public class clsdismemberatortester : MonoBehaviour
{
	public GameObject vargammodel;

	public Vector3 vargamspawnposition = new Vector3(0f, 0f, 0f);

	private Rigidbody[] varDbodies = new Rigidbody[0];

	private GameObject varmodelinstance;

	private void Start()
	{
		if (this.vargammodel != null)
		{
			if (this.vargammodel.transform.root == base.transform.root)
			{
				Debug.LogError("Can't host the tester on the target.\nPlease host the tester in a persistent scene object (for example the main camera).", base.gameObject);
				return;
			}
			this.metinstantiatemodel();
			this.varDbodies = this.varmodelinstance.GetComponentsInChildren<Rigidbody>();
			if (this.varDbodies.Length == 0)
			{
				Debug.LogError("There's no rigidbodies to test in the chosen target: make sure it's ragdolled and prefabbed.");
			}
		}
		else
		{
			Debug.LogError("Please assign a model to be able to test its separation.", base.transform);
		}
	}

	private void OnGUI()
	{
		if (this.vargammodel != null)
		{
			for (int i = 0; i < this.varDbodies.Length; i++)
			{
				if (this.varDbodies[i] != null && GUILayout.Button("Separate " + this.varDbodies[i].name, new GUILayoutOption[0]))
				{
					clsdismemberator componentInChildren = this.varmodelinstance.GetComponentInChildren<clsdismemberator>();
					if (componentInChildren != null)
					{
						clsurgutils.metdismember(this.varDbodies[i].transform, componentInChildren.vargamstumpmaterial, componentInChildren, componentInChildren.vargamparticleparent, componentInChildren.vargamparticlechild, true, true);
					}
					else
					{
						Debug.LogError("There's no dismemberator in the specified target.");
					}
				}
			}
			if (GUILayout.Button("Reistantiate [" + this.vargammodel.name + "]", new GUILayoutOption[0]))
			{
				UnityEngine.Object.Destroy(this.varmodelinstance);
				this.metinstantiatemodel();
				this.varDbodies = this.varmodelinstance.GetComponentsInChildren<Rigidbody>();
				Debug.Log("Reinstantiation complete.\n(please make sure vargammodel is a PREFAB)", this.varmodelinstance);
			}
		}
		else
		{
			GUILayout.Label("A dismemberator tester is on-scene, but it has no model to test.", new GUILayoutOption[0]);
		}
	}

	private void metinstantiatemodel()
	{
		this.varmodelinstance = (UnityEngine.Object.Instantiate(this.vargammodel, base.transform.position, Quaternion.identity) as GameObject);
		Transform transform = this.varmodelinstance.transform;
		transform.position = this.vargamspawnposition;
		transform.rotation = Quaternion.identity;
		transform.parent = null;
	}
}
