using System;
using System.Collections;
using UnityEngine;

public class TreeExplosion : MonoBehaviour
{
	public float BlastRange = 30f;

	public float BlastForce = 30000f;

	public GameObject DeadReplace;

	public GameObject Explosion;

	private void Explode()
	{
		UnityEngine.Object.Instantiate(this.Explosion, base.transform.position, Quaternion.identity);
		TerrainData terrainData = Terrain.activeTerrain.terrainData;
		ArrayList arrayList = new ArrayList();
		TreeInstance[] treeInstances = terrainData.treeInstances;
		for (int i = 0; i < treeInstances.Length; i++)
		{
			TreeInstance treeInstance = treeInstances[i];
			float num = Vector3.Distance(Vector3.Scale(treeInstance.position, terrainData.size) + Terrain.activeTerrain.transform.position, base.transform.position);
			if (num < this.BlastRange)
			{
				GameObject gameObject = UnityEngine.Object.Instantiate(this.DeadReplace, Vector3.Scale(treeInstance.position, terrainData.size) + Terrain.activeTerrain.transform.position, Quaternion.identity) as GameObject;
				gameObject.GetComponent<Rigidbody>().maxAngularVelocity = 1f;
				gameObject.GetComponent<Rigidbody>().AddExplosionForce(this.BlastForce, base.transform.position, 20f + this.BlastRange * 5f, -20f);
			}
			else
			{
				arrayList.Add(treeInstance);
			}
		}
		terrainData.treeInstances = (TreeInstance[])arrayList.ToArray(typeof(TreeInstance));
	}

	private void Update()
	{
		if (Input.GetButtonDown("Fire1"))
		{
			this.Explode();
		}
	}
}
