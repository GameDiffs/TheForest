using System;
using TheForest.Networking;
using TheForest.Utils;
using UnityEngine;

public class BreakStoneSimple : MonoBehaviour
{
	public bool HiddenCache;

	public bool RopePlace;

	public int CacheIndex;

	public GameObject Cut1;

	public GameObject Cut2;

	public GameObject Cut3;

	public GameObject Cut4;

	public GameObject Cut5;

	public Transform InsidePos;

	private GameObject MyItem;

	public GameObject[] WorldItem;

	public GameObject MyRopeMaker;

	private bool blownUp;

	public void ClientExplodeCheck()
	{
		if (!this.blownUp && CoopWeatherProxy.Instance && CoopWeatherProxy.Instance.state.ExplodeCaches[this.CacheIndex] == 1)
		{
			this.DoExplode();
		}
	}

	public void ClientHashExplodeCheck(string hash)
	{
		if (!this.blownUp && hash.Equals(HashPositionToName.GetHash(base.transform.position)))
		{
			this.DoExplode();
		}
	}

	private void Start()
	{
		if (BoltNetwork.isClient && !this.blownUp && GameObject.Find(HashPositionToName.GetHash(base.transform.position)))
		{
			this.DoExplode();
		}
	}

	private void DoExplode()
	{
		this.blownUp = true;
		this.Cut1.SetActive(true);
		this.Cut2.SetActive(true);
		this.Cut3.SetActive(true);
		this.Cut4.SetActive(true);
		this.Cut5.SetActive(true);
		this.Cut1.transform.parent = null;
		this.Cut2.transform.parent = null;
		this.Cut3.transform.parent = null;
		this.Cut4.transform.parent = null;
		this.Cut5.transform.parent = null;
		this.Cut1.GetComponent<Rigidbody>().AddForce((float)UnityEngine.Random.Range(30, 100), (float)UnityEngine.Random.Range(30, 100), (float)UnityEngine.Random.Range(30, 100));
		this.Cut2.GetComponent<Rigidbody>().AddForce((float)UnityEngine.Random.Range(30, 100), (float)UnityEngine.Random.Range(30, 100), (float)UnityEngine.Random.Range(30, 100));
		this.Cut3.GetComponent<Rigidbody>().AddForce((float)UnityEngine.Random.Range(30, 100), (float)UnityEngine.Random.Range(30, 100), (float)UnityEngine.Random.Range(30, 100));
		this.Cut4.GetComponent<Rigidbody>().AddForce((float)UnityEngine.Random.Range(30, 100), (float)UnityEngine.Random.Range(30, 100), (float)UnityEngine.Random.Range(30, 100));
		this.Cut5.GetComponent<Rigidbody>().AddForce((float)UnityEngine.Random.Range(30, 100), (float)UnityEngine.Random.Range(30, 100), (float)UnityEngine.Random.Range(30, 100));
		if (this.HiddenCache && this.WorldItem.Length > 0)
		{
			this.MyItem = (UnityEngine.Object.Instantiate(this.WorldItem[UnityEngine.Random.Range(0, this.WorldItem.Length - 1)], this.InsidePos.position, this.InsidePos.rotation) as GameObject);
			this.MyItem.transform.parent = this.InsidePos.transform;
		}
		else if (this.RopePlace)
		{
			this.MyRopeMaker.SetActive(true);
		}
		UnityEngine.Object.Destroy(base.gameObject);
	}

	private void Explosion()
	{
		if ((this.HiddenCache && BoltNetwork.isClient) || this.blownUp)
		{
			return;
		}
		if (BoltNetwork.isServer && this.HiddenCache)
		{
			CoopWeatherProxy.Instance.state.ExplodeCaches[this.CacheIndex] = 1;
		}
		else if (BoltNetwork.isRunning && !GameObject.Find(HashPositionToName.GetHash(base.transform.position)))
		{
			BoltNetwork.Instantiate(Prefabs.Instance.HashPositionToNamePrefab, base.transform.position, base.transform.rotation);
		}
		this.DoExplode();
	}
}
